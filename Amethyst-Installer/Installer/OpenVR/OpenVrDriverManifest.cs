using Newtonsoft.Json;
using System.Collections.Generic;

namespace amethyst_installer_gui.Installer {
    public class OpenVrDriverManifest {
        [JsonProperty("alwaysActivate")]
        public bool AlwaysActivate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("directory")]
        public string Directory { get; set; }

        [JsonProperty("resourceOnly")]
        public bool ResourceOnly { get; set; }

        [JsonProperty("hmd_presence")]
        public List<string> HmdPresence { get; set; }
    }


}
