using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Quandl.Shared.Models
{
    public class DatatableCollectionsResponse
    {
        [JsonProperty("datatable_collections")]
        public List<Provider> Providers { set; get; }
    }

    public class DatatableCollectionResponse
    {
        [JsonProperty("datatable_collection")]
        public Provider Provider { set; get; }
    }

    public class DatatableMetadata
    {
        public Datatable datatable { get; set; }
    }


    public class Datatable : DataHolderDefinition
    {
        public int Id;
        public new string Code
        {
            get { return $"{VendorCode}/{DatatableCode}"; }
        }
        public string VendorCode { get; set; }
        public string DatatableCode { get; set; }
        public object Description { get; set; }
        public List<string> Filters { get; set; }
        public bool Premium { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}