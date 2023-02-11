using amethyst_installer_gui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer.Modules {
    public class PsmsModule : ModuleBase {
        public override bool Install(string sourceFile, string path, ref InstallModuleProgress control, out TaskState state) {

            // Extract PSMSEX to %AME_DIR%\PSMSEX\PSMS

            string archivePath = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, sourceFile));

            // Extract PSMSEX to it
            Logger.Info(string.Format(LogStrings.ExtractingArchive, sourceFile));
            control.LogInfo(string.Format(LogStrings.ExtractingArchive, sourceFile));

            if ( !InstallUtil.ExtractZipToDirectory(archivePath, Path.GetFullPath(Path.Combine(path, "PSMSEX"))) ) {
                // Failed to extract ZIP! Abort!
                Logger.Fatal($"{string.Format(LogStrings.FailedExtractArchiveDiskFull, sourceFile)}!)");
                control.LogError($"{string.Format(LogStrings.FailedExtractArchiveDiskFull, sourceFile)}! {LogStrings.ViewLogs}");

                state = TaskState.Error;
                return false;
            }

            Logger.Info(string.Format(LogStrings.ExtractingArchiveSuccess, sourceFile));
            control.LogInfo(string.Format(LogStrings.ExtractingArchiveSuccess, sourceFile));

            // Idk do I make shortcuts or something?


            state = TaskState.Checkmark;
            return true;
        }

        // @TODO: Nuke %Appdata%\PSMoveService
    }
}
