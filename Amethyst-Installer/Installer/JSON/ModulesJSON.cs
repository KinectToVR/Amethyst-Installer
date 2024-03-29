﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace amethyst_installer_gui.Installer {

    public class Module {
        [JsonProperty("id")]
        public string Id;

        /// <summary>
        /// The version string shown to the user
        /// </summary>
        [JsonProperty("display_version")]
        public string DisplayVersion;

        /// <summary>
        /// The version ID used internally, basically ends up boiling down to a build number
        /// </summary>
        [JsonProperty("version")]
        public int InternalVersion;

        [JsonProperty("filesize")]
        public int FileSize;

        [JsonProperty("downloadsize")]
        public int DownloadSize;

        [JsonProperty("required")]
        public bool Required;

        [JsonProperty("visible")]
        public bool Visible;

        [JsonProperty("critical")]
        public bool IsCritical;

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

        [JsonProperty("version")]
        public string Version;

        [JsonProperty("variable")]
        public string Variable;
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

        [JsonProperty("format")]
        public string Format;
    }

    public class ModuleRemoteConfig {
        [JsonProperty("main_url")]
        public string MainUrl;

        [JsonProperty("mirror_url")]
        public string MirrorUrl;

        [JsonProperty("filename")]
        public string Filename;

        [JsonProperty("checksum")]
        public string Checksum;
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
