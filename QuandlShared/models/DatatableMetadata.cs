using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared.models
{
    public class DatatableMetadata
    {
        public Datatable datatable { get; set; }
    }

    public class Datatable
    {
        public string VendorCode { get; set; }
        public string DatatableCode { get; set; }
        public string Name { get; set; }
        public object Description { get; set; }
        public List<Column> Columns { get; set; }
        public List<string> Filters { get; set; }
        public List<string> PrimaryKey { get; set; }
        public bool Premium { get; set; }
    }

    public class Column
    {
        public string name { get; set; }
        public string type { get; set; }
    }

}
