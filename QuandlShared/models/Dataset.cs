using System.Collections.Generic;

namespace Quandl.Shared.Models
{
    class Dataset : IDataDefinition, IDataStructure
    {
        public string Name { get; set; }
        public string Code
        {
            get
            {
                return $"{DatabaseCode}/{DatasetCode}";
            }
        }
        public List<DataColumn> Column { get; set; }
        public int Id { get; set; }
        public string DatasetCode { get; set; }
        public string DatabaseCode { get; set; }
        public string Description { get; set; }
        public bool Premium { get; set; }
    }
}
