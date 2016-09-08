using System;

namespace Quandl.Shared.Excel
{
    public class NullStatusBar : IStatusBar
    {
        public NullStatusBar()
        {
        }

        public void AddMessage(string message)
        {
        }

        public void AddException(Exception e)
        {
        }
    }
}
