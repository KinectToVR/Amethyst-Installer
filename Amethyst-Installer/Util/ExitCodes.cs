namespace amethyst_installer_gui {
    public enum ExitCodes : int {

        /// <summary>
        /// The installer didn't encounter any errors
        /// </summary>
        OK                          =  0,
        /// <summary>
        /// The user's setup was deemed incompatible
        /// </summary>
        IncompatibleSetup           = -1,
        /// <summary>
        /// The user encountered an unknown exception which interrupted the install process and prevented it from executing properly.
        /// </summary>
        ExceptionUserClosed         = -2,
        /// <summary>
        /// The user encountered an unknown exception before the Main Window was shown
        /// </summary>
        ExceptionPreInit            = -3,

    }
}
