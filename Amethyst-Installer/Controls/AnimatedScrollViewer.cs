using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace amethyst_installer_gui.Controls {
    public static class AnimatedScrollViewer {

        public static DependencyProperty HorizontalOffsetProperty =
        DependencyProperty.RegisterAttached("HorizontalOffset",
                                            typeof(double),
                                            typeof(AnimatedScrollViewer),
                                            new UIPropertyMetadata(0.0, OnHorizontalOffsetChanged));

        public static void SetVerticalOffset(FrameworkElement target, double value) {
            target.SetValue(HorizontalOffsetProperty, value);
        }

        public static double GetHorizontalOffset(FrameworkElement target) {
            return ( double ) target.GetValue(HorizontalOffsetProperty);
        }

        private static void OnHorizontalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) {
            ScrollViewer scrollViewer = target as ScrollViewer;

            if ( scrollViewer != null ) {
                scrollViewer.ScrollToHorizontalOffset(( double ) e.NewValue);
            }
        }



        public static DependencyProperty OpacityProperty =
        DependencyProperty.RegisterAttached("Opacity",
                                            typeof(double),
                                            typeof(AnimatedScrollViewer),
                                            new UIPropertyMetadata(0.0, OnOpacityChanged));

        public static void SetOpacity(FrameworkElement target, double value) {
            target.SetValue(OpacityProperty, value);
        }

        public static double GetOpacity(FrameworkElement target) {
            return ( double ) target.GetValue(OpacityProperty);
        }

        private static void OnOpacityChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) {
            ContentControl contentControl = target as ContentControl;

            if ( contentControl != null ) {
                contentControl.Opacity = ( double ) e.NewValue;
            }
        }
    }
}
