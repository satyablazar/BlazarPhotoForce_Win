using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.MVVM;
using PhotoForce.App_Code;
using System.IO;
using PhotoForce.Error_Management;

namespace PhotoForce.WorkPlace
{
    public class ProgressBarRenameViewModel : ViewModelBase
    {
        #region Initialization
        PhotoSorterDBModelDataContext dbrename = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        static int PhotoShootID = 0;
        static string imagename = "";
        string Photoshootpath = "";
        string SelectedGridName = "";
        ArrayList arrStudentImageIDs = new ArrayList();
        clsStatic objstatic = new clsStatic();
        clsErrorLog objError = new clsErrorLog();
        internal Boolean isError = false;
        string lastname, firstname, sequentialnumber = "";
        StudentImage _objStudentImage = new StudentImage();
        IList<StudentImage> _StudentImageList;
        #endregion

        #region Constructors
        public ProgressBarRenameViewModel(int tempShootId, string tempShootPath, string tempSelectedGrid, ArrayList arrSelectedStudent)
        { 
            PhotoShootID = tempShootId;
            Photoshootpath = tempShootPath;
            SelectedGridName = tempSelectedGrid;
            arrStudentImageIDs = arrSelectedStudent;
            dbrename = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
            if (SelectedGridName == "PhotoShoot" && arrStudentImageIDs.Count == 0)
                _StudentImageList = clsDashBoard.GetStudentImgfromPhotoShootFMulti(dbrename, PhotoShootID);
            else
                _StudentImageList = clsDashBoard.GetStudentImgfromImages(dbrename, arrSelectedStudent);
            RenameSourceImages();
        }
        #endregion

        #region Methods
        public void RenameSourceImages()
        {
            try
            {
                PhotoSorterDBModelDataContext db1 = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);

                double value = 0;
                bool _error = false;
                bool tempError;

                foreach (StudentImage tempStuImage in _StudentImageList)
                {
                    tempError = false;
                    Renameerrorlog _objrnmimgerror = new Renameerrorlog();
                    try
                    {
                        value += 1;
                        imagename = tempStuImage.ImageName;
                        sequentialnumber = imagename.Substring(imagename.LastIndexOf('_') + 1);
                        lastname = tempStuImage.Lastname;
                        firstname = tempStuImage.FirstName;
                        string oldpath = Photoshootpath + "\\" + imagename;
                        string newpath = Photoshootpath + "\\" + lastname + "_" + firstname + "_" + sequentialnumber;
                        string oldreducedpath = Photoshootpath + "\\_reduced\\" + imagename;
                        string newreducedpath = Photoshootpath + "\\_reduced\\" + lastname + "_" + firstname + "_" + sequentialnumber;
                        string oldddpath = Photoshootpath + "\\_dd\\" + imagename;
                        string newddpath = Photoshootpath + "\\_dd\\" + lastname + "_" + firstname + "_" + sequentialnumber;
                        if (File.Exists(oldpath))
                        {
                            if (oldpath != newpath)
                            {
                                if (File.Exists(oldreducedpath))
                                {
                                    File.Move(oldpath, newpath);
                                    if (oldreducedpath != newreducedpath)
                                    {
                                        if (tempError == false) { File.Move(oldreducedpath, newreducedpath); }
                                        if (File.Exists(oldddpath))
                                        {
                                            if (oldddpath != newddpath)
                                            {
                                                if (tempError == false) { File.Move(oldddpath, newddpath); }
                                            }
                                        }
                                        //There is a chance of getting error here.
                                        //have to rename either both files or nothing
                                        //so it is better to check tempError==false two times
                                        //for original image and reduced image

                                        if (tempError == false)
                                        {
                                            int retvalupdate = clsDashBoard.updateStudentImgName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempStuImage.ID, lastname + "_" + firstname + "_" + sequentialnumber);
                                            int status = 0;
                                            if (retvalupdate == 1)
                                            {
                                                clsDashBoard.updateOriginalStudentImgName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempStuImage.ID, status);
                                            }
                                            else
                                            {
                                                status = 1;
                                                clsDashBoard.updateOriginalStudentImgName(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), tempStuImage.ID, status);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    isError = true;
                                    _error = true;
                                    tempError = true;

                                    _objrnmimgerror.message = imagename + "  image does not exist in " + Photoshootpath + "\\_reduced" + " path.";
                                    _objrnmimgerror.imagename = imagename;
                                    _objrnmimgerror.imagepath = Photoshootpath;
                                    _objrnmimgerror.currentdatetime = DateTime.Now;
                                    db1.Renameerrorlogs.InsertOnSubmit(_objrnmimgerror);

                                    clsErrorLog objErrorLog = new clsErrorLog();
                                    objErrorLog.Message = _objrnmimgerror.message;
                                    objErrorLog.ImagePath = Photoshootpath;
                                    objErrorLog.ImageName = imagename;
                                    objErrorLog.MethodName = "ProgressBarRename.RenameSourceImages()";
                                    clsStatic.WriteErrorLogRenameImages(objErrorLog, "ProgressBarRename");
                                }
                            }
                        }
                        else
                        {
                            isError = true; tempError = true;

                            _objrnmimgerror.message = imagename + "  image does not exist in " + Photoshootpath + " path.";
                            _objrnmimgerror.imagename = imagename;
                            _objrnmimgerror.imagepath = Photoshootpath;
                            _objrnmimgerror.currentdatetime = DateTime.Now;
                            db1.Renameerrorlogs.InsertOnSubmit(_objrnmimgerror);

                            clsErrorLog objErrorLog = new clsErrorLog();
                            objErrorLog.Message = _objrnmimgerror.message;
                            objErrorLog.ImagePath = Photoshootpath;
                            objErrorLog.ImageName = imagename;
                            objErrorLog.MethodName = "ProgressBarRename.RenameSourceImages()";
                            clsStatic.WriteErrorLogRenameImages(objErrorLog, "ProgressBarRename");

                        }
                    }
                    catch (Exception ex)
                    {
                        isError = true; tempError = true;
                        _objrnmimgerror.message = ex.Message;
                        _objrnmimgerror.imagename = imagename;
                        _objrnmimgerror.imagepath = Photoshootpath;
                        _objrnmimgerror.currentdatetime = DateTime.Now;
                        db1.Renameerrorlogs.InsertOnSubmit(_objrnmimgerror);

                        clsErrorLog objErrorLog = new clsErrorLog();
                        objErrorLog.Message = _objrnmimgerror.message;
                        objErrorLog.ImagePath = Photoshootpath;
                        objErrorLog.ImageName = imagename;
                        objErrorLog.MethodName = "ProgressBarRename.RenameSourceImages()";
                        clsStatic.WriteErrorLogRenameImages(objErrorLog, "ProgressBarRename");
                    }
                    if (tempError == false)
                    {
                        db1.SubmitChanges();
                    }
                }
                //db1.SubmitChanges();
                if (clsErrorLog.displaymsg == true)
                {
                    if (isError == false)
                    {
                        if (arrStudentImageIDs.Count == 0)
                            MVVMMessageService.ShowMessage(errorMessages.AFTER_RENAME_SOURCE_IMAGES);
                        else
                            MVVMMessageService.ShowMessage(errorMessages.AFTER_RENAME_SOURCE_IMAGES_SELECTED);
                        DialogResult = false;
                    }
                    else
                    {
                        //Renaming of images Completed with errors. Do you want to see the error details
                        string messageshow = "";
                        string captionshow = "";

                        if (_error == true)
                        {
                            messageshow = errorMessages.RENAME_WITHOUT_REDUCED_IMAGES_ERROR;
                            MVVMMessageService.ShowMessage(messageshow);
                        }
                        else
                        {
                            messageshow = errorMessages.AFTER_RENAME_SOURCE_IMAGES_ERROR;
                            captionshow = "Confirmation";
                            System.Windows.MessageBoxButton buttonsshow = System.Windows.MessageBoxButton.YesNo;
                            System.Windows.MessageBoxImage iconshow = System.Windows.MessageBoxImage.Question;


                            if (MVVMMessageService.ShowMessage(messageshow, captionshow, buttonsshow, iconshow) == System.Windows.MessageBoxResult.Yes)
                            {
                                ShowErrors objShowErrors = new ShowErrors("RenameImages");
                                objShowErrors.ShowDialog();
                            }
                        }
                    }
                }
                DialogResult = false;
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
                DialogResult = false;
            }
        }
        #endregion
    }
}
