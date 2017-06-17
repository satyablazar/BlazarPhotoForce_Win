using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoForce.MVVM
{
    /// <summary>
    /// A command whose sole purpose is to 
    /// relay its functionality to other
    /// objects by invoking delegates. The
    /// default return value for the CanExecute
    /// method is 'true'.
    /// </summary>
    /*
     * Commands are an implementation of the ICommand interface that is part of the .NET Framework.
      1.The method Execute(object) is called when the command is actuated. It has one parameter, which can be used to pass additional information from the caller to the command.
      2.The method CanExecute(object) returns a Boolean. If the return value is true, it means that the command can be executed. The parameter is the same one as for the Execute method. When used in XAML controls that support the Command property, the control will be automatically disabled if CanExecute returns false.
      3.The CanExecuteChanged event handler must be raised by the command implementation when the CanExecute method needs to be reevaluated. In XAML, when an instance of ICommand is bound to a control’s Command property through a data-binding, raising the CanExecuteChanged event will automatically call the CanExecute method, and the control will be enabled or disabled accordingly.
     */
    public class RelayCommand<T> : ICommand
    {
        Action<T> execute;
        Predicate<T> canExecute;
        public RelayCommand(Action<T> Execute, Predicate<T> CanExecute = null)
        {
            execute = Execute;
            canExecute = CanExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
            {
                return canExecute((T)parameter);
            }
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (execute != null)
            {
                execute((T)parameter);
            }
        }
    }
    public class RelayCommand : ICommand
    {

        Action execute;
        Predicate<object> canExecute;
        public RelayCommand(Action Execute, Predicate<object> CanExecute = null)
        {
            execute = Execute;
            canExecute = CanExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
            {
                return canExecute(parameter);
            }
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (execute != null)
            {
                execute();
            }
        }
    }
}
