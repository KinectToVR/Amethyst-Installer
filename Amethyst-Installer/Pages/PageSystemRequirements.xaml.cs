using amethyst_installer_gui.Installer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageSystemRequirements.xaml
    /// </summary>
    public partial class PageSystemRequirements : UserControl, IInstallerPage {

        bool canContinue = true;

        public PageSystemRequirements() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.SystemRequirements;
        }

        public string GetTitle() {
            return Properties.Resources.Page_Sysreq_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {

            if ( canContinue ) {
                // Advance to next page
                MainWindow.Instance.SetPage(InstallerState.Downloading);
            } else {
                // TODO: Ella pls tell me how to handle the UX part of this I'm not sure if straight up exiting is a good idea
                MainWindow.Instance.Close();
            }
        }

        public void OnSelected() {

            // Compute install requirements
            canContinue = InstallerStateManager.CanInstall;

            // TODO: Check storage
            diskSpaceDescription.Text = Properties.Resources.SystemRequirement_Description_Storage; // TODO: String format
            diskSpace.State = Controls.TaskState.Question;

            // TODO: Check USB controllers
            int goodControllerCount = 0;
            StringBuilder controllerStringBuffer = new StringBuilder();

            foreach ( var usbController in InstallerStateManager.UsbControllers ) {
                if ( controllerStringBuffer.Length > 0 )
                    controllerStringBuffer.Append(", ");
                controllerStringBuffer.Append(usbController.FriendlyString);
                goodControllerCount++;
            }

            usbControllersDescription.Text = string.Format(Properties.Resources.SystemRequirement_Description_UsbControllers,
                goodControllerCount, controllerStringBuffer.ToString());
            usbControllers.State = Controls.TaskState.Question;

            // Check VR headset
            vrSystemDescription.Text = GetVRHeadsetString();
            vrSystem.State =
                (( OpenVRUtil.ConnectionType == VRConnectionType.Tethered || OpenVRUtil.ConnectionType == VRConnectionType.OculusLink) &&
                    (OpenVRUtil.HmdType == VRHmdType.Quest || OpenVRUtil.HmdType == VRHmdType.Quest2)   // If it's a Quest
                ) ? Controls.TaskState.Question : Controls.TaskState.Checkmark;

            // TODO: Change depending on connection type
            string vrSystemFootnoteStringSrc = Properties.Resources.SystemRequirement_Footnote_StageTracking_VirtualDesktop;


            vrSystemFootnote.Inlines.Clear();
            vrSystemFootnote.Inlines.Add("Some text ");
            Hyperlink hyperLink3 = new Hyperlink()
            {
                NavigateUri = new Uri("http://somesite.com"),
                // Foreground = WindowsColorHelpers.Accent
            };
            hyperLink3.Inlines.Add("some site");
            hyperLink3.RequestNavigate += Hyperlink_RequestNavigate;
            vrSystemFootnote.Inlines.Add(hyperLink3);
            vrSystemFootnote.Inlines.Add(" Some more text");

            // TODO: Check target device

            StringBuilder compatibilityString = new StringBuilder();
            if ( InstallerStateManager.IsWindowsAncient ) {
                if ( compatibilityString.Length > 0 )
                    compatibilityString.Append(Environment.NewLine);
                compatibilityString.Append(Properties.Resources.InstallError_WindowsVersionIsOld);
            }
            if ( !InstallerStateManager.SteamVRInstalled ) {
                if ( compatibilityString.Length > 0 )
                    compatibilityString.Append(Environment.NewLine);
                compatibilityString.Append(Properties.Resources.InstallError_SteamVRNotFound);
            }
            if ( InstallerStateManager.IsCloudPC ) {
                if ( compatibilityString.Length > 0 )
                    compatibilityString.Append(Environment.NewLine);
                compatibilityString.Append(Properties.Resources.InstallError_CloudPC);
            }

            if ( compatibilityString.Length == 0 )
                compatibilityString.Append("Amogus (OK)");

            compatDevicesDescription.Text = compatibilityString.ToString();
            // compatDevices.State = Controls.TaskState.Question;
            compatDevices.State = canContinue ? Controls.TaskState.Checkmark : Controls.TaskState.Error;
        }

        private string GetVRHeadsetString() {

            switch ( OpenVRUtil.HmdType ) {
                case VRHmdType.Quest:
                case VRHmdType.Quest2:
                    return $"{OpenVRUtil.HmdType} ({OpenVRUtil.ConnectionType})";
            }

            return $"{OpenVRUtil.HmdType} ({OpenVRUtil.ConnectionType}) [{OpenVRUtil.TrackingType}]";
        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Properties.Resources.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            Process.Start(e.Uri.ToString());
        }
    }
}
