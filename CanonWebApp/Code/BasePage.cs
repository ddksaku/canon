using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxCallbackPanel;
using DevExpress.Web.ASPxClasses;
using Memos.Framework.Logging;
using Canon.Data;
using System.Web;
using log4net;

namespace CanonWebApp.Code
{
    /// <summary>
    /// Base class for all pages in the application.
    /// </summary>
    public class BasePage : Page
    {
        #region InitializeCulture
        /// <summary>
        /// Initialize culture.
        /// </summary>
        protected override void InitializeCulture()
        {
            base.InitializeCulture();

            Utilities.InitializeCulture();
        }
        #endregion

        #region AppVersion
        /// <summary>
        /// Version of the application. This version is saved in AssemblyInfo.
        /// </summary>
        public string AppVersion
        {
            get
            {
                return Memos.Framework.Utilities.GetVersion();
            }
        } 
        #endregion


        #region OnPreInit
        /// <summary>
        /// Set logged user's role image skin.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
        }
        #endregion

        #region OnLoad
        /// <summary>
        /// Logout if logged user is null.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (SessionManager.LoggedUser == null)
            {
                if (IsCallback)
                {
                    ASPxWebControl.RedirectOnCallback(FormsAuthentication.LoginUrl);
                }
                else
                {
                    FormsAuthentication.SignOut();
                    Response.Redirect(FormsAuthentication.LoginUrl);
                }
            }

            //Set web variables
            WebVariables.LoggedUserId = SessionManager.LoggedUser.UserId;
            WebVariables.ServerLanguage = ConfigSettings.ServerLanguage;
            WebVariables.Logger = LogManager.GetLogger(typeof(BasePage));

            base.OnLoad(e);
        }
        #endregion

        protected override void OnError(EventArgs e)
        {
            base.OnError(e);

            if (HttpContext.Current != null && HttpContext.Current.Error != null)
            {
                Logger.Log(HttpContext.Current.Error.ToString(), LogLevel.Error);
            }
        }
    }
}