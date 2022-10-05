namespace AmethystUtils.Protocol {
    public class ProtocolNotPowered : IProtocolCommand {
        public string Command { get => "notpowered"; set { } }

        public bool Execute(string parameters) {
            Util.PassToInstaller($"amethyst://{Command}");
            return true;
        }
    }
    public class ProtocolNotPoweredSilent : IProtocolCommand {
        public string Command { get => "notpowered/silent"; set { } }

        public bool Execute(string parameters) {
            Util.PassToInstaller($"amethyst://{Command}");
            return true;
        }
    }

    public class ProtocolFixMicrophone : IProtocolCommand {
        public string Command { get => "fixmicrophone"; set { } }

        public bool Execute(string parameters) {
            Util.PassToInstaller($"amethyst://{Command}");
            return true;
        }
    }

    // @TODO: amethyst://kinect/toolkit => Launches toolkit
    // @TODO: amethyst://kinect/toolkit/explorer => Launches Kinect Explorer D2D
}
