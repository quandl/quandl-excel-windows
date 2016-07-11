using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared.Models
{
    public interface IDataDefinition
    {
        string Name { get; set; }
        string Code { get; }
    }
}
