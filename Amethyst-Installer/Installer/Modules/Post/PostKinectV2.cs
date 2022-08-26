using amethyst_installer_gui.Controls;
using System.Diagnostics;
using System.Windows;

namespace amethyst_installer_gui.Installer.Modules {
    public class PostKinectV2 : PostBase {
        public override void OnPostOperation(ref InstallModuleProgress control) {

            CheckMicrophone(ref control);

        }

        private void CheckMicrophone(ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.CheckingKinectMicrophone);
            Logger.Info(LogStrings.CheckingKinectMicrophone);

            if ( KinectUtil.KinectV2MicrophonePresent() ) {

                control.LogInfo(LogStrings.KinectV2MicrophoneFound);
                Logger.Info(LogStrings.KinectV2MicrophoneFound);

                if ( KinectUtil.KinectV2MicrophoneDisabled() ) {

                    control.LogInfo(LogStrings.KinectMicrophoneDisabled);
                    Logger.Info(LogStrings.KinectMicrophoneDisabled);

                    Util.ShowMessageBox(Localisation.PostOp_Kinect_EnableMic_Description, Localisation.PostOp_Kinect_EnableMic_Title, MessageBoxButton.OK);

                    // Open sound control panel on the recording tab
                    // @TODO: See if automating this is possible
                    Process.Start("rundll32.exe", "shell32.dll,Control_RunDLL mmsys.cpl,,1");

                    // @TODO: I fucking hate microsoft
                    // https://www.codeproject.com/articles/31836/changing-your-windows-audio-device-programmaticall
                }
            }
        }
    }
}
