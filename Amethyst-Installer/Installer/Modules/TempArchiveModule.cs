using amethyst_installer_gui.Controls;
using System;
using System.IO.Compression;
using System.IO;

namespace amethyst_installer_gui.Installer.Modules {
    public class TempArchiveModule : ModuleBase {
        public override bool Install(string sourceFile, string path, ref InstallModuleProgress control, out TaskState state) {

            try {

                Logger.Info(string.Format(LogStrings.ExtractingArchive, sourceFile));
                control.LogInfo(string.Format(LogStrings.ExtractingArchive, sourceFile));

                string finalDirName = ( string ) Module.Install.Items[0];

                string sourceZip = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, sourceFile));
                string tempDirectory = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, finalDirName));

                if ( File.Exists(sourceZip) ) {

                    if ( !Directory.Exists(tempDirectory) )
                        Directory.CreateDirectory(tempDirectory);

                    ZipFile.ExtractToDirectory(sourceZip, tempDirectory);

                    Logger.Info(string.Format(LogStrings.ExtractingArchiveSuccess, sourceFile));
                    control.LogInfo(string.Format(LogStrings.ExtractingArchiveSuccess, sourceFile));
                    state = TaskState.Checkmark;
                    return true;
                }

            } catch ( Exception e ) {
                Logger.Fatal($"{string.Format(LogStrings.FailedExtractArchive, sourceFile)}:\n{Util.FormatException(e)})");
                control.LogError($"{string.Format(LogStrings.FailedExtractArchive, sourceFile)}! {LogStrings.ViewLogs}");
            }

            state = TaskState.Error;
            return false;
        }
    }
}
