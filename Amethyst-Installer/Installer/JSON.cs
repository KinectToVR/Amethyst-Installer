using Newtonsoft.Json;
using System.Collections.Generic;

namespace amethyst_installer_gui.Installer {

    public class Module {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("display_name")]
        public string DisplayName;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("filesize")]
        public int Filesize;

        [JsonProperty("downloadsize")]
        public int Downloadsize;

        [JsonProperty("required")]
        public bool Required;

        [JsonProperty("visible")]
        public bool Visible;

        [JsonProperty("depends")]
        public List<string> Depends;

        [JsonProperty("remote")]
        public ModuleRemoteConfig Remote;

        [JsonProperty("install")]
        public ModuleInstallConfig Install;

        [JsonProperty("detect")]
        public ModuleDetectConfig Detect;

        [JsonProperty("uninstall")]
        public ModuleUninstallConfig Uninstall;
    }
    public class ModuleDetectConfig {
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("removes")]
        public string Removes;
    }

    public class ModuleInstallConfig {
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("items")]
        public List<object> Items;

        [JsonProperty("showEula")]
        public bool? ShowEula;

        [JsonProperty("post")]
        public string Post;
    }

    public class ModuleRemoteConfig {
        [JsonProperty("main_url")]
        public string MainUrl;

        [JsonProperty("mirror_url")]
        public string MirrorUrl;

        [JsonProperty("filename")]
        public string Filename;
    }
    public class ModuleUninstallConfig {
        [JsonProperty("type")]
        public string Type;
    }

    public class AmeInstallApiResponse {
        [JsonProperty("modules")]
        public List<Module> Modules;

        [JsonProperty("splashes")]
        public List<string> Splashes;
    }
}
