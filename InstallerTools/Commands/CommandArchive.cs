﻿using System;
using System.IO;
using amethyst_installer_gui;

namespace InstallerTools.Commands {
    public class CommandArchive : ICommand {
        public string Command { get => "archive"; set { } }
        public string Description { get => "Archives a directory using the k2a archive format"; set { } }
        public string[] Aliases { get => new string[] { "a" }; set { } }

        public bool Execute(ref string[] parameters) {

            if ( parameters.Length == 0 ) {
                Console.Error.WriteLine("Invalid parameter count!");
                return true;
            }

            string filePath = string.Empty;
            string directoryPath = string.Empty;

            for ( int i = 0; i < parameters.Length; i++ ) {
                switch ( parameters[i] ) {
                    case "-dir":
                    case "-d":
                        directoryPath = parameters[i + 1];
                        i++;
                        break;
                    case "-out":
                    case "-o":
                        filePath = parameters[i + 1];
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

            K2Archive.CompressArchive(directoryPath, filePath, 5);

            return true;
        }
    }
}
