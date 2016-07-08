using System.Collections.Generic;

namespace Quandl.Shared.models
{
    public class DatasetCollection
    {
        public List<Dataset> Datasets { get; set; }
        public Meta Meta { get; set; }
    }
}