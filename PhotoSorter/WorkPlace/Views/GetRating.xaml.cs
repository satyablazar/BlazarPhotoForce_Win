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
    /// Interaction logic for GetRating.xaml
    /// </summary>
    public partial class GetRating : Window
    {
        public GetRating(int tempPhotoShootId, string tempSelctedGrid, int gridRowCount,List<PhotoForce.App_Code.StudentImage> visibleData)
        {
            InitializeComponent();
            this.DataContext = new GetRatingViewModel(tempPhotoShootId, tempSelctedGrid, gridRowCount, visibleData);
        }

        public GetRating(bool fromDashboard,string windowName)
        {
            InitializeComponent();
            this.DataContext = new GetRatingViewModel(fromDashboard, windowName);
        }
    }
}
