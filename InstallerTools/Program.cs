using CommandLine;
using CommandLine.Text;
using GenUninstallList.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GenUninstallList {

    internal class Program {

        private static Parser s_parser;
        private static Type[] s_types;

        static void Main(string[] args) {

            s_types = Util.LoadVerbs();

            s_parser = new Parser(with => {
                with.HelpWriter = null;
            });
            var parserResult = s_parser.ParseArguments(args, s_types);
            parserResult
                .WithParsed(Run)
                .WithNotParsed(e => HandleError(parserResult, e));

            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }

        private static void Run(object obj) {
            switch ( obj ) {
                case ChecksumOptions c:
                    CommandChecksum.Execute(c);
                    break;
            }
        }

        /// <summary>
        /// Handling command errors, handles showing help / version or defaulting to no args workflow
        /// </summary>
        private static object HandleError(ParserResult<object> parserResult, IEnumerable<Error> errors) {
            // If we have a command
            if ( errors.IsAny() ) {
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

        static void DisplayHelp<T>(ParserResult<T> result) {
            HelpText helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Heading = "Amethyst Installer Tools";
                h.Copyright = "";
                return HelpText.DefaultParsingErrorsHandler(result, h);
            } , e => e);
            Console.WriteLine(helpText);
        }

        public static void ExecuteNoCommand() {

        }
    }
}
