using amethyst_installer_gui.PInvoke;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZstdNet;

namespace amethyst_installer_gui {
    public partial class K2Archive {

        public static void Init() {

            // Load the ZStandard native library
            var zstdDll = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, "libzstd.dll"));

            // Extract the libzstd.dll binary to our temp directory
            Util.ExtractResourceToFile("Binaries.libzstd.dll", zstdDll);

            // Load the libzstd.dll unmanaged library using P/Invoke
            var result = Kernel.LoadLibrary(zstdDll);
            if ( result == IntPtr.Zero ) {
                Logger.Fatal("Failed to load libzstd.dll!");
            } else {
                Logger.Info("Successfully loaded libzstd.dll!");
            }
        }

        public static void ExtractArchive(string sourceFile, string target) {
            sourceFile = Path.GetFullPath(sourceFile);
            target = Path.GetFullPath(target);

            if ( !Directory.Exists(Path.GetDirectoryName(sourceFile)) )
                throw new DirectoryNotFoundException();

            if ( !Directory.Exists(target) )
                throw new DirectoryNotFoundException();

            // Decompress the entire file block.
            var compressor = new Decompressor(new DecompressionOptions());
            var fileBytes = compressor.Unwrap(File.ReadAllBytes(sourceFile));
            compressor.Dispose();

            // Verify header
            // MagicNumber
            for ( int i = 0; i < 32; i++ ) {
                if ( fileBytes[i] != K2_ARCHIVE_MAGIC_NUMBER[i] )
                    throw new ZstdException(ZSTD_ErrorCode.ZSTD_error_corruption_detected, "Invalid file");
            }

            // Get header metadata
            int fileCount = BitConverter.ToInt32(fileBytes, 32);

            // Now we only have file blocks!
            int currentPos = 36;
            for ( int i = 0; i < fileCount; i++ ) {

                // @TODO: Decompress file blocks to byte* and write to disk
                string fileName = Encoding.UTF8.GetString(fileBytes, currentPos, 100).TrimEnd('\0');
                int fileSize = BitConverter.ToInt32(fileBytes, currentPos + 100);
                int blockSize = BitConverter.ToInt32(fileBytes, currentPos + 100 + 4);
                currentPos += 108;
                string fullPath = Path.GetFullPath(Path.Combine(target, fileName));

                if ( blockSize == 0 ) {
                    // Assume directory
                    if ( !Directory.Exists(fullPath) )
                        Directory.CreateDirectory(fullPath);
                } else {
                    // @TODO: Reimplement but not slow?
                    // string dir = Path.GetFullPath(Path.GetDirectoryName(fullPath));
                    // if ( !Directory.Exists(dir) )
                    //     Directory.CreateDirectory(dir);

                    using (FileStream fs = File.OpenWrite(fullPath)) {
                        fs.Write(fileBytes, currentPos - 1, fileSize);
                        fs.Flush();
                    }
                }
                currentPos += fileSize;
            }

        }

        /// <summary>
        /// Creates a new archive containing a list of files at the specified file.
        /// </summary>
        /// <param name="rootDirectory">The top-level directory. All files must be inside this directory or compression will fail.</param>
        /// <param name="sourceFiles">An array of file paths to where the files should be generated from.</param>
        /// <param name="target">Where to write the file</param>
        public static void CompressArchive(string rootDirectory, string[] sourceFiles, string target, int compressionLevel = DEFAULT_COMPRESSOR_LEVEL) {
            rootDirectory = Path.GetFullPath(rootDirectory);
            target = Path.GetFullPath(target);

            if ( !Directory.Exists(rootDirectory) )
                throw new DirectoryNotFoundException();

            if ( !Directory.Exists(Path.GetDirectoryName(target)) )
                throw new DirectoryNotFoundException();

            // @TODO: Optimize

            List<string> directoryItems = new List<string>(512);
            List<string> fileItems = new List<string>(sourceFiles.Length);

            // Validate all files, and fix them so that they are relative to our rootDirectory
            string fileBuffer = new string('\0', 128);
            foreach ( var file in sourceFiles ) {
                fileBuffer = Path.GetFullPath(file);
                if ( File.Exists(fileBuffer) ) {
                    fileItems.Add(fileBuffer);

                    // Directory
                    string directory = Path.GetFullPath(Path.GetDirectoryName(fileBuffer));
                    if ( directory != rootDirectory && !directoryItems.Contains(directory) )
                        directoryItems.Add(directory);
                }
            }

            using ( var memStream = new MemoryStream() ) {
                // Create header block

                // MagicNumber
                memStream.Write(K2_ARCHIVE_MAGIC_NUMBER, 0, K2_ARCHIVE_MAGIC_NUMBER.Length);
                memStream.Write(BitConverter.GetBytes(fileItems.Count + directoryItems.Count), 0, 4);

                // Add directory entries
                byte[] dirBytes = new byte[] { };
                string rootDirName = Path.GetFileName(rootDirectory);
                string relativePath;
                foreach ( var dir in directoryItems ) {

                    // Relative path
                    relativePath = new Uri(rootDirectory).MakeRelativeUri(new Uri(dir)).ToString().Replace("\\", "/").Substring(rootDirName.Length + 1);
                    byte[] compressedFile = CreateFileBlock(relativePath, ref dirBytes, compressionLevel);
                    memStream.Write(compressedFile, 0, compressedFile.Length);
                }

                // Cleanup
                directoryItems.Clear();
                directoryItems = null;

                // Compress each file
                foreach ( var file in fileItems ) {
                    // Relative path
                    relativePath = new Uri(rootDirectory).MakeRelativeUri(new Uri(file)).ToString().Replace("\\", "/").Substring(rootDirName.Length + 1);
                    byte[] fileBytes = File.ReadAllBytes(file);
                    byte[] compressedFile = CreateFileBlock(relativePath, ref fileBytes, compressionLevel);
                    memStream.Write(compressedFile, 0, compressedFile.Length);
                }

                // Cleanup
                fileItems.Clear();
                fileItems = null;

                // Compress the entire file block.
                using ( FileStream fs = File.Create(target) ) {

                    // Decompress the entire file block.
                    var compressor = new Compressor(new CompressionOptions(FILE_COMPRESSOR_LEVEL));
                    var finalCompressedFile = compressor.Wrap(memStream.ToArray());

                    // Write to file
                    fs.Write(finalCompressedFile, 0, finalCompressedFile.Length);
                    fs.Flush();
                    Logger.Info($"Wrote archive to {target}");
                }
            }
        }

        public static void CompressArchive(string directory, string target, int compressionLevel = DEFAULT_COMPRESSOR_LEVEL) {
            directory = Path.GetFullPath(directory);
            target = Path.GetFullPath(target);

            if ( !Directory.Exists(directory) )
                throw new DirectoryNotFoundException();

            if ( !Directory.Exists(Path.GetDirectoryName(target)) )
                throw new DirectoryNotFoundException();

            CompressArchive(directory, Directory.GetFiles(directory, "*", SearchOption.AllDirectories), target, compressionLevel);
        }
    }
}
