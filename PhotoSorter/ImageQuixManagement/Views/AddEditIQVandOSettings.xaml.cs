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

namespace PhotoForce.ImageQuixManagement
{
    /// <summary>
    /// Interaction logic for AddEditIQVandOSettings.xaml
    /// </summary>
    public partial class AddEditIQVandOSettings : Window
    {
        public AddEditIQVandOSettings(string callFrom, IQVandoSetting selectedVandoSetting, IQAccount selectedIQAccount)
        {
            InitializeComponent();
            this.DataContext = new AddEditIQVandOSettingsViewModel(callFrom, selectedVandoSetting, selectedIQAccount);
        }
    }
}
