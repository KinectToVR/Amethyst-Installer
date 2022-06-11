using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui {
    internal class Download {
        public void DownloadFile(string url, string path = null, float timeout = 10) {
            var AAA = $"--connect-timeout {timeout.ToString("n1")} -L -O";
            Process.Start(new ProcessStartInfo() {
                FileName = "curl.exe",
                Arguments = $"--connect-timeout {timeout.ToString("n2")} --location --remote-name {url}",
                WorkingDirectory = path ?? Directory.GetCurrentDirectory()
            });
        }
    }
}
