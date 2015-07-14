using System;
using System.IO;
using System.Web;

namespace Memos.Administration
{
    /// <summary>
    /// Config values class of web administration.
    /// </summary>
    public static class ConfigSettings
    {
        // Application info

        #region ApplicationName
        /// <summary>
        /// Name of the application.
        /// </summary>
        public static string ApplicationName
        {
            get
            {
                return Framework.ConfigSettings.GetValueFromConfig("ApplicationName");
            }
        }
        #endregion

        // Administration user

        #region Login
        /// <summary>
        /// Login of the administration user.
        /// </summary>
        public static string Login
        {
            get
            {
                return Framework.ConfigSettings.GetValueFromConfig("Login");
            }
        }
        #endregion

        #region Password
        /// <summary>
        /// Password of the administration user.
        /// </summary>
        public static string Password
        {
            get
            {
                return Framework.ConfigSettings.GetValueFromConfig("Password");
            }
        }
        #endregion

        // Default application database info

        #region SqlServerName
        /// <summary>
        /// Default SQL server name string.
        /// </summary>
        public static string SqlServerName
        {
            get
            {
                return Framework.ConfigSettings.GetValueFromConfig("SqlServerName");
            }
        }
        #endregion

        #region SqlDatabaseName
        /// <summary>
        /// Default SQL database string.
        /// </summary>
        public static string SqlDatabaseName
        {
            get
            {
                return Framework.ConfigSettings.GetValueFromConfig("SqlDatabaseName");
            }
        }
        #endregion

        #region SqlUserName
        /// <summary>
        /// Default SQL user name string.
        /// </summary>
        public static string SqlUserName
        {
            get
            {
                return Framework.ConfigSettings.GetValueFromConfig("SqlUserName");
            }
        }
        #endregion

        #region SqlPassword
        /// <summary>
        /// Default SQL password string.
        /// </summary>
        public static string SqlPassword
        {
            get
            {
                return Framework.ConfigSettings.GetValueFromConfig("SqlPassword");
            }
        }
        #endregion

        // Paths to the appropriate libraries

        #region ApplicationFile
        /// <summary>
        /// Path to the application file.
        /// </summary>
        public static string ApplicationFile
        {
            get
            {
                string file = Framework.ConfigSettings.GetValueFromConfig("ApplicationFile");
                file = HttpContext.Current.Server.MapPath(file);

                if (!File.Exists(file))
                {
                    return String.Empty;
                }

                return file;
            }
        }
        #endregion

        #region UnitTestsFile
        /// <summary>
        /// Path to the file with unit tests.
        /// </summary>
        public static string UnitTestsFile
        {
            get
            {
                string file = Framework.ConfigSettings.GetValueFromConfig("UnitTestsFile");
                file = HttpContext.Current.Server.MapPath(file);

                if (!File.Exists(file))
                {
                    return String.Empty;
                }

                return file;
            }
        }
        #endregion

        #region AppTestsFile
        /// <summary>
        /// Path to the file with application tests.
        /// </summary>
        public static string AppTestsFile
        {
            get
            {
                string file = Framework.ConfigSettings.GetValueFromConfig("AppTestsFile");
                file = HttpContext.Current.Server.MapPath(file);

                if (!File.Exists(file))
                {
                    return String.Empty;
                }

                return file;
            }
        }
        #endregion
    }
}