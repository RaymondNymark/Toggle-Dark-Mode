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

        #region legacy-code

        //// Keeps track of userDateTimeInput. This is a temporary band-aid fix.
        //private static DateTime userDateTimeInput;

        ///// <summary>
        ///// Method to switch between the current Windows theme at a specific
        ///// time. Scheduled task can be canceled with CancelScheduledTask
        ///// method.
        ///// </summary>
        ///// <param name="whenToRunTask">At what DateTime to switch the current Windows theme.</param>
        ///// <param name="repeatTask">Should this task be repeated.</param>
        ///// <returns></returns>
        //public static async Task RunTaskAtSpecificTimeAsync(DateTime whenToRunTask, bool repeatTask)
        //{
        //    // There is something wrong with this method, something causing
        //    // timeSpanInMs to be calculated to -2.147b.  And I can't figure it
        //    // out for the life of me.
        //    int timeSpanInMS = (int)(whenToRunTask.ToUniversalTime() - DateTime.UtcNow).TotalMilliseconds;

        //    cancellationTokenSource = new CancellationTokenSource();

        //    if (timeSpanInMS > 0)
        //    {
        //        // Delay running a task by a set number of time. After it ran,
        //        // repeat it but make it run 24hr later.
        //        await RunTaskAfterDelay(timeSpanInMS).ContinueWith(async (x) => { if (repeatTask) await Task.Run(() => RunTaskAtSpecificTimeAsync(whenToRunTask.AddDays(1), true)); });
        //    }
        //    else
        //    {
        //        if (timeSpanInMS < 0)
        //        {
        //            // Instantly run the task.
        //            ToggleDarkMode.SwitchTheme();

        //            if (repeatTask) await RunTaskAtSpecificTimeAsync(whenToRunTask.AddDays(1), true);
        //        }
        //        else
        //        {
        //            throw new NotImplementedException();
        //        }
        //    }
        //}

        ///// <summary>
        ///// Runs SwitchTheme task after a delay in MS.
        ///// </summary>
        ///// <param name="howLongToDelayTaskForInMS">Delay in MS.</param>
        ///// <returns></returns>
        //private static Task RunTaskAfterDelay(int howLongToDelayTaskForInMS)
        //{
        //    return Task.Run(() =>
        //    {
        //        Task.Delay(howLongToDelayTaskForInMS).ContinueWith((x) =>
        //        {
        //            if (!cancellationTokenSource.IsCancellationRequested) ToggleDarkMode.SwitchTheme();
        //        });
        //    });
        //}

        //public static void CancelScheduledTask()
        //{
        //    cancellationTokenSource.Cancel();
        //}

        ///// <summary>
        ///// Task to run on program startup.
        ///// </summary>
        ///// <returns></returns>
        //public static async Task RunTaskAtSpecificTimeStartupAsync()
        //{
        //    // Encapsulate this somewhere else?
        //    if (Properties.Settings.Default.ScheduleEnabled)
        //    {
        //        var savedStartTime = Properties.Settings.Default.ScheduledTime;
        //        await RunTaskAtSpecificTimeAsync(savedStartTime, true);
        //    }
        //}



        //public static ScheduleState ScheduleState
        //{
        //    get
        //    {
        //        if (Properties.Settings.Default.ScheduleEnabled)
        //        {
        //            return ScheduleState.Enabled;
        //        }
        //        else
        //        {
        //            return ScheduleState.Disabled;
        //        }
        //    }
        //    set
        //    {
        //        //var userDateTimeInput = userDateTimeInput;
        //        if (value == ScheduleState.Enabled)
        //        {
        //            Properties.Settings.Default.ScheduleEnabled = true;
        //            Properties.Settings.Default.ScheduledTime = userDateTimeInput;
        //            Properties.Settings.Default.Save();

        //            RunTaskAtSpecificTimeAsync(userDateTimeInput, true);
        //        }
        //        else
        //        {
        //            if (value == ScheduleState.Disabled)
        //            {
        //                // Code to cancel schedule from occurring.
        //                //CancelScheduledTask();

        //                Properties.Settings.Default.ScheduleEnabled = false;
        //                Properties.Settings.Default.ScheduledTime = DateTime.UtcNow;
        //                Properties.Settings.Default.Save();
        //            }
        //            else
        //            {
        //                // If somehow a non existent enum is passed through.
        //                throw new NotSupportedException();
        //            }

        //        }
        //    }
        //}

        //// Debug method to retrieve DateTime entered in SettingsAndScheduleXaml.
        //public static void RetrieveDateTime(DateTime dateTime)
        //{
        //    userDateTimeInput = dateTime.ToUniversalTime();
        //}


        //public static void ScheduleStartup()
        //{
        //    var whenToRunTask = Properties.Settings.Default.ScheduledTime;

        //    if (cancellationTokenSource.Token.CanBeCanceled)
        //    {
        //        CancelScheduledTask();
        //    }

        //    if (Properties.Settings.Default.ScheduleEnabled)
        //    {
        //        RunTaskAtSpecificTimeAsync(whenToRunTask, true);
        //    }

        //}

        #endregion
    }
}