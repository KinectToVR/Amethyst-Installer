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

namespace amethyst_installer_gui.Controls {

    [TemplatePart(Name = "itemTitle", Type = typeof(TextBlock))]
    [TemplatePart(Name = "downloadedSizeText", Type = typeof(TextBlock))]
    [TemplatePart(Name = "totalSizeText", Type = typeof(TextBlock))]
    [TemplatePart(Name = "percentageText", Type = typeof(TextBlock))]
    [TemplatePart(Name = "progressBar", Type = typeof(ProgressBar))]
    [TemplatePart(Name = "errorMessage", Type = typeof(TextBlock))]
    public class DownloadItem : Control {
        static DownloadItem() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DownloadItem), new FrameworkPropertyMetadata(typeof(DownloadItem)));
        }

        private TextBlock itemTitle;
        private TextBlock downloadedSizeText;
        private TextBlock totalSizeText;
        private TextBlock percentageText;
        private ProgressBar progressBar;
        private TextBlock errorMessage;
        private Button retryButton;

        public RoutedEventHandler OnRetry;

        public string Title {
            get { return ( string ) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(DownloadItem), new UIPropertyMetadata("Amethyst Module", new PropertyChangedCallback(TitleChanged)));

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var thisControl = d as DownloadItem;
            if ( thisControl.itemTitle == null )
                return;
            thisControl.itemTitle.Text = ( string ) e.NewValue;
        }

        public long DownloadedBytes {
            get { return ( long ) GetValue(DownloadedBytesProperty); }
            set { SetValue(DownloadedBytesProperty, value); }
        }

        public static readonly DependencyProperty DownloadedBytesProperty =
            DependencyProperty.Register("DownloadedBytes", typeof(long), typeof(DownloadItem), new UIPropertyMetadata(0L, new PropertyChangedCallback(DownloadedBytesChanged)));

        private static void DownloadedBytesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var thisControl = d as DownloadItem;
            if ( thisControl.downloadedSizeText == null || thisControl.percentageText == null || thisControl.progressBar == null )
                return;
            thisControl.downloadedSizeText.Text = Util.SizeSuffix(( long ) e.NewValue) + " / ";
            double percentage = ( ( long ) e.NewValue / 1000000.0 ) / ( thisControl.TotalBytes / 1000000.0 );
            if ( double.IsNaN(percentage) )
                percentage = 0;
            thisControl.percentageText.Text = ( int ) Math.Round(percentage * 100, MidpointRounding.AwayFromZero) + "%";
            thisControl.progressBar.Value = Math.Max(Math.Min(percentage, 1.0), 0.0);
        }

        public long TotalBytes {
            get { return ( long ) GetValue(TotalBytesProperty); }
            set { SetValue(TotalBytesProperty, value); }
        }

        public static readonly DependencyProperty TotalBytesProperty =
            DependencyProperty.Register("TotalBytes", typeof(long), typeof(DownloadItem), new UIPropertyMetadata(0L, new PropertyChangedCallback(TotalBytesChanged)));

        private static void TotalBytesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var thisControl = d as DownloadItem;
            if ( thisControl.totalSizeText == null || thisControl.percentageText == null || thisControl.progressBar == null )
                return;
            thisControl.totalSizeText.Text = Util.SizeSuffix(( long ) e.NewValue);
            double percentage = ( thisControl.DownloadedBytes / 1000000.0 ) / ( ( long ) e.NewValue / 1000000.0 );
            if ( double.IsNaN(percentage) )
                percentage = 0;
            thisControl.percentageText.Text = ( int ) Math.Round(percentage * 100, MidpointRounding.AwayFromZero) + "%";
            thisControl.progressBar.Value = Math.Max(Math.Min(percentage, 1.0), 0.0);

            if ( !thisControl.Completed && !thisControl.IsPending ) {
                thisControl.totalSizeText.Text += $" ({Util.SizeSuffix(thisControl.TransferSpeed)}/s)";
            }
        }

        public long TransferSpeed {
            get { return ( long ) GetValue(TransferSpeedProperty); }
            set { SetValue(TransferSpeedProperty, value); }
        }

        public static readonly DependencyProperty TransferSpeedProperty =
            DependencyProperty.Register("TransferSpeed", typeof(long), typeof(DownloadItem), new UIPropertyMetadata(0L, new PropertyChangedCallback(TransferSpeedChanged)));

        private static void TransferSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var thisControl = d as DownloadItem;
            if ( thisControl.totalSizeText == null || thisControl.percentageText == null || thisControl.progressBar == null )
                return;
            thisControl.totalSizeText.Text = Util.SizeSuffix(thisControl.TotalBytes);

            if ( !thisControl.Completed && !thisControl.IsPending) {
                thisControl.totalSizeText.Text += $" ({Util.SizeSuffix(( long ) e.NewValue)}/s)";
            }
        }

        public bool IsPending {
            get { return ( bool ) GetValue(IsPendingProperty); }
            set { SetValue(IsPendingProperty, value); }
        }

        public static readonly DependencyProperty IsPendingProperty =
            DependencyProperty.Register("IsPending", typeof(bool), typeof(DownloadItem), new UIPropertyMetadata(true));
        

        public bool Completed {
            get { return ( bool ) GetValue(CompletedProperty); }
            set { SetValue(CompletedProperty, value); }
        }

        public static readonly DependencyProperty CompletedProperty =
            DependencyProperty.Register("Completed", typeof(bool), typeof(DownloadItem), new UIPropertyMetadata(false, new PropertyChangedCallback(CompletedChanged)));

        private static void CompletedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var thisControl = d as DownloadItem;
            if ( thisControl.totalSizeText == null || thisControl.percentageText == null || thisControl.progressBar == null )
                return;
            thisControl.totalSizeText.Text = Util.SizeSuffix(thisControl.TotalBytes);

            if ( !(bool)e.NewValue && !thisControl.IsPending ) {
                thisControl.totalSizeText.Text += $" ({Util.SizeSuffix(( long ) thisControl.TransferSpeed)}/s)";
            }
        }

        public string ErrorMessage {
            get { return ( string ) GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(DownloadItem), new UIPropertyMetadata(Localisation.Download_Failure, new PropertyChangedCallback(ErrorMessageChanged)));

        private static void ErrorMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var thisControl = d as DownloadItem;
            if ( thisControl.errorMessage == null )
                return;
            thisControl.errorMessage.Text = ( string ) e.NewValue;
        }

        public bool DownloadFailed {
            get { return ( bool ) GetValue(DownloadFailedProperty); }
            set { SetValue(DownloadFailedProperty, value); }
        }

        public static readonly DependencyProperty DownloadFailedProperty =
            DependencyProperty.Register("DownloadFailed", typeof(bool), typeof(DownloadItem), new UIPropertyMetadata(false));

        public bool IsErrorCritical {
            get { return ( bool ) GetValue(IsErrorCriticalProperty); }
            set { SetValue(IsErrorCriticalProperty, value); }
        }

        public static readonly DependencyProperty IsErrorCriticalProperty =
            DependencyProperty.Register("IsErrorCritical", typeof(bool), typeof(DownloadItem), new UIPropertyMetadata(false));

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            itemTitle           = GetTemplateChild("itemTitle") as TextBlock;
            downloadedSizeText  = GetTemplateChild("downloadedSizeText") as TextBlock;
            totalSizeText       = GetTemplateChild("totalSizeText") as TextBlock;
            percentageText      = GetTemplateChild("percentageText") as TextBlock;
            progressBar         = GetTemplateChild("progressBar") as ProgressBar;
            errorMessage        = GetTemplateChild("errorMessage") as TextBlock;
            retryButton         = GetTemplateChild("retryButton") as Button;

            itemTitle.Text = Title;
            errorMessage.Text = ErrorMessage;

            downloadedSizeText.Text = Util.SizeSuffix(DownloadedBytes) + " / ";
            totalSizeText.Text = Util.SizeSuffix(TotalBytes);
            double percentage = ( DownloadedBytes / 1000000.0 ) / ( TotalBytes / 1000000.0 );
            if ( double.IsNaN(percentage) )
                percentage = 0;

            percentageText.Text = ( int ) Math.Round(percentage * 100, MidpointRounding.AwayFromZero) + "%";
            progressBar.Value = Math.Max(Math.Min(percentage, 1.0), 0.0);

            if ( !Completed && !IsPending ) {
                totalSizeText.Text += $" ({Util.SizeSuffix(TransferSpeed)}/s)";
            }

            retryButton.Click += retryButton_Click;
        }

        private void retryButton_Click(object sender, RoutedEventArgs e) {
            if ( OnRetry != null )
                OnRetry(this, e);
        }
    }
}
