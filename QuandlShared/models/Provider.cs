using System.Collections.Generic;

namespace Quandl.Shared.Models
{
    public class Provider : IDataDefinition
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Type Type { get; set; }
        public List<IDataStructure> Collection { get; set; }
    }
}
