using System.Collections.Generic;
using Newtonsoft.Json;

namespace Quandl.Shared.Models
{
    public class DatabaseResponse
    {
        [JsonProperty("database")]
        public Provider Provider { set; get; }
    }

    public class DatabaseCollectionResponse
    {
        [JsonProperty("databases")]
        public List<Provider> Providers { set; get; }
    }

    public class Database
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DatabaseCode { get; set; }
        public string Description { get; set; }
        public long DatasetsCount { get; set; }
        public long Downloads { get; set; }
        public bool Premium { get; set; }
        public string Image { get; set; }
    }
}