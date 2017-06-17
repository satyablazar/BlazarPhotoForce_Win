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

namespace PhotoForce.Settings
{
    /// <summary>
    /// Interaction logic for AddEditStudio.xaml
    /// </summary>
    public partial class AddEditStudio : Window
    {
        public AddEditStudio()
        {
            InitializeComponent();
            this.DataContext = new AddEditStudioViewModel();
        }
        public AddEditStudio(Studio tempSelectedStudio )
        {
            InitializeComponent();
            this.DataContext = new AddEditStudioViewModel(tempSelectedStudio);
        }
    }
}
