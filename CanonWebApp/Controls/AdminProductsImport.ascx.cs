using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxUploadControl;
using Memos.Framework.Logging;
using CanonWebApp.Code;
using CanonWebApp.Business;
using Canon.Data.Import;
using Canon.Data.Business;

namespace CanonWebApp.Controls
{
    public partial class AdminProductsImport : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Localize();
        }

        protected void Localize()
        {
            UploadControl.ValidationSettings.MaxFileSizeErrorText =
                Utilities.GetResourceString("Validators", "MaxFileSizeErrorText");
        }

        protected void UploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            try
            {
                string contentType = HttpContext.Current.Request.Files[0].ContentType;
                Logger.Log(string.Format("Tryed content type is {0} ({1})", contentType, e.UploadedFile.FileName), LogLevel.Info);
                if (Path.GetExtension(e.UploadedFile.FileName) != ".xls")
                {
                    e.IsValid = false;
                    lblImportStatus.Text = e.ErrorText = Utilities.GetResourceString("Validators", "NotAllowedContentTypeError");
                    return;
                }
                string filename = FileUtils.SavePostedFile(e.UploadedFile);
                //Import uploaded file
                CanonProductImport<CanonProduct> import = new CanonProductImport<CanonProduct>(filename);
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
                //Format errors and warnings

                e.CallbackData = Utilities.BuildStringWithSeparator(
                                                Utilities.ConvertImportErrorsToStrings(import.ErrorMessages), "||");
            }
            catch (Exception ex)
            {
                e.IsValid = false;
                lblImportStatus.Text = e.ErrorText = Utilities.GetResourceString("Validators", "GeneralFileImportError");
                Logger.Log(string.Format("File {0}, exception {1}", e.UploadedFile.FileName, ex.ToString()),
                                    LogLevel.Error);
            }
        }

        
    }
}