using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace PhotoForce.MVVM
{
    public class ActivateBehavior : Behavior<Control>
    {
        //Test
        //Boolean isActivated;

        //public static readonly DependencyProperty ActivatedProperty =
        //  DependencyProperty.Register(
        //    "Activated",
        //    typeof(Boolean),
        //    typeof(ActivateBehavior),
        //    new PropertyMetadata(OnActivatedChanged)
        //  );

        //public Boolean Activated
        //{
        //    get { return (Boolean)GetValue(ActivatedProperty); }
        //    set { SetValue(ActivatedProperty, value); }
        //}

        //static void OnActivatedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        //{
        //    var behavior = (ActivateBehavior)dependencyObject;
        //    if (!behavior.Activated || behavior.isActivated)
        //        return;
        //    // The Activated property is set to true but the Activated event (tracked by the
        //    // isActivated field) hasn't been fired. Go ahead and activate the window.
        //    if (behavior.AssociatedObject.WindowState == WindowState.Minimized)
        //        behavior.AssociatedObject.WindowState = WindowState.Normal;
        //    behavior.AssociatedObject.Activate();
        //}

        //protected override void OnAttached()
        //{
        //    AssociatedObject.Activated += OnActivated;
        //    AssociatedObject.Deactivated += OnDeactivated;
        //}

        //protected override void OnDetaching()
        //{
        //    AssociatedObject.Activated -= OnActivated;
        //    AssociatedObject.Deactivated -= OnDeactivated;
        //}

        //void OnActivated(Object sender, EventArgs eventArgs)
        //{
        //    this.isActivated = true;
        //    Activated = true;
        //}

        //void OnDeactivated(Object sender, EventArgs eventArgs)
        //{
        //    this.isActivated = false;
        //    Activated = false;
        //}

        //public class FocusBehavior : Behavior<Control>
        //{
        //    protected override void OnAttached()
        //    {
        //        this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        //        base.OnAttached();
        //    }

        //    private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        //    {
        //        this.AssociatedObject.Loaded -= AssociatedObject_Loaded;
        //        if (this.HasInitialFocus || this.IsFocused)
        //        {
        //            this.GotFocus();
        //        }
        //    }

        //    private void GotFocus()
        //    {
        //        this.AssociatedObject.Focus();
        //        if (this.IsSelectAll)
        //        {
        //            if (this.AssociatedObject is TextBox)
        //            {
        //                (this.AssociatedObject as TextBox).SelectAll();
        //            }
        //            else if (this.AssociatedObject is PasswordBox)
        //            {
        //                (this.AssociatedObject as PasswordBox).SelectAll();
        //            }
        //            else if (this.AssociatedObject is RichTextBox)
        //            {
        //                (this.AssociatedObject as RichTextBox).SelectAll();
        //            }
        //        }
        //    }

        //    public static readonly DependencyProperty IsFocusedProperty =
        //        DependencyProperty.Register(
        //            "IsFocused",
        //            typeof(bool),
        //            typeof(FocusBehavior),
        //            new PropertyMetadata(false,
        //                (d, e) =>
        //                {
        //                    if ((bool)e.NewValue)
        //                    {
        //                        ((FocusBehavior)d).GotFocus();
        //                    }
        //                }));

        //    public bool IsFocused
        //    {
        //        get { return (bool)GetValue(IsFocusedProperty); }
        //        set { SetValue(IsFocusedProperty, value); }
        //    }

        //    public static readonly DependencyProperty HasInitialFocusProperty =
        //        DependencyProperty.Register(
        //            "HasInitialFocus",
        //            typeof(bool),
        //            typeof(FocusBehavior),
        //            new PropertyMetadata(false, null));

        //    public bool HasInitialFocus
        //    {
        //        get { return (bool)GetValue(HasInitialFocusProperty); }
        //        set { SetValue(HasInitialFocusProperty, value); }
        //    }

        //    public static readonly DependencyProperty IsSelectAllProperty =
        //        DependencyProperty.Register(
        //            "IsSelectAll",
        //            typeof(bool),
        //            typeof(FocusBehavior),
        //            new PropertyMetadata(false, null));

        //    public bool IsSelectAll
        //    {
        //        get { return (bool)GetValue(IsSelectAllProperty); }
        //        set { SetValue(IsSelectAllProperty, value); }
        //    }

        //}

        #region setting cursor focus
        protected override void OnAttached()
        {
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            base.OnAttached();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.Loaded -= AssociatedObject_Loaded;
            if (this.HasInitialFocus || this.IsFocused)
            {
                this.GotFocus();
            }
        }

        private void GotFocus()
        {
            this.AssociatedObject.Focus();
            if (this.IsSelectAll)
            {
                if (this.AssociatedObject is TextBox)
                {
                    (this.AssociatedObject as TextBox).SelectAll();
                }
                else if (this.AssociatedObject is PasswordBox)
                {
                    (this.AssociatedObject as PasswordBox).SelectAll();
                }
                else if (this.AssociatedObject is RichTextBox)
                {
                    (this.AssociatedObject as RichTextBox).SelectAll();
                }
            }
        }

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
                "IsFocused",
                typeof(bool),
                typeof(ActivateBehavior),
                new PropertyMetadata(false,
                    (d, e) =>
                    {
                        if ((bool)e.NewValue)
                        {
                            ((ActivateBehavior)d).GotFocus();
                        }
                    }));

        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            set { SetValue(IsFocusedProperty, value); }
        }

        public static readonly DependencyProperty HasInitialFocusProperty =
            DependencyProperty.Register(
                "HasInitialFocus",
                typeof(bool),
                typeof(ActivateBehavior),
                new PropertyMetadata(false, null));

        public bool HasInitialFocus
        {
            get { return (bool)GetValue(HasInitialFocusProperty); }
            set { SetValue(HasInitialFocusProperty, value); }
        }

        public static readonly DependencyProperty IsSelectAllProperty =
            DependencyProperty.Register(
                "IsSelectAll",
                typeof(bool),
                typeof(ActivateBehavior),
                new PropertyMetadata(false, null));

        public bool IsSelectAll
        {
            get { return (bool)GetValue(IsSelectAllProperty); }
            set { SetValue(IsSelectAllProperty, value); }
        }
        #endregion

    }
}
