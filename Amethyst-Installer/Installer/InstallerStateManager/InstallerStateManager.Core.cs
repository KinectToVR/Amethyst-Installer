using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using amethyst_installer_gui.Installer.Modules;
using amethyst_installer_gui.Installer.Modules.Checks;
using amethyst_installer_gui.PInvoke;
using Newtonsoft.Json;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Class that handles the installer's global state.
    /// UI updates this class' state, then this class calls all the actual logic.
    /// </summary>
    public static partial class InstallerStateManager {

        public static AmeInstallApiResponse API_Response { get; private set; }
        public static string[] AmeDocsLocaleList { get; private set; }

        public static List<Module> ModulesToInstall;

        public static Dictionary<string, ModuleBase> ModuleTypes { get; private set; }
        /// <summary>
        /// A mapping from string identifiers to an index, used to efficiently lookup an index given an id
        /// </summary>
        public static Dictionary<string, int> ModuleIdLUT { get; private set; }
        public static Dictionary<string, PostBase> ModulePostOps { get; private set; }
        public static Dictionary<string, CheckBase> ModuleCheckOps { get; private set; }
        public static Dictionary<string, ModuleDisplayStrings> ModuleStrings { get; private set; }

        public static string AmethystInstallDirectory;
        public static string AmethystOpenVrDirectory {
            get {
                return Path.Combine(AmethystInstallDirectory ?? "C:\\", "Plugins", "plugin_OpenVR");
            }
        }

        /// <summary>
        /// Whether the installer is in the middle of the install process. This prevents the user from closing the installer mid-download or install.
        /// </summary>
        public static bool CanClose = true;

        /// <summary>
        /// Whether the current process is an upgrade, as some installation steps are handled differently during an upgrade.
        /// An upgrade is defined as a user upgrading from K2EX to Amethyst.
        /// </summary>
        public static bool IsUpgrading = false;

        /// <summary>
        /// Whether the current process is an update, as some installation steps are handled differently during an update
        /// </summary>
        public static bool IsUpdating = false;

        /// <summary>
        /// Whether to create a start menu entry or not
        /// </summary>
        public static bool CreateStartMenuEntry = false;

        /// <summary>
        /// Whether to create a desktop shortcut or not
        /// </summary>
        public static bool CreateDesktopShortcut = false;

        /// <summary>
        /// Whether we must de-elevate processes or not
        /// </summary>
        public static bool MustDelevateProcesses = false;

        /// <summary>
        /// Whether Amethyst should default to OSC or SteamVR
        /// </summary>
        public static bool DefaultToOSC = false;

        /// <summary>
        /// Whether to forcefully attempt fixing E_NUI_NOTREADY. Usually good for forcing a manual install of the drivers
        /// </summary>
        public static bool ForceFixNotReady = false;

        /// <summary>
        /// Whether to automatically start Amethyst with SteanVR
        /// </summary>
        public static bool AutoStartAmethystWithSteamVr = false;

        public static void Initialize() {

            FetchInstallerJson();
            FetchLocaleCodes();
            ModulesToInstall = new List<Module>();

            // Create internal LUT for modules
            ModuleTypes     = new Dictionary<string, ModuleBase>();
            ModuleIdLUT     = new Dictionary<string, int>();
            ModulePostOps   = new Dictionary<string, PostBase>();
            ModuleCheckOps  = new Dictionary<string, CheckBase>();
            ModuleStrings   = new Dictionary<string, ModuleDisplayStrings>();

            ModuleTypes.Add("amethyst", new AmethystModule());
            ModuleTypes.Add("exe", new ExeModule());
            ModuleTypes.Add("dark-extract", new DarkModule());
            ModuleTypes.Add("temp-archive", new TempArchiveModule());
            ModuleTypes.Add("psmsex", new PsmsModule());
            ModuleTypes.Add("psmsex-vdm", new PsmsVdmModule());
            ModuleTypes.Add("psmsex-drivers", new PsmsDriversModule());

            ModulePostOps.Add("kinectv1", new PostKinectV1());
            ModulePostOps.Add("kinectv2", new PostKinectV2());

            ModuleCheckOps.Add("vcredist", new CheckVcredist());

            // Module strings
            ModuleStrings.Add("amethyst", new ModuleDisplayStrings() {
                Title       = Localisation.Manager.AmethystModule_Amethyst_Title,
                Summary     = Localisation.Manager.AmethystModule_Amethyst_Summary,
                Description = Localisation.Manager.AmethystModule_Amethyst_Description,
            });
            ModuleStrings.Add("vcredist2022", new ModuleDisplayStrings() {
                Title       = Localisation.Manager.AmethystModule_VCRedist_Title,
                Summary     = Localisation.Manager.AmethystModule_VCRedist_Summary,
                Description = Localisation.Manager.AmethystModule_VCRedist_Description,
            });
            ModuleStrings.Add("wix", new ModuleDisplayStrings() {
                Title       = Localisation.Manager.KinectModule_WIX_Title,
                Summary     = Localisation.Manager.KinectModule_WIX_Summary,
                Description = Localisation.Manager.KinectModule_WIX_Description,
            });
            ModuleStrings.Add("kinect-v1-sdk", new ModuleDisplayStrings() {
                Title       = Localisation.Manager.KinectModule_KinectV1_Title,
                Summary     = Localisation.Manager.KinectModule_KinectV1_Summary,
                Description = Localisation.Manager.KinectModule_KinectV1_Description,
            });
            ModuleStrings.Add("kinect-v1-toolkit", new ModuleDisplayStrings() {
                Title       = Localisation.Manager.KinectModule_KinectV1Toolkit_Title,
                Summary     = Localisation.Manager.KinectModule_KinectV1Toolkit_Summary,
                Description = Localisation.Manager.KinectModule_KinectV1Toolkit_Description,
            });
            ModuleStrings.Add("psmsex", new ModuleDisplayStrings() {
                Title       = Localisation.Manager.PsmoveModule_PSMS_Title,
                Summary     = Localisation.Manager.PsmoveModule_PSMS_Summary,
                Description = Localisation.Manager.PsmoveModule_PSMS_Description,
            });
            ModuleStrings.Add("psmsex-vdm", new ModuleDisplayStrings() {
                Title       = Localisation.Manager.PsmoveModule_PSMS_VDM_Title,
                Summary     = Localisation.Manager.PsmoveModule_PSMS_VDM_Summary,
                Description = Localisation.Manager.PsmoveModule_PSMS_VDM_Description,
            });
            ModuleStrings.Add("psmsex-drivers", new ModuleDisplayStrings() {
                Title       = Localisation.Manager.PsmoveModule_PSMS_Drivers_Title,
                Summary     = Localisation.Manager.PsmoveModule_PSMS_Drivers_Summary,
                Description = Localisation.Manager.PsmoveModule_PSMS_Drivers_Description,
            });

            // Parse modules
            for ( int i = 0; i < API_Response.Modules.Count; i++ ) {
                var module = API_Response.Modules[i];

                // Assign executable module to ModuleTypes
                if ( ModuleTypes.ContainsKey(module.Install.Type) ) {
                    ModuleTypes[module.Install.Type].Module = module;
                } else {
                    Logger.Warn($"Unknown install type {module.Install.Type} on module {module.Id}");
                }

                ModuleIdLUT.Add(module.Id, i);
            }

            ComputeRequirements();

            string currentUser = WindowsIdentity.GetCurrent().Name.Substring(Environment.MachineName.Length + 1);
            MustDelevateProcesses = CurrentUser.GetCurrentlyLoggedInUsername() != currentUser;

            Logger.Info($"Must de-elevate: {MustDelevateProcesses}");
        }

        private static void FetchInstallerJson() {

            Logger.Info("Fetching latest packages...");
#if !DIST
            string txtResponse = string.Empty;
            using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("amethyst_installer_gui.ame-installer-sample-api-response.json") ) {
                using ( StreamReader reader = new StreamReader(resource) ) {
                    txtResponse = reader.ReadToEnd();
                }
            }

            API_Response = JsonConvert.DeserializeObject<AmeInstallApiResponse>(txtResponse);
            Logger.Info("Loaded embedded package list successfully!");
#else
            try {

                var packagesJson = Download.GetStringAsync(Constants.ApiDomain + "installer/en/packages");

                API_Response = JsonConvert.DeserializeObject<AmeInstallApiResponse>(packagesJson);
                Logger.Info("Fetched packages successfully!");

            } catch ( Exception e ) {
                Logger.Fatal("Failed to fetch packages!");
                Logger.Fatal(Util.FormatException(e));

                string txtResponse = string.Empty;
                using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("amethyst_installer_gui.ame-installer-sample-api-response.json") ) {
                    using ( StreamReader reader = new StreamReader(resource) ) {
                        txtResponse = reader.ReadToEnd();
                    }
                }

                API_Response = JsonConvert.DeserializeObject<AmeInstallApiResponse>(txtResponse);
            }
#endif
        }

        private static void FetchLocaleCodes() {
            try {

                Logger.Info("Fetching supported documentation locales...");
                var localesJson = Download.GetStringAsync(Constants.DocsLocalesEndpoint);

                var ameDocsLocaleResponseParsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(localesJson);
                AmeDocsLocaleList = ameDocsLocaleResponseParsed.Keys.ToArray();
                Logger.Info("Fetched locale codes successfully!");

            } catch (Exception e) {
                Logger.Fatal("Failed to fetch locale codes!");
                Logger.Fatal(Util.FormatException(e));

                // @TODO: Update this dynamically later once we have a few languages implemented in the docs
                AmeDocsLocaleList = new string[] { "en", "es" };
            }
        }

        // @TODO: System to allow selecting modules to install / update, etc
        public static bool ShouldInstallModule(in Module currentModule) {

            if (currentModule.Detect != null && currentModule.Detect.Type != null) {
                if (ModuleCheckOps.ContainsKey(currentModule.Detect.Type)) {
                    return ModuleCheckOps[currentModule.Detect.Type].CheckShouldInstall(currentModule);
                }
            }

            return true;
        }
    }
}
