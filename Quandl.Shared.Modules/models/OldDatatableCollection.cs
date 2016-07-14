using System.Collections.Generic;

namespace Quandl.Shared.Models
{
    public class OldDatatableCollection
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //public Datatable[] Datatables { get; set; }
        public bool Active { get; set; }
        public bool Hidden { get; set; }
        public bool Premium { get; set; }
        public bool Subscribed { get; set; }
        public string Image { get; set; }
        public List<string> RelatedDatatableCollectionIds { get; set; }
        public List<string> PlanIds { get; set; }
        public List<string> PlanCategoryIds { get; set; }
        public List<string> CurrentUserSubscriptionIds { get; set; }

    }
}
