using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared.Excel
{
    /// <summary>
    /// Decouple update of status bar from Excel application instance
    /// </summary>
    public interface IHostService
    {
        void SetStatusBar(string message);
    }
}
