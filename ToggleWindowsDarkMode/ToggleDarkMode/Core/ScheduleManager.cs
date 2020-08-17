using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToggleWindowsDarkMode
{
    public static class ScheduleManager
    {
        /// <summary>
        /// Method to run a task at a specific time. For this application,
        /// certain task will be to toggle between dark mode at a certain time.
        /// Cancelable Task.
        /// </summary>
        /// <param name="scheduleTime">When to run a specific task.</param>
        /// <param name="repeatScheduledTask">Set to true or false depending on
        /// if the task will be repeated after completion.</param>
        /// <returns></returns>
        public async static Task RunTaskAtSpecificTimeAsync123(DateTime scheduleTime, bool repeatScheduledTask)
        {
            var ct = new CancellationTokenSource();
            var timeDifferenceInMS = (scheduleTime - DateTime.UtcNow).TotalMilliseconds;

        }

        public static void ToggleDarkModeAtSpecificTime(DateTime scheduleTime, bool repeatScheduledTask)
        {
            var ct = new CancellationTokenSource();
            var timeDifferenceInMS = (int)(scheduleTime - DateTime.UtcNow).TotalMilliseconds;

            Task.Delay(timeDifferenceInMS).ContinueWith((x) =>
            {
                // What method to run
                ToggleDarkMode.SwitchTheme();

                if (repeatScheduledTask)
                {
                    ToggleDarkModeAtSpecificTime(scheduleTime.AddDays(1), true);
                }
            }, ct.Token);
        }


        public static async Task RunTaskAtSpecificTimeStartupAsync()
        {
            // Encapsulate this somewhere else?
            if (Properties.Settings.Default.ScheduleEnabled)
            {
                var savedStartTime = Properties.Settings.Default.ScheduledTime;
                await RunTaskAtSpecificTimeAsync(savedStartTime, true);
            }
        }

        public static async Task RunTaskAtSpecificTimeAsync(DateTime WhenToRunTask, bool RepeatTask)
        {
            var TimeSpanInMS = (int)(WhenToRunTask - DateTime.UtcNow).TotalMilliseconds;

            if (TimeSpanInMS < 0)
            {
                // Shoot off task immediately, and 
                ToggleDarkMode.SwitchTheme();
                if (RepeatTask)
                {
                    await RunTaskAtSpecificTimeAsync(WhenToRunTask.AddDays(1), true);
                }
            }
            else
            {
                // Delay the running of task for set amount of time.
                throw new NotImplementedException();
            }

        }
    }
}
