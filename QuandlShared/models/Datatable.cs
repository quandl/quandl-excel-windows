using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared.Models
{
    class Datatable : IDataDefinition, IDataStructure
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
