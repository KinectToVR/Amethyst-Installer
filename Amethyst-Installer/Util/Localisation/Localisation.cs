namespace amethyst_installer_gui {
    /// <summary>
    /// A collection of getters so that I can use the JSON i18n system in XAML
    /// </summary>
    public static class Localisation {

        public static string Dialog_Description_CritError {
            get {
                return LocaleManager.GetString("Dialog_Description_CritError");
            }
        }
        public static string Dialog_Title_CritError {
            get {
                return LocaleManager.GetString("Dialog_Title_CritError");
            }
        }
        public static string Download_Failure {
            get {
                return LocaleManager.GetString("Download_Failure");
            }
        }
        public static string Download_Retry {
            get {
                return LocaleManager.GetString("Download_Retry");
            }
        }
        public static string Download_FailureCritical {
            get {
                return LocaleManager.GetString("Download_FailureCritical");
            }
        }
        public static string EULA_Agree {
            get {
                return LocaleManager.GetString("EULA_Agree");
            }
        }
        public static string EULA_DontAgree {
            get {
                return LocaleManager.GetString("EULA_DontAgree");
            }
        }
        public static string EULA_Licenses {
            get {
                return LocaleManager.GetString("EULA_Licenses");
            }
        }
        public static string EULA_Warranty {
            get {
                return LocaleManager.GetString("EULA_Warranty");
            }
        }
        public static string InstallDestination_AmeInstallFound {
            get {
                return LocaleManager.GetString("InstallDestination_AmeInstallFound");
            }
        }
        public static string InstallDestination_AmethystInstallLocationFootnote {
            get {
                return LocaleManager.GetString("InstallDestination_AmethystInstallLocationFootnote");
            }
        }
        public static string InstallDestination_DiskLabelFormat {
            get {
                return LocaleManager.GetString("InstallDestination_DiskLabelFormat");
            }
        }
        public static string InstallDestination_InvalidPathDescription {
            get {
                return LocaleManager.GetString("InstallDestination_InvalidPathDescription");
            }
        }
        public static string InstallDestination_InvalidPathTitle {
            get {
                return LocaleManager.GetString("InstallDestination_InvalidPathTitle");
            }
        }
        public static string InstallDestination_OpenFolder {
            get {
                return LocaleManager.GetString("InstallDestination_OpenFolder");
            }
        }
        public static string InstallDestination_PathPlaceholder {
            get {
                return LocaleManager.GetString("InstallDestination_PathPlaceholder");
            }
        }
        public static string InstallDestination_StorageFormatFree {
            get {
                return LocaleManager.GetString("InstallDestination_StorageFormatFree");
            }
        }
        public static string InstallDestination_CreateStartMenuShortcut {
            get {
                return LocaleManager.GetString("InstallDestination_CreateStartMenuShortcut");
            }
        }
        public static string InstallDestination_CreateDesktopShortcut {
            get {
                return LocaleManager.GetString("InstallDestination_CreateDesktopShortcut");
            }
        }
        public static string InstallOptions_ClickToViewSummary {
            get {
                return LocaleManager.GetString("InstallOptions_ClickToViewSummary");
            }
        }
        public static string InstallOptions_DownloadSize {
            get {
                return LocaleManager.GetString("InstallOptions_DownloadSize");
            }
        }
        public static string InstallOptions_InstallSize {
            get {
                return LocaleManager.GetString("InstallOptions_InstallSize");
            }
        }
        public static string InstallOptions_TotalDownloadSize {
            get {
                return LocaleManager.GetString("InstallOptions_TotalDownloadSize");
            }
        }
        public static string InstallOptions_TotalInstallSize {
            get {
                return LocaleManager.GetString("InstallOptions_TotalInstallSize");
            }
        }
        public static string InstallError_CloudPC {
            get {
                return LocaleManager.GetString("InstallError_CloudPC");
            }
        }
        public static string InstallError_SteamVRNotFound {
            get {
                return LocaleManager.GetString("InstallError_SteamVRNotFound");
            }
        }
        public static string InstallError_WindowsVersionIsOld {
            get {
                return LocaleManager.GetString("InstallError_WindowsVersionIsOld");
            }
        }
        public static string Device_NotDetected {
            get {
                return LocaleManager.GetString("Device_NotDetected");
            }
        }
        public static string Device_Xbox360Kinect {
            get {
                return LocaleManager.GetString("Device_Xbox360Kinect");
            }
        }
        public static string Device_XboxOneKinect {
            get {
                return LocaleManager.GetString("Device_XboxOneKinect");
            }
        }
        public static string DeviceInfo_XboxOneKinectLighthouseIncompatible {
            get {
                return LocaleManager.GetString("DeviceInfo_XboxOneKinectLighthouseIncompatible");
            }
        }
        public static string InstallFailure_Modal_Title {
            get {
                return LocaleManager.GetString("InstallFailure_Modal_Title");
            }
        }
        public static string InstallFailure_Modal_Description {
            get {
                return LocaleManager.GetString("InstallFailure_Modal_Description");
            }
        }
        public static string InstallFailure_DiskFull_Description {
            get {
                return LocaleManager.GetString("InstallFailure_DiskFull_Description");
            }
        }
        public static string InstallFailure_DiskFull_Title {
            get {
                return LocaleManager.GetString("InstallFailure_DiskFull_Title");
            }
        }
        public static string Installer_Action_Back {
            get {
                return LocaleManager.GetString("Installer_Action_Back");
            }
        }
        public static string Installer_Action_CopyError {
            get {
                return LocaleManager.GetString("Installer_Action_CopyError");
            }
        }
        public static string Installer_Action_Discord {
            get {
                return LocaleManager.GetString("Installer_Action_Discord");
            }
        }
        public static string Installer_Action_Exit {
            get {
                return LocaleManager.GetString("Installer_Action_Exit");
            }
        }
        public static string Installer_Action_Next {
            get {
                return LocaleManager.GetString("Installer_Action_Next");
            }
        }
        public static string InstallProhibited_Title {
            get {
                return LocaleManager.GetString("InstallProhibited_Title");
            }
        }
        public static string InstallProhibited_CloudPC {
            get {
                return LocaleManager.GetString("InstallProhibited_CloudPC");
            }
        }
        public static string InstallProhibited_NoSteamVR {
            get {
                return LocaleManager.GetString("InstallProhibited_NoSteamVR");
            }
        }
        public static string InstallProhibited_WindowsAncient {
            get {
                return LocaleManager.GetString("InstallProhibited_WindowsAncient");
            }
        }
        public static string InstallProhibited_PhoneVR {
            get {
                return LocaleManager.GetString("InstallProhibited_PhoneVR");
            }
        }
        public static string Logs_DirectoryIsLocatedHere {
            get {
                return LocaleManager.GetString("Logs_DirectoryIsLocatedHere");
            }
        }
        public static string Modal_Cancel {
            get {
                return LocaleManager.GetString("Modal_Cancel");
            }
        }
        public static string Modal_No {
            get {
                return LocaleManager.GetString("Modal_No");
            }
        }
        public static string Modal_OK {
            get {
                return LocaleManager.GetString("Modal_OK");
            }
        }
        public static string Modal_Yes {
            get {
                return LocaleManager.GetString("Modal_Yes");
            }
        }
        public static string Page_Done_Title {
            get {
                return LocaleManager.GetString("Page_Done_Title");
            }
        }
        public static string Page_Download_Title {
            get {
                return LocaleManager.GetString("Page_Download_Title");
            }
        }
        public static string Page_EULA_Title {
            get {
                return LocaleManager.GetString("Page_EULA_Title");
            }
        }
        public static string Page_Exception_Title {
            get {
                return LocaleManager.GetString("Page_Exception_Title");
            }
        }
        public static string Page_InstallOptions_Title {
            get {
                return LocaleManager.GetString("Page_InstallOptions_Title");
            }
        }
        public static string Page_Install_Title {
            get {
                return LocaleManager.GetString("Page_Install_Title");
            }
        }
        public static string Page_Location_Title {
            get {
                return LocaleManager.GetString("Page_Location_Title");
            }
        }
        public static string Page_Logs_Title {
            get {
                return LocaleManager.GetString("Page_Logs_Title");
            }
        }
        public static string Page_Sysreq_Title {
            get {
                return LocaleManager.GetString("Page_Sysreq_Title");
            }
        }
        public static string Page_Uninstall_Title {
            get {
                return LocaleManager.GetString("Page_Uninstall_Title");
            }
        }
        public static string Sidebar_Task_Done {
            get {
                return LocaleManager.GetString("Sidebar_Task_Done");
            }
        }
        public static string Sidebar_Task_Download {
            get {
                return LocaleManager.GetString("Sidebar_Task_Download");
            }
        }
        public static string Sidebar_Task_Install {
            get {
                return LocaleManager.GetString("Sidebar_Task_Install");
            }
        }
        public static string Sidebar_Task_InstallOptions {
            get {
                return LocaleManager.GetString("Sidebar_Task_InstallOptions");
            }
        }
        public static string Sidebar_Task_Location {
            get {
                return LocaleManager.GetString("Sidebar_Task_Location");
            }
        }
        public static string Sidebar_Task_Sysreq {
            get {
                return LocaleManager.GetString("Sidebar_Task_Sysreq");
            }
        }
        public static string Sidebar_Task_Welcome {
            get {
                return LocaleManager.GetString("Sidebar_Task_Welcome");
            }
        }
        public static string Sidebar_ViewLogs {
            get {
                return LocaleManager.GetString("Sidebar_ViewLogs");
            }
        }
        public static string Speedrunner_Description {
            get {
                return LocaleManager.GetString("Speedrunner_Description");
            }
        }
        public static string Speedrunner_Title {
            get {
                return LocaleManager.GetString("Speedrunner_Title");
            }
        }
        public static string SystemRequirement_Category_Compatibility {
            get {
                return LocaleManager.GetString("SystemRequirement_Category_Compatibility");
            }
        }
        public static string SystemRequirement_Category_Playspace {
            get {
                return LocaleManager.GetString("SystemRequirement_Category_Playspace");
            }
        }
        public static string SystemRequirement_Category_Storage {
            get {
                return LocaleManager.GetString("SystemRequirement_Category_Storage");
            }
        }
        public static string SystemRequirement_Category_UsbControllers {
            get {
                return LocaleManager.GetString("SystemRequirement_Category_UsbControllers");
            }
        }
        public static string SystemRequirement_Category_VrSystem {
            get {
                return LocaleManager.GetString("SystemRequirement_Category_VrSystem");
            }
        }
        public static string SystemRequirement_Description_NoDriversFound {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_NoDriversFound");
            }
        }
        public static string SystemRequirement_Description_Playspace_Good {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_Playspace_Good");
            }
        }
        public static string SystemRequirement_Description_Playspace_Small {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_Playspace_Small");
            }
        }
        public static string SystemRequirement_Description_Playspace_Unknown {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_Playspace_Unknown");
            }
        }
        public static string SystemRequirement_Description_Storage {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_Storage");
            }
        }
        public static string SystemRequirement_Description_UsbControllers {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_UsbControllers");
            }
        }
        public static string SystemRequirement_Description_UsbControllers_One {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_UsbControllers_One");
            }
        }
        public static string SystemRequirement_Description_UsbControllers_MultipleControllers {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_UsbControllers_MultipleControllers");
            }
        }
        public static string SystemRequirement_Footnote_StageTracking_VirtualDesktop {
            get {
                return LocaleManager.GetString("SystemRequirement_Footnote_StageTracking_VirtualDesktop");
            }
        }
        public static string SystemRequirement_Description_Headset_Via {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_Headset_Via");
            }
        }
        public static string SystemRequirement_Description_Headset_UsingLighthouse {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_Headset_UsingLighthouse");
            }
        }
        public static string SystemRequirement_Description_Headset_TrackingUnder {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_Headset_TrackingUnder");
            }
        }
        public static string SystemRequirement_Description_Headset_Phone {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_Headset_Phone");
            }
        }
        public static string SystemRequirement_Description_Headset_Not_Detected {
            get {
                return LocaleManager.GetString("SystemRequirement_Description_Headset_Not_Detected");
            }
        }
        public static string Welcome_OptIn {
            get {
                return LocaleManager.GetString("Welcome_OptIn");
            }
        }
        public static string Welcome_PrivacyPolicy {
            get {
                return LocaleManager.GetString("Welcome_PrivacyPolicy");
            }
        }
        public static string Welcome_ReadPrivacyPolicy {
            get {
                return LocaleManager.GetString("Welcome_ReadPrivacyPolicy");
            }
        }
        public static string Welcome_ThisIsATechnicalPreviewThisIsUnstablePleaseDoNotFuckingShareThisOrIWillShitBricks {
            get {
                return LocaleManager.GetString("Welcome_ThisIsATechnicalPreviewThisIsUnstablePleaseDoNotFuckingShareThisOrIWillShitBricks");
            }
        }
        public static string Done_InstallSuccess {
            get {
                return LocaleManager.GetString("Done_InstallSuccess");
            }
        }
        public static string Done_LaunchDesktop {
            get {
                return LocaleManager.GetString("Done_LaunchDesktop");
            }
        }
        public static string Done_LaunchStartMenu {
            get {
                return LocaleManager.GetString("Done_LaunchStartMenu");
            }
        }
        public static string Done_LaunchAmethyst {
            get {
                return LocaleManager.GetString("Done_LaunchAmethyst");
            }
        }
        public static string Done_LinkDocumentation {
            get {
                return LocaleManager.GetString("Done_LinkDocumentation");
            }
        }
        public static string Done_LinkDiscord {
            get {
                return LocaleManager.GetString("Done_LinkDiscord");
            }
        }
        public static string Done_LinkGitHub {
            get {
                return LocaleManager.GetString("Done_LinkGitHub");
            }
        }
        public static string Done_LinkDonations {
            get {
                return LocaleManager.GetString("Done_LinkDonations");
            }
        }
        public static string PostOp_Kinect_EnableMic_Title {
            get {
                return LocaleManager.GetString("PostOp_Kinect_EnableMic_Title");
            }
        }
        public static string PostOp_Kinect_EnableMic_Description {
            get {
                return LocaleManager.GetString("PostOp_Kinect_EnableMic_Description");
            }
        }
        public static string MustDisableMemoryIntegrity_Title {
            get {
                return LocaleManager.GetString("MustDisableMemoryIntegrity_Title");
            }
        }
        public static string MustDisableMemoryIntegrity_Description {
            get {
                return LocaleManager.GetString("MustDisableMemoryIntegrity_Description");
            }
        }
        public static string Updating_FailedReachApi {
            get {
                return LocaleManager.GetString("Updating_FailedReachApi");
            }
        }
        public static string Updating_InitFailed {
            get {
                return LocaleManager.GetString("Updating_InitFailed");
            }
        }
        public static string Updating_UpdatingAmethyst {
            get {
                return LocaleManager.GetString("Updating_UpdatingAmethyst");
            }
        }
        public static string Installer_QuitVerify_Modal_Title {
            get {
                return LocaleManager.GetString("Installer_QuitVerify_Modal_Title");
            }
        }
        public static string Installer_QuitVerify_Modal_Description {
            get {
                return LocaleManager.GetString("Installer_QuitVerify_Modal_Description");
            }
        }
        public static string Installer_K2EXUpgrade_Title {
            get {
                return LocaleManager.GetString("Installer_K2EXUpgrade_Title");
            }
        }
        public static string Installer_K2EXUpgrade_Description {
            get {
                return LocaleManager.GetString("Installer_K2EXUpgrade_Description");
            }
        }
        public static string Installer_K2EXUpgrade_LearnMore {
            get {
                return LocaleManager.GetString("Installer_K2EXUpgrade_LearnMore");
            }
        }
        public static string Installer_K2EXUpgrade_UpgradeButton {
            get {
                return LocaleManager.GetString("Installer_K2EXUpgrade_UpgradeButton");
            }
        }
    }
}
