﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer.Modules {
    public class CheckEnvVar : CheckBase {
        public override bool CheckShouldInstall(in Module module) {



            return true;
        }
    }
}
