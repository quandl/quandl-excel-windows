using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quandl.Shared.Models.Browse;

namespace Quandl.Shared.Models
{
    public class DatabaseResponse
    {
        [JsonProperty("database")]
        public Provider Provider { set; get; }
    }

    public class DatatableCollectionResponse
    {
        [JsonProperty("datatable_collection")]
        public Provider Provider { set; get; }
    }

    public class DatabaseCollectionResponse
    {
        [JsonProperty("databases")]
        public List<Provider> Providers { set; get; }
    }

    public class DatatableCollectionsResponse
    {
        [JsonProperty("datatable_collections")]
        public List<Provider> Providers { set; get; }
    }

    public class Provider : IDataDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
  
        public bool Premium { get; set; }

        public List<IDataStructure> Collection { get; set; }

        [JsonExtensionData] private IDictionary<string, JToken> _additionalData;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_additionalData.Keys.Contains("database_code"))
            {
                string databaseCode = (string) _additionalData["database_code"];
                Code = databaseCode;
            }
        }

        public Provider()
        {
            _additionalData = new Dictionary<string, JToken>();
        }

        public ViewData ToViewData(string type)
        {
            ViewData data = new ViewData(this.Id, this.Code, this.Premium, type);
            data.Name = this.Name;
            data.Description = this.Description;
            data.DataSource = this;
            return data;
        }
    }
}
