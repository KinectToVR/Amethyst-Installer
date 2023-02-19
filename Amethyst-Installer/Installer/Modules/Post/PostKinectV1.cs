using amethyst_installer_gui.Controls;
using amethyst_installer_gui.PInvoke;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
                    Thread.Sleep(100);
                    timer = 50000;
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

        #region E_NUI_NOTREADY

        private bool TryFixNotReady(ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.TestNotReady);
            Logger.Info(LogStrings.TestNotReady);

            // The fix
            bool result = true;
            if ( KinectUtil.MustFixNotReady() || InstallerStateManager.ForceFixNotReady) {

                control.LogInfo(LogStrings.NotReadyDetected);
                Logger.Info(LogStrings.NotReadyDetected);
                
                string pathToDriversInstaller = Path.Combine(Constants.AmethystTempDirectory, "AttachedContainer", "KinectDrivers-v1.8-x64.WHQL.msi");

                result |= CheckCoreIntegrity(ref control);
                result |= CheckMicrophone(ref control);
                result |= KinectUtil.PreFixUnknownDevices();
                result |= DumpDrivers(pathToDriversInstaller, ref control);
                result |= InstallDrivers(ref control);
                result |= KinectUtil.RescanDevices();
            }

            return result;
        }

        private bool CheckCoreIntegrity(ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.CheckingMemoryIntegrity);
            Logger.Info(LogStrings.CheckingMemoryIntegrity);

            // Check if Memory Integrity is enabled, and if so, early exit
            // (We don't know if this is going to be fixed until we restart the machine, let the user know that a restart is required!)
            if ( KinectUtil.IsMemoryIntegrityEnabled() ) {

                control.LogError(LogStrings.MemoryIntegrityEnabled);
                Logger.Error(LogStrings.MemoryIntegrityEnabled);

                Util.ShowMessageBox(Localisation.Manager.MustDisableMemoryIntegrity_Description, Localisation.Manager.MustDisableMemoryIntegrity_Title);

                // Open Windows Security on the Core Isolation page
                Process.Start("windowsdefender://coreisolation");

                return false;
            }

            if ( KinectUtil.IsCameraDriverFailing()) {

                // @TODO: Change strings?
                control.LogError(LogStrings.MemoryIntegrityEnabled);
                Logger.Error(LogStrings.MemoryIntegrityEnabled);

                Util.ShowMessageBox(Localisation.Manager.MustDisableMemoryIntegrity_Description, Localisation.Manager.MustDisableMemoryIntegrity_Title);

                // Open Windows Security on the Core Isolation page
                Process.Start("windowsdefender://coreisolation");

                return false;
            }

            return true;
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

        private bool DumpDrivers(string sourceFile, ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.DumpingDrivers);
            Logger.Info(LogStrings.DumpingDrivers);

            try {

                // Execute on file first
                string darkExecutablePath = Path.GetFullPath(Path.Combine(
                        Constants.AmethystTempDirectory,
                        (string)InstallerStateManager.API_Response.Modules[InstallerStateManager.ModuleIdLUT["wix"]].Install.Items[0],
                        "dark.exe"));

                string inputFileFullPath = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, sourceFile));

                Logger.Info(string.Format(LogStrings.ExtractingDark, sourceFile));
                control.LogInfo(string.Format(LogStrings.ExtractingDark, sourceFile));

                // dark.exe {sourceFile} -x {outDir}
                var procStart = new ProcessStartInfo() {
                    FileName = darkExecutablePath,
                    WorkingDirectory = Constants.AmethystTempDirectory,
                    Arguments = $"\"{inputFileFullPath}\" -x \"{Constants.AmethystTempDirectory}\"",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,

                    // Verbose error handling
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = false,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                };
                Logger.Info($"Starting dark.exe with params:\n{procStart.Arguments}");
                var proc = Process.Start(procStart);
                // Redirecting process output so that we can log what happened
                StringBuilder stdout = new StringBuilder();
                StringBuilder stderr = new StringBuilder();
                proc.OutputDataReceived += (sender, args) => {
                    if ( args.Data != null )
                        stdout.AppendLine(args.Data);
                };
                proc.ErrorDataReceived += (sender, args) => {
                    if ( args.Data != null )
                        stderr.AppendLine(args.Data);
                };
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
                proc.WaitForExit(10000);

                if ( stdout.Length > 0 )
                    Logger.Info(stdout.ToString().Trim());

                // https://github.com/wixtoolset/wix3/blob/6b461364c40e6d1c487043cd0eae7c1a3d15968c/src/tools/dark/dark.cs#L54
                // Exit codes for DARK:
                // 
                // 0 - Success
                // 1 - Error
                Logger.Info(string.Format(LogStrings.DarkExitCode, proc.ExitCode));
                if ( proc.ExitCode == 1 ) {
                    // Assume WiX failed
                    if ( stderr.Length > 0 )
                        Logger.Fatal(stderr.ToString().Trim());
                    Logger.Fatal($"{string.Format(LogStrings.FailedExtractDark, sourceFile)}!");
                    control.LogError($"{string.Format(LogStrings.FailedExtractDark, sourceFile)}! {LogStrings.ViewLogs}");
                    return false;
                } else {
                    Logger.Info(string.Format(LogStrings.ExtractDarkSuccess, sourceFile));
                    control.LogInfo(string.Format(LogStrings.ExtractDarkSuccess, sourceFile));
                }
            } catch ( Exception e ) {

                Logger.Fatal($"{string.Format(LogStrings.FailedExtractDark, sourceFile)}:\n{Util.FormatException(e)})");
                control.LogError($"{string.Format(LogStrings.FailedExtractDark, sourceFile)}! {LogStrings.ViewLogs}");
                return false;
            }

            return true;
        }

        private bool InstallDrivers(ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.InstallingDrivers);
            Logger.Info(LogStrings.InstallingDrivers);

            string pathToDriversDirectory = Path.Combine(Constants.AmethystTempDirectory, "File");
            string driverTemp = Path.Combine(Constants.AmethystTempDirectory, "File", "Temp");

            Directory.CreateDirectory(driverTemp);

            // Device Driver
            {
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Device_cat"),            Path.Combine(driverTemp, "kinect.cat"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Device_inf"),            Path.Combine(driverTemp, "kinectdevice.inf"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Device_WdfCo"),          Path.Combine(driverTemp, "WdfCoInstaller01009.dll"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Device_WinUsbCo"),       Path.Combine(driverTemp, "WinUSBCoInstaller.dll"));

                control.LogInfo(LogStrings.InstallDeviceDriver);
                Logger.Info(LogStrings.InstallDeviceDriver);
                KinectUtil.AssignDriverToDeviceId("USB\\VID_045E&PID_02B0&REV_0107", Path.Combine(driverTemp, "kinectdevice.inf"));
                control.LogInfo(LogStrings.InstallDeviceDriverSuccess);
                Logger.Info(LogStrings.InstallDeviceDriverSuccess);

                File.Move(Path.Combine(driverTemp, "kinect.cat"),                               Path.Combine(pathToDriversDirectory, "Driver_Device_cat"));
                File.Move(Path.Combine(driverTemp, "kinectdevice.inf"),                         Path.Combine(pathToDriversDirectory, "Driver_Device_inf"));
                File.Move(Path.Combine(driverTemp, "WdfCoInstaller01009.dll"),                  Path.Combine(pathToDriversDirectory, "Driver_Device_WdfCo"));
                File.Move(Path.Combine(driverTemp, "WinUSBCoInstaller.dll"),                    Path.Combine(pathToDriversDirectory, "Driver_Device_WinUsbCo"));
            }

            // Audio Driver
            {
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Audio_cat"),             Path.Combine(driverTemp, "kinect.cat"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Audio_inf"),             Path.Combine(driverTemp, "kinectaudio.inf"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Audio_WdfCo"),           Path.Combine(driverTemp, "WdfCoInstaller01009.dll"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Audio_WinUsbCo"),        Path.Combine(driverTemp, "WinUSBCoInstaller.dll"));

                control.LogInfo(LogStrings.InstallAudioDriver);
                Logger.Info(LogStrings.InstallAudioDriver);
                SetupApi.InstallDriverFromInf(Path.Combine(driverTemp, "kinectaudio.inf"));
                control.LogInfo(LogStrings.InstallAudioDriverSuccess);
                Logger.Info(LogStrings.InstallAudioDriverSuccess);

                File.Move(Path.Combine(driverTemp, "kinect.cat"),                               Path.Combine(pathToDriversDirectory, "Driver_Audio_cat"));
                File.Move(Path.Combine(driverTemp, "kinectaudio.inf"),                          Path.Combine(pathToDriversDirectory, "Driver_Audio_inf"));
                File.Move(Path.Combine(driverTemp, "WdfCoInstaller01009.dll"),                  Path.Combine(pathToDriversDirectory, "Driver_Audio_WdfCo"));
                File.Move(Path.Combine(driverTemp, "WinUSBCoInstaller.dll"),                    Path.Combine(pathToDriversDirectory, "Driver_Audio_WinUsbCo"));
            }

            // Audio Array Driver
            {
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_AudioArray_cat"),        Path.Combine(driverTemp, "kinect.cat"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_AudioArray_inf"),        Path.Combine(driverTemp, "kinectaudioarray.inf"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_AudioArray_WdfCo"),      Path.Combine(driverTemp, "WdfCoInstaller01009.dll"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_AudioArray_WinUsbCo"),   Path.Combine(driverTemp, "WinUSBCoInstaller.dll"));

                control.LogInfo(LogStrings.InstallAudioArrayDriver);
                Logger.Info(LogStrings.InstallAudioArrayDriver);
                KinectUtil.AssignDriverToDeviceId("USB\\VID_045E&PID_02BB&REV_0100&MI_00", Path.Combine(driverTemp, "kinectaudioarray.inf"));
                control.LogInfo(LogStrings.InstallAudioArrayDriverSuccess);
                Logger.Info(LogStrings.InstallAudioArrayDriverSuccess);

                File.Move(Path.Combine(driverTemp, "kinect.cat"),                               Path.Combine(pathToDriversDirectory, "Driver_AudioArray_cat"));
                File.Move(Path.Combine(driverTemp, "kinectaudioarray.inf"),                     Path.Combine(pathToDriversDirectory, "Driver_AudioArray_inf"));
                File.Move(Path.Combine(driverTemp, "WdfCoInstaller01009.dll"),                  Path.Combine(pathToDriversDirectory, "Driver_AudioArray_WdfCo"));
                File.Move(Path.Combine(driverTemp, "WinUSBCoInstaller.dll"),                    Path.Combine(pathToDriversDirectory, "Driver_AudioArray_WinUsbCo"));
            }

            // Camera Driver
            {
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Camera_cat"),            Path.Combine(driverTemp, "kinect.cat"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Camera_inf"),            Path.Combine(driverTemp, "kinectcamera.inf"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Camera_sys"),            Path.Combine(driverTemp, "kinectcamera.sys"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Camera_WdfCo"),          Path.Combine(driverTemp, "WdfCoInstaller01009.dll"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Camera_WinUsbCo"),       Path.Combine(driverTemp, "WinUSBCoInstaller.dll"));

                control.LogInfo(LogStrings.InstallCameraDriver);
                Logger.Info(LogStrings.InstallCameraDriver);
                KinectUtil.AssignDriverToDeviceId("USB\\VID_045E&PID_02AE&REV_010;", Path.Combine(driverTemp, "kinectcamera.inf"));
                control.LogInfo(LogStrings.InstallCameraDriverSuccess);
                Logger.Info(LogStrings.InstallCameraDriverSuccess);

                File.Move(Path.Combine(driverTemp, "kinect.cat"),                               Path.Combine(pathToDriversDirectory, "Driver_Camera_cat"));
                File.Move(Path.Combine(driverTemp, "kinectcamera.inf"),                         Path.Combine(pathToDriversDirectory, "Driver_Camera_inf"));
                File.Move(Path.Combine(driverTemp, "kinectcamera.sys"),                         Path.Combine(pathToDriversDirectory, "Driver_Camera_sys"));
                File.Move(Path.Combine(driverTemp, "WdfCoInstaller01009.dll"),                  Path.Combine(pathToDriversDirectory, "Driver_Camera_WdfCo"));
                File.Move(Path.Combine(driverTemp, "WinUSBCoInstaller.dll"),                    Path.Combine(pathToDriversDirectory, "Driver_Camera_WinUsbCo"));
               }

            // Security Driver
            {
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Security_cat"),          Path.Combine(driverTemp, "kinect.cat"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Security_inf"),          Path.Combine(driverTemp, "kinectsecurity.inf"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Security_WdfCo"),        Path.Combine(driverTemp, "WdfCoInstaller01009.dll"));
                File.Move(Path.Combine(pathToDriversDirectory, "Driver_Security_WinUsbCo"),     Path.Combine(driverTemp, "WinUSBCoInstaller.dll"));

                control.LogInfo(LogStrings.InstallSecurityDriver);
                Logger.Info(LogStrings.InstallSecurityDriver);
                KinectUtil.AssignDriverToDeviceId("USB\\VID_045E&PID_02BB&REV_0100&MI_01", Path.Combine(driverTemp, "kinectsecurity.inf"));
                control.LogInfo(LogStrings.InstallSecurityDriverSuccess);
                Logger.Info(LogStrings.InstallSecurityDriverSuccess);

                File.Move(Path.Combine(driverTemp, "kinect.cat"),                               Path.Combine(pathToDriversDirectory, "Driver_Security_cat"));
                File.Move(Path.Combine(driverTemp, "kinectsecurity.inf"),                       Path.Combine(pathToDriversDirectory, "Driver_Security_inf"));
                File.Move(Path.Combine(driverTemp, "WdfCoInstaller01009.dll"),                  Path.Combine(pathToDriversDirectory, "Driver_Security_WdfCo"));
                File.Move(Path.Combine(driverTemp, "WinUSBCoInstaller.dll"),                    Path.Combine(pathToDriversDirectory, "Driver_Security_WinUsbCo"));
            }

            // Microphone driver
            control.LogInfo(LogStrings.AssignMicrophoneDriver);
            Logger.Info(LogStrings.AssignMicrophoneDriver);
            KinectUtil.AssignGenericAudioDriver();
            control.LogInfo(LogStrings.AssignMicrophoneDriverSuccess);
            Logger.Info(LogStrings.AssignMicrophoneDriverSuccess);
            // We don't assign the endpoint driver because it's device ID is VERY GENERIC ( MMDEVAPI\AudioEndpoints )

            return true;
        }

        #endregion
    }
}
