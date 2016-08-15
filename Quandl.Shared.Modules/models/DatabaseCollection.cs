using System.Collections.Generic;

namespace Quandl.Shared.Models
{
    public class DatabaseCollection
    {
        public List<Database> Databases { get; set; }
        public OldMeta Meta { get; set; }
    }
}