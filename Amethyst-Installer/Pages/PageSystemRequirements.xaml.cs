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
            return Localisation.Page_Sysreq_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {

            if ( canContinue ) {
                // Advance to next page
                MainWindow.Instance.SetPage(InstallerState.Downloading);
                SoundPlayer.PlaySound(SoundEffect.MoveNext);
            } else {
                // TODO: Ella pls tell me how to handle the UX part of this I'm not sure if straight up exiting is a good idea
                MainWindow.Instance.Close();
            }
        }

        public void OnSelected() {

            // Compute install requirements
            canContinue = InstallerStateManager.CanInstall;

            // STORAGE CHECK
            DisplayStorage();

            // USB CONTROLLERS
            DisplayUSBControllers();


            // VR SYSTEM
            DisplayVRSystem();

            // COMPATIBLE DEVICES
            DisplayCompatibleDevices();

            // PLAYSPACE SIZE
            DisplayPlayspaceSize();
        }

        private void DisplayStorage() {

            // TODO: Check storage
            diskSpaceDescription.Text = Localisation.SystemRequirement_Description_Storage; // TODO: String format
            diskSpace.State = Controls.TaskState.Question;
        }

        private void DisplayUSBControllers() {

            // TODO: Check USB controllers
            int goodControllerCount = 0;
            StringBuilder controllerStringBuffer = new StringBuilder();

            foreach ( var usbController in InstallerStateManager.UsbControllers ) {
                if ( controllerStringBuffer.Length > 0 )
                    controllerStringBuffer.Append(", ");
                controllerStringBuffer.Append(usbController.FriendlyString);
                goodControllerCount++;
            }

            usbControllersDescription.Text = string.Format(Localisation.SystemRequirement_Description_UsbControllers,
                goodControllerCount, controllerStringBuffer.ToString());

            usbControllers.State = Controls.TaskState.Question;
        }

        private void DisplayVRSystem() {

            // Check VR headset
            vrSystemDescription.Text = GetVRHeadsetString();
            vrSystem.State =
                ( ( OpenVRUtil.ConnectionType == VRConnectionType.Tethered || OpenVRUtil.ConnectionType == VRConnectionType.OculusLink ) &&
                    ( OpenVRUtil.HmdType == VRHmdType.Quest || OpenVRUtil.HmdType == VRHmdType.Quest2 )   // If it's a Quest
                ) ? Controls.TaskState.Question : Controls.TaskState.Checkmark;

            // TODO: Change depending on connection type
            string vrSystemFootnoteStringSrc = Localisation.SystemRequirement_Footnote_StageTracking_VirtualDesktop;

            string vrSystemFootnoteStringFirstPart = vrSystemFootnoteStringSrc.Substring(0, vrSystemFootnoteStringSrc.IndexOf('['));
            string vrSystemFootnoteStringHyperlink = vrSystemFootnoteStringSrc.Substring(vrSystemFootnoteStringSrc.IndexOf('[') + 1, vrSystemFootnoteStringSrc.IndexOf(']') - vrSystemFootnoteStringSrc.IndexOf('[') - 1);
            string vrSystemFootnoteStringLastPart = vrSystemFootnoteStringSrc.Substring(vrSystemFootnoteStringSrc.IndexOf(']') + 1);

            vrSystemFootnote.Inlines.Clear();
            vrSystemFootnote.Inlines.Add(vrSystemFootnoteStringFirstPart);
            Hyperlink vrSystemFootnoteHyperlink = new Hyperlink()
                {
                NavigateUri = new Uri(Util.GenerateDocsURL("alvr")),
                // Foreground = WindowsColorHelpers.Accent
            };
            vrSystemFootnoteHyperlink.Inlines.Add(vrSystemFootnoteStringHyperlink);
            // Disable tabbing if not link
            if ( vrSystemFootnoteStringHyperlink.Length == 0 )
                vrSystemFootnoteHyperlink.Focusable = false;
            vrSystemFootnoteHyperlink.RequestNavigate += Hyperlink_RequestNavigate;
            vrSystemFootnote.Inlines.Add(vrSystemFootnoteHyperlink);
            vrSystemFootnote.Inlines.Add(vrSystemFootnoteStringLastPart);
        }

        private void DisplayCompatibleDevices() {

            // TODO: Check target device
            StringBuilder compatibilityString = new StringBuilder();
            if ( InstallerStateManager.IsWindowsAncient ) {
                if ( compatibilityString.Length > 0 )
                    compatibilityString.Append(Environment.NewLine);
                compatibilityString.Append(Localisation.InstallError_WindowsVersionIsOld);
            }
            if ( !InstallerStateManager.SteamVRInstalled ) {
                if ( compatibilityString.Length > 0 )
                    compatibilityString.Append(Environment.NewLine);
                compatibilityString.Append(Localisation.InstallError_SteamVRNotFound);
            }
            if ( InstallerStateManager.IsCloudPC ) {
                if ( compatibilityString.Length > 0 )
                    compatibilityString.Append(Environment.NewLine);
                compatibilityString.Append(Localisation.InstallError_CloudPC);
            }

            if ( compatibilityString.Length == 0 )
                compatibilityString.Append("Amogus (OK)");

            compatDevicesDescription.Text = compatibilityString.ToString();
            // compatDevices.State = Controls.TaskState.Question;
            compatDevices.State = canContinue ? Controls.TaskState.Checkmark : Controls.TaskState.Error;
        }
        
        private void DisplayPlayspaceSize() {

            double minAxis = Math.Min(InstallerStateManager.PlayspaceBounds.x, InstallerStateManager.PlayspaceBounds.y);
            string compatibleDeviceDescriptionStringSrc =
                    minAxis == 0 ? Localisation.SystemRequirement_Description_Playspace_Unknown :
                    (minAxis < Constants.MinimumPlayspaceSize ? Localisation.SystemRequirement_Description_Playspace_Small : Localisation.SystemRequirement_Description_Playspace_Good);


            // Subtitute playspace bounds into string
            compatibleDeviceDescriptionStringSrc = string.Format(compatibleDeviceDescriptionStringSrc,
                InstallerStateManager.PlayspaceBounds.x.ToString("n1"),
                InstallerStateManager.PlayspaceBounds.y.ToString("n1")
            );

            // Add a "link" if not present
            if ( compatibleDeviceDescriptionStringSrc.IndexOf('[') == -1 ) {
                compatibleDeviceDescriptionStringSrc += "[]";
            }
            string compatibleDeviceDescriptionStringFirstPart = compatibleDeviceDescriptionStringSrc.Substring(0, compatibleDeviceDescriptionStringSrc.IndexOf('['));
            string compatibleDeviceDescriptionStringHyperlink = compatibleDeviceDescriptionStringSrc.Substring(compatibleDeviceDescriptionStringSrc.IndexOf('[') + 1, compatibleDeviceDescriptionStringSrc.IndexOf(']') - compatibleDeviceDescriptionStringSrc.IndexOf('[') - 1);
            string compatibleDeviceDescriptionStringLastPart = compatibleDeviceDescriptionStringSrc.Substring(compatibleDeviceDescriptionStringSrc.IndexOf(']') + 1);

            minPlayspaceSizeDescription.Inlines.Clear();
            minPlayspaceSizeDescription.Inlines.Add(compatibleDeviceDescriptionStringFirstPart);
            Hyperlink minPlayspaceSizeHyperLink = new Hyperlink()
                {
                NavigateUri = new Uri(Util.GenerateDocsURL("playspace"))
            };
            minPlayspaceSizeHyperLink.Inlines.Add(compatibleDeviceDescriptionStringHyperlink);
            // Disable tabbing if not link
            if ( compatibleDeviceDescriptionStringHyperlink.Length == 0 )
                minPlayspaceSizeHyperLink.Focusable = false;
            minPlayspaceSizeHyperLink.RequestNavigate += Hyperlink_RequestNavigate;
            minPlayspaceSizeDescription.Inlines.Add(minPlayspaceSizeHyperLink);
            minPlayspaceSizeDescription.Inlines.Add(compatibleDeviceDescriptionStringLastPart);

            minPlayspaceSize.State = minAxis == 0 ? Controls.TaskState.Question :
                ( minAxis < Constants.MinimumPlayspaceSize ? Controls.TaskState.Error : Controls.TaskState.Checkmark );
        }

        private string GetVRHeadsetString() {

            string vrConnectionType = OpenVRUtil.ConnectionType.ToString();
            vrConnectionType = OpenVRUtil.ConnectionType.ToString();

            switch ( OpenVRUtil.ConnectionType ) {
                case VRConnectionType.ALVR:
                    return "ALVR";
                case VRConnectionType.VirtualDesktop:
                    vrConnectionType = "Virtual Desktop Streamer";
                    break;
                case VRConnectionType.OculusLink:
                    vrConnectionType = "Oculus Link / Air Link";
                    break;
            }

            string headsetString = string.Empty;

            switch ( OpenVRUtil.HmdType ) {

                case VRHmdType.Rift:
                    headsetString = "Oculus Rift CV1";
                    break;
                case VRHmdType.RiftS:
                    headsetString = "Oculus Rift S";
                    break;
                case VRHmdType.Quest:
                    headsetString = $"Meta Quest {Localisation.SystemRequirement_Description_Headset_Via} {vrConnectionType}";
                    break;
                case VRHmdType.Quest2:
                    headsetString = $"Meta Quest 2 {Localisation.SystemRequirement_Description_Headset_Via} {vrConnectionType}";
                    break;

                case VRHmdType.Vive:
                    return "HTC Vive";
                case VRHmdType.ViveCosmos:
                    headsetString = "HTC Vive Cosmos";
                    break;
                case VRHmdType.VivePro:
                    return "HTC Vive Pro";

                case VRHmdType.Pimax:
                    return "Pimax";

                case VRHmdType.WMR:
                    headsetString = "Windows Mixed Reality";
                    break;

                case VRHmdType.Index:
                    return "Valve Index";
                case VRHmdType.Deckard:
                    headsetString = "Valve Deckard";
                    break;

                case VRHmdType.PSVR:
                    headsetString = "PlayStation VR";
                    break;

                case VRHmdType.PicoNeo:
                    headsetString = "Pico Neo";
                    break;
                case VRHmdType.PicoNeo2:
                    headsetString = "Pico Neo 2";
                    break;
                case VRHmdType.PicoNeo3:
                    headsetString = "Pico Neo 3";
                    break;

                case VRHmdType.Phone:
                    headsetString = Localisation.SystemRequirement_Description_Headset_Phone;
                    break;
            }

            if ( headsetString.Length > 0 ) {
                if ( OpenVRUtil.TrackingType == VRTrackingType.Lighthouse ) {
                    return $"{headsetString} {Localisation.SystemRequirement_Description_Headset_UsingLighthouse}";
                }
            }

            return $"{OpenVRUtil.HmdType} {Localisation.SystemRequirement_Description_Headset_Via} {OpenVRUtil.ConnectionType} ({Localisation.SystemRequirement_Description_Headset_TrackingUnder} {OpenVRUtil.TrackingType})";
        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(false);
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Process.Start(e.Uri.ToString());
        }
    }
}
