using System;
using System.IO;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.PInvoke {
    public static class Shell {

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext, [Out] out IntPtr pidl, uint sfgaoIn, [Out] out uint psfgaoOut);

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
                // Log error, can't find folder
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
