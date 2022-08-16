using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TimeoutClock = System.Timers.Timer;

namespace amethyst_installer_gui {
    public static class HttpClientExtensions {

        // https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient
        public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, Action<long> progress = null, long timeout = 30000, CancellationToken cancellationToken = default) {

            try {
                // Get the http headers first to examine the content length
                using ( var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead) ) {
                    response.EnsureSuccessStatusCode();
                    var contentLength = response.Content.Headers.ContentLength;
                    Logger.Info($"Received file size ({contentLength} bytes)!");

                    using ( TimeoutClock timer = new TimeoutClock(timeout) ) {

                        bool timedOut = false;
                        timer.Elapsed += ( (sender, _) => {
                            timedOut = true;
                            response.Dispose();
                        } );
                        timer.Start();

                        using ( cancellationToken.Register(response.Dispose) ) {
                            using ( var download = await response.Content.ReadAsStreamAsync() ) {

                                // Ignore progress reporting when no progress reporter was 
                                // passed or when the content length is unknown
                                if ( progress == null || !contentLength.HasValue ) {
                                    await download.CopyToAsync(destination);
                                    return;
                                }

                                // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
                                var relativeProgress = new Progress<long>( (totalBytes) => {

                                    // If we timed out, explode the download
                                    if ( timedOut ) {
                                        response.Dispose();

                                        // The code following this won't execute. It is your responsibility going forward to
                                        // destroy the file stream and delete the incomplete file.
                                    }

                                    // Reset timeout timer
                                    timer.Stop();
                                    timer.Start();

                                    // Progess update callback
                                    progress.Invoke(totalBytes);
                                });

                                // Use extension method to report progress while downloading
                                // Use a 1MB buffer for download operations
                                await download.CopyToAsync(destination, 1024 * 1024, relativeProgress, cancellationToken);
                                timer.Stop();
                                progress.Invoke(contentLength.Value);
                            }
                        }
                    }
                }
            } catch ( ObjectDisposedException ex ) {
                // Convert to a more specific error if it makes sense.
                if ( cancellationToken.IsCancellationRequested )
                    throw new OperationCanceledException();

                if (ex.ObjectName == "SslStream" )
                    throw new TimeoutException("Connection timed out!");

                // Just throw the same error if it's something else
                throw;
            }
        }
    }
}
