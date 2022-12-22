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
    /// <summary>
    /// Interaction logic for RadioOptionDescriptive.xaml
    /// </summary>
    public partial class RadioOptionDescriptive : UserControl {
        public RadioOptionDescriptive() {
            InitializeComponent();
        }

        public bool IsChecked {
            get { return ( bool ) GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(RadioOptionDescriptive), new PropertyMetadata(false, new PropertyChangedCallback(IsCheckedChanged)));

        private static void IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ( d as RadioOptionDescriptive ).radioElementInternal.IsChecked = ( bool ) e.NewValue;
        }

        public string Title {
            get { return ( string ) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RadioOptionDescriptive), new PropertyMetadata("Title", new PropertyChangedCallback(TitleChanged)));

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ( d as RadioOptionDescriptive ).titleText.Text = ( string ) e.NewValue;
        }

        public string Description {
            get { return ( string ) GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(RadioOptionDescriptive), new PropertyMetadata("Lorem ipsum dolor sit amet constecteur adispiling.", new PropertyChangedCallback(DescriptionChanged)));

        private static void DescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ( d as RadioOptionDescriptive ).descriptionText.Text = ( string ) e.NewValue;
        }

        public string GroupName {
            get { return ( string ) GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(RadioOptionDescriptive), new PropertyMetadata(new PropertyChangedCallback(GroupNameChanged)));

        private static void GroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ( d as RadioOptionDescriptive ).radioElementInternal.GroupName = ( string ) e.NewValue;
        }

    }
}
