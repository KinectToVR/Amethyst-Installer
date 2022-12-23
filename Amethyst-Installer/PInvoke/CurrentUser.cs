using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        private static readonly string[] INVALID_PROFILE_DIRECTORIES = {
            @"c:\windows\serviceprofiles\ovrlibraryservice", // Wtf oculus has a user profile???
            @"c:\users\defaultapppool",
            @"c:\windows\system32\config\systemprofile",
            @"c:\windows\serviceprofiles\networkservice",
            @"c:\windows\serviceprofiles\localservice",

        };

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
                    if ( s_userProfileDirectory.Length == 0 || // This happens... I don't even know either
                        INVALID_PROFILE_DIRECTORIES.Contains(s_userProfileDirectory.ToLowerInvariant().TrimEnd('\\', '/'))) {
                        Logger.Warn($"Failed to get determine user directory!");
                        // @TODO: See whether this is a good approach to fixing the running as SYSTEM bug
                        // This is a bandaid fix I have no clue whether this is going to work or not
                        // Fixing bugs which are unreliable to reproduce is painful
                        s_userProfileDirectory = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "..", GetCurrentlyLoggedInUsername())); ;
                    }
                } catch ( InvalidOperationException e ) {
                    Logger.Warn($"Failed to get determine user directory!\n{Util.FormatException(e)}");
                    s_userProfileDirectory = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "..", GetCurrentlyLoggedInUsername()));
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
