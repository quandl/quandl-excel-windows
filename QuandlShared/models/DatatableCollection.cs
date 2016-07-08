using Quandl.Shared.models.ViewData;

namespace Quandl.Shared.models
{
    public class DatatableCollection
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
        public int[] RelatedDatatableCollectionIds { get; set; }
        public int[] PlanIds { get; set; }
        public int[] PlanCategoryIds { get; set; }
        public object[] CurrentUserSubscriptionIds { get; set; }

        public Data ToData(string type)
        {
            Data data = new Data(this.Id, this.Code, this.Premium, type);
            data.Name = this.Name;
            data.Description = this.Description;
            return data;
        }

    }
}
