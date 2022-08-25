using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenUninstallList.Commands {

    [Verb("checksum", HelpText = "Calculates the checksum of a given file.")]
    public class ChecksumOptions {

        [Option('p', HelpText="The path to the specified file")]
        public string FilePath { get; set; }
    }

    public static class CommandChecksum {

        public static void Execute(ChecksumOptions parameters) {
            


            return;
        }
    }
}
