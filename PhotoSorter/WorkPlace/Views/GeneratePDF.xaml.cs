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
    /// Interaction logic for GeneratePDF.xaml
    /// </summary>
    public partial class GeneratePDF : Window
    {
        public GeneratePDF(int schlId, int jobId, System.Collections.ArrayList arrStd, System.Collections.ArrayList grpId,bool isFiltred)
        {
            InitializeComponent();
            this.DataContext = new GeneratePDFViewModel(schlId, jobId, arrStd, grpId,isFiltred);
        }
    }
}
