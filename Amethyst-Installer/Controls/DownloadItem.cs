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
            if ( ( d as DownloadItem ).itemTitle == null )
                return;
            ( d as DownloadItem ).itemTitle.Text = ( string ) e.NewValue;
        }

        public long DownloadedBytes {
            get { return ( long ) GetValue(DownloadedBytesProperty); }
            set { SetValue(DownloadedBytesProperty, value); }
        }

        public static readonly DependencyProperty DownloadedBytesProperty =
            DependencyProperty.Register("DownloadedBytes", typeof(long), typeof(DownloadItem), new UIPropertyMetadata(0L, new PropertyChangedCallback(DownloadedBytesChanged)));

        private static void DownloadedBytesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as DownloadItem ).downloadedSizeText == null || ( d as DownloadItem ).percentageText == null || ( d as DownloadItem ).progressBar == null )
                return;
            ( d as DownloadItem ).downloadedSizeText.Text = Util.SizeSuffix(( long ) e.NewValue) + " / ";
            double percentage = ( ( long ) e.NewValue / 1000000.0 ) / ( ( d as DownloadItem ).TotalBytes / 1000000.0 );
            if ( double.IsNaN(percentage) )
                percentage = 0;
            ( d as DownloadItem ).percentageText.Text = ( int ) Math.Round(percentage * 100, MidpointRounding.AwayFromZero) + "%";
            ( d as DownloadItem ).progressBar.Value = percentage;
        }

        public long TotalBytes {
            get { return ( long ) GetValue(TotalBytesProperty); }
            set { SetValue(TotalBytesProperty, value); }
        }

        public static readonly DependencyProperty TotalBytesProperty =
            DependencyProperty.Register("TotalBytes", typeof(long), typeof(DownloadItem), new UIPropertyMetadata(0L, new PropertyChangedCallback(TotalBytesChanged)));

        private static void TotalBytesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as DownloadItem ).totalSizeText == null || ( d as DownloadItem ).percentageText == null || ( d as DownloadItem ).progressBar == null )
                return;
            ( d as DownloadItem ).totalSizeText.Text = Util.SizeSuffix(( long ) e.NewValue);
            double percentage = ( ( d as DownloadItem ).DownloadedBytes / 1000000.0 ) / ( ( long ) e.NewValue / 1000000.0 );
            if ( double.IsNaN(percentage) )
                percentage = 0;
            ( d as DownloadItem ).percentageText.Text = (int) Math.Round( percentage * 100, MidpointRounding.AwayFromZero) + "%";
            ( d as DownloadItem ).progressBar.Value = percentage;
        }

        public bool IsPending {
            get { return ( bool ) GetValue(IsPendingProperty); }
            set { SetValue(IsPendingProperty, value); }
        }

        public static readonly DependencyProperty IsPendingProperty =
            DependencyProperty.Register("IsPending", typeof(bool), typeof(DownloadItem), new UIPropertyMetadata(true));


        public string ErrorMessage {
            get { return ( string ) GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(DownloadItem), new UIPropertyMetadata("Ooopsy! We did a fucky wucky oh noes!", new PropertyChangedCallback(ErrorMessageChanged)));

        private static void ErrorMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as DownloadItem ).errorMessage == null )
                return;
            ( d as DownloadItem ).errorMessage.Text = ( string ) e.NewValue;
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
            progressBar.Value = percentage;

            retryButton.Click += retryButton_Click;
        }

        private void retryButton_Click(object sender, RoutedEventArgs e) {
            if ( OnRetry != null )
                OnRetry(this, e);
        }
    }
}
