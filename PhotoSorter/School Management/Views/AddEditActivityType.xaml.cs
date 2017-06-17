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

namespace PhotoForce.School_Management
{
    /// <summary>
    /// Interaction logic for AddEditActivityType.xaml
    /// </summary>
    public partial class AddEditActivityType : Window
    {
        public AddEditActivityType()
        {
            InitializeComponent();
            this.DataContext = new AddEditActivityTypeViewModel();
        }
    }
}
