using Hardcodet.Wpf.TaskbarNotification;
using System.Threading;
using System.Windows;


namespace ToggleWindowsDarkMode
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static TaskbarIcon notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Creating the notifyIcon.  The notifyIcon class specifies a
            // component that creates an icon in the notification area. (Bottom
            // right) The icon is declared in DarkModeUtilityResources.
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            // Launching the startup launch method on startup.
            ToggleDarkMode.Startup();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); // It should clean up after itself, but this is supposedly a 'cleaner' way of doing this.
            base.OnExit(e);
        }
    }
}
