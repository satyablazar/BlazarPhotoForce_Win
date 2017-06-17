using PhotoForce.App_Code;
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

namespace PhotoForce.Connection_Management
{
    /// <summary>
    /// Interaction logic for AddNewConnection.xaml
    /// </summary>
    public partial class AddNewConnection : Window
    {
        public AddNewConnection()
        {
            InitializeComponent();
            this.DataContext = new AddNewConnectionViewModel();
        }
        public AddNewConnection(Connections _Connection)
        {
            InitializeComponent();
            this.DataContext = new AddNewConnectionViewModel(_Connection);
        }
    }
}
