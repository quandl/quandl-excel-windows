using System;
using System.Net;

namespace Quandl.Shared.errors
{
    public class QuandlErrorBase : Exception
    {
        public string ErrorCode { get; internal set; }
        public HttpStatusCode StatusCode { get; internal set; }

        public QuandlErrorBase(HttpStatusCode statusCode, string errorCode = null, string message = null) : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
}
