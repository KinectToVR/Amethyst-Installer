using System.Diagnostics;

namespace AmethystUtils {
    public static class Util {
        public static void PassToInstaller(string argument) {
            var process = Process.Start(new ProcessStartInfo() {
                FileName = Constants.AmethystInstallerExecutablePath,
                Arguments = argument
            });
        }

        public static void PassToAmethyst(string argument) {
            var process = Process.Start(new ProcessStartInfo() {
                FileName = Constants.AmethystExecutablePath,
                Arguments = argument
            });
        }
    }
}
