using amethyst_installer_gui.Controls;

namespace amethyst_installer_gui.Installer.Modules {
    public abstract class ModuleBase {
        /// <summary>
        /// A means of installing a module
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract bool Install(string sourceFile, string path, ref InstallModuleProgress installModuleProgress, out TaskState state);

        /// <summary>
        /// JSON deserialized <see cref="Module"/> instance.
        /// </summary>
        public Module Module;

        /// <summary>
        /// Whether the current Module has any dependencies
        /// </summary>
        public bool HasDependencies {
            get {
                return ( Module?.Depends?.Count ?? 0 ) > 0;
            }
        }

        /// <summary>
        /// Whether the current module is installed
        /// </summary>
        public bool IsInstalled { get; protected set; }
    }

    public struct ModuleDisplayStrings {
        public string Title;
        public string Summary;
        public string Description;
    }
}
