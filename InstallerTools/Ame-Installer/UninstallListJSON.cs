using Newtonsoft.Json;

namespace amethyst_installer_gui.Installer {
    public class UninstallListJSON {

        [JsonProperty("files")]
        public string[] Files { get; set; }
        [JsonProperty("directories")]
        public string[] Directories { get; set; }
    }
}