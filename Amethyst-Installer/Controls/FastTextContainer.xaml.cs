using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// <summary>
    /// Interaction logic for FastTextContainer.xaml
    /// </summary>
    public partial class FastTextContainer : UserControl {

        public string Text {
            get { return text_view.Text; }
            set { text_view.Text = value; }
        }

        public FastTextContainer() {
            InitializeComponent();
            text_view.OnRecomputedValues += TextView_Recomputed;
        }

        private void TextView_Recomputed(object sender, EventArgs e) {
            scrollViewport.MaxHeight = double.PositiveInfinity;
            scrollViewport.Height = text_view.ComputedHeight;
        }

        private void scroller_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            text_view.HeightOffset = scroller.VerticalOffset;
        }
    }
}
