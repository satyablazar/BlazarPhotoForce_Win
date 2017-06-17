using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PhotoForce.MVVM
{
    public class MessageService : IMessageBoxService
    {
        public void ShowMessage(string text)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
            {
                System.Windows.MessageBox.Show(text);
            }));
        }
        public System.Windows.MessageBoxResult ShowMessage(string message, string caption, System.Windows.MessageBoxButton buttons, System.Windows.MessageBoxImage icon)
        {
            return System.Windows.MessageBox.Show(message, caption, buttons, icon);
        }
    }

    public interface IMessageBoxService
    {
        void ShowMessage(string text);
        System.Windows.MessageBoxResult ShowMessage(string message, string caption, System.Windows.MessageBoxButton buttons, System.Windows.MessageBoxImage icon);
    }
}
