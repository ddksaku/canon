using System;
using System.Web;

namespace Memos.Administration
{
    /// <summary>
    /// Sessions manager.
    /// </summary>
    public class SessionManager
    {
        #region Fields

        /// <summary>
        /// SQL database name key.
        /// </summary>
        protected const string LOGGED_USER_KEY = "MemosAdministrationLoggedUser";

        /// <summary>
        /// SQL database name key.
        /// </summary>
        protected const string SQL_DATABASE_NAME_KEY = "SqlDatabaseName";

        /// <summary>
        /// Checked connection string key.
        /// </summary>
        protected const string CHECKED_CONNECTION_STRING_KEY = "CheckedConnectionString";

        #endregion

        #region LoggedUser
        /// <summary>
        /// Logged user.
        /// </summary>
        public static AdministrationUser LoggedUser
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }

                if (HttpContext.Current.Session != null && HttpContext.Current.Session[LOGGED_USER_KEY] != null)
                {
                    return (HttpContext.Current.Session[LOGGED_USER_KEY] as AdministrationUser);
                }

                return null;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    return;
                }

                HttpContext.Current.Session[LOGGED_USER_KEY] = value;
            }
        }
        #endregion

        #region SqlDatabaseName
        /// <summary>
        /// SQL database name.
        /// </summary>
        public static string SqlDatabaseName
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return String.Empty;
                }

                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SQL_DATABASE_NAME_KEY] != null)
                {
                    return (Convert.ToString(HttpContext.Current.Session[SQL_DATABASE_NAME_KEY]));
                }

                return String.Empty;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    return;
                }

                HttpContext.Current.Session[SQL_DATABASE_NAME_KEY] = value;
            }
        }
        #endregion

        #region CheckedConnectionString
        /// <summary>
        /// Checked connection string.
        /// </summary>
        public static string CheckedConnectionString
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return String.Empty;
                }

                if (HttpContext.Current.Session != null && HttpContext.Current.Session[CHECKED_CONNECTION_STRING_KEY] != null)
                {
                    return (Convert.ToString(HttpContext.Current.Session[CHECKED_CONNECTION_STRING_KEY]));
                }

                return String.Empty;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    return;
                }

                HttpContext.Current.Session[CHECKED_CONNECTION_STRING_KEY] = value;
            }
        }
        #endregion
    }
}