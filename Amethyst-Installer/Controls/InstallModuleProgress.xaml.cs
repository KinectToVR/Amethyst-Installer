using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using AmeModule = amethyst_installer_gui.Installer.Module;

namespace amethyst_installer_gui.Controls {
    /// <summary>
    /// Interaction logic for InstallModuleProgress.xaml
    /// </summary>
    public partial class InstallModuleProgress : UserControl {

        public AmeModule module;

        public InstallModuleProgress() {
            InitializeComponent();
            detailedLog.Document.Blocks.Clear();
        }

        public TaskState State {
            get { return ( TaskState ) GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(TaskState), typeof(InstallModuleProgress), new PropertyMetadata(TaskState.Default, new PropertyChangedCallback(StateChanged)));

        public string Title {
            get { return ( string ) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(InstallModuleProgress), new PropertyMetadata("Module", new PropertyChangedCallback(TitleChanged)));

        private static void StateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            string stateString = "Default";
            switch ( ( TaskState ) e.NewValue ) {
                case TaskState.Checkmark:
                    stateString = "Checkmark";
                    break;
                case TaskState.Question:
                    stateString = "Question";
                    break;
                case TaskState.Error:
                    stateString = "Error";
                    break;

            }

            ( d as InstallModuleProgress ).taskStateIcon.Source = new BitmapImage(new Uri($"/Resources/Icons/4x/{stateString}.png", UriKind.Relative));

            ( d as InstallModuleProgress ).taskStateIcon.Visibility = ( TaskState ) e.NewValue == TaskState.Busy ? Visibility.Collapsed : Visibility.Visible;
            ( d as InstallModuleProgress ).taskStateSpinner.Visibility = ( TaskState ) e.NewValue == TaskState.Busy ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ( d as InstallModuleProgress ).moduleTitle.Text = ( string ) e.NewValue;
        }

        private void heading_MouseUp(object sender, MouseButtonEventArgs e) {
            // logContainer.Visibility = logContainer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public void LogInfo(string message) {
            LogLineInternal(message, Constants.ConsoleBrushColors[( int ) ConsoleColor.White]);
        }

        public void LogWarning(string message) {
            LogLineInternal(message, Constants.ConsoleBrushColors[( int ) ConsoleColor.Yellow]);
        }

        public void LogError(string message) {
            LogLineInternal(message, Constants.ConsoleBrushColors[( int ) ConsoleColor.DarkRed]);
        }

        private void LogLineInternal(string msg, SolidColorBrush color) {
            Paragraph paragraph = new Paragraph();
            Run run = new Run(msg);
            run.Foreground = color;
            paragraph.Inlines.Add(run);
            detailedLog.Document.Blocks.Add(paragraph);
        }
    }
}
