using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoForce.Extensions;
using System.Data;
using System.Data.SqlClient;
using PhotoForce.MVVM;
using PhotoForce.App_Code;

namespace PhotoForce.Mask_Management
{
    public class AddEditMasksViewModel : ViewModelBase
    {
        # region Initialization
        DataTable dt = new DataTable();
        int count;
        int maskDetailsGridCount = 0;
        int maskId = 0;
        internal bool isSave = false;
        IEnumerable<MaskDetail> _allMaskDetails;
        PhotoSorterDBModelDataContext db;// = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
        bool flagValue = true;
        string columnOrText = "";
        internal MaskDetailsItem _objMaskDetailsItem;
        # endregion

        #region Properties
        object _dgMaskDetailsData;
        string _lblRequired;
        string _lblRequiredSchool;
        string _lblPath;
        string _maskName;
        string _lstFieldsSelectedItem;
        string _newField;

        public string newField
        {
            get { return _newField; }
            set { _newField = value; NotifyPropertyChanged("newField"); }
        }
        public string lstFieldsSelectedItem
        {
            get { return _lstFieldsSelectedItem; }
            set { _lstFieldsSelectedItem = value; NotifyPropertyChanged("lstFieldsSelectedItem"); }
        }
        public string lblPath
        {
            get { return _lblPath; }
            set { _lblPath = value; NotifyPropertyChanged("lblPath"); }
        }
        public string maskName
        {
            get { return _maskName; }
            set { _maskName = value; NotifyPropertyChanged("maskName"); }
        }
        public string lblRequiredSchool
        {
            get { return _lblRequiredSchool; }
            set { _lblRequiredSchool = value; NotifyPropertyChanged("lblRequiredSchool"); }
        }
        public string lblRequired
        {
            get { return _lblRequired; }
            set { _lblRequired = value; NotifyPropertyChanged("lblRequired"); }
        }
        public object dgMaskDetailsData
        {
            get { return _dgMaskDetailsData; }
            set { _dgMaskDetailsData = value; NotifyPropertyChanged("dgMaskDetailsData"); }
        }
        #endregion

        # region Constructor
        public AddEditMasksViewModel(int tempMaskId, string tempMaskName)
        {
            try
            {
                maskId = tempMaskId;
                maskName = tempMaskName;
                dt.Columns.Add("MaskDetID");
                dt.Columns.Add("MaskID");
                dt.Columns.Add("MaskDetail1");
                dt.Columns.Add("Type");
                dt.Columns.Add("SortOrder");
                if ((maskId != 0))
                {
                    setValueForUpdate();
                }
            }
            catch (Exception ex)
            {
                MVVMMessageService.ShowMessage(ex.Message);
                clsStatic.WriteExceptionLogXML(ex);
            }
        }
        # endregion

        #region Commands
        public RelayCommand SendNewFieldCommand
        {
            get
            {
                return new RelayCommand(sendNewField);
            }
        }
        public RelayCommand SendMaskCommand
        {
            get
            {
                return new RelayCommand(sendMask);
            }
        }
        public RelayCommand<object> DeleteCommand
        {
            get
            {
                return new RelayCommand<object>(delete);
            }
        }
        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(save);
            }
        }
        public RelayCommand WindowCloseCommand
        {
            get
            {
                return new RelayCommand(windowClose);
            }
        }
        #endregion

        #region Methods

        # region Bind Grid
        /// <summary>
        /// This method is used to bind mask detail grid
        /// </summary>
        private void setValueForUpdate()
        {
            try
            {
                lblRequired = "";
                lblRequiredSchool = "";
                dgMaskDetailsData = clsDashBoard.getAllMaskDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), maskId);
                if (dgMaskDetailsData != null) { maskDetailsGridCount = (dgMaskDetailsData as IEnumerable<MaskDetail>).Count(); }
                bindMaskDetailsGrid();
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        # region Bind mask detail grid
        public void bindMaskDetailsGrid()
        {
            try
            {
                flagValue = false;
                lblPath = "";
                dgMaskDetailsData = clsDashBoard.getAllMaskDetails(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), maskId);
                _allMaskDetails = (IEnumerable<MaskDetail>)dgMaskDetailsData;
                if (dgMaskDetailsData != null)
                {
                    maskDetailsGridCount = _allMaskDetails.Count();
                    int i = 0;
                    for (i = 0; i < maskDetailsGridCount; i++)
                    {
                        string tempStringField = _allMaskDetails.ToList()[i].MaskDetail1;
                        if (tempStringField == "_")
                        {
                            tempStringField = tempStringField.Replace("_", "__");
                        }
                        lblPath += tempStringField;
                    }
                    dt = clsDashBoard.getAllMaskDetailsDatatable(new PhotoSorterDBModelDataContext(clsConnectionString.connectionString), maskId);
                    if (i == maskDetailsGridCount)
                    {
                        if (maskDetailsGridCount != 0)
                            count = Convert.ToInt32(dt.Rows[maskDetailsGridCount - 1]["sortorder"]);
                    }
                }

            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        # endregion

        private void delete(object SeletedRow)
        {
            try
            {
                string message = "Are you sure you want to delete?";
                string caption = "Confirmation";
                System.Windows.MessageBoxButton buttons = System.Windows.MessageBoxButton.YesNo;
                System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
                if (MVVMMessageService.ShowMessage(message, caption, buttons, icon) == System.Windows.MessageBoxResult.Yes)
                {
                    maskDetailsGridCount--;
                    if (flagValue == false)
                    {
                        if (SeletedRow != null)
                        {
                            int tempMaskDetailId = Convert.ToInt32(((PhotoForce.App_Code.MaskDetail)(SeletedRow)).MaskDetID);//(((System.Data.DataRowView)(SeletedRow)).Row[0]);   //mask deta id
                            SqlParameter[] param = new SqlParameter[1];
                            param[0] = new SqlParameter("@MaskDetid", tempMaskDetailId);
                            int ret = WCFSQLHelper.executeNonQuery_SP("sp_DeleteMaskDetailID", param);
                            bindMaskDetailsGrid();
                        }
                    }
                    else
                    {
                        var delindex = ((System.Data.DataRowView)(SeletedRow)).Row;
                        if (Convert.ToInt32(delindex[0]) != 0)
                        {
                            int tempMaskDetailId = Convert.ToInt32(delindex[0]);
                            SqlParameter[] param = new SqlParameter[1];
                            param[0] = new SqlParameter("@MaskDetid", tempMaskDetailId);
                            int ret = WCFSQLHelper.executeNonQuery_SP("sp_DeleteMaskDetailID", param);
                            bindMaskDetailsGrid();
                            if (columnOrText == "Text")
                                sendNewField();
                            else
                                sendMask();
                        }
                        else
                        {
                            var index = ((System.Data.DataRowView)(SeletedRow)).Row.ItemArray[4];
                            DataRow[] rows;
                            rows = dt.Select("SortOrder = '" + index + "'");
                            foreach (DataRow r in rows)
                                r.Delete();
                            dt.AcceptChanges();
                            lblPath = "";
                            for (int j = 0; j < dt.Rows.Count; j++)
                            {
                                string tempStringNewField = dt.Rows[j]["MaskDetail1"].ToString();
                                if (tempStringNewField == "_")
                                {
                                    tempStringNewField = tempStringNewField.Replace("_", "__");
                                }
                                lblPath += tempStringNewField;
                            }
                        }
                    }
                }
                else
                {
                    // Cancel code here
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void sendNewField()
        {
            try
            {
                maskDetailsGridCount++;
                columnOrText = "Text";
                flagValue = true;
                string strNewfield = newField;
                if (strNewfield == "_")
                {
                    strNewfield = strNewfield.Replace("_", "__");
                }
                lblPath += strNewfield;
                count = count + 1;
                dt.Rows.Add(0, 0, newField, "Label", count);
                dgMaskDetailsData = dt.DefaultView;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void sendMask()
        {
            try
            {
                maskDetailsGridCount++;
                columnOrText = "Column";
                flagValue = true;
                lblPath += lstFieldsSelectedItem;
                count = count + 1;
                dt.Rows.Add(0, 0, lstFieldsSelectedItem, "Field", count);

                dgMaskDetailsData = dt.DefaultView;
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void save()
        {
            try
            {
                db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                isSave = true;
                if (maskId == 0)
                {
                    if ((!string.IsNullOrEmpty(maskName)))
                    {
                        lblRequired = "";
                        lblRequiredSchool = "";
                        tblmask _objTableMask = new tblmask();
                        _objTableMask.MaskName = maskName;
                        if (_objTableMask != null)
                        {
                            db.tblmasks.InsertOnSubmit(_objTableMask);
                            db.SubmitChanges();

                            _objMaskDetailsItem = new MaskDetailsItem { maskName = _objTableMask.MaskName, maskId = _objTableMask.MaskID, SchoolId = _objTableMask.schoolid };

                            int i = 0;
                            // Need to save data in MaskDetails tables ..
                            for (i = 0; i < maskDetailsGridCount; i++)
                            {
                                try
                                {
                                    MaskDetail _objMaskDetail = new MaskDetail();
                                    _objMaskDetail.MaskID = Convert.ToInt32(clsDashBoard.getmaxmaskid(db));
                                    _objMaskDetail.Type = dt.Rows[i]["Type"].ToString();
                                    _objMaskDetail.MaskDetail1 = dt.Rows[i]["MaskDetail1"].ToString();
                                    _objMaskDetail.SortOrder = dt.Rows[i]["SortOrder"].ToString();
                                    if (_objMaskDetail != null)
                                    {
                                        db.MaskDetails.InsertOnSubmit(_objMaskDetail);
                                        db.SubmitChanges();
                                        DialogResult = false;
                                    }
                                }
                                catch (Exception ex)
                                { clsStatic.WriteExceptionLogXML(ex); }
                            }
                            if (i == maskDetailsGridCount)
                                DialogResult = false;
                        }
                    }
                    else
                        if ((maskName == ""))
                        {
                            lblRequired = "*";
                            lblRequiredSchool = "*";
                        }
                        else if ((maskName != ""))
                        {
                            lblRequiredSchool = "*";
                            lblRequired = "";
                        }
                        else if ((maskName == ""))
                        {
                            lblRequired = "*";
                            lblRequiredSchool = "";
                        }
                        else
                        {
                            lblRequired = "";
                            lblRequiredSchool = "";
                        }

                }
                else if (maskId != 0)
                {
                    if ((!string.IsNullOrEmpty(maskName)))
                    {
                        db = new PhotoSorterDBModelDataContext(clsConnectionString.connectionString);
                        lblRequired = "";
                        lblRequiredSchool = "";
                        tblmask _objmsk = new tblmask();
                        _objmsk = clsDashBoard._UpdateMask(db, maskId);
                        _objmsk.MaskName = maskName;
                        if (_objmsk != null)
                        {
                            int retdelval = 0;
                            db.SubmitChanges();

                            _objMaskDetailsItem = new MaskDetailsItem { maskName = _objmsk.MaskName, maskId = _objmsk.MaskID, SchoolId = _objmsk.schoolid };

                            SqlParameter[] param = new SqlParameter[1];
                            param[0] = new SqlParameter("@MaskID", maskId);
                            retdelval = WCFSQLHelper.executeNonQuery_SP("sp_DeleteMDetails", param);
                            int k = 0;
                            for (k = 0; k < dt.Rows.Count; k++)
                            {
                                try
                                {
                                    if ((Convert.ToInt32(dt.Rows[k]["MaskDetID"]) == 0) && ((Convert.ToInt32(dt.Rows[k]["MaskID"]) == 0)))
                                        dt.Rows[k]["MaskID"] = maskId;
                                    MaskDetail _objMD = new MaskDetail();
                                    _objMD.MaskID = Convert.ToInt32(dt.Rows[k]["MaskID"]);
                                    _objMD.MaskDetail1 = dt.Rows[k]["MaskDetail1"].ToString();
                                    _objMD.Type = dt.Rows[k]["Type"].ToString();
                                    _objMD.SortOrder = dt.Rows[k]["SortOrder"].ToString();
                                    if (_objMD != null)
                                    {
                                        db.MaskDetails.InsertOnSubmit(_objMD);
                                        db.SubmitChanges();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    clsStatic.WriteExceptionLogXML(ex);
                                    MVVMMessageService.ShowMessage(ex.Message);
                                }
                            }
                            if (k == dt.Rows.Count)
                                DialogResult = false;
                        }
                    }
                    else
                        if ((maskName == ""))
                        {
                            lblRequired = "*";
                            lblRequiredSchool = "*";
                        }
                        else if ((maskName != ""))
                        {
                            lblRequiredSchool = "*";
                            lblRequired = "";
                        }
                        else if ((maskName == ""))
                        {
                            lblRequired = "*";
                            lblRequiredSchool = "";
                        }
                        else
                        {
                            lblRequired = "";
                            lblRequiredSchool = "";
                        }
                }
            }
            catch (Exception ex)
            {
                clsStatic.WriteExceptionLogXML(ex);
                MVVMMessageService.ShowMessage(ex.Message);
            }
        }
        private void windowClose()
        {
            isSave = false;
            DialogResult = false;
        }
        #endregion
    }
}
