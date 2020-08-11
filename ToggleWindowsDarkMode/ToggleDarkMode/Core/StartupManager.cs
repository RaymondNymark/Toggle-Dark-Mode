using Microsoft.Win32;
using System;
using static ToggleWindowsDarkMode.Enums;

namespace ToggleWindowsDarkMode
{
    public abstract class StartupManager
    {
        /// <summary>
        /// Returns the name of the current Assembly. Useful for naming!
        /// </summary>
        private static string AssemblyName
        {
            get
            {
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
        /// This retrieves and/or assigns the current StartupState of the
        /// current application. The possible values are: Enabled, Disabled, or
        /// DisabledByUser. DisabledByUser means that the user has explicitly
        /// disabled this program to run on startup through the Task-manager or
        /// through other means.
        /// </summary>
        public static StartupState StartupState
        {
            get
            {
                // Checks registry if the key exists or not. Returns null if it doesn't exist.
                if (Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", AssemblyName, null) != null)
                {
                    // Retrieves a binary value as an array from the
                    // automatically generated key within StartupApproved\Run.
                    // Value starting with "02" means enabled. If it starts with
                    // "03", means the user explicitly disabled it.
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
