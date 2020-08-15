using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ToggleWindowsDarkMode
{
    public class DarkModeUtilityViewModel
    {
        // Toggles Dark-Mode
        public ICommand ToggleDarkModeCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        ToggleDarkMode.SwitchTheme();
                    }
                };
            }
        }

        // Todo: implement ShowAboutCommand
        public ICommand ShowAboutCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {

                    }
                };
            }
        }

        // Opens up the Settings and Schedule Window if no other windows are present.
        public ICommand ShowSettingsAndScheduleCommand
        {
            get
            {
                return new DelegateCommand
                {
                    //Temp commenting
                    //CanExecuteFunc = () => Application.Current.MainWindow == null,
                    CommandAction = () =>
                    {
                        Application.Current.MainWindow = new SettingsAndSchedule();
                        Application.Current.MainWindow.Show();
                    }
                };
            }
        }

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }
    }

    /// <summary>
    /// Simplistic delegate command.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }


}
