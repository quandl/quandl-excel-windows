using System.Collections.Generic;

namespace Quandl.Shared.Models
{
    public class DataArray
    {
        public List<List<object>> DataPoints { get; set; }
        public List<DataColumn> Columns { get; set; }
    }
}
