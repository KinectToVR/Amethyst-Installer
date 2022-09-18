using InstallerTools.Commands;
using System;
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

            CommandParser parser = new CommandParser();
            args = new string[] { "--help" };
            if ( !parser.ParseCommands(args) ) {
                // Woo command!
            } else {
                // Regular execution
            }
        }
    }
}
