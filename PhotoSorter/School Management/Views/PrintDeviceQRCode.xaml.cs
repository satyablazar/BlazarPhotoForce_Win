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
using PhotoForce.App_Code;
using PhotoForce.MVVM;
using DevExpress.Xpf.Printing;
using System.ComponentModel;

namespace PhotoForce.School_Management
{
    /// <summary>
    /// Interaction logic for PrintDeviceQRcode.xaml
    /// </summary>
    public partial class PrintDeviceQRCode : Window
    {
        #region Initialization
        string SchoolName = "";
        string DeviceId = "";
        private MW6QRCode.Font QRCodeFontObj = new MW6QRCode.Font();
        #endregion

        #region Constructors
        public PrintDeviceQRCode(string sclName, string tempSyncText)
        {
            InitializeComponent();
            this.DataContext = this;

            SchoolName = sclName;
            DeviceId = tempSyncText;
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Create a link and bind it to the PrintPreview instance.
            CollectionViewLink link = new CollectionViewLink();
            preview.Model = new LinkPreviewModel(link);

            // Create an ICollectionView object.
            link.CollectionView = CreateMonthCollectionView();

            // Provide export templates.
            link.DetailTemplate = (DataTemplate)Resources["StudentName"];

            // Create a document.
            link.CreateDocument(true);
        }

        private ICollectionView CreateMonthCollectionView()
        {
            // Version :Indicates which version is used.
            int Version = 4;

            // Level
            //  0: L - recovery rate 7%
            //  1: M - recovery rate 15%
            //  2: Q - recovery rate 25%
            //  3: H - recovery rate 30%
            int Level = 3;

            // Mask:Indicates the mask pattern for improving the readability, this parameter can be one of the following values.
            //Value     Comment
            //0         Auto
            //1         Mask 0
            //2         Mask 1
            //3         Mask 2
            //4         Mask 3
            //5         Mask 4
            //6         Mask 5
            //7         Mask 6
            //8         Mask 7
            int Mask = 8;



            StudentsDetail[] data = new StudentsDetail[1];            
            for (int i = 0; i < data.Length; i++)
            {
                    // encode data using QRCode
                    QRCodeFontObj.Encode(DeviceId, Version, Level, Mask);

                    // How many rows?
                    int RowCount = QRCodeFontObj.GetRows();

                    // Produce string for DataMatrix font
                    string EncodedMsg = "" + System.Convert.ToChar(13) + System.Convert.ToChar(10);
                    for (int j = 0; j < RowCount; j++)
                    {
                        EncodedMsg = EncodedMsg + QRCodeFontObj.GetRowStringAt(j);
                        EncodedMsg = EncodedMsg + System.Convert.ToChar(13) + System.Convert.ToChar(10);
                    }
                    data[i] = new StudentsDetail("School Name: " + SchoolName, "Date: " + DateTime.Now.ToShortDateString(), "*" + EncodedMsg + "*", DeviceId);
            }
            CollectionViewSource source = new CollectionViewSource();
            source.Source = data;

            return source.View;
        }

        public class StudentsDetail
        {
            public string SchoolName { get; set; }
            public string Date { get; set; }
            public string IDQRcode { get; set; }
            public string ID { get; set; }

            public StudentsDetail(string schoolname, string date, string idQRcode, string id)
            {
                SchoolName = schoolname;
                Date = date;
                IDQRcode = idQRcode;
                ID = id;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && IsActive)
                this.Close();
        }


    }
}
