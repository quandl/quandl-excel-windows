using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Quandl.Shared.Models
{
    public class DatasetsResponse
    {
        [JsonProperty("datasets")]
        public List<Dataset> Datasets { get; set; }

        [JsonProperty("meta")]
        public DatasetMetadata Meta { get; set; }
    }

    public class DatasetResponse
    {
        public Dataset Dataset { get; set; }
    }

    public class DatasetMetadata
    {
        public int? PerPage { get; set; }
        public string Query { get; set; }
        public int? CurrentPage { get; set; }
        public int? PrevPage { get; set; }
        public int? TotalPages { get; set; }
        public int? TotalCount { get; set; }
        public int? NextPage { get; set; }
        public int CurrentFirstItem { get; set; }
        public int CurrentLastItem { get; set; }
    }

    public class Dataset : DataHolderDefinition
    {
        [JsonProperty("newest_available_date")] public DateTime NewestAvailableDate;
        [JsonProperty("oldest_available_date")] public DateTime OldestAvailableDate;
        public new string Code => $"{DatabaseCode}/{DatasetCode}";
        public string DatasetCode { get; set; }
        public string DatabaseCode { get; set; }
        public string Description { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}