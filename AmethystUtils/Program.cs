using AmethystUtils.Protocol;

namespace AmethystUtils {
    public class Program {
        static void Main(string[] args) {

            // AmethystUtils is a program solely designed to handle amethyst protocol URLS
            // Prior to this executable existing, we were handling them via AmethystInstaller,
            // which is probelematic because something as simple as opening the logs folder would require administrator
            // when such an action should not. This allows us to avoid requiring adminstrator for certain events

            // This also let's us pass certain arguments into Amethyst itself!
            ProtocolParser.ParseCommands(args);
        }
    }
}
