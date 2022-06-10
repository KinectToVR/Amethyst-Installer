using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Util {
    internal class Download {
        public void DownloadFile(string url, string path, float timeout = 10) {
            Process.Start(new ProcessStartInfo() {
                FileName = "curl.exe",
                Arguments = $"--connect-timeout {timeout.ToString("n2")}",
            });
        }
    }
}
