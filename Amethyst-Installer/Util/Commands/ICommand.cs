namespace amethyst_installer_gui.Commands {
    /// <summary>
    /// A generic command the installer supports
    /// </summary>
    public interface ICommand {

        /// <summary>
        /// The full command.
        /// </summary>
        string Command { get; set; }

        /// <summary>
        /// A string describing the usage of this command.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// A collection of aliases or shorthands which can be used in place of <see cref="Command"/>.
        /// </summary>
        string[] Aliases { get; set; }

        /// <summary>
        /// Executes this command
        /// </summary>
        bool Execute(ref string[] parameters);
    }
}
