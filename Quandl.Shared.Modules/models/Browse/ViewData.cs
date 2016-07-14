using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quandl.Shared.Models.Browse
{
    public class Category
    {
        public string Name { get; set; }
        public ObservableCollection<SubCategory> SubCategories { get; set; }

        public Category()
        {
            SubCategories = new ObservableCollection<SubCategory>();
        }
    }

    public class SubCategory
    {
        public string Name { get; set; }
        public ObservableCollection<LeafCategory> LeafCategories { get; set; }

        public SubCategory()
        {
            LeafCategories = new ObservableCollection<LeafCategory>();
        }
    }

    public class LeafCategory
    {
        public string Name { get; set; }

        public List<OrderedResourceIds> OrderList { get; set; }

        public List<ViewData> DataList;

        public LeafCategory(string name, List<OrderedResourceIds> orderList)
        {
            Name = name;
            OrderList = orderList;
            DataList = new List<ViewData>();
        }
    }


    public class ViewData
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool Premium { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string DataType { get; set; }

        public Object DataSource { get; set; }

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
    }
}