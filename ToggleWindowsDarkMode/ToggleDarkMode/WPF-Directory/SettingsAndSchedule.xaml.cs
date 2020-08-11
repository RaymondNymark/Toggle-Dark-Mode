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
using System.Windows.Shapes;

namespace ToggleWindowsDarkMode
{
    /// <summary>
    /// Interaction logic for SettingsAndSchedule.xaml
    /// </summary>
    public partial class SettingsAndSchedule : Window
    {
        public SettingsAndSchedule()
        {
            InitializeComponent();

            if (StartupManager.StartupState == Enums.StartupState.Enabled)
            {
                EnableRunOnStartup.IsChecked = true;
            }
            else
            {
                EnableRunOnStartup.IsChecked = false;
            }
        }

        private void EnableRunOnStartup_Checked(object sender, RoutedEventArgs e)
        {
            StartupManager.StartupState = Enums.StartupState.Enabled;
        }

        private void EnableRunOnStartup_Unchecked(object sender, RoutedEventArgs e)
        {
            StartupManager.StartupState = Enums.StartupState.Disabled;
        }
    }
}
