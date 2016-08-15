using System.Collections.Generic;

namespace Quandl.Shared.Models.Browse
{
    public class BrowseCollection
    {
        public string Name { get; set; }
        public List<BrowseCollection> Items { get; set; }

        public List<OrderedResourceIds> OrderedResourceIds { get; set; }
    }

    public class OrderedResourceIds
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
}