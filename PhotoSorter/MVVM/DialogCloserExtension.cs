using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoForce.MVVM
{
    public static class DialogCloserExtension
    {
        // Register the attached property.
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached(
            "DialogResult",
            typeof(bool?),
            typeof(DialogCloserExtension),
            new PropertyMetadata(DialogResultChanged));

        // Handle the property has changed by actually closing the window itself which
        // was passed in as part of the argument list.
        private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window != null && window.IsVisible)
            {
                window.DialogResult = e.NewValue as bool?;
            }
        }

        // Property getter.
        public static object GetDialogResult(Window target)
        {
            bool? bReturn = null;

            try
            {
                if (target != null)
                {
                    bReturn = target.GetValue(DialogResultProperty) as bool?;
                }
            }
            catch
            {
                // Just eat the exception if there is one so that the application will not crash.
            }
            return (object)bReturn;
        }

        // Property setter.
        public static void SetDialogResult(Window target, bool? value)
        {
            if (target != null)
            {
                target.SetValue(DialogResultProperty, value);
            }
        }
    }

    //public class OrderHelperBehavior : Behavior<TableView>
    //{
    //    public object OrderID
    //    {
    //        get { return (object)GetValue(OrderIDProperty); }
    //        set { SetValue(OrderIDProperty, value); }
    //    }

    //    public static readonly DependencyProperty OrderIDProperty =
    //        DependencyProperty.Register("OrderID", typeof(object), typeof(OrderHelperBehavior), null);


    //    protected override void OnAttached()
    //    {
    //        AssociatedObject.FocusedRowHandleChanged += AssociatedObject_FocusedRowHandleChanged;
    //    }
    //    GridControl Grid
    //    {
    //        get
    //        {
    //            return AssociatedObject.DataControl as GridControl;
    //        }
    //    }
    //    void AssociatedObject_FocusedRowHandleChanged(object sender, FocusedRowHandleChangedEventArgs e)
    //    {
    //        OrderID = Grid.GetCellValue(AssociatedObject.FocusedRowHandle, Grid.Columns["OrderId"]);
    //    }
    //}
}
