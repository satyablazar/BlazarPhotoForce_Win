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

namespace PhotoForce.PhotographyJobManagement
{
    /// <summary>
    /// Interaction logic for AddNewPhotographyJob.xaml
    /// </summary>
    public partial class AddNewPhotographyJob : Window
    {
        public AddNewPhotographyJob()
        {
            InitializeComponent();
            this.DataContext = new AddNewPhotographyJobViewModel();
        }
        public AddNewPhotographyJob(string jobName, int startYear, int endYear, int photoJobId)
        {
            InitializeComponent();
            this.DataContext = new AddNewPhotographyJobViewModel(jobName, startYear, endYear, photoJobId);
        }
    }
}
