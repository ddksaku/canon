using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using DevExpress.Web.ASPxUploadControl;

namespace CanonWebApp.Code
{
    public class FileUtils
    {
        public static string GetUploadedFileLocalName(string name)
        {
            string resFileName = HttpContext.Current.Request.MapPath(ConfigSettings.UploadDirectory) + "\\" + name;
            if (File.Exists(resFileName))
                resFileName = HttpContext.Current.Request.MapPath(ConfigSettings.UploadDirectory) + "\\" + Path.GetRandomFileName() + name;
            return resFileName;
        }

        public static string SavePostedFile(UploadedFile uploadedFile)
        {
            string ret = "";
            if (uploadedFile.IsValid)
            {
                string fileToSave = FileUtils.GetUploadedFileLocalName(uploadedFile.FileName);
                uploadedFile.SaveAs(fileToSave);
                ret = fileToSave;
            }
            return ret;
        }

        public static bool IsAllowedFileType(string contentType, string[] allowedTypes)
        {
            foreach (string allowed in allowedTypes)
                if (contentType == allowed)
                    return true;
            return false;
        }
    }
}
