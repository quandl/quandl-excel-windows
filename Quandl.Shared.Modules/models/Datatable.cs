using System.Collections.Generic;

namespace Quandl.Shared.Models
{
    public class Datatable : DataHolderDefinition
    {
        public new string Code => $"{VendorCode}/{DatatableCode}";
        public string VendorCode { get; set; }
        public string DatatableCode { get; set; }
        public object Description { get; set; }
        public List<string> Filters { get; set; }
        public bool Premium { get; set; }
    }
}
