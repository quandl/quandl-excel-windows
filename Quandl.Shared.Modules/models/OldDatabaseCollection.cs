using System.Collections.Generic;

namespace Quandl.Shared.Models
{
    public class OldDatabaseCollection
    {
        public List<OldDatabase> Databases { get; set; }
        public OldMeta Meta { get; set; }
    }
}