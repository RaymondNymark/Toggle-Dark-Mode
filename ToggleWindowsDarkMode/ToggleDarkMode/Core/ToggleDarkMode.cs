using System;
using Microsoft.Win32;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Media.Imaging;
using static ToggleWindowsDarkMode.Enums;

namespace ToggleWindowsDarkMode
{
    public abstract class ToggleDarkMode
    {
        // Path to the Registry root used for managing the Windows theme. Used
        // to improve code readability.
        private const string RegistryRoot = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";

        /// <summary>
        /// Retrieves or sets the current Theme state of Windows. Possible
        /// values for this are: Dark (0), and Light (1). When ThemeState is
        /// retrieved, it immediately checks if the value "SystemUsesLightTheme"
        /// exists or not. If it doesn't exist, the user's version of Windows 10
        /// is severely out of date as System wide dark/light mode were
        /// implemented a while after the first Windows 10 release.
        /// </summary>
        public static ThemeState ThemeState
        {
            get
            {
                if (Registry.GetValue(RegistryRoot, "SystemUsesLightTheme", null) != null)
                {
                    int SystemUsesLightTheme = (int)Registry.GetValue(RegistryRoot, "SystemUsesLightTheme", null);
                    int AppsUseLightTheme = (int)Registry.GetValue(RegistryRoot, "AppsUseLightTheme", null);

                    if (SystemUsesLightTheme == AppsUseLightTheme && SystemUsesLightTheme == 1)
                    {
                        return ThemeState.Light;
                    }
                    else
                    {
                        return ThemeState.Dark;
                    }
                }
                else
                {
                    WarnUserThatWindows10IsOutOfDate();
                    return ThemeState.Light;
                }
            }
            set
            {
                if (value == ThemeState.Light)
                {
                    Registry.SetValue(RegistryRoot, "SystemUsesLightTheme", 1);
                    Registry.SetValue(RegistryRoot, "AppsUseLightTheme", 1);
                    IconState = IconState.LightIcon;
                }
                else
                {
                    if (value == ThemeState.Dark)
                    {
                        Registry.SetValue(RegistryRoot, "SystemUsesLightTheme", 0);
                        Registry.SetValue(RegistryRoot, "AppsUseLightTheme", 0);
                        IconState = IconState.DarkIcon;
                    }
                    else { throw new NotSupportedException(); }
                }
            }
        }

        /// <summary>
        /// Retrieves or sets the system tray icon's image-source. Possible
        /// values for this are: DarkIcon (0), and LightIcon (1).
        /// </summary>
        public static IconState IconState
        {
            get
            {
                string CurrentIconSource = App.notifyIcon.IconSource.ToString();
                if (CurrentIconSource == "pack://application:,,,/Toggle-Dark-Mode;component/toggledarkmode/wpf-directory/Resources/Light.ico")
                {
                    return IconState.LightIcon;
                }
                else // App.NotifyIcon.IconSource == "pack://application:,,,/Toggle-Dark-Mode;component/toggledarkmode/wpf-directory/Resources/Dark.ico"
                {
                    return IconState.DarkIcon;
                }
            }
            set
            {
                if (value == IconState.LightIcon)
                {
                    var IconFilePath = "pack://application:,,,/Toggle-Dark-Mode;component/toggledarkmode/wpf-directory/Resources/Light.ico";
                    App.notifyIcon.IconSource = new BitmapImage(new Uri(IconFilePath));
                }
                else // value == IconState.DarkIcon
                {
                    var IconFilePath = "pack://application:,,,/Toggle-Dark-Mode;component/toggledarkmode/wpf-directory/Resources/Dark.ico";
                    App.notifyIcon.IconSource = new BitmapImage(new Uri(IconFilePath));
                }
            }
        }

        /// <summary>
        /// Flips the theme between Dark and Light. If it's currently Light, it
        /// turns to Dark. If it's Dark it turns to Light.
        /// </summary>
        public static void SwitchTheme()
        {
            if (ThemeState == ThemeState.Light)
            {
                ThemeState = ThemeState.Dark;
            }
            else // ThemeState == ThemeState.Dark
            {
                ThemeState = ThemeState.Light;
            }
        }

        /// <summary>
        /// Warns the user once that their version of Windows 10 may potentially
        /// be out of date, as system wide dark mode feature isn't found. This
        /// only happens once after this app has been launched.
        /// </summary>
        private static void WarnUserThatWindows10IsOutOfDate()
        {
            if (!Properties.Settings.Default.UserHasBeenWarnedAboutWin10BeingOutOfDate)
            {
                App.notifyIcon.ShowBalloonTip("Windows 10 may be out of date.", "Your version of Windows 10 may be out of date since it lacks system dark-mode. This app should still function, but certain parts may not work.", default);

                Properties.Settings.Default.UserHasBeenWarnedAboutWin10BeingOutOfDate = true;
                Properties.Settings.Default.Save();
            }
        }
    }
}
