using System;
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

        //TODO: Remove this TMP function.  Serves temporary purpose of input data.  This will be made into something more useful soon.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ScheduleManager.RunTaskAtSpecificTimeAsync(DateTime.UtcNow.AddSeconds(5), true);
            //ScheduleManager.TestTask();
            
            //ScheduleManager.RunTaskAtSpecificTimeAsync(DateTime.UtcNow.AddSeconds(5), true);

            //if (ScheduleTime.Value != null)
            //{ 
            //Properties.Settings.Default.ScheduledTime = (System.DateTime)ScheduleTime.Value;
            //Properties.Settings.Default.Save();

            //ScheduleManager.ToggleDarkModeAtSpecificTime(Properties.Settings.Default.ScheduledTime, true);
            //}
            Application.Current.MainWindow.Close();
        }
        #region Debug-Methods

        private void ScheduleRunButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ScheduleCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
