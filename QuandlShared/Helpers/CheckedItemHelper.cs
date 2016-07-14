using System;
using System.Linq;
using System.Windows;
using Quandl.Shared.Models;

namespace Quandl.Excel.Addin.UI.Helpers
{
    // http://stackoverflow.com/questions/2251260/how-to-develop-treeview-with-checkboxes-in-wpf
    public class CheckedItemHelper : DependencyObject
    {
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.RegisterAttached("IsChecked",
            typeof(bool?), typeof(CheckedItemHelper), new PropertyMetadata(false, OnIsCheckedPropertyChanged));

        public static readonly DependencyProperty ParentProperty = DependencyProperty.RegisterAttached("Parent",
            typeof(object), typeof(CheckedItemHelper));

        public static event EventHandler CheckedChanged;

        private static void OnIsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataHolderDefinition && ((bool?) e.NewValue).HasValue)
                foreach (var p in (d as DataHolderDefinition).Columns)
                    SetIsChecked(p, (bool?) e.NewValue);

            if (d is DataColumn)
            {
                var columns = (d.GetValue(ParentProperty) as DataHolderDefinition).Columns;
                var checkedd = columns.Count(x => GetIsChecked(x) == true);
                var uncheckedd = columns.Count(x => GetIsChecked(x) == false);
                if (uncheckedd > 0 && checkedd > 0)
                {
                    SetIsChecked(d.GetValue(ParentProperty) as DependencyObject, null);
                    NotifyPropertyChanged(d);
                    return;
                }
                if (checkedd > 0)
                {
                    SetIsChecked(d.GetValue(ParentProperty) as DependencyObject, true);
                    NotifyPropertyChanged(d);
                    return;
                }
                SetIsChecked(d.GetValue(ParentProperty) as DependencyObject, false);
            }

            NotifyPropertyChanged(d);
        }

        public static void SetIsChecked(DependencyObject element, bool? IsChecked)
        {
            element.SetValue(IsCheckedProperty, IsChecked);
        }

        public static bool? GetIsChecked(DependencyObject element)
        {
            return (bool?) element.GetValue(IsCheckedProperty);
        }

        public static void SetParent(DependencyObject element, object Parent)
        {
            element.SetValue(ParentProperty, Parent);
        }

        public static object GetParent(DependencyObject element)
        {
            return element.GetValue(ParentProperty);
        }

        protected static void NotifyPropertyChanged(DependencyObject d)
        {
            CheckedChanged?.Invoke(d, new EventArgs());
        }
    }
}