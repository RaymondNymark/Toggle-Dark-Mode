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
            if (StartupManager.StartupState == Enums.StartupState.Enabled) EnableRunOnStartup.IsChecked = true;
            else EnableRunOnStartup.IsChecked = false;
            
            if (ScheduleManager.ScheduleState == Enums.ScheduleState.Enabled) EnableScheduling.IsChecked = true;
            else EnableScheduling.IsChecked = false;
            
            // Changes ScheduleNotice label to tell the user when next automatic
            // theme switch is scheduled at.
            if (Properties.Settings.Default.SchedulingIsEnabled) ScheduleNotice.Content = $"The next automated theme switch is at: {Properties.Settings.Default.ScheduledTime.ToLocalTime()}"; else
            {
                ScheduleNotice.Content = "";
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void EnableScheduling_Checked(object sender, RoutedEventArgs e)
        {
            ScheduleTime.IsEnabled = true;
            SaveButton.IsEnabled = true;

            if (EnableScheduling.IsChecked != Properties.Settings.Default.SchedulingIsEnabled) SaveSettings.IsEnabled = true;
        }

        private void EnableScheduling_Unchecked(object sender, RoutedEventArgs e)
        {
            if (EnableScheduling.IsChecked != Properties.Settings.Default.SchedulingIsEnabled) SaveSettings.IsEnabled = true;
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

        private void SaveButtonV2_Click(object sender, RoutedEventArgs e)
        {
            if (ScheduleManager.cancellationTokenSource.Token.CanBeCanceled)
            {
                ScheduleManager.cancellationTokenSource.Cancel();
            }

            var scheduleInput = ((DateTime)ScheduleTime.Value).ToUniversalTime();

            // Checks if any of the inputs are different, if they are it enables
            // the main save button.
            if (scheduleInput != Properties.Settings.Default.ScheduledTime || EnableScheduling.IsChecked != Properties.Settings.Default.SchedulingIsEnabled) SaveSettings.IsEnabled = true;

            Properties.Settings.Default.ScheduledTime = scheduleInput;
            Properties.Settings.Default.Save();
        }

        // Hyperlink event handler.
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }


        // Bellow here are only debug methods. Debug tab is disabled by default.
        // Mostly related to testing ScheduleManager.
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

        #endregion
    }
}
