using amethyst_installer_gui.Installer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace amethyst_installer_gui.Commands {
    public class CommandUpdate : ICommand {

        public string Command { get => "update"; set { } }
        public string Description { get => "Attempts to update Amethyst"; set { } }
        public string[] Aliases { get => new string[] { "u" }; set { } }

        private static Dictionary<string, UpdateJSON> UpdateEndpoint;

        public bool Execute(string parameters) {

            App.Init();

            // Try fetching the updates endpoint
            try {
                FetchUpdates();
            } catch (Exception e) {
                Logger.Fatal("An exception was thrown while trying to fetch the update list!");
                Logger.Fatal(Util.FormatException(e));
                Util.ShowMessageBox(Localisation.Updating_FailedReachApi, Localisation.Updating_InitFailed);
                return true;
            }

            // Load the installer config
            // @TODO: Amogus
            string installerConfigPath = Path.GetFullPath(Path.Combine(Constants.AmethystConfigDirectory, "Modules.json"));
            Dictionary<string, int> config;
            if ( !File.Exists(installerConfigPath) ) {
                // Assume only amethyst
                config = new Dictionary<string, int>();
                config.Add("amethyst", 0);
            } else {

                // The file exists! Try reading it
                try {

                    string fileContents = File.ReadAllText(installerConfigPath);
                    config = JsonConvert.DeserializeObject<Dictionary<string, int>>(fileContents);
                        
                } catch (Exception e) {
                    Logger.Fatal($"Failed to load or parse file \"{installerConfigPath}\"!");
                    Logger.Fatal(Util.FormatException(e));

                    // Assume file is corrupt
                    config = new Dictionary<string, int>();
                    config.Add("amethyst", 0);
                }
            }

            // We have a valid config!
            List<string> modulesToUpdate = new List<string>();

            // Go through each key, mark it as "requires update"
            foreach ( var x in UpdateEndpoint ) {
                Logger.Info($"{x.Key} :: {x.Value.Version} ; {x.Value.VersionString}");
                if ( config.ContainsKey(x.Key) ) {
                    if ( config[x.Key] < x.Value.Version ) {
                        modulesToUpdate.Add(x.Key);
                    }
                }
            }

            // Now queue the modules to the updater
            // @TODO: Queue modules to updater

            App.InitialPage = InstallerState.Updating;

            return false;
        }


        private static void FetchUpdates() {

#if !DIST
            string txtResponse = string.Empty;
            using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("amethyst_installer_gui.ame-installer-sample-api-response-update.json") ) {
                using ( StreamReader reader = new StreamReader(resource) ) {
                    txtResponse = reader.ReadToEnd();
                }
            }

            UpdateEndpoint = JsonConvert.DeserializeObject<Dictionary<string, UpdateJSON>>(txtResponse);
#else
            try {
                Logger.Info("Checking latest versions...");
                var packagesJson = Download.GetStringAsync(Constants.ApiDomain + "update");

                UpdateEndpoint = JsonConvert.DeserializeObject<Dictionary<string, UpdateJSON>>(packagesJson);
                Logger.Info("Fetched latest versions successfully!");
                Logger.Info("Checking for updates...");
                Logger.Info("Determined update list!");

            } catch ( Exception e ) {
                Logger.Fatal("Failed to fetch update list!");
                Logger.Fatal(Util.FormatException(e));

                // @TODO: Explode?
                throw new Exception();
            }
#endif
        }
    }
}
