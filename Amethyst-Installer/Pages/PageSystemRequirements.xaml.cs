using amethyst_installer_gui.Controls;
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

        public static long RequiredStorage = 0;
        public static long FreeDriveSpace = 0;

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

        public void OnSelected() {

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

            // Compute install requirements
            canContinue = InstallerStateManager.CanInstall;
            if ( !canContinue ) {
                SoundPlayer.PlaySound(SoundEffect.Error);
                MainWindow.Instance.sidebar_sysreq.State = Controls.TaskState.Error;
                ActionButtonPrimary.Content = Localisation.Installer_Action_Exit;
            }
        }

        private void DisplayStorage() {

            diskSpaceDescription.Text = string.Format(Localisation.SystemRequirement_Description_Storage, Util.SizeSuffix(RequiredStorage)); // @TODO: String format
            diskSpace.State = Controls.TaskState.Checkmark;

            // If less than 2GB free on drive
            if ( ( FreeDriveSpace < ( 1024L * 1024L * 1024L * 2L ) ) ) {
                InstallerStateManager.CanInstall = false;
                diskSpace.State = Controls.TaskState.Error;
            }
        }

        private void DisplayUSBControllers() {

            int goodControllerCount = 0;

            Dictionary<string, int> usbControllerCount = new Dictionary<string, int>();
            Dictionary<string, int> usbControllerIdMapping = new Dictionary<string, int>();

            bool overallQuality = false;

            // Go through each controller and determine how we should treat it visually
            // This is a separate loop so that we can group multiple controllers together
            for ( int i = 0; i < InstallerStateManager.UsbControllers.Count; i++ ) {

                switch ( InstallerStateManager.UsbControllers[i].ControllerQuality ) {
                    case UsbControllerQuality.Ignore:
                        break;
                    case UsbControllerQuality.Good:
                    case UsbControllerQuality.OK:
                    case UsbControllerQuality.Unknown:
                        goodControllerCount++;
                        overallQuality = overallQuality || true;
                        goto default;
                    case UsbControllerQuality.Unusable:
                        overallQuality = overallQuality || false;
                        goto default;
                    default:
                        
                        if (usbControllerCount.ContainsKey(InstallerStateManager.UsbControllers[i].FriendlyString) ) {
                            usbControllerCount[InstallerStateManager.UsbControllers[i].FriendlyString]++;
                        } else {
                            usbControllerCount.Add(InstallerStateManager.UsbControllers[i].FriendlyString, 1);
                            usbControllerIdMapping.Add(InstallerStateManager.UsbControllers[i].FriendlyString, i);
                        }

                        break;
                }
            }

            // Actually spawn the controls
            foreach (var entry in usbControllerIdMapping ) {
                UsbControllerItem controllerItem = new UsbControllerItem(InstallerStateManager.UsbControllers[entry.Value], usbControllerCount[entry.Key]);
                controllerItem.Margin = new Thickness(0, 6, 0, 0);
                usbControllersContainer.Children.Add(controllerItem);
            }

            usbControllersDescription.Text = string.Format(Localisation.SystemRequirement_Description_UsbControllers, goodControllerCount);

            usbControllers.State = overallQuality ? TaskState.Checkmark : TaskState.Error;

            if ( overallQuality && InstallerStateManager.IsLaptop ) {
                usbControllers.State = Controls.TaskState.Warning;
                usbControllersCard.Visibility = Visibility.Visible;
            }
        }

        private void DisplayVRSystem() {

            // Check VR headset
            vrSystemDescription.Text = GetVRHeadsetString();
            
            vrSystem.State = Controls.TaskState.Checkmark;

            // If we aren't wireless
            if (( OpenVRUtil.ConnectionType == VRConnectionType.Tethered || OpenVRUtil.ConnectionType == VRConnectionType.OculusLink ) &&
                // If it's a Quest
                ( OpenVRUtil.HmdType == VRHmdType.Quest || OpenVRUtil.HmdType == VRHmdType.Quest2 )) {
                vrSystem.State = Controls.TaskState.Question;
            }
            // If it's a mobile VR headset, WEE WOO YOU CAN'T INSTALL ERROR ERROR SOUND THE ALARM!!!
            if ( OpenVRUtil.HmdType == VRHmdType.Phone ) {
                vrSystem.State = Controls.TaskState.Error;
            }

            string vrSystemFootnoteStringSrc = Localisation.SystemRequirement_Footnote_StageTracking_VirtualDesktop;

            string vrSystemFootnoteStringFirstPart = vrSystemFootnoteStringSrc.Substring(0, vrSystemFootnoteStringSrc.IndexOf('['));
            string vrSystemFootnoteStringHyperlink = vrSystemFootnoteStringSrc.Substring(vrSystemFootnoteStringSrc.IndexOf('[') + 1, vrSystemFootnoteStringSrc.IndexOf(']') - vrSystemFootnoteStringSrc.IndexOf('[') - 1);
            string vrSystemFootnoteStringLastPart = vrSystemFootnoteStringSrc.Substring(vrSystemFootnoteStringSrc.IndexOf(']') + 1);

            vrSystemFootnote.Inlines.Clear();
            vrSystemFootnote.Inlines.Add(vrSystemFootnoteStringFirstPart);
            Hyperlink vrSystemFootnoteHyperlink = new Hyperlink() {
                NavigateUri = new Uri(Util.GenerateDocsURL("standalones")),
                Foreground = WindowsColorHelpers.AccentLight,
            };
            vrSystemFootnoteHyperlink.Inlines.Add(vrSystemFootnoteStringHyperlink);

            // Disable tabbing if not link
            if ( vrSystemFootnoteStringHyperlink.Length == 0 )
                vrSystemFootnoteHyperlink.Focusable = false;
            vrSystemFootnoteHyperlink.RequestNavigate += Hyperlink_RequestNavigate;
            vrSystemFootnote.Inlines.Add(vrSystemFootnoteHyperlink);
            vrSystemFootnote.Inlines.Add(vrSystemFootnoteStringLastPart);

            // Could the headset have wireless?
            if ( OpenVRUtil.HmdType == VRHmdType.Quest ||
                OpenVRUtil.HmdType == VRHmdType.Quest2 ||
                OpenVRUtil.HmdType == VRHmdType.PicoNeo ||
                OpenVRUtil.HmdType == VRHmdType.PicoNeo2 ||
                OpenVRUtil.HmdType == VRHmdType.PicoNeo3 ) {
                // OK, are they using ALVR?
                if ( OpenVRUtil.ConnectionType == VRConnectionType.ALVR ) {
                    vrSystemFootnote.Visibility = Visibility.Collapsed;
                }
                vrSystemFootnote.Visibility = Visibility.Visible;
            }
        }

        private void DisplayCompatibleDevices() {
            compatDevices.State = canContinue ? Controls.TaskState.Checkmark : Controls.TaskState.Error;

            StringBuilder compatibilityString = new StringBuilder();
            // Check if your setup is fucked (this should never be encountered, but better safe than sorry)
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

            // Check for Kinects
            if ( KinectUtil.IsKinectV1Present() ) {
                if ( compatibilityString.Length > 0 )
                    compatibilityString.Append(Environment.NewLine);
                compatibilityString.Append(Localisation.Device_Xbox360Kinect);
            }
            if ( KinectUtil.IsKinectV2Present() ) {
                if ( compatibilityString.Length > 0 )
                    compatibilityString.Append(Environment.NewLine);
                compatibilityString.Append(Localisation.Device_XboxOneKinect);

                if ( OpenVRUtil.TrackingType == VRTrackingType.Lighthouse ) {
                    // Oh no...
                    compatDevices.State = Controls.TaskState.Warning;
                    compatDevicesCard.Visibility = Visibility.Visible;
                }
            }

            if ( compatibilityString.Length == 0 )
                compatibilityString.Append(Localisation.Device_NotDetected);

            compatDevicesDescription.Text = compatibilityString.ToString();
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
                NavigateUri = new Uri(Util.GenerateDocsURL("playspace")),
                Foreground = WindowsColorHelpers.AccentLight,
            };
            minPlayspaceSizeHyperLink.Inlines.Add(compatibleDeviceDescriptionStringHyperlink);
            // Disable tabbing if not link
            if ( compatibleDeviceDescriptionStringHyperlink.Length == 0 )
                minPlayspaceSizeHyperLink.Focusable = false;
            minPlayspaceSizeHyperLink.RequestNavigate += Hyperlink_RequestNavigate;
            minPlayspaceSizeDescription.Inlines.Add(minPlayspaceSizeHyperLink);
            minPlayspaceSizeDescription.Inlines.Add(compatibleDeviceDescriptionStringLastPart);

            minPlayspaceSize.State = minAxis == 0 ? Controls.TaskState.Warning :
                ( minAxis < Constants.MinimumPlayspaceSize ? Controls.TaskState.Question : Controls.TaskState.Checkmark );
        }

        private string GetVRHeadsetString() {

            string vrConnectionType = OpenVRUtil.ConnectionType.ToString();

            switch ( OpenVRUtil.ConnectionType ) {
                case VRConnectionType.ALVR:
                    return "ALVR";
                case VRConnectionType.VirtualDesktop:
                    vrConnectionType = "Virtual Desktop Streamer";
                    break;
                case VRConnectionType.OculusLink:
                    vrConnectionType = "Oculus Link";
                    break;
                case VRConnectionType.OculusAirLink:
                    vrConnectionType = "Oculus Air Link";
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
                    headsetString = string.Format(Localisation.SystemRequirement_Description_Headset_Via, "Meta Quest", vrConnectionType);
                    break;
                case VRHmdType.Quest2:
                    headsetString = string.Format(Localisation.SystemRequirement_Description_Headset_Via, "Meta Quest 2", vrConnectionType);
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
                    return string.Format(Localisation.SystemRequirement_Description_Headset_UsingLighthouse, headsetString);
                } else {
                    return headsetString;
                }
            }

            if ( OpenVRUtil.HmdType == VRHmdType.Unknown ) {
                return Localisation.SystemRequirement_Description_Headset_Not_Detected;
            }

            return $"{string.Format(Localisation.SystemRequirement_Description_Headset_Via, OpenVRUtil.HmdType.ToString(), OpenVRUtil.ConnectionType.ToString())} ({string.Format(Localisation.SystemRequirement_Description_Headset_TrackingUnder, OpenVRUtil.TrackingType.ToString())})";
        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(true);
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {}
        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Process.Start(e.Uri.ToString());
        }

        private void ActionButtonPrimary_Click(object sender, RoutedEventArgs e) {
            Util.HandleKeyboardFocus(e);

            if ( MainWindow.HandleSpeedrun() ) {
                if ( canContinue ) {
                    // Advance to next page
                    MainWindow.Instance.SetPage(InstallerState.Downloading);
                    SoundPlayer.PlaySound(SoundEffect.MoveNext);
                } else {
                    // @TODO: Ella pls tell me how to handle the UX part of this I'm not sure if straight up exiting is a good idea
                    MainWindow.Instance.Close();
                }
            }
        }
    }
}
