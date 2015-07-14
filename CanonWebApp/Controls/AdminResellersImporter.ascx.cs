using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Memos.Framework.Logging;
using System.IO;
using CanonWebApp.Code;
using Canon.Data.Import;
using Canon.Data.Business;

namespace CanonWebApp.Controls
{
    public partial class AdminResellersImporter : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public string ParentPopupClientID
        {
            set
            {
                btnClose.ClientSideEvents.Click = "function(s,e){" + value + ".Hide();}";
            }
        }

        protected void UploadControl_FileUploadComplete(object sender, DevExpress.Web.ASPxUploadControl.FileUploadCompleteEventArgs e)
        {
            try
            {
                string contentType = HttpContext.Current.Request.Files[0].ContentType;
                Logger.Log(string.Format("Tryed content type is {0} ({1})", contentType, e.UploadedFile.FileName), LogLevel.Info);
                if (Path.GetExtension(e.UploadedFile.FileName) != ".xls")
                {
                    e.IsValid = false;
                    lblImportStatus.Text = e.ErrorText = "Typ dokumentu není povolen pro nahrávání";
                    return;
                }

                string filename = FileUtils.SavePostedFile(e.UploadedFile);
                // import uploaded file
                CanonResellersImport<CanonImportResellerRecord> import = new CanonResellersImport<CanonImportResellerRecord>(filename, e.UploadedFile.FileName);
                if (!import.ExportToDb())
                {
                    e.IsValid = false;
                    lblImportStatus.Text = e.ErrorText = Utilities.GetResourceString("Validators", "GeneralFileImportError");
                }
                else
                {
                    e.IsValid = true;
                    lblImportStatus.Text = e.ErrorText = Utilities.GetResourceString("Validators", "FileImportedSuccessfully");
                }

                e.CallbackData = Utilities.BuildStringWithSeparator(
                                                Utilities.ConvertImportErrorsToStrings(import.ErrorMessages), "||");


            }
            catch(Exception ex)
            {
                e.IsValid = false;
                lblImportStatus.Text = e.ErrorText = Utilities.GetResourceString("Validators", "GeneralFileImportError");

                Logger.Log(string.Format("File {0}, exception {1}", e.UploadedFile.FileName, ex.ToString()),
                                    LogLevel.Error);
            }
        }
    }
}