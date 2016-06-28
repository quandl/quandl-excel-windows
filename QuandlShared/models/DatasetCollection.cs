using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared.models
{
    public class DatasetCollection
    {
        public List<Dataset> Datasets { get; set; }
        public Meta Meta { get; set; }
    }
}
