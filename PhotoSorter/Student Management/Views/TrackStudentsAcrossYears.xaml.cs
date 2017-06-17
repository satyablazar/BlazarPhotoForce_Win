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

namespace PhotoForce.Student_Management
{
    /// <summary>
    /// Interaction logic for TrackStudentsAcrossYears.xaml
    /// </summary>
    public partial class TrackStudentsAcrossYears : Window
    {
        public TrackStudentsAcrossYears()
        {
            InitializeComponent();
            this.DataContext = new TrackStudentsAcrossYearsViewModel();
        }

        private void TrackStudents_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
