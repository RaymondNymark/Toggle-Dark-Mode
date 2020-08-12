using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToggleWindowsDarkMode
{
    public static class Enums
    {
        public enum StartupState
        {
            Disabled,
            DisabledByUser,
            Enabled
        }

        public enum ThemeState
        {
            Dark,
            Light
        }

        public enum IconState
        {
            DarkIcon,
            LightIcon
        }
    }
}
