using System;
using System.Text;
using System.Web;

namespace Memos.Framework
{
    /// <summary>
    /// Session manager.
    /// </summary>
    public class SessionManager
    {
        #region UserActions
        /// <summary>
        /// Contains all actions that user did during his session (button clicks, dropdown item selection
        /// </summary>
        public static string UserActions
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null &&
                    HttpContext.Current.Session["UserActions"] != null)
                {
                    return (HttpContext.Current.Session["UserActions"].ToString());
                }

                return String.Empty;
            }
            set
            {
                string prefix = "";
                if (String.IsNullOrEmpty(UserActions) == false)
                {
                    prefix = "->";
                }

                HttpContext.Current.Session["UserActions"] += prefix + value;
            }
        } 
        #endregion

        #region ToString
        /// <summary>
        /// Returns a string, that contains all object saved in Session and its values.
        /// </summary>
        /// <returns></returns>
        public new static string ToString()
        {
            StringBuilder str = new StringBuilder();

            if (HttpContext.Current == null)
            {
                return String.Empty;
            }

            try
            {
                int i = 0;

                foreach (object sessionObject in HttpContext.Current.Session)
                {
                    str.AppendLine(sessionObject + ":" + HttpContext.Current.Session[i]);

                    i++;
                }
            }
            catch
            {
                // If we log the exception here, we can get into neverending recursion, because it is used by logger. So we do nothing
            }

            return str.ToString();
        }
        #endregion
    }
}