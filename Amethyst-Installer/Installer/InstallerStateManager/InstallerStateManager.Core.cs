using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using amethyst_installer_gui.Installer.Modules;
using Newtonsoft.Json;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Class that handles the installer's global state.
    /// UI updates this class' state, then this class calls all the actual logic.
    /// </summary>
    public static partial class InstallerStateManager {

        public static AmeInstallApiResponse API_Response { get; private set; }

        public static List<Module> ModulesToInstall;

        public static Dictionary<string, ModuleBase> ModuleTypes { get; private set; }

        public static string AmethystInstallDirectory;

        /// <summary>
        /// Whether the current process is an upgrade, as some installation steps are handled differently during an upgrade
        /// </summary>
        public static bool IsUpgrading = false;

        /// <summary>
        /// Whether to create a start menu entry or not
        /// </summary>
        public static bool CreateStartMenuEntry = false;

        /// <summary>
        /// Whether to create a desktop shortcut or not
        /// </summary>
        public static bool CreateDesktopShortcut = false;

        public static void Initialize() {

            // Fetch JSON Response, and load it
            // var txtResponse = File.ReadAllText(Path.GetFullPath("ame-installer-sample-api-response.json"));

            string txtResponse = string.Empty;
            using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("amethyst_installer_gui.ame-installer-sample-api-response.json") ) {
                using ( StreamReader reader = new StreamReader(resource) ) {
                    txtResponse = reader.ReadToEnd();
                }
            }

            API_Response = JsonConvert.DeserializeObject<AmeInstallApiResponse>(txtResponse);
            ModulesToInstall = new List<Module>();

            // Create internal LUT for modules
            ModuleTypes = new Dictionary<string, ModuleBase>();

            ModuleTypes.Add("amethyst", new AmethystModule());
            ModuleTypes.Add("exe", new ExeModule());

            foreach (var module in API_Response.Modules ) {
                if (ModuleTypes.ContainsKey(module.Install.Type) ) {
                    ModuleTypes[module.Install.Type].Module = module;
                } else {
                    Logger.Warn($"Unknown install type {module.Install.Type} on module {module.DisplayName}");
                }
            }

            ComputeRequirements();
        }


        // TODO: System to allow selecting modules to install / update, etc
    }
}
