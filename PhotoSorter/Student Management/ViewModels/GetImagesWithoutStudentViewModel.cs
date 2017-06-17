using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using PhotoForce.MVVM;
using PhotoForce.App_Code;


namespace PhotoForce.Student_Management
{
    public class GetImagesWithoutStudentViewModel : ViewModelBase
    {
        #region Initialization
        ArrayList arrPhotoShootId = new ArrayList();
        #endregion

        #region Properties
        private IEnumerable<StudentImage> _dgImagesWithoutStudentData;

        public IEnumerable<StudentImage> dgImagesWithoutStudentData
        {
            get { return _dgImagesWithoutStudentData; }
            set { _dgImagesWithoutStudentData = value; NotifyPropertyChanged("dgImagesWithoutStudentData"); }
        }
        #endregion

        #region Constructor
        public GetImagesWithoutStudentViewModel(ArrayList tempArrShootId)
        {
            arrPhotoShootId = tempArrShootId;
            bindStudentGrid();
        }
        #endregion

        #region Methods
        public void bindStudentGrid()
        {
            try
            {
                PhotoSorterDBModelDataContext db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                dgImagesWithoutStudentData = clsDashBoard.getImgWithoutStudents(db, arrPhotoShootId);
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        #endregion
    }
}
