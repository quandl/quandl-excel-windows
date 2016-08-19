using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared.Models
{
    public class DatasetMetaResponse
    {
        [JsonProperty("dataset")]
        public DatasetMeta Metadata { get; set; }
    }

    public class DatasetMeta
    {
        [JsonProperty("dataset_code")]
        public string DatasetCode { get; set; }
        [JsonProperty("database_code")]
        public string DatabaseCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonProperty("refreshed_at")]
        public string RefreshedAt { get; set; }
        [JsonProperty("newest_available_date")]
        public string NewestAvailableDate { get; set; }
        [JsonProperty("oldest_available_date")]
        public string OldestAvailableDate { get; set; }
        [JsonProperty("column_names")]
        public List<string> Columns { get; set; }
        public string Frequency { get; set; }
        public string Type { get; set; }
        public bool Premium { get; set; }
    }
}