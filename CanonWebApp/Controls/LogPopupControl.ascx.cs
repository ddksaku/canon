using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CanonWebApp.Enums;
using CanonWebApp.Code;
using Canon.Data;
using DevExpress.Web.ASPxGridView;
using Canon.Data.Enums;

namespace CanonWebApp.Controls
{
    public partial class LogPopupControl : System.Web.UI.UserControl
    {
        public int LogTypeValue { get; set; }
        public LogTypesEnum LogType
        {
            get
            {
                return (LogTypesEnum)this.LogTypeValue;
            }
        }
        public int[] Parameters { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (gridLogRecords.Columns.Count == 0)
            {
                ReCreateColumns();
            }
            if ((Parameters == null)||(Parameters.Length == 0))
                Parameters = SessionManager.PopupGridCurrentIds;
            if ((LogType == LogTypesEnum.ProductMappingHistory) && (Parameters.Length > 1))
                Bind();
            else if 
                ((LogType != LogTypesEnum.ProductImportHistory)&&
                (LogType != LogTypesEnum.ProductMappingHistory)&&
                (Parameters.Length > 0))
                Bind();
        }

        protected void ReCreateColumns()
        {
            gridLogRecords.Columns.Clear();
            //create columns
            switch (this.LogType)
            {
                case LogTypesEnum.ProductMappingHistory:
                    gridLogRecords.KeyFieldName = "RecordId";
                    GridViewDataDateColumn dateColumn = new GridViewDataDateColumn();
                    dateColumn.Caption = Utilities.GetResourceString("Common", "LogDate");
                    dateColumn.FieldName = "LogDate";
                    dateColumn.PropertiesDateEdit.DisplayFormatString = "dd.MM.yyyy HH:mm";
                    dateColumn.VisibleIndex = 0;
                    gridLogRecords.Columns.Add(dateColumn);
                    GridViewDataTextColumn stateColumn = new GridViewDataTextColumn();
                    stateColumn.Caption = Utilities.GetResourceString("Common", "LogStatus");
                    stateColumn.FieldName = string.Format("Enum.Name{0}", SessionManager.CurrentShortLanguage);
                    stateColumn.VisibleIndex = 1;
                    gridLogRecords.Columns.Add(stateColumn);
                    break;
                case LogTypesEnum.ChannelMonitoringHistory:
                    gridLogRecords.KeyFieldName = "LogId";
                    GridViewDataDateColumn logDateColumn = new GridViewDataDateColumn();
                    logDateColumn.Caption = Utilities.GetResourceString("Common", "LogDate");
                    logDateColumn.FieldName = "LogDate";
                    logDateColumn.PropertiesDateEdit.DisplayFormatString = "dd.MM.yyyy HH:mm";
                    logDateColumn.VisibleIndex = 0;
                    gridLogRecords.Columns.Add(logDateColumn);
                    GridViewDataTextColumn statusColumn = new GridViewDataTextColumn();
                    statusColumn.Caption = Utilities.GetResourceString("Common", "LogStatus");
                    statusColumn.FieldName = string.Format("Enum.Name{0}", SessionManager.CurrentShortLanguage);
                    statusColumn.VisibleIndex = 1;
                    gridLogRecords.Columns.Add(statusColumn);
                    break;
                case LogTypesEnum.ProductImportHistory:
                    GridViewDataDateColumn impDateColumn = new GridViewDataDateColumn();
                    impDateColumn.Caption = Utilities.GetResourceString("Common", "LogDate");
                    impDateColumn.FieldName = "LogDate";
                    impDateColumn.PropertiesDateEdit.DisplayFormatString = "dd.MM.yyyy HH:mm";
                    impDateColumn.VisibleIndex = 0;
                    gridLogRecords.Columns.Add(impDateColumn);
                    GridViewDataTextColumn userColumn = new GridViewDataTextColumn();
                    userColumn.Caption = Utilities.GetResourceString("Common", "FullName");
                    userColumn.FieldName = "FullName";
                    userColumn.VisibleIndex = 1;
                    gridLogRecords.Columns.Add(userColumn);
                    break;
                case LogTypesEnum.RecommendedPriceHistory:
                    gridLogRecords.KeyFieldName = "LogId";
                    GridViewDataDateColumn recDateColumn = new GridViewDataDateColumn();
                    recDateColumn.Caption = Utilities.GetResourceString("Common", "LogDate");
                    recDateColumn.FieldName = "RecomDate";
                    recDateColumn.PropertiesDateEdit.DisplayFormatString = "dd.MM.yyyy";
                    recDateColumn.VisibleIndex = 0;
                    gridLogRecords.Columns.Add(recDateColumn);
                    GridViewDataTextColumn recUserColumn = new GridViewDataTextColumn();
                    recUserColumn.Caption = Utilities.GetResourceString("Common", "FullName");
                    recUserColumn.FieldName = "User.FullName";
                    recUserColumn.VisibleIndex = 1;
                    gridLogRecords.Columns.Add(recUserColumn);
                    GridViewDataTextColumn recTypeColumn = new GridViewDataTextColumn();
                    recTypeColumn.Caption = Utilities.GetResourceString("Common", "ChangeType");
                    recTypeColumn.FieldName = string.Format("Enum.Name{0}", SessionManager.CurrentShortLanguage);
                    recTypeColumn.VisibleIndex = 2;
                    gridLogRecords.Columns.Add(recTypeColumn);
                    GridViewDataTextColumn recPriceColumn = new GridViewDataTextColumn();
                    recPriceColumn.Caption = Utilities.GetResourceString("Common", "NewPrice");
                    recPriceColumn.FieldName = "Price";
                    recPriceColumn.VisibleIndex = 3;
                    gridLogRecords.Columns.Add(recPriceColumn);
                    break;
                case LogTypesEnum.ChannelLog:
                    gridLogRecords.KeyFieldName = "ErrorId";
                    GridViewDataTextColumn errorColumn = new GridViewDataTextColumn();
                    errorColumn.Caption = Utilities.GetResourceString("Common", "ErrorType");
                    errorColumn.FieldName = string.Format("Enum.Name{0}", SessionManager.CurrentShortLanguage);
                    errorColumn.VisibleIndex = 0;
                    gridLogRecords.Columns.Add(errorColumn);
                    GridViewDataTextColumn prodColumn = new GridViewDataTextColumn();
                    prodColumn.Caption = Utilities.GetResourceString("Common", "ProductName");
                    prodColumn.FieldName = "ProductName";
                    prodColumn.VisibleIndex = 1;
                    gridLogRecords.Columns.Add(prodColumn);
                    break;
            }
        }

        public int Bind()
        {
            CanonDataContext db = Cdb.Instance;
            int returnRes = 0;
            //bind data
            switch (this.LogType)
            {
                case LogTypesEnum.ProductMappingHistory:
                    var values = db.ProductsLogs.OrderByDescending(p=> p.LogDate).Where(p=> 
                                        p.ChannelId==this.Parameters[0] && p.ProductId==this.Parameters[1]);
                    gridLogRecords.DataSource = values;
                    break;
                case LogTypesEnum.ChannelMonitoringHistory:
                    var records = db.ImportLogs.OrderByDescending(p => p.LogDate).Where(p =>
                                                                p.ChannelId == this.Parameters[0]);
                    gridLogRecords.DataSource = records;
                    break;
                case LogTypesEnum.ProductImportHistory:
                    var recs = db.GetProductImportLog();
                    gridLogRecords.DataSource = recs;
                    break;
                case LogTypesEnum.RecommendedPriceHistory:
                    this.ReCreateColumns();
                    var recoms = db.RecommendedLogs.Where(p=> p.ProductId==this.Parameters[0]).OrderByDescending(d=>d.RecomDate);
                    gridLogRecords.DataSource = recoms;
                    break;
                case LogTypesEnum.ChannelLog:
                    var errors = db.ImportLogErrors.Where(p=> p.MainLogId==this.Parameters[0]);
                    gridLogRecords.DataSource = errors;
                    returnRes = errors.ToList().Count;
                    break;
            }
            gridLogRecords.DataBind();
            return returnRes;
        }

        protected void gridLogRecords_HtmlRowPrepared(object sender, DevExpress.Web.ASPxGridView.ASPxGridViewTableRowEventArgs e)
        {
            if (this.LogType != LogTypesEnum.ChannelMonitoringHistory) return;
            if (e.RowType != DevExpress.Web.ASPxGridView.GridViewRowType.Data) return;
            ChannelLogEnum value = (ChannelLogEnum)e.GetValue("LogType");
            if (value == ChannelLogEnum.ChannelError)
                e.Row.BackColor = System.Drawing.Color.Red;
            else if (value == ChannelLogEnum.ProductError)
                e.Row.BackColor = System.Drawing.Color.Orange;
        }


    }
}