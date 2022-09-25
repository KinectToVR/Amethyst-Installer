using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace amethyst_installer_gui.Controls {
    [TemplatePart(Name = "itemTitle", Type = typeof(TextBlock))]
    [TemplatePart(Name = "itemDescription", Type = typeof(TextBlock))]
    [TemplatePart(Name = "itemCheckbox", Type = typeof(CheckBox))]
    [TemplatePart(Name = "controlContainer", Type = typeof(Border))]
    public class InstallableItem : ContentControl {
        static InstallableItem() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InstallableItem), new FrameworkPropertyMetadata(typeof(InstallableItem)));
        }

        private TextBlock   itemTitle;
        private TextBlock   itemDescription;
        public CheckBox     itemCheckbox;
        private Border      controlContainer;

        public MouseButtonEventHandler OnMouseClickReleased;
        public RoutedEventHandler OnToggled;

        public bool Checked {
            get { return ( bool ) GetValue(CheckedProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        public static readonly DependencyProperty CheckedProperty =
            DependencyProperty.Register("Checked", typeof(bool), typeof(InstallableItem), new UIPropertyMetadata(false, new PropertyChangedCallback(CheckedChanged)));

        private static void CheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var thisControl = d as InstallableItem;
            if ( thisControl.itemCheckbox == null )
                return;
            if ( !thisControl.Disabled ) {
                thisControl.itemCheckbox.IsChecked = ( bool ) e.NewValue;

                if ( thisControl.OnToggled != null )
                    thisControl.OnToggled(thisControl, new RoutedEventArgs());
            }
        }

        public bool Disabled {
            get { return ( bool ) GetValue(DisabledProperty); }
            set { SetValue(DisabledProperty, value); }
        }

        public static readonly DependencyProperty DisabledProperty =
            DependencyProperty.Register("Disabled", typeof(bool), typeof(InstallableItem), new UIPropertyMetadata(false, new PropertyChangedCallback(DisabledChanged)));

        public string Title {
            get { return ( string ) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(InstallableItem), new UIPropertyMetadata("Among us", new PropertyChangedCallback(TitleChanged)));
        
        public string Description {
            get { return ( string ) GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(InstallableItem), new UIPropertyMetadata("Lorem ipsum dolor sit amet", new PropertyChangedCallback(DescriptionChanged)));

        
        private static void DisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as InstallableItem ).itemCheckbox == null )
                return;
            ( d as InstallableItem ).itemCheckbox.IsEnabled = !( bool ) e.NewValue;
        }

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as InstallableItem ).itemTitle == null )
                return;
            ( d as InstallableItem ).itemTitle.Text = ( string ) e.NewValue;
        }

        private static void DescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as InstallableItem ).itemDescription == null )
                return;
            ( d as InstallableItem ).itemDescription.Text = ( string ) e.NewValue;
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            itemCheckbox = GetTemplateChild("itemCheckbox") as CheckBox;
            itemTitle = GetTemplateChild("itemTitle") as TextBlock;
            itemDescription = GetTemplateChild("itemDescription") as TextBlock;
            controlContainer = GetTemplateChild("controlContainer") as Border;

            // Apply props
            itemCheckbox.IsEnabled = !Disabled;
            itemCheckbox.IsChecked = Checked;
            itemTitle.Text = Title;
            itemDescription.Text = Description;

            controlContainer.MouseLeftButtonUp += container_MouseLeftButtonDown;
            itemCheckbox.Checked += itemCheckbox_CheckToggled;
            itemCheckbox.Unchecked += itemCheckbox_CheckToggled;
        }

        private void container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            // @TODO: Abstract for light mode?
            // Background = #FF474444
            Background = new SolidColorBrush(Color.FromRgb(0x47, 0x44, 0x44));

            if ( !Disabled ) {
                Checked = !Checked;
            }
            if ( OnMouseClickReleased != null)
                OnMouseClickReleased(this, e);
        }
        private void itemCheckbox_CheckToggled(object sender, RoutedEventArgs e) {

            if ( OnToggled != null)
                OnToggled(this, e);
        }

        public void Click() {
            container_MouseLeftButtonDown(this, null);
        }
    }
}
