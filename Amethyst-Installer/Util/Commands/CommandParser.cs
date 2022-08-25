using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Commands {
    public static class CommandParser {

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

        private static bool continueExecution = true;

        private static Parser s_parser;
        private static Type[] s_types;

        /// <summary>
        /// Parses a given series of commands
        /// </summary>
        /// <param name="args">Array of paremters, typically from Main's string[] args</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ParseCommands(string[] args) {

            s_types = LoadVerbs();

            s_parser = new Parser(with => {
                with.HelpWriter = null;
            });
            var parserResult = s_parser.ParseArguments(args, s_types);
            parserResult
                .WithParsed(Exec)
                .WithNotParsed(e => HandleError(parserResult, e));

            return continueExecution;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Exec(object obj) {
            if ( obj is ICommand ) {
                ( ( ICommand ) obj ).Execute();
                continueExecution = false;
            }
        }

        /// <summary>
        /// Handling command errors, handles showing help / version or defaulting to no args workflow
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object HandleError(ParserResult<object> parserResult, IEnumerable<Error> errors) {
            // If we have a command
            if ( errors != null && errors.Any() ) {
                Error firstError = errors.First();

                // Switch on error type...
                // This syntax is ugly, but "actually decent command parser!"
                switch ( firstError.GetType() ) {

                    // No Verb :: array is empty
                    case Type nv when nv == typeof(NoVerbSelectedError):
                        ExecuteNoCommand();
                        break;

                    // Invalid command
                    case Type bv when bv == typeof(BadVerbSelectedError):
                        // If verb has any length, it means you are likely trying to use the CLI, so assume 0 length is no command
                        if ( ( ( CommandLine.BadVerbSelectedError ) firstError ).Token.Trim().Length > 0 ) {
                            DisplayHelp(parserResult);
                        } else {
                            ExecuteNoCommand();
                        }
                        break;

                    // Help requested command
                    case Type hv1 when hv1 == typeof(HelpVerbRequestedError):
                    case Type hv2 when hv2 == typeof(HelpRequestedError):
                        DisplayHelp(parserResult);
                        break;

                    // Version requested command
                    case Type hv1 when hv1 == typeof(VersionRequestedError):
                        string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                        Console.WriteLine($"Amethyst Installer Tools v{version.Remove(version.Length - 2)}");
                        break;

                    // Other cases
                    default:
                        ExecuteNoCommand();
                        break;
                }

            } else {
                ExecuteNoCommand();
            }

            return errors;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DisplayHelp<T>(ParserResult<T> result) {
            HelpText helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.AddDashesToOption = true;
                h.Heading = "Amethyst Installer";
                h.Copyright = "";
                return h;
            } , e => e, verbsIndex:true);
            Console.WriteLine(helpText);
            continueExecution = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Type[] LoadVerbs() {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ExecuteNoCommand() {
            continueExecution = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HaltProgram() {
            continueExecution = false;
        }
    }
}
