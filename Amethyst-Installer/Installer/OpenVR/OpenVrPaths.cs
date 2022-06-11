using System.Collections.Generic;

namespace amethyst_installer_gui.Installer {
    public class OpenVrPaths {
        public List<string> config { get; set; }
        public List<string> external_drivers { get; set; }
        public string jsonid { get; set; }
        public List<string> log { get; set; }
        public List<string> runtime { get; set; }
        public int version { get; set; }
    }
}
