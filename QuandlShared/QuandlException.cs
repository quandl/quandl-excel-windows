using System;

namespace Quandl.Shared
{
    public class QuandlDataNotFoundException: Exception
    {
        public QuandlDataNotFoundException() 
            : base("Quandl data is not found!")
        { }
    }
}
