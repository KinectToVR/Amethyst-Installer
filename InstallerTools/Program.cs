using InstallerTools.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace InstallerTools {

    internal class Program {


        #region ANSI CMD
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();


        public static void EnableAnsiCmd() {
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            if ( !GetConsoleMode(iStdOut, out uint outConsoleMode) ) {
                Console.Error.WriteLine("Failed to get output console mode");
                return;
            }

            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if ( !SetConsoleMode(iStdOut, outConsoleMode) ) {
                Console.Error.WriteLine($"Failed to set output console mode, error code: {GetLastError()}");
                return;
            }
        }

        #endregion

        static void Main(string[] args) {

            Console.OutputEncoding = Encoding.Unicode;
            EnableAnsiCmd();

            Console.WriteLine(@"  ╭─────────────────────────────────────────────────────────────╮
  |                                                             |
  |   AMETHYST INSTALLER                                        |
  |                                                             |
  |   --help, -h          Shows help                            |
  |   --update -u         Attempts to update Amethyst           |
  |   --uninstall -x      Attempts to uninstall Amethyst        |
  |   --modify -m         Attempts to modify an existing        |
  |                       Amethyst install                      |
  |   --silent -s         Executes the installer silently       |
  |   --install-dir       Sets the install directory,           |
  |   --debug             Forces the installer to run in Debug  |
  |                       mode                                  |
  |                                                             |
  ╘═════════════════════════════════════════════════════════════╛");

#if DEBUG
            args = @"checksum --p F:\Downloads\Amethyst-Release-22a89a9.zip".Split();
            // args = @"--help checksum".Split();
#endif

            CommandParser parser = new CommandParser();
            if ( !parser.ParseCommands(args) ) {
                // Woo command!
            } else {
                // Regular execution
            }
#if DEBUG
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
#endif
        }
    }
}
