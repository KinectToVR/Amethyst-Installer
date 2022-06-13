namespace amethyst_installer_gui.Installer.Modules {
    public abstract class ModuleBase {
        /// <summary>
        /// A means of installing a module
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Install(string path) { return false; }

        /// <summary>
        /// JSON deserialized <see cref="Module"/> instance.
        /// </summary>
        Module Module { get; }

        /// <summary>
        /// Whether the current Module has any dependencies
        /// </summary>
        bool HasDependencies {
            get {
                return (Module?.Depends?.Count ?? 0) > 0;
            }
        }

        /// <summary>
        /// Whether the current module is installed
        /// </summary>
        bool IsInstalled { get; }
    }
}
