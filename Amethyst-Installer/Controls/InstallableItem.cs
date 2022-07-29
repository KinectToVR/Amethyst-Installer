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
        private CheckBox    itemCheckbox;
        private Border      controlContainer;

        public MouseButtonEventHandler OnMouseClickReleased;

        public bool Checked {
            get { return ( bool ) GetValue(CheckedProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        public static readonly DependencyProperty CheckedProperty =
            DependencyProperty.Register("Checked", typeof(bool), typeof(InstallableItem), new UIPropertyMetadata(false, new PropertyChangedCallback(CheckedChanged)));
        

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


        private static void CheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as InstallableItem ).itemCheckbox == null )
                return;
            ( d as InstallableItem ).itemCheckbox.IsChecked = ( bool ) e.NewValue;
        }
        
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
            itemCheckbox.IsChecked = Checked;
            itemCheckbox.IsEnabled = !Disabled;
            itemTitle.Text = Title;
            itemDescription.Text = Description;

            controlContainer.MouseLeftButtonUp += container_MouseLeftButtonDown;
        }

        private void container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            // TODO: Abstract for light mode?
            // Background = #FF474444
            Background = new SolidColorBrush(Color.FromRgb(0x47, 0x44, 0x44));

            Checked = !Checked;
            if ( OnMouseClickReleased != null)
                OnMouseClickReleased(this, e);
        }
    }
}
