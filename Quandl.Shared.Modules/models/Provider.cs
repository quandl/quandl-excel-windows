using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quandl.Shared.Models.Browse;

namespace Quandl.Shared.Models
{
    public class Provider : IDataDefinition
    {
        [JsonExtensionData] public readonly IDictionary<string, JToken> _additionalData;

        public Provider()
        {
            _additionalData = new Dictionary<string, JToken>();
        }

        public int Id { get; set; }

        public string Description { get; set; }

        public bool Premium { get; set; }

        public List<IDataStructure> Collection { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_additionalData.Keys.Contains("database_code"))
            {
                var databaseCode = (string) _additionalData["database_code"];
                Code = databaseCode;
            }
        }

        // Following is helper methods
        public ArrayList ToDatatablesViewData()
        {
            ArrayList list = new ArrayList();
            foreach (var dt in _additionalData["datatables"])
            {
                var code = ((Newtonsoft.Json.Linq.JContainer) dt.First).First.ToString();
                var data = new ViewData(Id, code, Premium, null);
                data.Name = ((Newtonsoft.Json.Linq.JContainer)dt.Last).First.ToString();
                list.Add(data);
            }
            return list;
        }

        public ViewData ToViewData(string type)
        {
            var data = new ViewData(Id, Code, Premium, type);
            data.Name = Name;
            data.Description = Description;
            data.DataSource = this;
            return data;
        }
    }
}