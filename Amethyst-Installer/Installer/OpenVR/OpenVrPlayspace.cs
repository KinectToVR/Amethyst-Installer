using Newtonsoft.Json;
using System.Collections.Generic;

namespace amethyst_installer_gui.Installer {
    public class OpenVrPlayspace {
        [JsonProperty("jsonid")]
        public string Jsonid { get; set; }

        [JsonProperty("universes")]
        public List<Universe> Universes { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }

    public class Seated {
        [JsonProperty("translation")]
        public List<double> Translation { get; set; }

        [JsonProperty("yaw")]
        public double Yaw { get; set; }
    }

    public class Standing {
        [JsonProperty("translation")]
        public List<double> Translation { get; set; }

        [JsonProperty("yaw")]
        public double Yaw { get; set; }
    }

    public class Universe {
        [JsonProperty("collision_bounds")]
        public List<List<List<double>>> CollisionBounds { get; set; }

        [JsonProperty("play_area")]
        public List<double> PlayArea { get; set; }

        [JsonProperty("standing")]
        public Standing Standing { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("universeID")]
        public string UniverseID { get; set; }

        [JsonProperty("seated")]
        public Seated Seated { get; set; }

    }
}
