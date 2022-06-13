using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Class that handles the installer's global state.
    /// UI updates this class' state, then this class calls all the actual logic.
    /// </summary>
    public static partial class InstallerStateManager {
        public static bool CanInstall = false;


        // TODO: Detect if using Shadow, prevent an install because Ame doesn't support networked environments
        public static bool IsSystemSupported = true;

        public static AmeInstallApiResponse API_Response { get; private set; }

        public static void Initialize() {

            // Fetch JSON Response
            var txtResponse = File.ReadAllText(Path.GetFullPath("ame-installer-sample-api-response.json"));
            API_Response = JsonConvert.DeserializeObject<AmeInstallApiResponse>(txtResponse);
        }

    }
}
