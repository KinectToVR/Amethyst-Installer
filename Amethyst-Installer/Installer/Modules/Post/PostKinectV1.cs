﻿using amethyst_installer_gui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

            if ( KinectUtil.KinectV2MicrophonePresent() ) {

                control.LogInfo(LogStrings.KinectV1MicrophoneFound);
                Logger.Info(LogStrings.KinectV1MicrophoneFound);

                if ( KinectUtil.KinectV2MicrophoneDisabled() ) {

                    control.LogInfo(LogStrings.KinectMicrophoneDisabled);
                    Logger.Info(LogStrings.KinectMicrophoneDisabled);

                    Util.ShowMessageBox(Localisation.PostOp_Kinect_EnableMic_Description, Localisation.PostOp_Kinect_EnableMic_Title, MessageBoxButton.OK);

                    // Open sound control panel on the recording tab
                    // @TODO: See if automating this is possible
                    Process.Start("rundll32.exe", "shell32.dll,Control_RunDLL mmsys.cpl,,1");
                }
            }
        }
        

        private void TryFixNotPowered(ref InstallModuleProgress control) {

            control.LogInfo(LogStrings.CheckingKinectMicrophone);
            Logger.Info(LogStrings.CheckingKinectMicrophone);

            Logger.Info("KinectUtil.MustFixNotPowered: " + KinectUtil.MustFixNotPowered());

            KinectUtil.FixNotPowered();

        }


    }
}