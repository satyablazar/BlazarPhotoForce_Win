using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhotoForce.WorkPlace
{
    /// <summary>
    /// Interaction logic for SyncDeleteMsg.xaml
    /// </summary>
    public partial class SyncDeleteMessage : Window
    {
        public SyncDeleteMessage()
        {
            InitializeComponent();
            this.DataContext = new SyncDeleteMessageViewModel();
        }
    }
}
