using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared
{
    public class QuandlConfig
    {
        public static string ApiKey
        {
            get { return Properties.Settings.Default.ApiKey; }
            set
            {
                Properties.Settings.Default.ApiKey = value;
                Properties.Settings.Default.Save();
            }
        }

        public static bool AutoUpdate
        {
            get { return Properties.Settings.Default.AutoUpdate; }
            set
            {
                Properties.Settings.Default.AutoUpdate = value;
                Properties.Settings.Default.Save();
            }
        }

        public static void Reset()
        {
            Properties.Settings.Default.Reset();
        }
    }
}
