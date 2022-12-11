using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.PInvoke {
    public static class Kernel {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr module, string procName);

        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);


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

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr WindowHandle);
        private const int SW_RESTORE = 9;


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

        public static void FocusProcess(Process proc) {
            IntPtr hWnd = proc.MainWindowHandle;
            ShowWindowAsync(new HandleRef(null, hWnd), SW_RESTORE);
            SetForegroundWindow(proc.MainWindowHandle);
        }
    }
}
