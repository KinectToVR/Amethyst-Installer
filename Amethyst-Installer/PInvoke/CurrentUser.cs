using System;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.PInvoke {
    public static class CurrentUser {

        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WtsInfoClass wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);
        [DllImport("Wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pointer);

        private enum WtsInfoClass {
            WTSUserName = 5,
            WTSDomainName = 7,
        }

        public static string GetUsername(int sessionId, bool prependDomain = false) {
            IntPtr buffer;
            int strLen;
            string username = "SYSTEM";
            if ( WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSUserName, out buffer, out strLen) && strLen > 1 ) {
                username = Marshal.PtrToStringAnsi(buffer);
                WTSFreeMemory(buffer);
                if ( prependDomain ) {
                    if ( WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSDomainName, out buffer, out strLen) && strLen > 1 ) {
                        username = Marshal.PtrToStringAnsi(buffer) + "\\" + username;
                        WTSFreeMemory(buffer);
                    }
                }
            }
            return username;
        }

        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        private const uint INVALID_SESSION_ID = 0xFFFFFFFF;

        public static uint GetCurrentSessionID() {
            var activeSessionId = WTSGetActiveConsoleSessionId();
            if ( activeSessionId == INVALID_SESSION_ID ) //failed
            {
                throw new InvalidOperationException("Can't get current Session ID");
            }
            return activeSessionId;
        }

        public static string GetCurrentlyLoggedInUsername() {
            try {
                return GetUsername((int) GetCurrentSessionID(), false);
            } catch ( InvalidOperationException e ) {
                return Environment.UserName;
            }
        }
    }
}
