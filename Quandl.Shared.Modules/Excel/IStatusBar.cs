﻿namespace Quandl.Shared.Excel
{
    public interface IStatusBar
    {
        void AddMessage(string message);

        void AddException(System.Exception e);
    }
}
