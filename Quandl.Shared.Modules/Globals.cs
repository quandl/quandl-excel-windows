using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared
{
    /// <summary>
    /// Class factory pattern
    /// </summary>
    public class Globals
    {
        private readonly static Globals _instance = new Globals();

        public static Globals Instance
        {
            get { return _instance; }
        }

        private Excel.IHostService _hostService;
        public Excel.IHostService HostService
        {
            get { return _hostService = _hostService ?? new Excel.DummyHostService(); }
            set { _hostService = value; }
        }

        private Excel.IStatusBar _statusBar;

        public Excel.IStatusBar StatusBar
        {
            get
            {
                return _statusBar = _statusBar ?? new Excel.StatusBar(this.HostService);
            }
            set
            {
                _statusBar = value;
            }
        }
    }
}
