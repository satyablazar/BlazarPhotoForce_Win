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

namespace PhotoForce.Error_Management
{
    /// <summary>
    /// Interaction logic for ShowErrors.xaml
    /// </summary>
    public partial class ShowErrors : Window
    {
        public ShowErrors()
        {
            InitializeComponent();
            this.DataContext = new ShowErrorsViewModel();
        }
        public ShowErrors(string tempRenameImages)
        {
            InitializeComponent();
            this.DataContext = new ShowErrorsViewModel(tempRenameImages);
        }
    }
}
