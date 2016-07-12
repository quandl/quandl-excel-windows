using System.Collections.Generic;

namespace Quandl.Shared.Models
{
    public interface IDataStructure
    {
        List<DataColumn> Column { get; set; }
    }
}
