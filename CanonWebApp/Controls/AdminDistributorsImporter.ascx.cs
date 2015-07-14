using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Canon.Data;
using Memos.Framework.Logging;
using System.Web.UI.HtmlControls;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxUploadControl;
using CanonWebApp.Code;
using System.IO;
using Canon.Data.Business;
using Canon.Data.Import;
using DevExpress.Web.ASPxPopupControl;

namespace CanonWebApp.Controls
{
    public partial class AdminDistributorsImporter : CanonPageControl
    {
        public void RefreshComboBoxes()
        {
            this.BindData();

            cboDistributors1.SelectedItem = null;
            cboDistributors2.SelectedItem = null;
            cboDistributors3.SelectedItem = null;
            cboDistributors4.SelectedItem = null;
            cboDistributors5.SelectedItem = null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (IsPostBack)
                {
                    //string value = Request.Form["hidUploadCount"].ToString();
                    //int uploadCount = int.Parse(value);
                    //for (int i = 1; i <= uploadCount; i++)
                    //{
                    //    Control ctrl = clbPanelDistributorsImporter.FindControl("tblUploadControlRow" + i.ToString());
                    //    HtmlTableRow row = ctrl as HtmlTableRow;
                    //    row.Style["Display"] = "";
                    //}
                }
                else
                {
                    distributorIds.Clear();
                }
                
                base.PageLoadEvent(sender, e);
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected override void BindData()
        {
            CanonDataContext db = Cdb.Instance;

            List<Distributor> distributors = db.Distributors.Where(d => d.IsDeleted == false).ToList();

            cboDistributors1.DataSource = distributors;
            cboDistributors1.DataBind();

            cboDistributors2.DataSource = distributors;
            cboDistributors2.DataBind();

            cboDistributors3.DataSource = distributors;
            cboDistributors3.DataBind();

            cboDistributors4.DataSource = distributors;
            cboDistributors4.DataBind();

            cboDistributors5.DataSource = distributors;
            cboDistributors5.DataBind();

        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
        }

        protected void clbPanelDistributorsImporter_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void UploadControl1_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            try
            {
                string contentType = HttpContext.Current.Request.Files[0].ContentType;
                Logger.Log(string.Format("Tryed content type is {0} ({1})", contentType, e.UploadedFile.FileName), LogLevel.Info);
                if (Path.GetExtension(e.UploadedFile.FileName) != ".xls")
                {
                    e.IsValid = false;
                    e.CallbackData = Utilities.GetResourceString("Validators", "NotAllowedContentTypeError");
                    return;
                }

                // getting DistributorId
                ASPxUploadControl ctrl = sender as ASPxUploadControl;
                if (ctrl != null && distributorIds.Keys.FirstOrDefault(k=>k == ctrl.ID) == null)
                {
                    e.IsValid = false;
                    e.CallbackData = "Vyberte distributora";
                    return;
                }

                int distributorId = distributorIds[ctrl.ID];

                string filename = FileUtils.SavePostedFile(e.UploadedFile);
                CanonDistributorImport<CanonImportDistributorRecord> import = new CanonDistributorImport<CanonImportDistributorRecord>(filename, distributorId, e.UploadedFile.FileName);
                if (!import.ExportToDb())
                {
                    e.IsValid = false;
                    e.CallbackData = "Chyba při zpracování. Importovací soubor nemá správný formát.";
                }
                else
                {
                    e.IsValid = true;
                    e.CallbackData = "Import proběhl úspěšně.";
                }
            }
            catch (Exception ex)
            {
                e.IsValid = false;
                e.ErrorText = Utilities.GetResourceString("Validators", "GeneralFileImportError");
                Logger.Log(string.Format("File {0}, exception {1}", e.UploadedFile.FileName, ex.ToString()),
                                    LogLevel.Error);
            }
        }

        protected void cboDistributors1_Validation(object sender, ValidationEventArgs e)
        {
        }

        protected void clbPanelForDistributors_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                int distributorId = -1;
                if (e.Parameter == "Upload1")
                {
                    distributorId = int.Parse(cboDistributors1.Value.ToString());
                    distributorIds["UploadControl1"] = distributorId;
                    idd = distributorId;
                }
                else if (e.Parameter == "Upload2")
                {
                    distributorId = int.Parse(cboDistributors2.Value.ToString());
                    distributorIds["UploadControl2"] = distributorId;
                }
                else if (e.Parameter == "Upload3")
                {
                    distributorId = int.Parse(cboDistributors3.Value.ToString());
                    distributorIds["UploadControl3"] = distributorId;
                }
                else if (e.Parameter == "Upload4")
                {
                    distributorId = int.Parse(cboDistributors4.Value.ToString());
                    distributorIds["UploadControl4"] = distributorId;
                }
                else if (e.Parameter == "Upload5")
                {
                    distributorId = int.Parse(cboDistributors5.Value.ToString());
                    distributorIds["UploadControl5"] = distributorId;
                }

                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
           
        }

        private static Dictionary<string, int> distributorIds = new Dictionary<string, int>();
        private static int idd = -1;

    }
}