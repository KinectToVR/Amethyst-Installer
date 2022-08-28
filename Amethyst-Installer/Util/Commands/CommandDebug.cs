using System.Reflection;

namespace amethyst_installer_gui.Commands {
    public class CommandDebug : ICommand {

        public string Command { get => "debug"; set { } }
        public string Description { get => "Enables debug mode"; set { } }
        public string[] Aliases { get => new string[] { "d" }; set { } }

        public bool Execute(string parameters) {
            
            // Abuse to override MainWindow.DebugMode while enforcing the private setter
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            PropertyInfo debugProperty = typeof(MainWindow).GetProperty("DebugMode", bindFlags);
            debugProperty.SetValue(null, true);

            return false;
        }
    }
}
