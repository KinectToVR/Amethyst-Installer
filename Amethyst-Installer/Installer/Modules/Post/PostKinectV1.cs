using amethyst_installer_gui.Controls;
using amethyst_installer_gui.PInvoke;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace amethyst_installer_gui.Installer.Modules {
    public class PostKinectV1 : PostBase {

        public override void OnPostOperation(ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.ApplyingKinectFixes);
            Logger.Info(LogStrings.ApplyingKinectFixes);

            // Get Kinect Devices
            var deviceTree = new DeviceTree();
            int deviceCount = deviceTree.DeviceNodes.Where(d => d.ClassGuid == DeviceClasses.KinectForWindows).Count();
            deviceTree.Dispose();

            // If we don't have ALL Kinect devices available, wait until we have an update...
            if ( deviceCount < 4 ) {
                // Wait until a USB device shows up, or for 30 seconds so that the Kinect devices will show up, so that we can then check
                // for errors during the install and attempt to fix them
                int timer = 0;

                DeviceManaged.OnDeviceAdded += () => {
                    Thread.Sleep(1000);
                    timer = int.MaxValue;
                };

                while ( timer < 3000 ) {
                    Thread.Sleep(10);
                    timer++;
                }

                // Cleanup
                DeviceManaged.OnDeviceAdded = null;
            }

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
