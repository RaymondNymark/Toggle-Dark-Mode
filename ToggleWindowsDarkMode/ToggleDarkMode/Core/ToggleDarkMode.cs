using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using System.Drawing;
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
        /// Warns the user once that their version of Windows 10 may be out of
        /// date by lacking system wide dark / light themes.
        /// </summary>
        private static void WarnUserThatWindows10IsOutOfDate()
        {
            if (!Properties.Settings.Default.UserHasBeenWarnedAboutWin10BeingOutOfDate)
            {
                //TODO FIX THIS.
                App.notifyIcon.ShowBalloonTip("Windows 10 may be out of date.", "Your current version of Windows 10 lacks system wide dark mode, and thus may be out of date. This app should still function, but certain parts may not work.", default);

                Properties.Settings.Default.UserHasBeenWarnedAboutWin10BeingOutOfDate = true;
                Properties.Settings.Default.Save();
            }
        }

        #region Legacy-Code (graveyard)
        //#region Startup setting vars
        //private static bool _correctVersionFlag;
        //private static bool _systemHasBeenChecked;
        //private static int _currentTheme;
        //#endregion

        //#region Private Variable Declarations
        //private const string _userRoot = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";

        //// The two keys within Windows registry responsible for default Windows dark mode.
        //private const string _subKeyOne = "SystemUsesLightTheme";
        //private const string _subKeyTwo = "AppsUseLightTheme";

        //private const string _keyOne = _userRoot + "\\" + "SystemUsesLightTheme";
        //private const string _keyTwo = _userRoot + "\\" + "AppsUseLightTheme";
        //#endregion
        //#region Darkmode Methods
        //// Method that toggles the current theme between dark and light.
        //public async static Task ToggleTheme()
        //{
        //    try
        //    {
        //        ChangeTheme(GetNextTheme());
        //        ChangeIcon(GetNextTheme());
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }

        //}

        //private static int GetNextTheme()
        //{
        //    var currentTheme = GetCurrentTheme();
        //    var nextTheme = 1 - currentTheme;
        //    return nextTheme;
        //}

        //// Changes the registry value behind Window theme. 0 = Dark, 1 = Light.
        //private static void ChangeTheme(int nextTheme)
        //{
        //    try
        //    {
        //        Registry.SetValue(_userRoot, _subKeyOne, nextTheme);
        //        Registry.SetValue(_userRoot, _subKeyTwo, nextTheme);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Something went wrong...");
        //    }
        //}



        ///// <summary>
        ///// Sets the iconSource of this application to Dark.ico / Light.ico
        ///// depending on input parameter. 0 = Dark.ico, 1 = Light.ico. 
        ///// </summary>
        ///// <param name="nextTheme">0 = Dark.ico, 1 = Light.ico</param>
        //public static void ChangeIcon(int nextTheme)
        //{
        //    string filePath;

        //    if (nextTheme == 1)
        //        filePath = "pack://application:,,,/Toggle-Dark-Mode;component/toggledarkmode/wpf-directory/Resources/Dark.ico";
        //    else
        //        filePath = "pack://application:,,,/Toggle-Dark-Mode;component/toggledarkmode/wpf-directory/Resources/Light.ico";

        //    var uri = new Uri(filePath);

        //    try
        //    {
        //        App.notifyIcon.IconSource = new BitmapImage(uri);
        //    }
        //    catch
        //    {
        //        throw new Exception();
        //    }
        //}

        //private static int GetCurrentTheme()
        //{
        //    string currentTheme = Registry.GetValue(_userRoot, _subKeyOne, 0).ToString();
        //    try
        //    {
        //        int currentThemeInt = Int32.Parse(currentTheme);
        //        return currentThemeInt;
        //    }
        //    //TODO: IMPLEMENT BETTER VALIDATION
        //    catch (FormatException e)
        //    {
        //        // If it can't figure out what the theme is for whatever reason, it makes it 0.
        //        Registry.SetValue(_userRoot, _subKeyOne, 0);
        //        Registry.SetValue(_userRoot, _subKeyTwo, 0);
        //        return 0;
        //    }
        //}


        //#endregion

        //#region LaunchMethods and system check.
        //public static async Task Startup()
        //{
        //    // Checking if system has ran first-time system check. This warns
        //    // the user if their Windows lacks ability to be able to switch
        //    // between dark / light mode.
        //    _systemHasBeenChecked = Properties.Settings.Default.SystemHasBeenChecked;
        //    if (!_systemHasBeenChecked)
        //    {
        //        await FirstTimeSetup();
        //    }

        //    // Sets the Icon Source to the current theme.
        //    ChangeIcon(GetNextTheme());
        //}

        ///// <summary>
        ///// Method that will only launch once after running it for the first
        ///// time. This method checks if the registry keys required to use the
        ///// in-built Dark / Light mode features exist on the user's computer.
        ///// If the keys don't exist, the user is warned about this.
        ///// </summary>
        ///// <returns></returns>
        //private static async Task FirstTimeSetup()
        //{
        //    // Checking if registry keys exist.
        //    var keysExist = await Task.Run(() => CheckIfKeysExist());

        //    if (!keysExist)
        //    {
        //        // Warns a user that their system may be out of date if the keys
        //        // don't exist.
        //        SendOutOfDateWarning();
        //    }

        //    // Sets SystemHasBeenChecked flag to true and saves it.
        //    Properties.Settings.Default.SystemHasBeenChecked = true;
        //    Properties.Settings.Default.Save();
        //}

        ///// <summary>
        ///// TODO: Implement this.
        ///// </summary>
        //private static void SendOutOfDateWarning()
        //{
        //    throw new NotImplementedException();
        //}

        //// Checks if the sub-keys required for dark / light theme exist in the
        //// user's windows build. Dark mode was partially implemented at windows
        //// 10 release, but the full system wide dark mode implementation came
        //// later. Older versions of Windows 10 will not have the correct
        //// registry keys.
        //private static bool CheckIfKeysExist()
        //{
        //    var keyOne = Registry.GetValue(_userRoot, _subKeyOne, null);
        //    var keyTwo = Registry.GetValue(_userRoot, _subKeyTwo, null);
        //    // Returns true if both keys exist, returns false if neither, or one of the keys don't exist.
        //    return !((keyOne == null) || (keyTwo == null));
        //}
        //#endregion
        #endregion
    }
}
