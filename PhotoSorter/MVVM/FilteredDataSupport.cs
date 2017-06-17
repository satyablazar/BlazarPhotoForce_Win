using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Collections;
using DevExpress.Xpf.Grid;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Mvvm.UI.Interactivity;

namespace PhotoForce.MVVM
{
    public class FilteredDataSupport
    {
        public static readonly DependencyProperty VisibleDataProperty =
            DependencyProperty.RegisterAttached("VisibleData", typeof(IList), typeof(FilteredDataSupport), new PropertyMetadata(onVisibleDataChanged));

        public static void SetVisibleData(UIElement element, IList value)
        {
            element.SetValue(VisibleDataProperty, value);
        }
        public static IList GetVisibleData(UIElement element)
        {
            return (IList)element.GetValue(VisibleDataProperty);
        }

        private static void onVisibleDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GridControl grid = sender as GridControl;
            if (grid == null)
                return;
            if (e.OldValue == null && e.NewValue != null)
                grid.FilterChanged += onFilterChanged;
            else if (e.OldValue != null && e.NewValue == null)
                grid.FilterChanged -= onFilterChanged;
        }

        static void onFilterChanged(object sender, RoutedEventArgs e)
        {
            GridControl grid = sender as GridControl;
            IList visibleData = grid.GetValue(VisibleDataProperty) as IList;
            IEnumerable data = grid.ItemsSource as IEnumerable;
            if (data == null)
                return;
            Type t = null;
            foreach (object item in data)
            {
                t = item.GetType();
                break;
            }
            if (t == null)
                return;
            visibleData.Clear();
            if (!grid.IsFilterEnabled)
                foreach (object item in data)
                    visibleData.Add(item);
            else
            {
                ExpressionEvaluator evaluator = new ExpressionEvaluator(TypeDescriptor.GetProperties(t), grid.FilterCriteria, false);
                ICollection filteredCollection = evaluator.Filter(data);
                foreach (object item in filteredCollection)
                    visibleData.Add(item);
            }
        }
    }
}
