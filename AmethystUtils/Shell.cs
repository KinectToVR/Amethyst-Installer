using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AmethystUtils {
    public static class Shell {

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern uint SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext, [Out] out IntPtr pidl, uint sfgaoIn, [Out] out uint psfgaoOut);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHGetNameFromIDList(IntPtr pidl, SIGDN sigdnName, [Out] out IntPtr ppszName);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFile(string name);

        private enum SIGDN : uint {
            NORMALDISPLAY = 0x00000000,
            PARENTRELATIVEPARSING = 0x80018001,
            DESKTOPABSOLUTEPARSING = 0x80028000,
            PARENTRELATIVEEDITING = 0x80031001,
            DESKTOPABSOLUTEEDITING = 0x8004c000,
            FILESYSPATH = 0x80058000,
            URL = 0x80068000,
            PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
            PARENTRELATIVE = 0x80080001
        }

        // An alternative to:
        //      Process.Start("explorer.exe", $"/select,{filePath}");
        // P/Invoke version allows us to do the same task without spawning a new instance of explorer.exe
        public static void OpenFolderAndSelectItem(string filePath) {

            filePath = Path.GetFullPath(filePath); // Resolve absolute path
            string folderPath = Path.GetDirectoryName(filePath);
            string file = Path.GetFileName(filePath);

            IntPtr nativeFolder;
            uint psfgaoOut;


            SHParseDisplayName(folderPath, IntPtr.Zero, out nativeFolder, 0, out psfgaoOut);

            if ( nativeFolder == IntPtr.Zero ) {
                Console.Error.WriteLine($"Failed to find directory {filePath}!");
                return;
            }

            IntPtr nativeFile;
            SHParseDisplayName(Path.Combine(folderPath, file), IntPtr.Zero, out nativeFile, 0, out psfgaoOut);

            IntPtr[] fileArray;
            if ( nativeFile == IntPtr.Zero ) {
                // Open the folder without the file selected if we can't find the file
                fileArray = new IntPtr[] { nativeFolder };
            } else {
                fileArray = new IntPtr[] { nativeFile };
            }

            SHOpenFolderAndSelectItems(nativeFolder, ( uint ) fileArray.Length, fileArray, 0);

            Marshal.FreeCoTaskMem(nativeFolder);
            if ( nativeFile != IntPtr.Zero ) {
                Marshal.FreeCoTaskMem(nativeFile);
            }
        }
    }
}