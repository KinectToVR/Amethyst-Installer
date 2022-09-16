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
            
            // var compressor = new Compressor(new CompressionOptions(5));
            // var compressedData = compressor.Wrap(sourceData);

        }

        /// <summary>
        /// Creates a new archive containing a list of files at the specified file.
        /// </summary>
        /// <param name="rootDirectory">The top-level directory. All files must be inside this directory or compression will fail.</param>
        /// <param name="sourceFiles">An array of file paths to where the files should be generated from.</param>
        /// <param name="target">Where to write the file</param>
        public static void CompressArchive(string rootDirectory, string[] sourceFiles, string target, int compressionLevel = 5) {
            rootDirectory = Path.GetFullPath(rootDirectory);
            target = Path.GetFullPath(target);

            if ( !Directory.Exists(rootDirectory) )
                throw new DirectoryNotFoundException();

            if ( !Directory.Exists(Path.GetDirectoryName(target)) )
                throw new DirectoryNotFoundException();

            List<string> directoryItems = new List<string>(512);
            List<string> fileItems = new List<string>(sourceFiles.Length);

            // Validate all files, and fix them so that they are relative to our rootDirectory
            foreach ( var file in sourceFiles ) {
                if ( File.Exists(file) ) {
                    fileItems.Add(file);

                    // Directory
                    string directory = Path.GetDirectoryName(file);
                    if ( !directoryItems.Contains(directory) )
                        directoryItems.Add(directory);
                }
            }

            List<byte> archiveBytes = new List<byte>(40);

            // Create header block

            // MagicNumber
            for ( int i = 0; i < 32; i++ ) {
                archiveBytes.Add(K2_ARCHIVE_MAGIC_NUMBER[i]);
            }
            archiveBytes.AddRange(BitConverter.GetBytes(compressionLevel));     // Compression Level
            archiveBytes.AddRange(BitConverter.GetBytes(fileItems.Count));      // FileCount


            // Add directory entries
            byte[] dirBytes = new byte[] { };
            foreach ( var dir in directoryItems ) {
                // Relative path
                string relativePath = new Uri(rootDirectory).MakeRelativeUri(new Uri(dir)).ToString().Replace("/", "\\").Substring(rootDirectory.Length + 1);
                byte[] compressedFile = CreateFileBlock(relativePath, ref dirBytes, compressionLevel);
                archiveBytes.AddRange(compressedFile);
            }

            // Compress each file
            foreach ( var file in fileItems ) {
                byte[] fileBytes = File.ReadAllBytes(file);
                // Relative path
                string relativePath = new Uri(rootDirectory).MakeRelativeUri(new Uri(file)).ToString().Replace("/", "\\").Substring(rootDirectory.Length + 1);
                byte[] compressedFile = CreateFileBlock(relativePath, ref fileBytes, compressionLevel);
                archiveBytes.AddRange(compressedFile);
            }

            // Decompress the entire file block.
            var compressor = new Compressor(new CompressionOptions(6));
            var finalCompressedFile = compressor.Wrap(archiveBytes.ToArray());

            // Write to file
            File.WriteAllBytes(target, finalCompressedFile);
            Logger.Info($"Wrote archive to {target}");
        }

        public static void CompressArchive(string directory, string target, int compressionLevel = 5) {
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
