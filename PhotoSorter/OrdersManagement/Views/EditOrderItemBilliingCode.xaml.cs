using System;
using System.Collections.Generic;
using System.IO;
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

namespace PhotoForce.OrdersManagement
{
    /// <summary>
    /// Interaction logic for EditOrderItemBilliingCode.xaml
    /// </summary>
    public partial class EditOrderItemBilliingCode : Window
    {
        #region Initialization
        string appDataPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\" + PhotoForce.App_Code.clsStatic.APP_DATA_FOLDER_NAME;
        #endregion

        public EditOrderItemBilliingCode(List<int> studentPhotoOrdersList)
        {
            InitializeComponent();
            this.DataContext = new EditOrderItemBilliingCodeViewModel(studentPhotoOrdersList);
        }

        private void EditOrderItem_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (File.Exists(appDataPath + "\\" + "EditOrderItemLayout.xml"))
                dgEditOrderItems.RestoreLayoutFromXml(appDataPath + "\\" + "EditOrderItemLayout.xml");
        }

        private void EditOrderItem_UnLoaded_1(object sender, RoutedEventArgs e)
        {
            dgEditOrderItems.SaveLayoutToXml(appDataPath + "\\" + "EditOrderItemLayout.xml");
        }
    }
}
