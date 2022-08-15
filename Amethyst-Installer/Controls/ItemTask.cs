using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace amethyst_installer_gui.Controls {
    [TemplatePart(Name = "taskStateIcon", Type = typeof(Image))]
    [TemplatePart(Name = "taskTitle", Type = typeof(Label))]
    public class ItemTask : ContentControl {
        static ItemTask() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemTask), new FrameworkPropertyMetadata(typeof(ItemTask)));
        }

        private Image taskStateIcon;
        private Label taskTitle;

        public TaskState State {
            get { return ( TaskState ) GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(TaskState), typeof(ItemTask), new UIPropertyMetadata(TaskState.Default, new PropertyChangedCallback(StateChanged)));

        public string Title {
            get { return ( string ) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ItemTask), new UIPropertyMetadata("Task", new PropertyChangedCallback(TitleChanged)));


        private static void StateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as ItemTask ).taskStateIcon == null )
                return;
            string stateString = "Default";
            switch ( ( TaskState ) e.NewValue ) {
                case TaskState.Checkmark:
                    stateString = "Checkmark";
                    break;
                case TaskState.Question:
                    stateString = "Question";
                    break;
                case TaskState.Warning:
                    stateString = "Warning";
                    break;
                case TaskState.Error:
                    stateString = "Error";
                    break;
            }

            ( d as ItemTask ).taskStateIcon.Source = new BitmapImage(new Uri($"/Resources/Icons/4x/{stateString}.png", UriKind.Relative));
        }

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ( ( d as ItemTask ).taskTitle == null )
                return;
            ( d as ItemTask ).taskTitle.Content = ( string ) e.NewValue;
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            taskStateIcon = GetTemplateChild("taskStateIcon") as Image;
            taskTitle = GetTemplateChild("taskTitle") as Label;

            // Apply props
            string stateString = "Default";
            switch ( State ) {
                case TaskState.Checkmark:
                    stateString = "Checkmark";
                    break;
                case TaskState.Question:
                    stateString = "Question";
                    break;
                case TaskState.Error:
                    stateString = "Error";
                    break;
                case TaskState.Warning:
                    stateString = "Warning";
                    break;

            }
            taskStateIcon.Source = new BitmapImage(new Uri($"/Resources/Icons/4x/{stateString}.png", UriKind.Relative));
            taskTitle.Content = Title;
        }
    }
}
