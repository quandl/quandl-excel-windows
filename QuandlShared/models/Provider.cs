using System.Collections.Generic;
using Newtonsoft.Json;
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

        public string DatabaseCode { get; set; }

        public string Description { get; set; }
        //public Type Type { get; set; }

        public bool Premium { get; set; }

        public List<IDataStructure> Collection { get; set; }

        public ViewData ToViewData(string type)
        {
            ViewData data = new ViewData(this.Id, this.Code, this.Premium, type);
            data.Name = this.Name;
            data.Description = this.Description;
            data.DataSource = this;
            if (this.Code == null)
            {
                data.Code = this.DatabaseCode;
            }
            return data;
        }
    }
}
