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

namespace ToggleWindowsDarkMode
{
    public static class ToggleDarkMode
    {
        #region Startup setting vars
        private static bool _correctVersionFlag;
        private static bool _systemHasBeenChecked;
        private static int _currentTheme;
        #endregion

        #region Private Variable Declarations
        private const string _userRoot = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";

        // The two keys within Windows registry responsible for default Windows dark mode.
        private const string _subKeyOne = "SystemUsesLightTheme";
        private const string _subKeyTwo = "AppsUseLightTheme";

        private const string _keyOne = _userRoot + "\\" + "SystemUsesLightTheme";
        private const string _keyTwo = _userRoot + "\\" + "AppsUseLightTheme";
        #endregion
        #region Darkmode Methods
        // Method that toggles the current theme between dark and light.
        public async static Task ToggleTheme()
        {
            try
            {
                ChangeTheme(GetNextTheme());
                ChangeIcon(GetNextTheme());
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private static int GetNextTheme()
        {
            var currentTheme = GetCurrentTheme();
            var nextTheme = 1 - currentTheme;
            return nextTheme;
        }

        // Changes the registry value behind Window theme. 0 = Dark, 1 = Light.
        private static void ChangeTheme(int nextTheme)
        {
            try
            {
                Registry.SetValue(_userRoot, _subKeyOne, nextTheme);
                Registry.SetValue(_userRoot, _subKeyTwo, nextTheme);
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong...");
            }
        }



        /// <summary>
        /// Sets the iconSource of this application to Dark.ico / Light.ico
        /// depending on input parameter. 0 = Dark.ico, 1 = Light.ico. 
        /// </summary>
        /// <param name="nextTheme">0 = Dark.ico, 1 = Light.ico</param>
        public static void ChangeIcon(int nextTheme)
        {
            string filePath;

            if (nextTheme == 1)
                filePath = "pack://application:,,,/Toggle-Dark-Mode;component/toggledarkmode/wpf-directory/Resources/Dark.ico";
            else
                filePath = "pack://application:,,,/Toggle-Dark-Mode;component/toggledarkmode/wpf-directory/Resources/Light.ico";

            var uri = new Uri(filePath);

            try
            {
                App.notifyIcon.IconSource = new BitmapImage(uri);
            }
            catch
            {
                throw new Exception();
            }
        }

        private static int GetCurrentTheme()
        {
            string currentTheme = Registry.GetValue(_userRoot, _subKeyOne, 0).ToString();
            try
            {
                int currentThemeInt = Int32.Parse(currentTheme);
                return currentThemeInt;
            }
            //TODO: IMPLEMENT BETTER VALIDATION
            catch (FormatException e)
            {
                // If it can't figure out what the theme is for whatever reason, it makes it 0.
                Registry.SetValue(_userRoot, _subKeyOne, 0);
                Registry.SetValue(_userRoot, _subKeyTwo, 0);
                return 0;
            }
        }


        #endregion

        #region LaunchMethods and system check.
        public static async Task Startup()
        {
            // Checking if system has ran first-time system check. This warns
            // the user if their Windows lacks ability to be able to switch
            // between dark / light mode.
            _systemHasBeenChecked = Properties.Settings.Default.SystemHasBeenChecked;
            if (!_systemHasBeenChecked)
            {
                await FirstTimeSetup();
            }

            // Sets the Icon Source to the current theme.
            ChangeIcon(GetNextTheme());
        }

        /// <summary>
        /// Method that will only launch once after running it for the first
        /// time. This method checks if the registry keys required to use the
        /// in-built Dark / Light mode features exist on the user's computer.
        /// If the keys don't exist, the user is warned about this.
        /// </summary>
        /// <returns></returns>
        private static async Task FirstTimeSetup()
        {
            // Checking if registry keys exist.
            var keysExist = await Task.Run(() => CheckIfKeysExist());

            if (!keysExist)
            {
                // Warns a user that their system may be out of date if the keys
                // don't exist.
                SendOutOfDateWarning();
            }

            // Sets SystemHasBeenChecked flag to true and saves it.
            Properties.Settings.Default.SystemHasBeenChecked = true;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// TODO: Implement this.
        /// </summary>
        private static void SendOutOfDateWarning()
        {
            throw new NotImplementedException();
        }

        // Checks if the sub-keys required for dark / light theme exist in the
        // user's windows build. Dark mode was partially implemented at windows
        // 10 release, but the full system wide dark mode implementation came
        // later. Older versions of Windows 10 will not have the correct
        // registry keys.
        private static bool CheckIfKeysExist()
        {
            var keyOne = Registry.GetValue(_userRoot, _subKeyOne, null);
            var keyTwo = Registry.GetValue(_userRoot, _subKeyTwo, null);
            // Returns true if both keys exist, returns false if neither, or one of the keys don't exist.
            return !((keyOne == null) || (keyTwo == null));
        }
        #endregion

    }
}
