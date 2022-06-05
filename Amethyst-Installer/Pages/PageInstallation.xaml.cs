using amethyst_installer_gui.Installer;
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

namespace amethyst_installer_gui.Pages
{
    /// <summary>
    /// Interaction logic for PageInstallation.xaml
    /// </summary>
    public partial class PageInstallation : UserControl, IInstallerPage
    {
        public PageInstallation()
        {
            InitializeComponent();
        }

        public InstallerState GetInstallerState()
        {
            return InstallerState.Installation;
        }

        public string GetTitle()
        {
            return Properties.Resources.Page_Install_Title;
        }

        public void OnSelected()
        {
            
        }
    }
}
