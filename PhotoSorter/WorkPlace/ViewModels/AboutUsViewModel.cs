using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;

namespace PhotoForce.WorkPlace
{
    public class AboutUsViewModel : ViewModelBase
    {
        #region Properties
        private string _userName;
        private string _version;
        private string _server;
        private string _dataBaseName;
        private int? _creditsLeft;

        public int? creditsLeft
        {
            get { return _creditsLeft; }
            set { _creditsLeft = value; NotifyPropertyChanged("creditsLeft"); }
        }
        public string dataBaseName
        {
            get { return _dataBaseName; }
            set { _dataBaseName = value; NotifyPropertyChanged("dataBaseName"); }
        }
        public string server
        {
            get { return _server; }
            set { _server = value; NotifyPropertyChanged("server"); }
        }
        public string version
        {
            get { return _version; }
            set { _version = value; NotifyPropertyChanged("version"); }
        }
        public string userName
        {
            get { return _userName; }
            set { _userName = value; NotifyPropertyChanged("userName"); }
        }
        #endregion

        #region Constructors
        public AboutUsViewModel()
        {
            //get the versiom number
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            version = assembly.GetName().Version.ToString();
            string connectionstring = clsConnectionString.connectionString;
            string[] splitserver = connectionstring.Split(';');
            string[] servername = splitserver[0].Split('=');
            string[] splitdatabase = connectionstring.Split(';');
            string[] databasename = splitdatabase[1].Split('=');
            server = servername[1].ToString();
            dataBaseName = databasename[1].ToString();
        }
        public AboutUsViewModel(string name, int? credits)
            : this()
        {
            userName = name;
            creditsLeft = credits;
        }
        #endregion

        #region Commands
        public RelayCommand WindowCloseCommand
        {
            get { return new RelayCommand(WindowClose); }
        }
        #endregion

        #region Methods
        private void WindowClose()
        {
            DialogResult = false;
        }
        #endregion
    }
}
