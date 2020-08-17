using System;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ToggleWindowsDarkMode
{
    public static class ScheduleManager
    {
        private static CancellationTokenSource cancellationTokenSource;

        public static void CancelScheduledTask()
        {
            cancellationTokenSource.Cancel();
        }

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
        public static async Task RunTaskAtSpecificTimeAsync123(DateTime WhenToRunTask, bool RepeatTask)
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

        /// <summary>
        /// Runs SwitchTheme task after a delay in MS.
        /// </summary>
        /// <param name="howLongToDelayTaskForInMS">Delay in MS.</param>
        /// <returns></returns>
        private static Task RunTaskAfterDelay(int howLongToDelayTaskForInMS)
        {
            //cancellationTokenSource = new CancellationTokenSource();
            return Task.Run(() =>
            {
                Task.Delay(howLongToDelayTaskForInMS).ContinueWith((x) =>
                {
                    if (!cancellationTokenSource.IsCancellationRequested) ToggleDarkMode.SwitchTheme();
                });
            });
        }


        public static async Task RunTaskAtSpecificTimeAsync(DateTime whenToRunTask, bool repeatTask)
        {
            cancellationTokenSource = new CancellationTokenSource();
            var timeSpanInMS = (int)(whenToRunTask - DateTime.UtcNow).TotalMilliseconds;

            if (timeSpanInMS < 0)
            {
                // Instantly run the task.
                ToggleDarkMode.SwitchTheme();

                if (repeatTask)
                {
                    await RunTaskAtSpecificTimeAsync(whenToRunTask.AddDays(1), true);
                }
            }
            else
            {
                // Delay running the task for a set amount of time.
                await RunTaskAfterDelay(timeSpanInMS).ContinueWith(async (x)=> { if (repeatTask) await Task.Run(() => RunTaskAtSpecificTimeAsync(whenToRunTask.AddSeconds(5), true)); });

                //if (repeatTask)
                //{
                //    await RunTaskAtSpecificTimeAsync(whenToRunTask.AddDays(1), true);
                //}
            }
        }


        public static async Task ExecuteTaskAsync()
        {
            cancellationTokenSource = new CancellationTokenSource();
            using (cancellationTokenSource)
            {
                try
                {
                    await RunTaskAfterDelay(5000);
                }
                catch (TaskCanceledException)
                {
                    //CanceledStatus.Content = "Canceled";
                    // Dont do anything
                }
            }
        }

    }
}
