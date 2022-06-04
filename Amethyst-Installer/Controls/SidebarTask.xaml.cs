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

        public SiderbarTaskState State
        {
            get { return (SiderbarTaskState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(SiderbarTaskState), typeof(SidebarTask), new PropertyMetadata(SiderbarTaskState.Default, new PropertyChangedCallback(StateChanged)));

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
            switch ( (SiderbarTaskState)e.NewValue )
            {
                case SiderbarTaskState.Default:
                    stateString = "Default";
                    break;
                case SiderbarTaskState.Checkmark:
                    stateString = "Checkmark";
                    break;
                case SiderbarTaskState.Question:
                    stateString = "Question";
                    break;

            }

            Console.WriteLine($"/Resources/Icons/4x/{stateString}.png");
            (d as SidebarTask).taskStateIcon.Source = new BitmapImage(new Uri($"/Resources/Icons/4x/{stateString}.png", UriKind.Relative)); ;
        }

        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SidebarTask).taskDescription.Content = (string)e.NewValue;
        }
    }

    public enum SiderbarTaskState
	{
        Default = 0,
        Checkmark = 1,
        Question = 2,
	}
}
