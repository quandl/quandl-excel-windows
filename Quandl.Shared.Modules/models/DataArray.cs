using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Quandl.Shared.Models
{
    public class DataArray
    {
        [JsonExtensionData] private IDictionary<string, JToken> _additionalData;

        public List<List<object>> DataPoints { get; set; }
        public List<DataColumn> Columns { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_additionalData.ContainsKey("dataset_data"))
            {
                var jTokenColumns = _additionalData["dataset_data"].SelectToken("column_names").Values<string>();
                Columns = jTokenColumns.Select(c => new DataColumn
                {
                    Name = c,
                    ProviderType = ProviderType.TimeSeries
                }).ToList();
                DataPoints = _additionalData["dataset_data"].SelectToken("data").ToObject<List<List<object>>>();
            }
        }
    }
}