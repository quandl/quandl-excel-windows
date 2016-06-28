using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared.errors
{
    class QuandlErrorBase : Exception
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
