using System;
using System.IO;
using amethyst_installer_gui;

namespace InstallerTools.Commands {
    public class CommandExtract : ICommand {
        public string Command { get => "extract"; set { } }
        public string Description { get => "Extracts an archive using the k2a archive format to a directory"; set { } }
        public string[] Aliases { get => new string[] { "x" }; set { } }

        public bool Execute(ref string[] parameters) {

            if ( parameters.Length == 0 ) {
                Console.Error.WriteLine("Invalid parameter count!");
                return true;
            }

            string filePath = string.Empty;
            string directoryPath = string.Empty;

            for ( int i = 0; i < parameters.Length; i++ ) {
                switch ( parameters[i] ) {
                    case "-file":
                    case "-f":
                        filePath = parameters[i + 1];
                        i++;
                        break;
                    case "-out":
                    case "-o":
                        directoryPath = parameters[i + 1];
                        i++;
                        break;
                }
            }

            if ( filePath.Length == 0 )
                throw new ArgumentException("Invalid file path!");
            if ( directoryPath.Length == 0 )
                throw new ArgumentException("Invalid directory path!");

            filePath = Path.GetFullPath(filePath);
            directoryPath = Path.GetFullPath(directoryPath);

            if ( !Directory.Exists(directoryPath) )
                throw new ArgumentException("Invalid directory path!");

            if ( !File.Exists(filePath) )
                throw new ArgumentException("File doesn't exist!");

            K2Archive.ExtractArchive(filePath, directoryPath);

            return true;
        }
    }
}
