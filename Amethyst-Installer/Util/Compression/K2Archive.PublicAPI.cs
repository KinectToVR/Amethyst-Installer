using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdNet;

namespace amethyst_installer_gui {
    public partial class K2Archive {
        public static void ExtractArchive(string sourceFile, string target) {
            sourceFile = Path.GetFullPath(sourceFile);
            target = Path.GetFullPath(target);

            if ( !Directory.Exists(Path.GetDirectoryName(sourceFile)) )
                throw new DirectoryNotFoundException();

            if ( !Directory.Exists(target) )
                throw new DirectoryNotFoundException();

            // byte[] finalCompressedBytes = File.ReadAllBytes(sourceFile);

            // Decompress the entire file block.
            var compressor = new Decompressor(new DecompressionOptions());
            var fileBytes = compressor.Unwrap(File.ReadAllBytes(sourceFile));

            // Tell the GC that it can clean this up
            // finalCompressedBytes = null;

            // Verify header
            // MagicNumber
            for ( int i = 0; i < 32; i++ ) {
                if ( fileBytes[i] != K2_ARCHIVE_MAGIC_NUMBER[i] )
                    throw new ZstdException(ZSTD_ErrorCode.ZSTD_error_corruption_detected, "Invalid file");
            }

            // Get header metadata
            int compressionLevel = BitConverter.ToInt32(fileBytes, 32);
            int fileCount = BitConverter.ToInt32(fileBytes, 36);

            // Now we only have file blocks!
            for ( int i = 0; i < fileCount; i++ ) {

                // @TODO: Decompress file blocks to byte* and write to disk
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

            List<byte> archiveBytes = new List<byte>(40);

            // Create header block

            // MagicNumber
            for ( int i = 0; i < 32; i++ ) {
                archiveBytes.Add(K2_ARCHIVE_MAGIC_NUMBER[i]);
            }
            archiveBytes.AddRange(BitConverter.GetBytes(compressionLevel));                         // Compression Level
            archiveBytes.AddRange(BitConverter.GetBytes(fileItems.Count + directoryItems.Count));   // FileCount


            // Add directory entries
            byte[] dirBytes = new byte[] { };
            string rootDirName = Path.GetFileName(rootDirectory);
            foreach ( var dir in directoryItems ) {

                // Relative path
                // string relativePath = new Uri(rootDirectory).MakeRelativeUri(new Uri(dir)).ToString();
                string relativePath = new Uri(rootDirectory).MakeRelativeUri(new Uri(dir)).ToString().Replace("\\", "/").Substring(rootDirName.Length + 1);
                byte[] compressedFile = CreateFileBlock(relativePath, ref dirBytes, compressionLevel);
                archiveBytes.AddRange(compressedFile);
            }

            // Compress each file
            foreach ( var file in fileItems ) {
                // Relative path
                // var fileUri = new Uri(rootDirectory).MakeRelativeUri(new Uri(file));
                string relativePath = new Uri(rootDirectory).MakeRelativeUri(new Uri(file)).ToString().Replace("\\", "/").Substring(rootDirName.Length + 1);
                byte[] fileBytes = File.ReadAllBytes(file);
                byte[] compressedFile = CreateFileBlock(relativePath, ref fileBytes, compressionLevel);
                archiveBytes.AddRange(compressedFile);
            }

            // Decompress the entire file block.
            var compressor = new Compressor(new CompressionOptions(FILE_COMPRESSOR_LEVEL));
            var finalCompressedFile = compressor.Wrap(archiveBytes.ToArray());

            // Write to file
            File.WriteAllBytes(target, finalCompressedFile);
            Logger.Info($"Wrote archive to {target}");
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
