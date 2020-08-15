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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var i2 = ScheduleTime.Value;
            Application.Current.MainWindow.Close();
        }
    }
}
