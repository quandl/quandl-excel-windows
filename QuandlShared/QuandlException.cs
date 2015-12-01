using System;

namespace Quandl.Shared.QuandlException
{
    public class QuandlDataNotFoundException: Exception
    {
        public QuandlDataNotFoundException() 
            : base("Quandl data is not found!")
        { }
    }
}
