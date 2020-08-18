﻿using System;
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

        private void EnableScheduling_Checked(object sender, RoutedEventArgs e)
        {
            //ScheduleManager.ScheduleState = Enums.ScheduleState.Enabled;
        }

        private void EnableScheduling_Unchecked(object sender, RoutedEventArgs e)
        {
            //ScheduleManager.ScheduleState = Enums.ScheduleState.Disabled;
        }

        // This event handler handles saving changes made to Scheduling.
        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)EnableScheduling.IsChecked)
            {
                // TODO : Implement data binding. This sends the class new datetime.
                ScheduleManager.RetrieveDateTime((DateTime)ScheduleTime.Value);

                ScheduleManager.ScheduleState = Enums.ScheduleState.Enabled;
            }
            else
            {
                ScheduleManager.ScheduleState = Enums.ScheduleState.Disabled;
            }
        }
    }
}
