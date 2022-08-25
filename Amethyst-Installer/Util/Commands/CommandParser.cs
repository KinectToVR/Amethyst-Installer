using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
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
        --silent -s :: Executes the installer silently
        --install-dir :: Sets the install directory, 
        --debug :: Forces the installer to run in Debug mode

         */

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
        public void ParseCommands(string[] args) {
            for ( int i = 0; i < args.Length; i++ ) {
                // Check if this item matches a command or not
                if ( IsCommand(args[i], out string cmd) ) {

                    bool hasExecutedAnything = false;

                    // For each command
                    for ( int j = 0; j < m_commandList.Length; j++ ) {

                        if ( ShouldExecute(m_commandList[j], cmd) ) {
                            hasExecutedAnything = true;
                            m_commandList[j].Execute();
                            return;
                        }
                    }
                    if ( !hasExecutedAnything ) {
                        Console.WriteLine($"Unknown command \"{cmd}\"!");
                    }
                }
            }
        }

        /// <summary>
        /// Returns whether the input string is a command, and a formatted command (without a command prefix) if valid
        /// </summary>
        /// <param name="input">The input parameter</param>
        /// <param name="formattedCommand">The command itself</param>
        /// <returns>Whether the input is a valid command or not</returns>
        private bool IsCommand(string input, out string formattedCommand) {

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
            return formattedCommand.Length > 0;
        }

        /// <summary>
        /// Returns whether the command should be executed or not
        /// </summary>
        /// <param name="command">The command to check</param>
        /// <param name="cmd">A formatted command string</param>
        /// <returns>Whether the command should execute or not</returns>
        private bool ShouldExecute(ICommand command, string cmd) {

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
    }
}
