namespace amethyst_installer_gui.Protocol {
    public interface IProtocolCommand {

        /// <summary>
        /// The command part of this command. This is case insensitive.
        /// </summary>
        string Command { get; set; }
        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="parameters">Additional parameters for this command</param>
        /// <returns>Whether regular execution of the program shall be interrupted.</returns>
        bool Execute(string parameters);
    }
}
