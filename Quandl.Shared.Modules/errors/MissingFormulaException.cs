using System;

namespace Quandl.Shared.Errors
{
    public class MissingFormulaException : Exception
    {
        public MissingFormulaException(string message) : base(message)
        {
        }
    }
}