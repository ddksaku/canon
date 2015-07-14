using System;
using System.IO;
using System.Web;
using Memos.Framework.Logging;

namespace CanonWebApp.Code
{
    /// <summary>
    /// Config values class.
    /// </summary>
    public static class ConfigSettings
    {
        #region DefaultLanguage
        /// <summary>
        /// Default language.
        /// </summary>
        public static string DefaultLanguage
        {
            get
            {
                return Memos.Framework.ConfigSettings.GetValueFromConfig("DefaultLanguage", "en-GB");
            }
        }
        #endregion

        #region ServerLanguage
        /// <summary>
        /// Default language.
        /// </summary>
        public static string ServerLanguage
        {
            get
            {
                return Memos.Framework.ConfigSettings.GetValueFromConfig("ServerLanguage", "cs-CZ");
            }
        }
        #endregion

        #region DefaultPageSize
        /// <summary>
        /// Default language.
        /// </summary>
        public static int DefaultPageSize
        {
            get
            {
                return 15;
            }
        }
        #endregion

        #region UploadDirectory
        /// <summary>
        /// Directory to upload files.
        /// </summary>
        public static string UploadDirectory
        {
            get
            {
                return "Uploaded";
            }
        }
        #endregion
    }
}