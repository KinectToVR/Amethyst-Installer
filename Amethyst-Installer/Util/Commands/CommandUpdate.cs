using amethyst_installer_gui.Installer;
using amethyst_installer_gui.Pages;
using Microsoft.Build.Framework.XamlTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AmethystModule = amethyst_installer_gui.Installer.Module;

namespace amethyst_installer_gui.Commands {
    public class CommandUpdate : ICommand {

        public string Command { get => "update"; set { } }
        public string Description { get => "Attempts to update Amethyst"; set { } }
        public string[] Aliases { get => new string[] { "u" }; set { } }

        private static Dictionary<string, UpdateJSON> UpdateEndpoint;

        public bool Execute(ref string[] args) {

            // @HACK: for determining the Amethyst path from the arguments
            // Amethyst-Installer.exe --update -o -path="C:\Program Files\Amethyst"

            string location = string.Empty;

            bool isExtractingPath = false;

            foreach ( var argument in args ) {
                if ( argument == "-o" ) {
                    // Open Amethyst flag
                    PageUpdating.OpenAmethystOnSuccess = true;
                }
                else if ( argument == "-path" ) {
                    isExtractingPath = true;
                    continue;
                }

                if ( isExtractingPath ) {
                    // Amethyst install directory
                    location = argument;
                }

                isExtractingPath = false;
            }

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

            if ( modulesToUpdate.Count == 0 ) {
                Util.ShowMessageBox("No applicable items had updates available! The updater will now close...", "Nothing to update");
                Util.Quit(ExitCodes.NoUpdates);
                return true;
            }

            // Now queue the modules to the updater
            QueueModules(modulesToUpdate);

            // Set Amethyst install directory
            if ( location.Length == 0 ) {
                location = InstallUtil.LocateAmethystInstall();
            }
            InstallerStateManager.AmethystInstallDirectory = location;

            InstallerStateManager.IsUpdating = true;
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

        private void QueueModules(List<string> modulesToUpdate) {

            InstallerStateManager.ModulesToInstall.Clear();
            List<AmethystModule> modulesPostBuffer = new List<AmethystModule>();

            for ( int i = 0; i < modulesToUpdate.Count; i++ ) {

                var module = InstallerStateManager.API_Response.Modules[InstallerStateManager.ModuleIdLUT[modulesToUpdate[i]]];

                // Go through dependencies
                for ( int j = 0; j < module.Depends.Count; j++ ) {

                    var thisModule = InstallerStateManager.API_Response.Modules[InstallerStateManager.ModuleIdLUT[module.Depends[j]]];

                    // For dependency in X
                    Logger.Info($"Queueing dependency \"{thisModule.DisplayName}\"...");
                    if ( !InstallerStateManager.ModulesToInstall.Contains(thisModule) ) {
                        InstallerStateManager.ModulesToInstall.Add(thisModule);
                    }
                }

                Logger.Info($"Queueing module \"{module.DisplayName}\"...");
                modulesPostBuffer.Add(module);
            }

            // Merge the dependencies and modules lists together, so that dependencies are earlier than modules.
            // This should resolve dependency chain issues where a module installs out of order
            for ( int i = 0; i < InstallerStateManager.ModulesToInstall.Count; i++ ) {
                for ( int j = 0; j < modulesPostBuffer.Count; j++ ) {
                    var deps = InstallerStateManager.ModulesToInstall[i];
                    var mod = modulesPostBuffer[j];

                    if ( deps.Id == mod.Id ) {
                        InstallerStateManager.ModulesToInstall.RemoveAt(i);
                        i--;
                    }
                }
            }

            // Add the list of modules which depend on other modules to the back of the modules to install vector
            InstallerStateManager.ModulesToInstall.AddRange(modulesPostBuffer);
        }
    }
}
