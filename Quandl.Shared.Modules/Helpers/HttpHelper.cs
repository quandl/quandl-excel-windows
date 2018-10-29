using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared.Helpers
{
    /// <summary>
    /// Configure .NET HTTP client to work with TLS
    /// </summary>
    public static class HttpHelper
    {
        public static void EnableTlsSupport()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

    }
}
