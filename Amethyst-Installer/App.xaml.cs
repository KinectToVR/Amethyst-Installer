using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace amethyst_installer_gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        const string KinectV1MicrophoneFriendlyName = "Kinect USB Audio";
        const string KinectV2MicrophoneFriendlyName = "Xbox NUI Sensor";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MessageBox.Show("big chungus", "no way");
            // Check if the Kinect microphone is muted, and if so, prompt the user to enable it.
            var enumerator = new MMDeviceEnumerator();
            foreach (MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All))
            {
                // Skip devices which aren't plugged in (otherwise we'd get a COM Exception upon querying their friendly names)
                if (wasapi.State == DeviceState.NotPresent)
                    continue;
                // Skip "Render" devices, i.e. Playback devices like headphone, speakers, etc.
                if (wasapi.DataFlow == DataFlow.Render)
                    continue;
                
                if (wasapi.DeviceFriendlyName == KinectV1MicrophoneFriendlyName || wasapi.DeviceFriendlyName == KinectV2MicrophoneFriendlyName)
                {
                    // Debug
                    Console.WriteLine($"DataFlow: { wasapi.DataFlow } FriendlyName: { wasapi.FriendlyName } DeviceFriendlyName: { wasapi.DeviceFriendlyName } State: { wasapi.State } ID: { wasapi.ID }");

                    if (wasapi.State != DeviceState.Active)
                    {
                        // Open sound control panel on the recording tab
                        System.Diagnostics.Process.Start("rundll32.exe", "shell32.dll,Control_RunDLL mmsys.cpl,,1");
                    }
                }
            }
        }
    }
}
