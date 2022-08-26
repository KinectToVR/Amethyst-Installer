using InstallerTools.Ame_Installer;
using System;
using System.IO;

namespace InstallerTools.Commands {

    public class CommandChecksum : ICommand {

        public string Command { get => "uninstall"; set { } }
        public string Description { get => "Starts the uninstall workflow"; set { } }
        public string[] Aliases { get => new string[] { "x" }; set { } }

        public bool Execute(ref string[] parameters) {

            if ( parameters.Length == 0 ) {
                Console.Error.WriteLine("Invalid parameter count!");
                return true;
            }

            string filePath = Path.GetFullPath(string.Join(" ", parameters));

            var checksum = AmeUtil.GetChecksum(filePath);
            Console.WriteLine(checksum);

            return true;
        }
    }
}
