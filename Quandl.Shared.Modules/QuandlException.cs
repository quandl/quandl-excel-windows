using System;

namespace Quandl.Shared
{
    public class QuandlDataNotFoundException : Exception
    {
        public QuandlDataNotFoundException()
            : base("Data could not be found!")
        {
        }
    }

    public class QuandlDateCanNotBlankException : Exception
    {
        public QuandlDateCanNotBlankException()
            : base("Date can not be blank!")
        {
        }
    }

    public class QuandlInvalidDateFormatException : Exception
    {
        public QuandlInvalidDateFormatException()
            : base("Invalid date fromat!")
        {
        }
    }

    public class QuandlFromDateIsGreaterThanEndDateException : Exception
    {
        public QuandlFromDateIsGreaterThanEndDateException()
            : base("Start date is greater than end date!")
        {
        }
    }

}