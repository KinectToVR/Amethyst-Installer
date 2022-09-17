using amethyst_installer_gui.Installer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InstallerTools.Commands {

    public class CommandGenUninstallList : ICommand {

        public string Command { get => "uninstalllist"; set { } }
        public string Description { get => "Generates a JSON file of uninstallable items; Saves to ./list.json"; set { } }
        public string[] Aliases { get => new string[] { "ul" }; set { } }

        public bool Execute(ref string[] parameters) {

            if ( parameters.Length == 0 ) {
                Console.Error.WriteLine("Invalid parameter count!");
                return true;
            }

            string directory = Path.GetFullPath(parameters[0]);
            string dirName = directory.Substring(directory.LastIndexOf("\\") + 1);

            if ( !Directory.Exists(directory) ) {
                Console.Error.WriteLine($"{directory} doesn't exist!");
                return true;
            }

            UninstallListJSON list = new UninstallListJSON();
            List<string> filesList = new List<string>();
            List<string> directoryList = new List<string>();

            foreach ( string file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories) ) {
                filesList.Add(new Uri(directory).MakeRelativeUri(new Uri(file)).ToString().Replace("/", "\\").Substring(dirName.Length + 1));
            }

            foreach ( string dir in Directory.GetDirectories(directory, "*", SearchOption.AllDirectories) ) {
                directoryList.Add(new Uri(directory).MakeRelativeUri(new Uri(dir)).ToString().Replace("/", "\\").Substring(dirName.Length + 1));
            }

            // Convert variable vectors to arrays
            list.Files = filesList.ToArray();
            list.Directories = directoryList.ToArray();

            // Sort by least nested
            Array.Sort(list.Files, (x, y) => x.Count(z => (z == '\\' || z == '/')).CompareTo(y.Count(z => ( z == '\\' || z == '/' ))));
            Array.Sort(list.Directories, (x, y) => x.Count(z => (z == '\\' || z == '/')).CompareTo(y.Count(z => ( z == '\\' || z == '/' ))));
            // Reverse so that they're sorted by most nested
            Array.Reverse(list.Files);
            Array.Reverse(list.Directories);

            // Serialize
            File.WriteAllText("list.json", JsonConvert.SerializeObject(list, Formatting.None));

            return true;
        }
    }
}

