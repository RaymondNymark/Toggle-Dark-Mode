using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Navigation;

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

            // TODO : Implement data binding to avoid this.

            if (StartupManager.StartupState == Enums.StartupState.Enabled)
            {
                EnableRunOnStartup.IsChecked = true;
            }
            else
            {
                EnableRunOnStartup.IsChecked = false;
            }

            if (ScheduleManager.ScheduleState == Enums.ScheduleState.Enabled)
            {
                EnableScheduling.IsChecked = true;
            }
            else
            {
                EnableScheduling.IsChecked = false;
            }

            //ScheduleTime.Value = Properties.Settings.Default.ScheduledTime;
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
            //ScheduleManager.RunTaskAtSpecificTimeAsync(DateTime.UtcNow.AddSeconds(8), true);
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

        private void EnableScheduling_Checked(object sender, RoutedEventArgs e)
        {
            ScheduleTime.IsEnabled = true;
            SaveButton.IsEnabled = true;

            //ScheduleTime.IsEnabled = true;
            //SaveSettings.IsEnabled = true;
        }

        private void EnableScheduling_Unchecked(object sender, RoutedEventArgs e)
        {
            ScheduleTime.IsEnabled = false;
            SaveButton.IsEnabled = false;
        }

        // This event handler handles saving changes made to Scheduling.
        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)EnableScheduling.IsChecked)
            {
                ScheduleManager.ScheduleState = Enums.ScheduleState.Enabled;
            }
            else
            {
                ScheduleManager.ScheduleState = Enums.ScheduleState.Disabled;
            }

            SaveSettings.IsEnabled = false;
        }

        private void DebugScheduleStart_Click(object sender, RoutedEventArgs e)
        {
            DateTime time = (DateTime)DebugScheduleTimePicker.Value;
            ScheduleManager.SwitchThemeAt(time.ToUniversalTime());
        }

        private void DebugScheduleCancel_Click(object sender, RoutedEventArgs e)
        {
            ScheduleManager.CancelScheduledTask();
        }

        private void ToggleThemeDebugBuggon_Click(object sender, RoutedEventArgs e)
        {
            ToggleDarkMode.SwitchTheme();
        }

        private void ClearScheduleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButtonV2_Click(object sender, RoutedEventArgs e)
        {
            if (ScheduleManager.cancellationTokenSource.Token.CanBeCanceled)
            {
                ScheduleManager.cancellationTokenSource.Cancel();
            }

            var scheduleInput = ScheduleTime.Value;
            if (scheduleInput != Properties.Settings.Default.ScheduledTime) SaveSettings.IsEnabled = true;

            Properties.Settings.Default.ScheduledTime = ((DateTime)scheduleInput).ToUniversalTime();
            Properties.Settings.Default.Save();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
