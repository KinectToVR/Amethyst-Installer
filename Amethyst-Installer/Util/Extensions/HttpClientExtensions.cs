using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace amethyst_installer_gui {
    public static class HttpClientExtensions {

        // https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient
        public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, Action<long> progress = null, CancellationToken cancellationToken = default) {
            // Get the http headers first to examine the content length
            using ( var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead) ) {
                response.EnsureSuccessStatusCode();
                var contentLength = response.Content.Headers.ContentLength;
                Logger.Info($"Received file size ({contentLength} bytes)!");

                using ( var download = await response.Content.ReadAsStreamAsync() ) {

                    // Ignore progress reporting when no progress reporter was 
                    // passed or when the content length is unknown
                    if ( progress == null || !contentLength.HasValue ) {
                        await download.CopyToAsync(destination);
                        return;
                    }

                    // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
                    var relativeProgress = new Progress<long>(totalBytes => progress.Invoke(totalBytes));

                    // Use extension method to report progress while downloading
                    // Use a 1MB buffer for download operations
                    await download.CopyToAsync(destination, 1024 * 1024, relativeProgress, cancellationToken);
                    progress.Invoke(contentLength ?? long.MaxValue);
                }
            }
        }
    }
}
