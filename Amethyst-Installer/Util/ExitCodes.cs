namespace amethyst_installer_gui {
    public enum ExitCodes : int {


        // =====================================================
        //                           OK
        // =====================================================


        /// <summary>
        /// The installer didn't encounter any errors
        /// </summary>
        OK                          =  0,

        /// <summary>
        /// The installer didn't encounter any errors. The installer was invoked using a command which returned before the GUI appeared.
        /// </summary>
        Command                     =  1,
        /// <summary>
        /// The installer was required to elevate to admin. A new, different process is now executing with administrative privileges.
        /// </summary>
        RequiredAdmin               =  3,
        /// <summary>
        /// The installer was run in update mode, but no modules had any updates available.
        /// </summary>
        NoUpdates                   =  4,
        /// <summary>
        /// The installer has updated all modules successfully.
        /// </summary>
        UpdateSuccess               =  5,


        // =====================================================
        //                        ERRORS
        // =====================================================


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
        /// <summary>
        /// The user encountered an unknown exception while installing a module
        /// </summary>
        ExceptionInstall            = -4,

    }
}
