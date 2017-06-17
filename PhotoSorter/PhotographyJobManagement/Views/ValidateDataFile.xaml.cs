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

namespace PhotoForce.PhotographyJobManagement
{
    /// <summary>
    /// Interaction logic for ValidateDataFile.xaml
    /// </summary>
    public partial class ValidateDataFile : Window
    {
        public ValidateDataFile()//DataTable tempImageNamesInDataFile, DataTable tempImageNamesInFolder, string tempSelectedPath)
        {
            InitializeComponent();
            this.DataContext = new ValidateDataFileViewModel();//(tempImageNamesInDataFile, tempImageNamesInFolder, tempSelectedPath);
        }
    }
}
