using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared.Models
{
    public class DataArray
    {
        public List<List<object>> DataPoints { get; set; }
        public List<DataColumn> Columns { get; set; }
    }
}
