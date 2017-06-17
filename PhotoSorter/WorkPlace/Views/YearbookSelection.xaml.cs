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
    /// Interaction logic for YearbookSelection.xaml
    /// </summary>
    public partial class YearbookSelection : Window
    {
        public YearbookSelection(int photoJobId, System.Collections.ArrayList tempArrStuImgId, string tempClassName)
        {
            InitializeComponent();
            this.DataContext = new YearbookSelectionViewModel(photoJobId, tempArrStuImgId, tempClassName);
        }
    }
}
