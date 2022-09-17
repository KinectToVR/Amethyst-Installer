using InstallerTools.Ame_Installer;
using System;
using System.IO;

namespace InstallerTools.Commands {

    public class CommandChecksum : ICommand {

        public string Command { get => "checksum"; set { } }
        public string Description { get => "Computes the MD5 checksum of the given file"; set { } }
        public string[] Aliases { get => new string[] { "c" }; set { } }

        public bool Execute(ref string[] parameters) {

            if ( parameters.Length == 0 ) {
                Console.Error.WriteLine("Invalid parameter count!");
                return true;
            }

            string filePath = Path.GetFullPath(parameters[0]);

            var checksum = AmeUtil.GetChecksum(filePath);
            Console.WriteLine(checksum);

            return true;
        }
    }
}
