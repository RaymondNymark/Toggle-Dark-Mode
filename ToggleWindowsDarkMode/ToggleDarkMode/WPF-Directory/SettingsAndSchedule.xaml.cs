using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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

        #region Debug-Options and Methods
        private void ScheduleRunButton_Click(object sender, RoutedEventArgs e)
        {
            // Should toggle darkmode after 5 seconds.
            ScheduleManager.RunTaskAtSpecificTimeAsync(DateTime.UtcNow.AddSeconds(8), true);
        }

        private void ScheduleCancel_Click(object sender, RoutedEventArgs e)
        {
            ScheduleManager.CancelScheduledTask();
        }

        #endregion

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
