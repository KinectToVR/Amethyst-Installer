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

    [TemplatePart(Name = "diskText", Type = typeof(TextBlock))]
    [TemplatePart(Name = "progressBar", Type = typeof(ProgressBar))]
    public class DriveSelectionControl : Control {
        static DriveSelectionControl() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DriveSelectionControl), new FrameworkPropertyMetadata(typeof(DriveSelectionControl)));
        }

        private TextBlock diskText;
        private ProgressBar progressBar;

        public string DiskLabel {
            get { return ( string ) GetValue(DiskLabelProperty); }
            set { SetValue(DiskLabelProperty, value); }
        }

        public static readonly DependencyProperty DiskLabelProperty =
            DependencyProperty.Register("DiskLabel", typeof(string), typeof(DriveSelectionControl), new UIPropertyMetadata("Local Disk", new PropertyChangedCallback(DiskLabelChanged)));

        private static void DiskLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as DriveSelectionControl ).diskText == null )
                return;
            ( d as DriveSelectionControl ).diskText.Text = ( string ) e.NewValue + Environment.NewLine + ( d as DriveSelectionControl ).FreeSpaceLabel;
        }
        
        public string FreeSpaceLabel {
            get { return ( string ) GetValue(FreeSpaceLabelProperty); }
            set { SetValue(FreeSpaceLabelProperty, value); }
        }

        public static readonly DependencyProperty FreeSpaceLabelProperty =
            DependencyProperty.Register("FreeSpaceLabel", typeof(string), typeof(DriveSelectionControl), new UIPropertyMetadata("0 bytes free", new PropertyChangedCallback(FreeSpaceLabelChanged)));

        private static void FreeSpaceLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as DriveSelectionControl ).diskText == null )
                return;
            ( d as DriveSelectionControl ).diskText.Text = ( d as DriveSelectionControl ).DiskLabel + Environment.NewLine + ( string ) e.NewValue;
        }
        
        public double DiskPercentage {
            get { return ( double ) GetValue(DiskPercentageProperty); }
            set { SetValue(DiskPercentageProperty, value); }
        }

        public static readonly DependencyProperty DiskPercentageProperty =
            DependencyProperty.Register("DiskPercentage", typeof(double), typeof(DriveSelectionControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(DiskPercentageChanged)));

        private static void DiskPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as DriveSelectionControl ).progressBar == null )
                return;
            ( d as DriveSelectionControl ).progressBar.Value = ( double ) e.NewValue;
        }
        
        public bool Selected {
            get { return ( bool ) GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register("Selected", typeof(bool), typeof(DriveSelectionControl), new UIPropertyMetadata(false));

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            diskText = GetTemplateChild("diskText") as TextBlock;
            progressBar = GetTemplateChild("progressBar") as ProgressBar;

            diskText.Text = DiskLabel + Environment.NewLine + FreeSpaceLabel;
            progressBar.Value = DiskPercentage;
        }
    }
}
