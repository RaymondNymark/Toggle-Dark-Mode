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
        /// Method to run a task at a specific time.  For this application, a
        /// certain task will be to toggle between dark mode at a certain time.
        /// This task can be canceled. 
        /// </summary>
        /// <param name="WhenToRunTask">At what DateTime to run a specific
        /// Task</param>
        /// <param name="RepeatTask">Boolean, if set to true the task will
        /// repeat running.</param>
        /// <returns></returns>
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
                var cancellationTokenSource = new CancellationTokenSource();
                {
                    try
                    {
                        await Task.Delay(TimeSpanInMS).ContinueWith((x) =>
                        {
                            ToggleDarkMode.SwitchTheme();
                        }, cancellationTokenSource.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        // Don't really do anything.
                    }
                }
            }
        }
        public static void RunTaskAtSpecificTime(DateTime whenToRunTask, bool repeatTask)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var TimeSpanInMS = (int)(whenToRunTask - DateTime.UtcNow).TotalMilliseconds;


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


        public static void TestTask()
        {
            var ctSource = new CancellationTokenSource();

            var tsinMS = 5000;

            Task.Delay(tsinMS).ContinueWith((x) =>
            {
                ToggleDarkMode.SwitchTheme();

                TestTask();
            }, ctSource.Token);

        }

    }
}
