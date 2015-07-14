using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;
using System.IO;
using Memos.Framework.Logging;
using CanonWebApp.Enums;
using Canon.Data;

namespace CanonWebApp.Code
{
    /// <summary>
    /// Sessions manager.
    /// </summary>
    public class SessionManager
    {
        #region Fields

        /// <summary>
        /// If edit form is created
        /// </summary>
        protected const string IS_EDIT_FORM_CREATED = "IsEditFormCreated";

        /// <summary>
        /// If page is filtered
        /// </summary>
        protected const string IS_FILTERED_PAGE = "IsFiltered";

        /// <summary>
        /// If page is filtered
        /// </summary>
        protected const string FILTER_TEXT_PAGE = "FilterTextPage";

        /// <summary>
        /// ASP CheckBoxList helper
        /// </summary>
        protected const string CHECK_LIST_BOX_HELPER = "CheckBoxListHelper";

        /// <summary>
        /// Logged user key.
        /// </summary>
        protected const string LOGGED_USER_KEY = "LoggedUser";

        /// <summary>
        /// Reservation number key.
        /// </summary>
        protected const string RESERVATION_NUMBER_KEY = "ReservationNumber";

        /// <summary>
        /// Current language key.
        /// </summary>
        protected const string CURRENT_LANGUAGE_KEY = "CurrentLanguage";

        /// <summary>
        /// Current language key.
        /// </summary>
        public const string SQL_QUERIES_MONITOR_KEY = "SqlQueriesMonitor";

        /// <summary>
        /// Current categories
        /// </summary>
        public const string MONITOR_SELECTED_CATEGORIES = "MonitorSelectedCategories";

        /// <summary>
        /// Current channels
        /// </summary>
        public const string MONITOR_SELECTED_CHANNELS = "MonitorSelectedChannels";

        /// <summary>
        /// Current channels
        /// </summary>
        public const string POPUP_GRID_CURRENT_IDS = "POPUP_GRID_CURRENT_VALUES";

        #endregion

        #region IsEditFormCreated
        /// <summary>
        /// If edit form is created, it's used in custom edit forms in gridviews
        /// </summary>
        public static bool IsEditFormCreated
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return false;
                }
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[IS_EDIT_FORM_CREATED] != null)
                {
                    return ((bool)HttpContext.Current.Session[IS_EDIT_FORM_CREATED]);
                }
                return false;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    return;
                }

                HttpContext.Current.Session[IS_EDIT_FORM_CREATED] = value;
            }
        }
        #endregion

        #region CheckBoxList Helper
        /// <summary>
        /// Contains values which were added to CheckBoxList while editing
        /// </summary>
        public static Dictionary<int,int> CheckBoxListHelper
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[CHECK_LIST_BOX_HELPER] != null)
                {
                    return ((Dictionary<int,int>)HttpContext.Current.Session[CHECK_LIST_BOX_HELPER]);
                }
                return null;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    return;
                }

                HttpContext.Current.Session[CHECK_LIST_BOX_HELPER] = value;
            }
        }
        #endregion

        #region IsFilteredPage
        /// <summary>
        /// If page is filtered
        /// </summary>
        public static bool IsFilteredPage
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return false;
                }
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[IS_FILTERED_PAGE] != null)
                {
                    return ((bool)HttpContext.Current.Session[IS_FILTERED_PAGE]);
                }
                return false;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    return;
                }

                HttpContext.Current.Session[IS_FILTERED_PAGE] = value;
            }
        }
        #endregion

        #region FilterText
        /// <summary>
        /// Filter text used on pages for filtering
        /// </summary>
        public static string FilterText
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return string.Empty;
                }
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[FILTER_TEXT_PAGE] != null)
                {
                    return ((string)HttpContext.Current.Session[FILTER_TEXT_PAGE]);
                }
                return string.Empty;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    return;
                }

                HttpContext.Current.Session[FILTER_TEXT_PAGE] = value;
            }
        }
        #endregion

        #region LoggedUser
        /// <summary>
        /// Logged user.
        /// </summary>
        public static User LoggedUser
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }

                if (HttpContext.Current.Session != null && HttpContext.Current.Session[LOGGED_USER_KEY] != null)
                {
                    return (HttpContext.Current.Session[LOGGED_USER_KEY] as User);
                }

                string login = HttpContext.Current.User.Identity.Name;

                if (String.IsNullOrEmpty(login))
                {
                    //We don't need to ask database for empty login name
                    return null;
                }

                User user = Business.User.GetUserByLogin(login);

                if (user != null)
                {
                    if (HttpContext.Current.Session != null)
                    {
                        HttpContext.Current.Session[LOGGED_USER_KEY] = user;
                    }
                    
                    return user;
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

        #region ReservationNumber
        /// <summary>
        /// Reservation number.
        /// </summary>
        public static string ReservationNumber
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return String.Empty;
                }

                if (HttpContext.Current.Session != null && HttpContext.Current.Session[RESERVATION_NUMBER_KEY] != null)
                {
                    return (Convert.ToString(HttpContext.Current.Session[RESERVATION_NUMBER_KEY]));
                }

                return String.Empty;
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    return;
                }

                HttpContext.Current.Session[RESERVATION_NUMBER_KEY] = value;
            }
        }
        #endregion

        #region CurrentLanguage
        /// <summary>
        /// Current language.
        /// </summary>
        public static string CurrentLanguage
        {
            get
            {
                // 1. Check if we have saved language in the cookies
                if (HttpContext.Current.Request != null && HttpContext.Current.Request.Cookies[CURRENT_LANGUAGE_KEY] != null)
                {
                    return (HttpContext.Current.Request.Cookies[CURRENT_LANGUAGE_KEY].Value);
                }

                // 3. Browser settings language
                if (HttpContext.Current.Request != null && HttpContext.Current.Request.UserLanguages != null)
                {
                    foreach (string userLanguage in HttpContext.Current.Request.UserLanguages)
                    {
                        foreach (KeyValuePair<Languages, string> pair in Business.Language.SupportedLanguages)
                        {
                            if (String.Equals(userLanguage.Substring(0, 2), pair.Value.Substring(0, 2), StringComparison.CurrentCultureIgnoreCase))
                            {
                                CurrentLanguage = pair.Value;
                                return pair.Value;
                            }
                        }
                    }
                }

                // 4. web.config default language
                CurrentLanguage = ConfigSettings.DefaultLanguage;
                return ConfigSettings.DefaultLanguage;
            }
            set
            {
                HttpCookie cookie = new HttpCookie(CURRENT_LANGUAGE_KEY, value);
                cookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        #endregion

        #region CurrentShortLanguage
        /// <summary>
        /// Returns En or Cz
        /// </summary>
        public static string CurrentShortLanguage
        {
            get
            {
                switch (SessionManager.CurrentLanguage)
                {
                    case "en-GB":
                        return "En";
                    case "cs-CZ":
                        return "Cz";
                    default:
                        return "En";
                }
            }
        }
        #endregion

        #region CurrentLanguageCookiesTime
        /// <summary>
        /// Current language cookies time in seconds.
        /// </summary>
        public static double CurrentLanguageCookiesTime
        {
            get
            {
                if (HttpContext.Current.Request != null && HttpContext.Current.Request.Cookies[CURRENT_LANGUAGE_KEY] != null)
                {
                    DateTime cookieExpiredTime = HttpContext.Current.Request.Cookies[CURRENT_LANGUAGE_KEY].Expires;

                    TimeSpan duration = DateTime.Now.AddYears(1) - cookieExpiredTime;

                    return duration.TotalSeconds;
                }

                return 0;
            }
        } 
        #endregion

        #region SqlQueriesMonitor
        /// <summary>
        /// All sql queries that are sent to database using CanonDataContext are logged into this object.
        /// Only queries during one request are logged (when response is sent to client, this object is removed). HttpContext.Current.Items is used to save all queries
        /// </summary>
        public static TextWriter SqlQueriesMonitor
        {
            get
            {
                if (HttpContext.Current == null || HttpContext.Current.Session == null)
                {
                    return new StringWriter();
                }

                if (HttpContext.Current.Session[SQL_QUERIES_MONITOR_KEY] == null)
                {
                    TextWriter textWriter = new StringWriter();
                    HttpContext.Current.Session[SQL_QUERIES_MONITOR_KEY] = textWriter;

                    return textWriter;
                }

                return HttpContext.Current.Session[SQL_QUERIES_MONITOR_KEY] as TextWriter;
            }
            set
            {
                HttpContext.Current.Session[SQL_QUERIES_MONITOR_KEY] = value;
            }
        } 
        #endregion

        #region ToString
        /// <summary>
        /// Returns a string, that contains all object saved in Session and its values
        /// </summary>
        /// <returns></returns>
        public new static string ToString()
        {
            string str = "";

            try
            {
                PropertyInfo[] properties = Assembly.GetExecutingAssembly().GetType("CanonWebApp.Code.SessionManager").GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    object value = property.GetValue(null, null);

                    str += property.Name + ":" + value + Environment.NewLine;
                }
            }
            catch
            {
                // If we log the exception here, we can get into neverending recursion. So we do nothing
            }

            return str;
        } 
        #endregion

        #region CurrentCategories
        /// <summary>
        /// current selected categories
        /// </summary>
        public static List<int> CurrentCategories
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return new List<int>();
                }
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[MONITOR_SELECTED_CATEGORIES] != null)
                {
                    return ((List<int>)HttpContext.Current.Session[MONITOR_SELECTED_CATEGORIES]);
                }
                return new List<int>();
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    return;
                }

                HttpContext.Current.Session[MONITOR_SELECTED_CATEGORIES] = value;
            }
        }
        #endregion

        #region CurrentChannels
        /// <summary>
        /// current selected channels
        /// </summary>
        public static List<int> CurrentChannels
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return new List<int>();
                }
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[MONITOR_SELECTED_CHANNELS] != null)
                {
                    return ((List<int>)HttpContext.Current.Session[MONITOR_SELECTED_CHANNELS]);
                }
                return new List<int>();
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    return;
                }

                HttpContext.Current.Session[MONITOR_SELECTED_CHANNELS] = value;
            }
        }
        #endregion

        #region PopupGridCurrentIds
        /// <summary>
        /// current selected channels
        /// </summary>
        public static int[] PopupGridCurrentIds
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return new int[0];
                }
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[POPUP_GRID_CURRENT_IDS] != null)
                {
                    return ((int[])HttpContext.Current.Session[POPUP_GRID_CURRENT_IDS]);
                }
                return new int[0];
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    return;
                }

                HttpContext.Current.Session[POPUP_GRID_CURRENT_IDS] = value;
            }
        }
        #endregion
    }
}