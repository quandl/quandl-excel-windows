using System.Collections.Generic;

namespace Quandl.Shared.Models
{
    public interface IDataStructure
    {
        IList<DataColumn> Columns { get; set; }
    }
}
