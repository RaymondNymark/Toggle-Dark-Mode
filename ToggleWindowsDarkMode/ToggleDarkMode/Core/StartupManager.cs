using Microsoft.Win32;
using System;
using static ToggleWindowsDarkMode.Enums;

namespace ToggleWindowsDarkMode
{
    public abstract class StartupManager
    {
        public static string AssemblyName
        {
            get
            {
                // Returns the name of the current Assembly. Useful for naming!
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            }
        }

        /// <summary>
        /// Returns the current complete application target path as a string so
        /// it can be used within the Windows registry. Example string:
        /// "C:\Windows\...\thisprogram.exe"
        /// </summary>
        private static string ApplicationTargetPath
        {
            get
            {
                // Fetches the base directory of the current running program.
                var baseDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                // Fetches the "program.exe" as a string where program is the
                // name of the currently running process.
                var applicationExe = System.AppDomain.CurrentDomain.FriendlyName;
                // Returns full path of the current running application.
                return baseDirectory + "\\" + applicationExe;
            }
        }


        /// <summary>
        /// Accessing the current StartupState.  Possible states are Enabled, Disabled, Or DisabledByUser.
        /// </summary>
        public static StartupState StartupState
        {
            get
            {
                if (Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", AssemblyName, null) != null)
                {
                    byte[] status = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StartupApproved\\Run", AssemblyName, null) as byte[];

                    if (status != null && status.Length > 0 && status[0] == 3)
                    {
                        return StartupState.DisabledByUser;
                    }
                    else
                    {
                        return StartupState.Enabled;
                    }
                }
                else
                {
                    return StartupState.Disabled;
                }
            }
            set
            {
                if (value == StartupState.Enabled)
                {
                    Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", AssemblyName, ApplicationTargetPath);
                }
                else
                {
                    if (value == StartupState.Disabled)
                    {
                        Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", AssemblyName, String.Empty);
                    }
                    else { throw new NotSupportedException(); }
                }

            }
        }
    }
}
