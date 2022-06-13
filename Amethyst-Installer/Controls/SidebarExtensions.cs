using System.Windows;

namespace amethyst_installer_gui.Controls {
    public class SidebarExtensions {
        public static readonly DependencyProperty ScrollIconPaddingProperty =
        DependencyProperty.RegisterAttached("ScrollIconPadding", typeof(Thickness), typeof(SidebarExtensions), new PropertyMetadata(default(Thickness)));

        public static void SetScrollIconPadding(UIElement element, Thickness value) {
            element.SetValue(ScrollIconPaddingProperty, value);
        }

        public static Thickness GetScrollIconPadding(UIElement element) {
            return ( Thickness ) element.GetValue(ScrollIconPaddingProperty);
        }
    }
}
