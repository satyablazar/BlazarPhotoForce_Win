using PhotoForce.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace PhotoForce.View_Management.UserControls
{
    /// <summary>
    /// Interaction logic for AddEdit_Users.xaml
    /// </summary>
    public partial class AddEditUsers : Window
    {
        public AddEditUsers()
        {
            InitializeComponent();
            this.DataContext = new AddEditUsersViewModel();
        }
        public AddEditUsers(User selectedUser)
        {
            InitializeComponent();
            this.DataContext = new AddEditUsersViewModel(selectedUser);
        }
    }
}
