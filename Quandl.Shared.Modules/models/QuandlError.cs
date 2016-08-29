using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Quandl.Shared.Models
{
    public class QuandlError
    {
        [JsonExtensionData]
        private IDictionary<string, JToken> _additionalData;

        public string Code { get; set; }
        public string Message { get; set; }


        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_additionalData.Keys.Contains("quandl_error"))
            {
                var error = _additionalData["quandl_error"];
                Code = error.Value<string>("code");
                Message = error.Value<string>("message");
            }
        }

    }
}
