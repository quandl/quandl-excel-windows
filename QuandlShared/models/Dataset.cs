using Newtonsoft.Json;
using System.Collections.Generic;

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

    public class Dataset : IDataDefinition //, IDataStructure
    {
        public string Name { get; set; }
        public string Code
        {
            get
            {
                return $"{DatabaseCode}/{DatasetCode}";
            }
        }
        [JsonProperty("column_names")]
        public List<string> Column { get; set; }
        public int Id { get; set; }
        public string DatasetCode { get; set; }
        public string DatabaseCode { get; set; }
        public string Description { get; set; }
        public bool Premium { get; set; }
    }
}
