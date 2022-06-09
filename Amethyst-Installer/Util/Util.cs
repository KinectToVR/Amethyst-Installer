using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui
{
    public static class Util
    {
        /// <summary>
        /// Returns the version number of Amethyst Installer
        /// </summary>
        public static string InstallerVersionString
        {
            get { 
                string verison = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return "Amethyst Installer v" + verison.Remove(verison.Length - 2);
            }
        }
    }
}
