using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared.Excel
{

    /// <summary>
    /// Required by class factory. does nothing.
    /// </summary>
    class DummyHostService : IHostService
    {
        public void SetStatusBar(string message)
        {
            // do nothing
        }
    }
}
