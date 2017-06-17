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

namespace PhotoForce.GroupManagement
{
    /// <summary>
    /// Interaction logic for AddGroup.xaml
    /// </summary>
    public partial class AddGroup : Window
    {
        public AddGroup(int sclId, string sclName, int grpId)
        {
            InitializeComponent();
            this.DataContext = new AddGroupViewModel(sclId, sclName, grpId);
        }
        public AddGroup(int sclId, string sclName)
        {
            InitializeComponent();
            this.DataContext = new AddGroupViewModel(sclId, sclName);
        }
    }
}
