using System.Collections.Generic;

namespace Quandl.Shared.Models
{
    public class Datatable : IDataDefinition, IDataStructure
    {
        public string Name { get; set; }
        public string Code
        {
            get
            {
                return $"{VendorCode}/{DatatableCode}";
            }
        }
        public List<DataColumn> Column { get; set; }
        public string VendorCode { get; set; }
        public string DatatableCode { get; set; }
        public object Description { get; set; }
        public List<string> Filters { get; set; }
        public bool Premium { get; set; }
    }
}
