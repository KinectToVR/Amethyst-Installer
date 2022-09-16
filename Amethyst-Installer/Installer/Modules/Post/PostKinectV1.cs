using amethyst_installer_gui.Controls;
using amethyst_installer_gui.PInvoke;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace amethyst_installer_gui.Installer.Modules {
    public class PostKinectV1 : PostBase {
        public override void OnPostOperation(ref InstallModuleProgress control) {

            CheckMicrophone(ref control);

            // Not powered fix
            TryFixNotPowered(ref control);
        }

        private void CheckMicrophone(ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.CheckingKinectMicrophone);
            Logger.Info(LogStrings.CheckingKinectMicrophone);

            if ( KinectUtil.KinectV1MicrophonePresent() ) {

                control.LogInfo(LogStrings.KinectV1MicrophoneFound);
                Logger.Info(LogStrings.KinectV1MicrophoneFound);

                if ( KinectUtil.KinectV1MicrophoneDisabled() ) {

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
        

        private void TryFixNotPowered(ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.ApplyingKinectFixes);
            Logger.Info(LogStrings.ApplyingKinectFixes);

            // Wait until a USB device shows up, or for 30 seconds so that the Kinect devices will show up, so that we can then check E_NUI_NOTPOWERED
            bool doWait = true;

            var task = Task.Run(() => {
                var timer = new System.Timers.Timer(30000);
                timer.Elapsed += (sender, _) => {
                    doWait = false;
                    timer.Stop();
                    timer.Dispose();
                };
                DeviceManaged.OnDeviceAdded += () => {
                    Thread.Sleep(1000);
                    doWait = false;
                    timer.Stop();
                    timer.Dispose();
                };
            });

            while ( doWait ) { }
            task.Dispose();

            // Cleanup
            DeviceManaged.OnDeviceAdded = null;

            // The fix
            if ( KinectUtil.MustFixNotPowered() ) {

                control.LogInfo(LogStrings.NotPoweredDetected);
                Logger.Info(LogStrings.NotPoweredDetected);

                if (KinectUtil.FixNotPowered()) {

                    control.LogInfo(LogStrings.NotPoweredFixed);
                    Logger.Info(LogStrings.NotPoweredFixed);
                } else {

                    control.LogError(LogStrings.NotPoweredFixFailure);
                    Logger.Fatal(LogStrings.NotPoweredFixFailure);
                }
            }

        }
    }
}
