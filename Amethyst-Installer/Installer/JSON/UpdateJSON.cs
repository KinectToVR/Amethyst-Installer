using Newtonsoft.Json;

namespace amethyst_installer_gui.Installer {
    public class UpdateJSON {
        [JsonProperty("version")]
        public int Version { get; set; }
        [JsonProperty("version_string")]
        public string VersionString { get; set; }
        [JsonProperty("changelog")]
        public string[] Changelog { get; set; }
    }
}