using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace amethyst_installer_gui.Controls
{
    /// <summary>
    /// Interaction logic for SidebarTask.xaml
    /// </summary>
    public partial class SidebarTask : UserControl
    {
        public SidebarTask()
        {
            InitializeComponent();
        }

        public TaskState State
        {
            get { return (TaskState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(TaskState), typeof(SidebarTask), new PropertyMetadata(TaskState.Default, new PropertyChangedCallback(StateChanged)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SidebarTask), new PropertyMetadata("Task", new PropertyChangedCallback(TextChanged)));


        private static void StateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string stateString = "Default";
            switch ( (TaskState)e.NewValue )
            {
                case TaskState.Checkmark:
                    stateString = "Checkmark";
                    break;
                case TaskState.Question:
                    stateString = "Question";
                    break;

            }

            (d as SidebarTask).taskStateIcon.Source = new BitmapImage(new Uri($"/Resources/Icons/4x/{stateString}.png", UriKind.Relative)); ;
        }

        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SidebarTask).taskDescription.Content = (string)e.NewValue;
        }
    }
}
