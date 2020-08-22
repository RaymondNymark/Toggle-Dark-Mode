using System;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static ToggleWindowsDarkMode.Enums;

namespace ToggleWindowsDarkMode
{
    public static class ScheduleManager
    {
        // Private CTS used to keep track if scheduled task was canceled. 
        public static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public static void CancelScheduledTask()
        {
            cancellationTokenSource.Cancel();
        }

        #region New Horizons
        public static void SwitchThemeAt(DateTime whenToSwitch)
        {
            cancellationTokenSource = new CancellationTokenSource();

            var whenToSwitchUTC = whenToSwitch.ToUniversalTime();

            var dateNow = DateTime.UtcNow;
            TimeSpan howLongToDelayFor;

            if (whenToSwitch > dateNow)
            {
                howLongToDelayFor = whenToSwitch - dateNow;

                // Fix settings
                Properties.Settings.Default.ScheduledTime = whenToSwitch;
                Properties.Settings.Default.Save();
            }
            else
            {
                // Switches if it's due to a change.
                Application.Current.Dispatcher.Invoke(() => ToggleDarkMode.SwitchTheme());

                while (whenToSwitch < dateNow)
                {
                    whenToSwitch = whenToSwitch.AddDays(1);
                }
                // Fix settings
                Properties.Settings.Default.ScheduledTime = whenToSwitch;
                Properties.Settings.Default.Save();

                howLongToDelayFor = whenToSwitch - dateNow;
            }

            Task.Delay(howLongToDelayFor).ContinueWith((x) =>
            {
                // Method to run once the delay is over. Dispatcher has to be
                // invoked since the UI thread is being updated by a non main
                // thread.
                Application.Current.Dispatcher.Invoke(() => ToggleDarkMode.SwitchTheme());

                whenToSwitch = whenToSwitch.AddDays(1);
                // Set up the method to run the next day.
                SwitchThemeAt(whenToSwitch);
                // Fix settings for new date.

            }, cancellationTokenSource.Token);
        }

        public static ScheduleState ScheduleState
        {
            get
            {
                if (Properties.Settings.Default.SchedulingIsEnabled)
                {
                    return ScheduleState.Enabled;
                }
                else
                {
                    return ScheduleState.Disabled;
                }
            }
            set
            {
                if (value == ScheduleState.Enabled)
                {
                    // Sets up system settings.
                    Properties.Settings.Default.SchedulingIsEnabled = true;
                    var whenToSwitch = Properties.Settings.Default.ScheduledTime;
                    Properties.Settings.Default.Save();

                    SwitchThemeAt(whenToSwitch);
                }
                else // Properties for ScheduleState.Disabled.
                {
                    Properties.Settings.Default.SchedulingIsEnabled = false;
                    // Default to UTCnow so var is never left as null.
                    Properties.Settings.Default.ScheduledTime = DateTime.UtcNow;

                    Properties.Settings.Default.Save();

                    // Cancel any running tasks if any can be canceled, so it
                    // doesn't randomly fire off.
                    if (cancellationTokenSource.Token.CanBeCanceled)
                    {
                        CancelScheduledTask();
                    }
                }
            }
        }

        /// <summary>
        /// This should be ran at startup to fire off a theme switch if
        /// scheduled time is overdue. Otherwise the program could never be
        /// turned off.
        /// </summary>
        public static void ScheduleStartupProcess()
        {
            if (Properties.Settings.Default.SchedulingIsEnabled)
            {
                var whenToSwitch = Properties.Settings.Default.ScheduledTime;
                SwitchThemeAt(whenToSwitch);
            }
        }

        #endregion
    }
}