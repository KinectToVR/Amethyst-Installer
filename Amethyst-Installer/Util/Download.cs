using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeoutClock = System.Timers.Timer;

namespace amethyst_installer_gui {
    public static class Download {

        private static HttpClient s_httpClient;

        static Download() {
            s_httpClient = new HttpClient();
            s_httpClient.Timeout = TimeSpan.FromSeconds(Constants.DownloadTimeout);
        }

        /// <summary>
        /// Downloads a file from a remote URL, with an optional timeout
        /// </summary>
        /// <param name="url">The URL that points to the file</param>
        /// <param name="filename">The output name of the file</param>
        /// <param name="path"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task DownloadFileAsync(string url, string filename = null, string path = null, Action<long, int> progress = null, Action onComplete = null, int identifier = -1) {

            string fullPath = Path.GetFullPath(Path.Combine(path, filename));
            Logger.Info(fullPath);

#if DEBUG && DOWNLOAD_CACHE
            // True Debug mode has a download cache, Release builds will have this disabled, as this doesn't help us in Releases.
            // Basically this is only here so that I can debug faster
            // If the file exists assume it's been downloaded properly and is cached

            if ( File.Exists(fullPath) ) {
                Logger.Info($"Using cached file {fullPath}...");
                progress.Invoke(long.MaxValue, identifier);
                if ( onComplete != null )
                    onComplete();
                return;
            }
#endif

            Logger.Info($"Downloading file {filename} from {url}...");

            using ( var fileStream = File.OpenWrite(fullPath) ) {
                await s_httpClient.DownloadAsync(url, fileStream, progress, ( long ) ( s_httpClient.Timeout.TotalMilliseconds ), identifier);
            }
            if ( onComplete != null )
                onComplete();
        }

        /// <summary>
        /// Performs a GET request at the designated url
        /// </summary>
        public static string GetStringAsync(string url) {

            var response = s_httpClient.GetAsync(url).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}