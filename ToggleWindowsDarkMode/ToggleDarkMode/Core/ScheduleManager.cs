using System;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ToggleWindowsDarkMode
{
    public static class ScheduleManager
    {
        // Private CTS used to keep track if scheduled task was canceled. 
        private static CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Method to switch between the current Windows theme at a specific
        /// time. Scheduled task can be canceled with CancelScheduledTask
        /// method.
        /// </summary>
        /// <param name="whenToRunTask">At what DateTime to switch the current Windows theme.</param>
        /// <param name="repeatTask">Should this task be repeated.</param>
        /// <returns></returns>
        public static async Task RunTaskAtSpecificTimeAsync(DateTime whenToRunTask, bool repeatTask)
        {
            cancellationTokenSource = new CancellationTokenSource();
            var timeSpanInMS = (int)(whenToRunTask - DateTime.UtcNow).TotalMilliseconds;

            if (timeSpanInMS < 0)
            {
                // Instantly run the task.
                ToggleDarkMode.SwitchTheme();

                if (repeatTask) await RunTaskAtSpecificTimeAsync(whenToRunTask.AddDays(1), true);
            }
            else
            {
                // Delay running a task by a set number of time. After it ran,
                // repeat it but make it run 24hr later.
                await RunTaskAfterDelay(timeSpanInMS).ContinueWith(async (x) => { if (repeatTask) await Task.Run(() => RunTaskAtSpecificTimeAsync(whenToRunTask.AddSeconds(5), true)); });
            }
        }




        /// <summary>
        /// Runs SwitchTheme task after a delay in MS.
        /// </summary>
        /// <param name="howLongToDelayTaskForInMS">Delay in MS.</param>
        /// <returns></returns>
        private static Task RunTaskAfterDelay(int howLongToDelayTaskForInMS)
        {
            return Task.Run(() =>
            {
                Task.Delay(howLongToDelayTaskForInMS).ContinueWith((x) =>
                {
                    if (!cancellationTokenSource.IsCancellationRequested) ToggleDarkMode.SwitchTheme();
                });
            });
        }

        public static void CancelScheduledTask()
        {
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Task to run on program startup.
        /// </summary>
        /// <returns></returns>
        public static async Task RunTaskAtSpecificTimeStartupAsync()
        {
            // Encapsulate this somewhere else?
            if (Properties.Settings.Default.ScheduleEnabled)
            {
                var savedStartTime = Properties.Settings.Default.ScheduledTime;
                await RunTaskAtSpecificTimeAsync(savedStartTime, true);
            }
        }
    }
}
