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

            // Get Kinect Devices
            var deviceTree = new DeviceTree();
            int deviceCount = deviceTree.DeviceNodes.Where(d => d.ClassGuid == DeviceClasses.KinectForWindows).Count();
            deviceTree.Dispose();

            Logger.Info($"Found {deviceCount} Kinect devices");

            // If we don't have ALL Kinect devices available, wait until we have an update...
            if ( deviceCount < 4 ) {

                control.LogInfo(LogStrings.WaitingForDeviceApi);
                Logger.Info(LogStrings.WaitingForDeviceApi);
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

            control.LogInfo(LogStrings.ApplyingKinectFixes);
            Logger.Info(LogStrings.ApplyingKinectFixes);

            bool result = true;

            // Not powered fix
            result |= TryFixNotPowered(ref control);

            // Not ready fix
            result |= TryFixNotReady(ref control);

            if ( result ) {
                control.LogInfo(LogStrings.ApplyingKinectFixesSuccess);
                Logger.Info(LogStrings.ApplyingKinectFixesSuccess);
            } else {
                control.LogError($"{LogStrings.ApplyingKinectFixesFailure} {LogStrings.ViewLogs}");
                Logger.Info($"{LogStrings.ApplyingKinectFixesFailure} {LogStrings.ViewLogs}");
            }
        }

        private bool TryFixNotReady(ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.TestNotReady);
            Logger.Info(LogStrings.TestNotReady);

            // The fix
            bool result = true;
            if ( KinectUtil.MustFixNotPowered() ) {

                control.LogInfo(LogStrings.NotReadyDetected);
                Logger.Info(LogStrings.NotReadyDetected);

                result |= CheckMicrophone(ref control);
            }

            return result;
        }

        private bool CheckMicrophone(ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.CheckingKinectMicrophone);
            Logger.Info(LogStrings.CheckingKinectMicrophone);

            if ( KinectUtil.KinectV1MicrophonePresent() ) {

                control.LogInfo(LogStrings.KinectV1MicrophoneFound);
                Logger.Info(LogStrings.KinectV1MicrophoneFound);

                if ( KinectUtil.KinectV1MicrophoneDisabled() ) {

                    control.LogInfo(LogStrings.KinectMicrophoneDisabled);
                    Logger.Info(LogStrings.KinectMicrophoneDisabled);

                    return KinectUtil.FixMicrophoneV1();

                }
            }

            return true;
        }

        private bool TryFixNotPowered(ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.TestNotPowered);
            Logger.Info(LogStrings.TestNotPowered);

            // The fix
            if ( KinectUtil.MustFixNotPowered() ) {

                control.LogInfo(LogStrings.NotPoweredDetected);
                Logger.Info(LogStrings.NotPoweredDetected);

                if ( KinectUtil.FixNotPowered() ) {
                    control.LogInfo(LogStrings.NotPoweredFixed);
                    Logger.Info(LogStrings.NotPoweredFixed);
                    return true;
                } else {
                    control.LogError(LogStrings.NotPoweredFixFailure);
                    Logger.Fatal(LogStrings.NotPoweredFixFailure);
                    return false;
                }
            }

            return true;
        }
    }
}
