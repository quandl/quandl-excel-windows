using System.Collections.Generic;

namespace Quandl.Shared.Models
{
    public class OldDatasetCollection
    {
        public List<OldDataset> Datasets { get; set; }
        public OldMeta Meta { get; set; }
    }
}