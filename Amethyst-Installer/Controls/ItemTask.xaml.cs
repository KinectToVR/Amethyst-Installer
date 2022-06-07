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
    /// Interaction logic for ItemTask.xaml
    /// </summary>
    [TemplatePart(Name = "taskStateIcon", Type = typeof(Image))]
    [TemplatePart(Name = "taskTitle", Type = typeof(Label))]
    public partial class ItemTask : UserControl
    {
        public ItemTask()
        {
            InitializeComponent();
        }

        private Image taskStateIcon;
        private Label taskTitle;

        public TaskState State
        {
            get { return (TaskState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(TaskState), typeof(ItemTask), new PropertyMetadata(TaskState.Default, new PropertyChangedCallback(StateChanged)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ItemTask), new PropertyMetadata("Task", new PropertyChangedCallback(TitleChanged)));


        private static void StateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as ItemTask).taskStateIcon == null) return;
            string stateString = "Default";
            switch ((TaskState)e.NewValue)
            {
                case TaskState.Checkmark:
                    stateString = "Checkmark";
                    break;
                case TaskState.Question:
                    stateString = "Question";
                    break;

            }

            (d as ItemTask).taskStateIcon.Source = new BitmapImage(new Uri($"/Resources/Icons/4x/{stateString}.png", UriKind.Relative)); ;
        }

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as ItemTask).taskTitle == null) return;
            (d as ItemTask).taskTitle.Content = (string)e.NewValue;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            taskStateIcon = GetTemplateChild("taskStateIcon") as Image;
            taskTitle = GetTemplateChild("taskTitle") as Label;

            // Apply props
            string stateString = "Default";
            switch (State)
            {
                case TaskState.Checkmark:
                    stateString = "Checkmark";
                    break;
                case TaskState.Question:
                    stateString = "Question";
                    break;

            }
            taskStateIcon.Source = new BitmapImage(new Uri($"/Resources/Icons/4x/{stateString}.png", UriKind.Relative)); ;
            taskTitle.Content = Title;
        }
    }
}
