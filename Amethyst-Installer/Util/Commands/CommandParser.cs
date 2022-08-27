using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Commands {
    public class CommandParser {

        private ICommand[] m_commandList;

        /*

        COMMAND LINE ARGUMENTS:

        --help, -h :: Shows help
        --update -u :: Attempts to update Amethyst
        --uninstall -x :: Attempts to uninstall Amethyst
        --modify -m :: Attempts to modify an existing Amethyst install
        --silent -s :: Executes the installer silently
        --install-dir :: Sets the install directory, 
        --debug :: Forces the installer to run in Debug mode

         */

        const int MAX_WIDTH = 64;

        public CommandParser() {
            // Init command list
            m_commandList = new ICommand[] {
                new CommandUninstall(),
            };
        }

        /// <summary>
        /// Parses a given series of commands
        /// </summary>
        /// <param name="args">Array of paremters, typically from Main's string[] args</param>
        /// <returns>Whether regular execution of the program shall be interrupted.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ParseCommands(string[] args) {
            for ( int i = 0; i < args.Length; i++ ) {

                // Check if this item matches a command or not
                if ( IsCommand(ref args[i], out string cmd) ) {

                    // For each command
                    if (cmd == "help" || cmd == "h") {
                        ShowHelpMessage();
                        return true;
                    }
                    for ( int j = 0; j < m_commandList.Length; j++ ) {

                        if ( ShouldExecute(ref m_commandList[j], ref cmd) ) {
                            return m_commandList[j].Execute(ExtractParameters(ref args, i));
                        }
                    }
                    ShowErrorMessage(ref cmd);
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ShowHelpMessage() {

            // @TODO: idk what sort of formatting we want sooooo this is a stub
            // @TODO: Reflection to grab all commands and dynamically compute width and stuff etc

            // ╭──────╮
            // |      |
            // ╘══════╛

            /*

            ╭─────────────────────────────────────────────────────────────╮
            |                                                             |
            |   AMETHYST INSTALLER                                        |
            |                                                             |
            |   --help, -h          Shows help                            |
            |   --update -u         Attempts to update Amethyst           |
            |   --uninstall -x      Attempts to uninstall Amethyst        |
            |   --modify -m         Attempts to modify an existing        |
            |                       Amethyst install                      |
            |   --silent -s         Executes the installer silently       |
            |   --install-dir       Sets the install directory,           |
            |   --debug             Forces the installer to run in Debug  |
            |                       mode                                  |
            |                                                             |
            ╘═════════════════════════════════════════════════════════════╛

            */

            int maxCommandLength = 0;

            // @TODO: Implement fancy help

            Console.WriteLine("help message will eventually be in place of this");
            Console.WriteLine("\n--help\tShows this message");
            Console.WriteLine("--uninstall\tStarts uninstall flow");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ShowErrorMessage(ref string cmd) {
            Console.WriteLine($"Unknown command \"{cmd}\"!");
        }

        /// <summary>
        /// Returns whether the input string is a command, and a formatted command (without a command prefix) if valid
        /// </summary>
        /// <param name="input">The input parameter</param>
        /// <param name="formattedCommand">The command itself</param>
        /// <returns>Whether the input is a valid command or not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCommand(ref string input, out string formattedCommand) {

            formattedCommand = string.Empty;

            // --XXXXX
            if ( input.Length > 3 && input[0] == '-' && input[1] == '-' ) {

                formattedCommand = input.Substring(2);

            // -XXXXX
            } else if ( input.Length > 2 && input[0] == '-' ) {

                formattedCommand = input.Substring(1);

            // /XXXXX
            } else if ( input.Length > 2 && input[0] == '/' ) {

                formattedCommand = input.Substring(1);
            }
            formattedCommand = formattedCommand.ToLowerInvariant();
            return formattedCommand.Length > 0;
        }

        /// <summary>
        /// Returns whether the command should be executed or not
        /// </summary>
        /// <param name="command">The command to check</param>
        /// <param name="cmd">A formatted command string</param>
        /// <returns>Whether the command should execute or not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ShouldExecute(ref ICommand command, ref string cmd) {

            if ( command.Command == cmd ) {
                return true;
            } else {
                for ( int i = 0; i < command.Aliases.Length; i++ ) {
                    if ( command.Aliases[i] == cmd ) {
                        return true;
                    }
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ExtractParameters(ref string[] args, int index) {

            // If less than minimum parameters
            if (args.Length - index < 2 ) {
                return "";
            }

            // i + 1 is our first entry
            StringBuilder stringBuffer = new StringBuilder();
            for ( int i = index + 1; i < args.Length; i++ ) {
                stringBuffer.Append(args[i] + " ");
            }
            stringBuffer.Remove(stringBuffer.Length - 1, 1);

            return stringBuffer.ToString().Trim();
        }
    }
}
