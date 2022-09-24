using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace amethyst_installer_gui.Protocol {
    public static class ProtocolParser {

        private static IProtocolCommand[] m_commandList;
        private static Type[] m_types;

        static ProtocolParser() {
            // Init command list
            try {
                m_types = Assembly.GetExecutingAssembly().GetTypes();
            } catch ( ReflectionTypeLoadException e ) {
                m_types = e.Types.Where(t => t != null).ToArray();
            }
            m_types = m_types.Where(typeof(IProtocolCommand).IsAssignableFrom).ToArray();

            // Init command list from above list
            m_commandList = new IProtocolCommand[m_types.Length - 1]; // subtract 1 because the interface itself is to be excluded
            int indexer = 0;
            for ( int i = 0; i < m_types.Length; i++ ) {
                // Can't implement the interface itself, skip it!
                if ( m_types[i] == typeof(IProtocolCommand) )
                    continue;
                m_commandList[indexer] = ( IProtocolCommand ) Activator.CreateInstance(m_types[i]);
                indexer++;
            }
        }

        /// <summary>
        /// Parses a given series of commands
        /// </summary>
        /// <param name="args">Array of paremters, typically from Main's string[] args</param>
        /// <returns>Whether regular execution of the program shall be interrupted.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ParseCommands(string[] args) {

            // Is this even a protocol command?
            if ( args.Length == 0 || !args[0].ToLowerInvariant().StartsWith("amethyst://") ) {
                return false;
            }

            // Remove amethyst:// from args
            args[0] = args[0].Substring(11);
            // Remove trailing /
            args[args.Length - 1] = args[args.Length - 1].TrimEnd('/');

            for ( int i = 0; i < args.Length; i++ ) {

                // For each command
                for ( int j = 0; j < m_commandList.Length; j++ ) {

                    if ( ShouldExecute(ref m_commandList[j], ref args[i]) ) {
                        return m_commandList[j].Execute(ExtractParameters(ref args, i));
                    }
                }
            }

            Console.ReadKey();
            return false;
        }

        /// <summary>
        /// Returns whether the command should be executed or not
        /// </summary>
        /// <param name="command">The command to check</param>
        /// <param name="cmd">A formatted command string</param>
        /// <returns>Whether the command should execute or not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ShouldExecute(ref IProtocolCommand command, ref string cmd) {

            if ( command.Command.Equals(cmd, StringComparison.InvariantCultureIgnoreCase) ) {
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ExtractParameters(ref string[] args, int index) {

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
}
