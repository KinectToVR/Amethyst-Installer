using amethyst_installer_gui.Installer;
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

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageEULA.xaml
    /// </summary>
    public partial class PageEULA : UserControl, IInstallerPage {
        public PageEULA() {
            InitializeComponent();

            // eulaBoxContainer.Document.Blocks.Clear();

            // Render the text
            // RenderLine(Properties.Resources.EULA_Warranty, ConsoleColor.Yellow, 14);
            // RenderLine(Properties.Resources.EULA_Licenses + Environment.NewLine, ConsoleColor.White, 14);

            // Read EULA text
            string LicenseTexts = Util.ExtractResourceAsString("Licenses.txt");
            // RenderLine(LicenseTexts, ConsoleColor.White);

            /*
            // return;
            // Convert to stack allocated char*
            ReadOnlySpan<char> licenseFileStringView = stackalloc char[LicenseTexts.Length];
            licenseFileStringView = LicenseTexts.ToCharArray();
            ReadOnlySpan<char> searchStrSringView = stackalloc char[6];
            searchStrSringView = "\r\n\r\n\r\n".ToCharArray();

            int start = 0;
            bool strComp = false;

            for ( int i = 0; i < licenseFileStringView.Length - 5; i++ ) {
                strComp = licenseFileStringView.Slice(i, 6).SequenceEqual(searchStrSringView);
                if ( strComp ) {
                    RenderLine(licenseFileStringView.Slice(start, i - start).ToString(), ConsoleColor.White);
                    start = i + 1;
                }
            }
            */

            // eulaBoxContainer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            // eulaBoxContainer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            // eulaBoxContainer.ApplyTemplate();

            // RenderLine(LicenseTexts, ConsoleColor.White);

            string pretext = Properties.Resources.EULA_Warranty + Environment.NewLine + Environment.NewLine + Properties.Resources.EULA_Licenses;

            // Use our better control :trol:
            fastTextbox.Text = pretext + Environment.NewLine + LicenseTexts;
            fastTextbox.text_view.TypeFace = new Typeface("Consolas");
            fastTextbox.text_view.FontSize = 12;
            // Yellow warning
            fastTextbox.text_view.FontColors.Add(new Controls.FontColorFormatting() {
                Start = 0,
                Length = Properties.Resources.EULA_Warranty.Length,
                TargetBrush = Constants.ConsoleBrushColors[14]
            });
            // Font size
            fastTextbox.text_view.FontSizes.Add(new Controls.FontSizeFormatting() {
                Start = 0,
                Length = pretext.Length,
                TargetFontSize = 14
            });
        }

        public InstallerState GetInstallerState() {
            return InstallerState.EULA;
        }

        public string GetTitle() {
            return Properties.Resources.Page_EULA_Title;
        }

        private void eulaRadioButton_Checked(object sender, RoutedEventArgs e) {
            RadioButton elButton = sender as RadioButton;
            if ( elButton != null && elButton.ActualWidth > 0 && elButton.ActualHeight > 0)
                SoundPlayer.PlaySound(SoundEffect.Invoke);

            if ( proceedButton != null )
                proceedButton.IsEnabled = eulaAgree != null && eulaAgree.IsChecked.HasValue && eulaAgree.IsChecked.Value == true;
        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonPrimary.Content = Properties.Resources.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;
            proceedButton.IsEnabled = eulaAgree != null && eulaAgree.IsChecked.HasValue && eulaAgree.IsChecked.Value == true;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(true);
        }

        public void OnSelected() {

            MainWindow.Instance.m_layoutTimer.Reset();
            MainWindow.Instance.m_layoutTimer.Start();

            MainWindow.Instance.sidebar_welcome.State = Controls.TaskState.Busy;
        }

        protected override void OnRender(DrawingContext drawingContext) {
            MainWindow.Instance.MainWindow_ContentRendered(null, null);
            base.OnRender(drawingContext);
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) { }
        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }

        public void RenderLine(string text, ConsoleColor color, double fontSize = 12) {
            // Paragraph paragraph = new Paragraph();
            // Run run = new Run(text);
            // paragraph.Inlines.Add(run);
            // if (color != ConsoleColor.White)
            //     paragraph.Foreground = Constants.ConsoleBrushColors[( int ) color];
            // if (fontSize != 12)
            //     paragraph.FontSize = fontSize;
            // eulaBoxContainer.Document.Blocks.Add(paragraph);
            
            TextBlock paragraph = new TextBlock();
            paragraph.Text = text;
            paragraph.Foreground = Constants.ConsoleBrushColors[( int ) color];
            paragraph.FontSize = fontSize;
            paragraph.TextWrapping = TextWrapping.WrapWithOverflow;

            // Run run = new Run(text);
            // paragraph.Inlines.Add(run);
            // if (color != ConsoleColor.White)
            //     paragraph.Foreground = Constants.ConsoleBrushColors[( int ) color];
            // if (fontSize != 12)
            //     paragraph.FontSize = fontSize;

            // eulaContentBox.Children.Add(paragraph);
            // fastTextbox.
        }

        public void proceedButton_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.MoveNext);
            MainWindow.Instance.SetButtonsHidden(false);
            MainWindow.Instance.SetPage(InstallerState.InstallOptions);
        }
    }
}
