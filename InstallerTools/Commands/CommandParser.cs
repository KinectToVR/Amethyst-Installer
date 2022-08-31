using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace InstallerTools.Commands {
    public class CommandParser {

        private ICommand[] m_commandList;
        private Type[] m_types;

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

        public CommandParser() {
            // Init command list
            try {
                m_types = Assembly.GetExecutingAssembly().GetTypes();
            } catch ( ReflectionTypeLoadException e ) {
                m_types = e.Types.Where(t => t != null).ToArray();
            }
            m_types = m_types.Where(typeof(ICommand).IsAssignableFrom).ToArray();

            // Init command list from above list
            m_commandList = new ICommand[m_types.Length - 1]; // subtract 1 because the interface itself is to be excluded
            for ( int i = 0; i < m_types.Length; i++ ) {
                // Can't implement the interface itself, skip it!
                if ( m_types[i] == typeof(ICommand) )
                    continue;
                m_commandList[i] = ( ICommand ) Activator.CreateInstance(m_types[i]);
            }
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
                    if ( cmd == "help" || cmd == "h" ) {
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

            // @HACK: MAKE THIS COMMAND ACTUALLY HANDLE MULTILINE PROPERLY

            Console.WriteLine("--help, -h\t\tShows this message");
            Console.WriteLine("--uninstalllist, -ul\tGenerates a JSON file of uninstallable items; Saves to ./list.json");
            Console.WriteLine("--checksum, -c\t\tComputes the MD5 checksum of the given file");

            return;

            var assemblyName = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title.ToUpperInvariant() + " - HELP";
            int minPaddingBetweenCommandAndDescription = 6;
            int windowWidth = Math.Max(63, assemblyName.Length);
            int borderPaddingX = 2;
            int borderPaddingY = 1;

            var commandList = new List<(string commandStr, string description)>();

            // Border top
            Console.Write("  ╭");
            Console.Write(new string('─', windowWidth - 2));
            Console.WriteLine('╮');

            // Whitespace line
            for ( int i = 0; i < borderPaddingY; i++ ) {
                Console.Write("  |");
                Console.Write(new string(' ', 2));
                Console.Write(new string(' ', windowWidth - 4));
                Console.WriteLine('|');
            }

            // Title line
            Console.Write("  |");
            Console.Write(new string(' ', 2));
            Console.Write(assemblyName);
            Console.Write(new string(' ', windowWidth - 4 - assemblyName.Length));
            Console.WriteLine('|');

            // Whitespace line
            Console.Write("  |");
            Console.Write(new string(' ', windowWidth - 2));
            Console.WriteLine('|');

            StringBuilder line = new StringBuilder();
            int maxLength = 0;
            // Get longest length of a command
            foreach ( var command in m_commandList ) {
                maxLength = Math.Max(maxLength, command.Command.Length + command.Aliases.Length * 4);
            }
            // Spit out commands
            maxLength += minPaddingBetweenCommandAndDescription;
            int whitespaceFirst;
            foreach ( var command in m_commandList ) {
                line.Clear();
                line.Append("  |");
                line.Append(new string(' ', borderPaddingX));
                line.Append("--");
                line.Append(command.Command);
                foreach ( var alias in command.Aliases ) {
                    line.Append(", -");
                    line.Append(alias);
                }
                whitespaceFirst = Math.Max(minPaddingBetweenCommandAndDescription, maxLength - command.Command.Length - command.Aliases.Length * 4);
                line.Append(new string(' ', whitespaceFirst));
                line.Append(command.Description);
                line.Append(new string(' ', windowWidth - borderPaddingX - 4 - whitespaceFirst - command.Command.Length - command.Aliases.Length * 4 - command.Description.Length));
                line.Append("|");
                Console.WriteLine(line.ToString());
            }

            // Whitespace line
            for ( int i = 0; i < borderPaddingY; i++ ) {
                Console.Write("  |");
                Console.Write(new string(' ', 2));
                Console.Write(new string(' ', windowWidth - 4));
                Console.WriteLine('|');
            }

            // Bottom border
            Console.Write("  ╘");
            Console.Write(new string('═', windowWidth - 2));
            Console.WriteLine('╛');
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
            if ( args.Length - index < 2 ) {
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

    // Stub for auto-gen
    public class CommandHelp : ICommand {
        public string Command { get => "help"; set { } }
        public string Description { get => "Shows this command"; set { } }
        public string[] Aliases { get => new string[] { "h" }; set { } }

        public bool Execute(string parameters) {
            return true;
        }
    }

}
