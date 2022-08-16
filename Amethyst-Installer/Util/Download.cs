using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace amethyst_installer_gui {
    public static class Download {

        private static HttpClient s_httpClient;

        static Download() {
            s_httpClient = new HttpClient();
        }

        /// <summary>
        /// Downloads a file from a remote URL, with an optional timeout
        /// </summary>
        /// <param name="url">The URL that points to the file</param>
        /// <param name="filename">The output name of the file</param>
        /// <param name="path"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task DownloadFileAsync(string url, string filename = null, string path = null, Action<long> progress = null, float timeout = 10) {

            // using ( var httpClient = new HttpClient() ) {
                s_httpClient.Timeout = TimeSpan.FromSeconds(timeout);

                Logger.Info($"Downloading file {filename} from {url}...");

                // Stream response = await httpClient.GetStreamAsync(url);

                using ( var fileStream = File.OpenWrite(Path.GetFullPath(Path.Combine(path, filename))) ) {
                    // await response.CopyToAsync(fileStream)
                    await s_httpClient.DownloadAsync(url, fileStream, progress, (long)(timeout * 1000L));
                }
            // }

            return;

            var AAA = $"--connect-timeout {timeout.ToString("n2")} -LO {(filename == null ? filename.Trim() + " " : "")}{url.Trim()}";
            Process.Start(new ProcessStartInfo() {
                FileName = "curl.exe",
                Arguments = $"--connect-timeout {timeout.ToString("n2")} -LO {( filename == null ? filename.Trim() + " " : "" )}{url.Trim()}",
                WorkingDirectory = path ?? Directory.GetCurrentDirectory()
            });
        }
    }
}
