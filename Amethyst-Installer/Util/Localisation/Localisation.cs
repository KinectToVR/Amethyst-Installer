using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Timers;

namespace amethyst_installer_gui {
    /// <summary>
    /// A collection of getters so that I can use the JSON i18n system in XAML
    /// </summary>
    public class LocalizationManager : INotifyPropertyChanged {
        private FileSystemWatcher ResourceWatcher { get; }

        public LocalizationManager() {
            // Only setup hot reload if --debug
            if ( !MainWindow.DebugMode ) {
                return;
            }

            // Setup string hot reload watchdog
            ResourceWatcher = new FileSystemWatcher {
                Path = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)),
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName |
                               NotifyFilters.LastWrite | NotifyFilters.DirectoryName,
                IncludeSubdirectories = true,
                Filter = "*.json",
                EnableRaisingEvents = true
            };

            // Add event handlers : local
            ResourceWatcher.Changed += OnWatcherOnChanged;
            ResourceWatcher.Created += OnWatcherOnChanged;
            ResourceWatcher.Deleted += OnWatcherOnChanged;
            ResourceWatcher.Renamed += OnWatcherOnChanged;
        }

        // Send a non-dismissible tip about reloading the app
        private void OnWatcherOnChanged(object o, FileSystemEventArgs fileSystemEventArgs) {
            Logger.Info(
                $"One of [probably] localization files has changed! Reloading... File: \"{fileSystemEventArgs.FullPath}\"");

            LocaleManager.ReloadLocale();
            OnPropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region String resources

        // The comment below marks the beginning of the resource strings matcher
        // It will be used by Program.cs@AutoGenerateLoc, please don't touch it!

        // GEN_LOCALE_BEGIN_GETTERS

		public string Dialog_Description_CritError => 
			LocaleManager.GetString("Dialog_Description_CritError");

		public string Dialog_Title_CritError => 
			LocaleManager.GetString("Dialog_Title_CritError");

		public string Download_Failure => 
			LocaleManager.GetString("Download_Failure");

		public string Download_Retry => 
			LocaleManager.GetString("Download_Retry");

		public string Download_FailureCritical => 
			LocaleManager.GetString("Download_FailureCritical");

		public string EULA_Agree => 
			LocaleManager.GetString("EULA_Agree");

		public string EULA_DontAgree => 
			LocaleManager.GetString("EULA_DontAgree");

		public string EULA_Licenses => 
			LocaleManager.GetString("EULA_Licenses");

		public string EULA_Warranty => 
			LocaleManager.GetString("EULA_Warranty");

		public string InstallDestination_AmeInstallFound => 
			LocaleManager.GetString("InstallDestination_AmeInstallFound");

		public string InstallDestination_AmethystInstallLocationFootnote => 
			LocaleManager.GetString("InstallDestination_AmethystInstallLocationFootnote");

		public string InstallDestination_DiskLabelFormat => 
			LocaleManager.GetString("InstallDestination_DiskLabelFormat");

		public string InstallDestination_InvalidPathDescription => 
			LocaleManager.GetString("InstallDestination_InvalidPathDescription");

		public string InstallDestination_InvalidPathTitle => 
			LocaleManager.GetString("InstallDestination_InvalidPathTitle");

		public string InstallDestination_OpenFolder => 
			LocaleManager.GetString("InstallDestination_OpenFolder");

		public string InstallDestination_PathPlaceholder => 
			LocaleManager.GetString("InstallDestination_PathPlaceholder");

		public string InstallDestination_StorageFormatFree => 
			LocaleManager.GetString("InstallDestination_StorageFormatFree");

		public string InstallDestination_CreateStartMenuShortcut => 
			LocaleManager.GetString("InstallDestination_CreateStartMenuShortcut");

		public string InstallDestination_CreateDesktopShortcut => 
			LocaleManager.GetString("InstallDestination_CreateDesktopShortcut");

		public string InstallOptions_ClickToViewSummary => 
			LocaleManager.GetString("InstallOptions_ClickToViewSummary");

		public string InstallOptions_DownloadSize => 
			LocaleManager.GetString("InstallOptions_DownloadSize");

		public string InstallOptions_InstallSize => 
			LocaleManager.GetString("InstallOptions_InstallSize");

		public string InstallOptions_TotalDownloadSize => 
			LocaleManager.GetString("InstallOptions_TotalDownloadSize");

		public string InstallOptions_TotalInstallSize => 
			LocaleManager.GetString("InstallOptions_TotalInstallSize");

		public string InstallError_CloudPC => 
			LocaleManager.GetString("InstallError_CloudPC");

		public string InstallError_SteamVRNotFound => 
			LocaleManager.GetString("InstallError_SteamVRNotFound");

		public string InstallError_WindowsVersionIsOld => 
			LocaleManager.GetString("InstallError_WindowsVersionIsOld");

		public string Device_NotDetected => 
			LocaleManager.GetString("Device_NotDetected");

		public string Device_Xbox360Kinect => 
			LocaleManager.GetString("Device_Xbox360Kinect");

		public string Device_XboxOneKinect => 
			LocaleManager.GetString("Device_XboxOneKinect");

		public string Device_PsmoveEye => 
			LocaleManager.GetString("Device_PsmoveEye");

		public string DeviceInfo_XboxOneKinectLighthouseIncompatible => 
			LocaleManager.GetString("DeviceInfo_XboxOneKinectLighthouseIncompatible");

		public string DeviceInfo_PsmoveEyeLibusbWarning => 
			LocaleManager.GetString("DeviceInfo_PsmoveEyeLibusbWarning");

		public string DeviceInfo_PsmoveEyeLibusbWarningOverwrite => 
			LocaleManager.GetString("DeviceInfo_PsmoveEyeLibusbWarningOverwrite");

		public string InstallFailure_Modal_Title => 
			LocaleManager.GetString("InstallFailure_Modal_Title");

		public string InstallFailure_Modal_Description => 
			LocaleManager.GetString("InstallFailure_Modal_Description");

		public string InstallFailure_DiskFull_Description => 
			LocaleManager.GetString("InstallFailure_DiskFull_Description");

		public string InstallFailure_DiskFull_Title => 
			LocaleManager.GetString("InstallFailure_DiskFull_Title");

		public string Installer_Action_Back => 
			LocaleManager.GetString("Installer_Action_Back");

		public string Installer_Action_CopyError => 
			LocaleManager.GetString("Installer_Action_CopyError");

		public string Installer_Action_Discord => 
			LocaleManager.GetString("Installer_Action_Discord");

		public string Installer_Action_Exit => 
			LocaleManager.GetString("Installer_Action_Exit");

		public string Installer_Action_Next => 
			LocaleManager.GetString("Installer_Action_Next");

		public string Installer_Action_Finish => 
			LocaleManager.GetString("Installer_Action_Finish");

		public string InstallProhibited_Title => 
			LocaleManager.GetString("InstallProhibited_Title");

		public string InstallProhibited_CloudPC => 
			LocaleManager.GetString("InstallProhibited_CloudPC");

		public string InstallProhibited_WindowsAncient => 
			LocaleManager.GetString("InstallProhibited_WindowsAncient");

		public string InstallProhibited_PhoneVR => 
			LocaleManager.GetString("InstallProhibited_PhoneVR");

		public string InstallProhibited_DiskFull => 
			LocaleManager.GetString("InstallProhibited_DiskFull");

		public string Logs_DirectoryIsLocatedHere => 
			LocaleManager.GetString("Logs_DirectoryIsLocatedHere");

		public string Modal_Cancel => 
			LocaleManager.GetString("Modal_Cancel");

		public string Modal_No => 
			LocaleManager.GetString("Modal_No");

		public string Modal_OK => 
			LocaleManager.GetString("Modal_OK");

		public string Modal_Yes => 
			LocaleManager.GetString("Modal_Yes");

		public string Page_Done_Title => 
			LocaleManager.GetString("Page_Done_Title");

		public string Page_Download_Title => 
			LocaleManager.GetString("Page_Download_Title");

		public string Page_EULA_Title => 
			LocaleManager.GetString("Page_EULA_Title");

		public string Page_SelectAmethystMode_Title => 
			LocaleManager.GetString("Page_SelectAmethystMode_Title");

		public string Page_Exception_Title => 
			LocaleManager.GetString("Page_Exception_Title");

		public string Page_InstallOptions_Title => 
			LocaleManager.GetString("Page_InstallOptions_Title");

		public string Page_Install_Title => 
			LocaleManager.GetString("Page_Install_Title");

		public string Page_Location_Title => 
			LocaleManager.GetString("Page_Location_Title");

		public string Page_Logs_Title => 
			LocaleManager.GetString("Page_Logs_Title");

		public string Page_Sysreq_Title => 
			LocaleManager.GetString("Page_Sysreq_Title");

		public string Page_Uninstall_Title => 
			LocaleManager.GetString("Page_Uninstall_Title");

		public string Sidebar_Task_Done => 
			LocaleManager.GetString("Sidebar_Task_Done");

		public string Sidebar_Task_Download => 
			LocaleManager.GetString("Sidebar_Task_Download");

		public string Sidebar_Task_Install => 
			LocaleManager.GetString("Sidebar_Task_Install");

		public string Sidebar_Task_InstallOptions => 
			LocaleManager.GetString("Sidebar_Task_InstallOptions");

		public string Sidebar_Task_Location => 
			LocaleManager.GetString("Sidebar_Task_Location");

		public string Sidebar_Task_Sysreq => 
			LocaleManager.GetString("Sidebar_Task_Sysreq");

		public string Sidebar_Task_Welcome => 
			LocaleManager.GetString("Sidebar_Task_Welcome");

		public string Sidebar_ViewLogs => 
			LocaleManager.GetString("Sidebar_ViewLogs");

		public string Sidebar_Uninstall_WhatToRemove => 
			LocaleManager.GetString("Sidebar_Uninstall_WhatToRemove");

		public string Sidebar_Uninstall_Working => 
			LocaleManager.GetString("Sidebar_Uninstall_Working");

		public string Sidebar_Uninstall_Complete => 
			LocaleManager.GetString("Sidebar_Uninstall_Complete");

		public string Speedrunner_Description => 
			LocaleManager.GetString("Speedrunner_Description");

		public string Speedrunner_Title => 
			LocaleManager.GetString("Speedrunner_Title");

		public string AmethystMode_Description => 
			LocaleManager.GetString("AmethystMode_Description");

		public string AmethystMode_OpenVR_Title => 
			LocaleManager.GetString("AmethystMode_OpenVR_Title");

		public string AmethystMode_OpenVR_Description => 
			LocaleManager.GetString("AmethystMode_OpenVR_Description");

		public string AmethystMode_OSC_Title => 
			LocaleManager.GetString("AmethystMode_OSC_Title");

		public string AmethystMode_OSC_Description => 
			LocaleManager.GetString("AmethystMode_OSC_Description");

		public string AmethystModule_Amethyst_Title => 
			LocaleManager.GetString("AmethystModule_Amethyst_Title");

		public string AmethystModule_Amethyst_Summary => 
			LocaleManager.GetString("AmethystModule_Amethyst_Summary");

		public string AmethystModule_Amethyst_Description => 
			LocaleManager.GetString("AmethystModule_Amethyst_Description");

		public string AmethystModule_VCRedist_Title => 
			LocaleManager.GetString("AmethystModule_VCRedist_Title");

		public string AmethystModule_VCRedist_Summary => 
			LocaleManager.GetString("AmethystModule_VCRedist_Summary");

		public string AmethystModule_VCRedist_Description => 
			LocaleManager.GetString("AmethystModule_VCRedist_Description");

		public string AmethystModule_WIX_Title => 
			LocaleManager.GetString("AmethystModule_WIX_Title");

		public string AmethystModule_WIX_Summary => 
			LocaleManager.GetString("AmethystModule_WIX_Summary");

		public string AmethystModule_WIX_Description => 
			LocaleManager.GetString("AmethystModule_WIX_Description");

		public string AmethystModule_KinectV1_Title => 
			LocaleManager.GetString("AmethystModule_KinectV1_Title");

		public string AmethystModule_KinectV1_Summary => 
			LocaleManager.GetString("AmethystModule_KinectV1_Summary");

		public string AmethystModule_KinectV1_Description => 
			LocaleManager.GetString("AmethystModule_KinectV1_Description");

		public string AmethystModule_KinectV1Toolkit_Title => 
			LocaleManager.GetString("AmethystModule_KinectV1Toolkit_Title");

		public string AmethystModule_KinectV1Toolkit_Summary => 
			LocaleManager.GetString("AmethystModule_KinectV1Toolkit_Summary");

		public string AmethystModule_KinectV1Toolkit_Description => 
			LocaleManager.GetString("AmethystModule_KinectV1Toolkit_Description");

		public string AmethystModule_PSMS_Title => 
			LocaleManager.GetString("AmethystModule_PSMS_Title");

		public string AmethystModule_PSMS_Summary => 
			LocaleManager.GetString("AmethystModule_PSMS_Summary");

		public string AmethystModule_PSMS_Description => 
			LocaleManager.GetString("AmethystModule_PSMS_Description");

		public string AmethystModule_PSMS_VDM_Title => 
			LocaleManager.GetString("AmethystModule_PSMS_VDM_Title");

		public string AmethystModule_PSMS_VDM_Summary => 
			LocaleManager.GetString("AmethystModule_PSMS_VDM_Summary");

		public string AmethystModule_PSMS_VDM_Description => 
			LocaleManager.GetString("AmethystModule_PSMS_VDM_Description");

		public string AmethystModule_PSMS_Drivers_Title => 
			LocaleManager.GetString("AmethystModule_PSMS_Drivers_Title");

		public string AmethystModule_PSMS_Drivers_Summary => 
			LocaleManager.GetString("AmethystModule_PSMS_Drivers_Summary");

		public string AmethystModule_PSMS_Drivers_Description => 
			LocaleManager.GetString("AmethystModule_PSMS_Drivers_Description");

		public string SystemRequirement_Category_Compatibility => 
			LocaleManager.GetString("SystemRequirement_Category_Compatibility");

		public string SystemRequirement_Category_Playspace => 
			LocaleManager.GetString("SystemRequirement_Category_Playspace");

		public string SystemRequirement_Category_Storage => 
			LocaleManager.GetString("SystemRequirement_Category_Storage");

		public string SystemRequirement_Category_UsbControllers => 
			LocaleManager.GetString("SystemRequirement_Category_UsbControllers");

		public string SystemRequirement_Category_VrSystem => 
			LocaleManager.GetString("SystemRequirement_Category_VrSystem");

		public string SystemRequirement_Description_NoDriversFound => 
			LocaleManager.GetString("SystemRequirement_Description_NoDriversFound");

		public string SystemRequirement_Description_Playspace_Good => 
			LocaleManager.GetString("SystemRequirement_Description_Playspace_Good");

		public string SystemRequirement_Description_Playspace_Small => 
			LocaleManager.GetString("SystemRequirement_Description_Playspace_Small");

		public string SystemRequirement_Description_Playspace_Unknown => 
			LocaleManager.GetString("SystemRequirement_Description_Playspace_Unknown");

		public string SystemRequirement_Description_Storage => 
			LocaleManager.GetString("SystemRequirement_Description_Storage");

		public string SystemRequirement_Description_UsbControllers => 
			LocaleManager.GetString("SystemRequirement_Description_UsbControllers");

		public string SystemRequirement_Description_UsbControllers_One => 
			LocaleManager.GetString("SystemRequirement_Description_UsbControllers_One");

		public string SystemRequirement_Description_UsbControllers_MultipleControllers => 
			LocaleManager.GetString("SystemRequirement_Description_UsbControllers_MultipleControllers");

		public string SystemRequirement_Description_UsbControllers_Laptop => 
			LocaleManager.GetString("SystemRequirement_Description_UsbControllers_Laptop");

		public string SystemRequirement_Footnote_StageTracking_VirtualDesktop => 
			LocaleManager.GetString("SystemRequirement_Footnote_StageTracking_VirtualDesktop");

		public string SystemRequirement_Description_Headset_Via => 
			LocaleManager.GetString("SystemRequirement_Description_Headset_Via");

		public string SystemRequirement_Description_Headset_UsingLighthouse => 
			LocaleManager.GetString("SystemRequirement_Description_Headset_UsingLighthouse");

		public string SystemRequirement_Description_Headset_TrackingUnder => 
			LocaleManager.GetString("SystemRequirement_Description_Headset_TrackingUnder");

		public string SystemRequirement_Description_Headset_Phone => 
			LocaleManager.GetString("SystemRequirement_Description_Headset_Phone");

		public string SystemRequirement_Description_Headset_Not_Detected => 
			LocaleManager.GetString("SystemRequirement_Description_Headset_Not_Detected");

		public string Welcome_OptIn => 
			LocaleManager.GetString("Welcome_OptIn");

		public string Welcome_PrivacyPolicy => 
			LocaleManager.GetString("Welcome_PrivacyPolicy");

		public string Welcome_ReadPrivacyPolicy => 
			LocaleManager.GetString("Welcome_ReadPrivacyPolicy");

		public string Welcome_ThisIsATechnicalPreviewThisIsUnstablePleaseDoNotFuckingShareThisOrIWillShitBricks => 
			LocaleManager.GetString("Welcome_ThisIsATechnicalPreviewThisIsUnstablePleaseDoNotFuckingShareThisOrIWillShitBricks");

		public string Done_InstallSuccess => 
			LocaleManager.GetString("Done_InstallSuccess");

		public string Done_LaunchDesktop => 
			LocaleManager.GetString("Done_LaunchDesktop");

		public string Done_LaunchStartMenu => 
			LocaleManager.GetString("Done_LaunchStartMenu");

		public string Done_LaunchAmethyst => 
			LocaleManager.GetString("Done_LaunchAmethyst");

		public string Done_AutoStartSteamVr => 
			LocaleManager.GetString("Done_AutoStartSteamVr");

		public string Done_LinkDocumentation => 
			LocaleManager.GetString("Done_LinkDocumentation");

		public string Done_LinkDiscord => 
			LocaleManager.GetString("Done_LinkDiscord");

		public string Done_LinkGitHub => 
			LocaleManager.GetString("Done_LinkGitHub");

		public string Done_LinkDonations => 
			LocaleManager.GetString("Done_LinkDonations");

		public string PostOp_Kinect_EnableMic_Title => 
			LocaleManager.GetString("PostOp_Kinect_EnableMic_Title");

		public string PostOp_Kinect_EnableMic_Description => 
			LocaleManager.GetString("PostOp_Kinect_EnableMic_Description");

		public string MustDisableMemoryIntegrity_Title => 
			LocaleManager.GetString("MustDisableMemoryIntegrity_Title");

		public string MustDisableMemoryIntegrity_Description => 
			LocaleManager.GetString("MustDisableMemoryIntegrity_Description");

		public string Updating_FailedReachApi => 
			LocaleManager.GetString("Updating_FailedReachApi");

		public string Updating_InitFailed => 
			LocaleManager.GetString("Updating_InitFailed");

		public string Updating_UpdatingAmethyst => 
			LocaleManager.GetString("Updating_UpdatingAmethyst");

		public string Installer_QuitVerify_Modal_Title => 
			LocaleManager.GetString("Installer_QuitVerify_Modal_Title");

		public string Installer_QuitVerify_Modal_Description => 
			LocaleManager.GetString("Installer_QuitVerify_Modal_Description");

		public string Installer_K2EXUpgrade_Title => 
			LocaleManager.GetString("Installer_K2EXUpgrade_Title");

		public string Installer_K2EXUpgrade_Description => 
			LocaleManager.GetString("Installer_K2EXUpgrade_Description");

		public string Installer_K2EXUpgrade_LearnMore => 
			LocaleManager.GetString("Installer_K2EXUpgrade_LearnMore");

		public string Installer_K2EXUpgrade_UpgradeButton => 
			LocaleManager.GetString("Installer_K2EXUpgrade_UpgradeButton");

		public string Uninstall_Success => 
			LocaleManager.GetString("Uninstall_Success");

		// GEN_LOCALE_END_GETTERS

        // The upper comment marks the end of the resource strings regex matcher
        // It will be used by Program.cs@AutoGenerateLoc, please don't touch it!

        #endregion
    }

    public static class Localisation {
        // Private manger instance
        private static LocalizationManager _instance = new LocalizationManager();

        // Instance getter: provides strings INotify hot reload
        public static LocalizationManager Manager => _instance;
    }
}