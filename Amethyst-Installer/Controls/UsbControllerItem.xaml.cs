using amethyst_installer_gui.Installer;
using System;
using System.Diagnostics.PerformanceData;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace amethyst_installer_gui.Controls {
    /// <summary>
    /// Interaction logic for UsbControllerItem.xaml
    /// </summary>
    public partial class UsbControllerItem : UserControl {
        public UsbControllerItem(UsbControllerData data, int count) {
            InitializeComponent();
            if (count > 1) {
                this.content.Text = string.Format(Localisation.SystemRequirement_Description_UsbControllers_MultipleControllers, count, data.FriendlyString);
            } else {
                this.content.Text = data.FriendlyString;
            }

            // Apply icon
            string stateString = "Default";
            switch ( data.ControllerQuality ) {
                case UsbControllerQuality.Good:
                    stateString = "Checkmark";
                    break;
                case UsbControllerQuality.Unusable:
                    stateString = "Error";
                    break;
                case UsbControllerQuality.OK:
                    stateString = "Warning";
                    break;
                case UsbControllerQuality.Unknown:
                    stateString = "Question";
                    break;

            }
            taskStateIcon.Source = new BitmapImage(new Uri($"/Resources/Icons/4x/{stateString}.png", UriKind.Relative));
        }
    }
}
