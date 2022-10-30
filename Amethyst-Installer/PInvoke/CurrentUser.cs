using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace amethyst_installer_gui.PInvoke {
    public static class CurrentUser {

        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WtsInfoClass wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);
        [DllImport("Wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pointer);
        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetUserProfileDirectory(IntPtr hToken, StringBuilder path, ref int dwSize);

        private const uint TOKEN_QUERY = 0x0008;
        private const uint INVALID_SESSION_ID = 0xFFFFFFFF;

        private static string s_userProfileDirectory = string.Empty;

        private enum WtsInfoClass {
            WTSUserName = 5,
            WTSDomainName = 7,
        }

        private static uint GetCurrentSessionID() {
            var activeSessionId = WTSGetActiveConsoleSessionId();
            if ( activeSessionId == INVALID_SESSION_ID ) //failed
            {
                throw new InvalidOperationException("Can't get current Session ID");
            }
            return activeSessionId;
        }

        private static string GetUsername(int sessionId) {
            IntPtr buffer;
            int strLen;
            string username = "SYSTEM";
            if ( WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSUserName, out buffer, out strLen) && strLen > 1 ) {
                username = Marshal.PtrToStringAnsi(buffer);
                WTSFreeMemory(buffer);
            }
            return username;
        }

        private static IntPtr GetLoggedInUserToken() {

            IntPtr finalHandle = IntPtr.Zero;
            int currentSessionId = (int) GetCurrentSessionID();

            var procs = Process.GetProcesses();
            foreach ( var process in procs ) {
                try {
                    if ( process.SessionId == currentSessionId ) {
                        if ( process.HandleCount > 0 && OpenProcessToken(process.Handle, TOKEN_QUERY, out finalHandle) ) {
                            break;
                        }
                    }
                } catch ( Exception e ) {
                    Logger.PrivateDoNotUseLogExecption(Util.FormatException(e), $"Unhandled Exception: {e.GetType().Name} in {e.Source}: {e.Message}");
                }
            }

            return finalHandle;
        }

        public static string GetUserProfileDirectory() {
            if ( s_userProfileDirectory.Length == 0 ) {
                try {
                    IntPtr user = GetLoggedInUserToken();
                    int size = 256;
                    StringBuilder sBuilder = new StringBuilder(size);
                    GetUserProfileDirectory(user, sBuilder, ref size);
                    s_userProfileDirectory = sBuilder.ToString();
                    if ( s_userProfileDirectory.ToLowerInvariant() == @"c:\\windows\\system32\\config\\systemprofile") {
                        s_userProfileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    }
                } catch ( InvalidOperationException ) {
                    s_userProfileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                }
            }
            return s_userProfileDirectory;
        }

        public static string GetCurrentlyLoggedInUsername() {
            try {
                return GetUsername(( int ) GetCurrentSessionID());
            } catch ( InvalidOperationException ) {
                return Environment.UserName;
            }
        }

    }
}
