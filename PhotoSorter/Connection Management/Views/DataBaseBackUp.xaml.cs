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
    /// Interaction logic for DataBaseBackUp.xaml
    /// </summary>
    public partial class DataBaseBackUp : Window
    {
        public DataBaseBackUp()
        {
            InitializeComponent();
            this.DataContext = new DataBaseBackUpViewModel();
        }
    }
}
