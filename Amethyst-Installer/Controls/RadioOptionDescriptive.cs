using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace amethyst_installer_gui.Controls {
    [TemplatePart(Name = "controlRoot", Type = typeof(Border))]
    [TemplatePart(Name = "radioElementInternal", Type = typeof(RadioButton))]
    [TemplatePart(Name = "titleText", Type = typeof(TextBlock))]
    [TemplatePart(Name = "descriptionText", Type = typeof(TextBlock))]
    public class RadioOptionDescriptive : Control {
        static RadioOptionDescriptive() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioOptionDescriptive), new FrameworkPropertyMetadata(typeof(RadioOptionDescriptive)));
        }

        public MouseButtonEventHandler OnMouseClickReleased;
        public RoutedEventHandler OnToggled;

        public string Title {
            get { return ( string ) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RadioOptionDescriptive), new UIPropertyMetadata("Lorem Ipsum", new PropertyChangedCallback(TitleChanged)));

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as RadioOptionDescriptive ).titleText == null )
                return;
            ( d as RadioOptionDescriptive ).titleText.Text = ( string ) e.NewValue;
        }

        public string Description {
            get { return ( string ) GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(RadioOptionDescriptive), new UIPropertyMetadata("Lorem ipsum dolor sit amet constecteur adispiling.", new PropertyChangedCallback(DescriptionChanged)));

        private static void DescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as RadioOptionDescriptive ).descriptionText == null )
                return;
            ( d as RadioOptionDescriptive ).descriptionText.Text = ( string ) e.NewValue;
        }

        public bool IsChecked {
            get { return ( bool ) GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(RadioOptionDescriptive), new UIPropertyMetadata(false, new PropertyChangedCallback(IsCheckedChanged)));

        private static void IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as RadioOptionDescriptive ).radioElementInternal == null )
                return;
            ( d as RadioOptionDescriptive ).radioElementInternal.IsChecked = ( bool ) e.NewValue;
        }

        public string GroupName {
            get { return ( string ) GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(RadioOptionDescriptive), new UIPropertyMetadata(new PropertyChangedCallback(GroupNameChanged)));

        private static void GroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as RadioOptionDescriptive ).radioElementInternal == null )
                return;
            ( d as RadioOptionDescriptive ).radioElementInternal.GroupName = ( string ) e.NewValue;
        }

        private Border controlRoot;
        private RadioButton radioElementInternal;
        private TextBlock titleText;
        private TextBlock descriptionText;

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            radioElementInternal    = GetTemplateChild("radioElementInternal") as RadioButton;
            controlRoot             = GetTemplateChild("controlRoot") as Border;
            titleText               = GetTemplateChild("titleText") as TextBlock;
            descriptionText         = GetTemplateChild("descriptionText") as TextBlock;

            radioElementInternal.IsChecked = IsChecked;
            radioElementInternal.GroupName = GroupName;
            titleText.Text = Title;
            descriptionText.Text = Description;
            if ( radioElementInternal.IsChecked.HasValue && radioElementInternal.IsChecked.Value == true ) {
                controlRoot.SetResourceReference(BorderBrushProperty, "BrushAccent");
            } else {
                controlRoot.BorderBrush = Brushes.Transparent;
            }

            controlRoot.MouseLeftButtonUp += container_MouseLeftButtonDown;
            radioElementInternal.Checked += radioElementInternal_CheckToggled;
            radioElementInternal.Unchecked += radioElementInternal_CheckToggled;
        }

        private void container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            // Sets to accent colour
            radioElementInternal.IsChecked = true;

            if ( OnMouseClickReleased != null )
                OnMouseClickReleased(this, e);
        }

        private void radioElementInternal_CheckToggled(object sender, RoutedEventArgs e) {
            if ( radioElementInternal.IsChecked.HasValue && radioElementInternal.IsChecked.Value == true ) {
                controlRoot.SetResourceReference(BorderBrushProperty, "BrushAccent");
            } else {
                controlRoot.BorderBrush = Brushes.Transparent;
            }
            if ( OnToggled != null )
                OnToggled(this, e);
        }
    }
}
