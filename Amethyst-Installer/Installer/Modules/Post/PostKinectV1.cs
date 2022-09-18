using amethyst_installer_gui.Controls;
using amethyst_installer_gui.PInvoke;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace amethyst_installer_gui.Installer.Modules {
    public class PostKinectV1 : PostBase {

        // Wait until a USB device shows up, or for 30 seconds so that the Kinect devices will show up, so that we can then check E_NUI_NOTPOWERED
        private static bool doWait = false;
        private static Task task;

        public static void Listen() {
            if ( doWait )
                return;

            doWait = true;

            task = Task.Run(() => {
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
        }

        public override void OnPostOperation(ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.ApplyingKinectFixes);
            Logger.Info(LogStrings.ApplyingKinectFixes);

            while ( doWait ) { }
            task.Dispose();

            // Not powered fix
            TryFixNotPowered(ref control);

            // Not ready fix
            TryFixNotReady(ref control);
        }

        private void TryFixNotReady(ref InstallModuleProgress control) {

            // The fix
            if ( KinectUtil.MustFixNotPowered() ) {

                control.LogInfo(LogStrings.NotReadyDetected);
                Logger.Info(LogStrings.NotReadyDetected);


                CheckMicrophone(ref control);
            }
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

                    KinectUtil.FixMicrophoneV1();

                }
            }
        }

        private void TryFixNotPowered(ref InstallModuleProgress control) {
            // The fix
            if ( KinectUtil.MustFixNotPowered() ) {

                control.LogInfo(LogStrings.NotPoweredDetected);
                Logger.Info(LogStrings.NotPoweredDetected);

                if ( KinectUtil.FixNotPowered() ) {
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
