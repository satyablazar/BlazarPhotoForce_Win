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

namespace PhotoForce.PhotoShoot_Management
{
    /// <summary>
    /// Interaction logic for AddEditPhotoShoot.xaml
    /// </summary>
    public partial class AddEditPhotoShoot : Window
    {
        public AddEditPhotoShoot(int tempPhotoShootId, PhotographyJob tempPhotography)
        {
            InitializeComponent();
            this.DataContext = new AddEditPhotoShootViewModel(tempPhotoShootId, tempPhotography);
        }
        public AddEditPhotoShoot()
        {
            InitializeComponent();
            this.DataContext = new AddEditPhotoShootViewModel();
        }
    }
}
