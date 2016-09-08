using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Quandl.Shared.Models
{
    public class DataArray
    {
        [JsonExtensionData] private IDictionary<string, JToken> _additionalData = null;

        public List<List<object>> DataPoints { get; set; }
        public List<DataColumn> Columns { get; set; }
        public string Cursor { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // Dealing with a dataset data response
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

            // Dealing with a datatable data response
            else if (_additionalData.ContainsKey("datatable"))
            {
                Columns = new List<DataColumn>();
                foreach (var c in _additionalData["datatable"].SelectToken("columns"))
                {
                    Columns.Add(new DataColumn {
                        Name = c.SelectToken("name").Value<string>().ToUpper(),
                        ProviderType = ProviderType.DataTable
                    });
                }
                DataPoints = _additionalData["datatable"].SelectToken("data").ToObject<List<List<object>>>();
                Cursor = _additionalData["meta"].SelectToken("next_cursor_id").Value<string>();
            }
        }
    }
}