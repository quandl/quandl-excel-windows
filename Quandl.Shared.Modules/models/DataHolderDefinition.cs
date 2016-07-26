using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Quandl.Shared.Models
{
    public abstract class DataHolderDefinition : DependencyObject, IDataDefinition, IDataStructure
    {
        [JsonExtensionData] private IDictionary<string, JToken> _additionalData;

        public DataArray Data { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public IList<DataColumn> Columns { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_additionalData.ContainsKey("column_names"))
            {
                Columns = _additionalData["column_names"].Select(c =>
                {
                    var dc = new DataColumn();
                    dc.Name = c.Value<string>();
                    dc.Type = ProviderType.TimeSeries;
                    return dc;
                }).ToList();
            }
        }
    }
}