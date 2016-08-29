using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quandl.Shared.Models.Browse
{
    public class Category
    {
        public Category()
        {
            SubCategories = new ObservableCollection<SubCategory>();
        }

        public string Name { get; set; }
        public ObservableCollection<SubCategory> SubCategories { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SubCategory
    {
        public SubCategory()
        {
            LeafCategories = new ObservableCollection<LeafCategory>();
        }

        public string Name { get; set; }
        public ObservableCollection<LeafCategory> LeafCategories { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class LeafCategory
    {
        public List<ViewData> DataList;

        public LeafCategory(string name, List<OrderedResourceIds> orderList)
        {
            Name = name;
            OrderList = orderList;
            DataList = new List<ViewData>();
        }

        public string Name { get; set; }

        public List<OrderedResourceIds> OrderList { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }


    public class ViewData
    {
        public ViewData(int id, string code, bool premium, string type)
        {
            Id = id;
            Code = code;
            Premium = premium;
            Type = type;
            if (premium)
            {
                DataType = "Premium";
            }
            else
            {
                DataType = "Free";
            }
        }

        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool Premium { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string DataType { get; set; }

        public object DataSource { get; set; }

        public override string ToString()
        {
            return $"{DataType} {Code} {Name}";
        }
    }
}