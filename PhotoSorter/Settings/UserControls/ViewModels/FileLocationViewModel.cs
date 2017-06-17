using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using Ookii.Dialogs.Wpf;

namespace PhotoForce.Settings.UserControls
{
    public class FileLocationViewModel : ViewModelBase
    {
        # region Initialization and Declaration
        PhotoSorterDBModelDataContext db ;
        # endregion

        #region Properties
        private string _imagePath;
        private string _templateA;
        private string _templateB;
        private string _templateBackA;

        public string templateBackA
        {
            get { return _templateBackA; }
            set { _templateBackA = value; NotifyPropertyChanged("templateBackA"); }
        }
        public string templateB
        {
            get { return _templateB; }
            set { _templateB = value; NotifyPropertyChanged("templateB"); }
        }
        public string templateA
        {
            get { return _templateA; }
            set { _templateA = value; NotifyPropertyChanged("templateA"); }
        }
        public string imagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; NotifyPropertyChanged("imagePath"); }
        }
        #endregion

        # region Constructor
        public FileLocationViewModel()
        {
            getSettingsData();
        }
        # endregion

        #region Commands
        public RelayCommand BrowseImagePathCommand
        {
            get
            {
                return new RelayCommand(browseImagePath);
            }
        }
        public RelayCommand TemplateABrowseCommand
        {
            get 
            {
                return new RelayCommand(templateABrowse);
            }
        }
        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(saveLocations);
            }
        }
        public RelayCommand TemplateBBrowseCommand
        {
            get
            {
                return new RelayCommand(templateBBrowse);
            }
        }
        public RelayCommand TemplateBackABrowseCommand
        {
            get 
            {
                return new RelayCommand(templateBackABrowse);
            }
        }
        #endregion

        #region Methods

        # region Get template path
        internal void getSettingsData()
        {
            db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            //get Image folder
            imagePath = clsDashBoard.getSettingByName(db, "ImageFolderLocation").settingValue.Trim();
            //Get Template A front
            templateA = clsDashBoard.getTemplateByCode(db, "Template A").templatePath.Trim();
            //Get Template A back
            templateBackA = clsDashBoard.getTemplateByCode(db, "Template A1").templatePath.Trim();       //changed on 22 oct by abhilasha
            //Get Template B
            templateB = clsDashBoard.getTemplateByCode(db, "Template B").templatePath.Trim();              //changed on 22 oct by abhilasha
            //Get Template C
        }
        # endregion

        # region Browse for default location
        private void browseImagePath()
        {
            try
            {
                VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                dlg.SelectedPath = imagePath;
                var res = dlg.ShowDialog();
                if (res != false)
                    imagePath = dlg.SelectedPath;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        # region Update Path
        private void saveLocations()
        {
            try
            {
                int result = clsDashBoard.updateSettings(imagePath.Trim(), templateA.Trim(), templateBackA.Trim(), templateB.Trim());           // changed on 22 oct by abhilasha
                if (result > 0)
                    MVVMMessageService.ShowMessage("Photo Sorter settings saved.");
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        # region Browse for template A front location
        private void templateABrowse()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Pdf file only (*.pdf)|*.PDF";
                bool? result = dlg.ShowDialog();
                if (result == true)
                {
                    templateA = dlg.FileName;
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        # region Browse for template B front location
        private void templateBBrowse()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Pdf file only (*.pdf)|*.PDF";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    templateB = dlg.FileName;
                }
            }
            catch (Exception ex)
            { 
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        # region Browse for template A back location
        private void templateBackABrowse()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Pdf file only (*.pdf)|*.PDF";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    templateBackA = dlg.FileName;
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        #endregion

        internal void setVisibilityForButtons()
        {
            (System.Windows.Application.Current as App).setAllButtonsVisibility();
        }

        #endregion
    }
}
