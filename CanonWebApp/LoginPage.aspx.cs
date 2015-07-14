using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxPopupControl;
using CanonWebApp.Code;
using Memos.Framework.Logging;
using Canon.Data;
using Canon.Data.Business;
using Enum = Canon.Data.Enum;
using System.Web.SessionState;
using CanonWebApp.Enums;
using Canon.Data.Import;
using Canon.Data.Enums;
using Memos.Import.Xml;

namespace CanonWebApp
{
    public partial class LoginPage : System.Web.UI.Page
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

        #region Page_Load
        /// <summary>
        /// Localizes the page and set user's login from cookies.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Logger.Log("Session started", LogLevel.Info);
            Localize();

            if (IsPostBack || IsCallback)
            {
                return;
            }

            ASPxTextBox UserName = (ASPxTextBox)LoginControl.FindControl("UserName");
            ASPxTextBox Password = (ASPxTextBox)LoginControl.FindControl("Password");
            ASPxCheckBox RememberMeCheckBox = (ASPxCheckBox)LoginControl.FindControl("RememberMeCheckBox");

            if (Request.Cookies["username"] != null)
            {
                UserName.Text = Request.Cookies["username"].Value;
                RememberMeCheckBox.Checked = true;
                Password.Focus();
            }
            else
            {
                RememberMeCheckBox.Checked = false;
                UserName.Focus();
            }
        }
        #endregion

        #region Localize
        /// <summary>
        /// Localizes text depending on selected language.
        /// </summary>
        private void Localize()
        {
            ASPxTextBox UserName = (ASPxTextBox)LoginControl.FindControl("UserName");
            ASPxTextBox Password = (ASPxTextBox)LoginControl.FindControl("Password");
            UserName.ValidationSettings.RequiredField.ErrorText = Utilities.GetResourceString("Validators", "UserNameRequiredValidator");
            Password.ValidationSettings.RequiredField.ErrorText = Utilities.GetResourceString("Validators", "PasswordRequiredValidator");

            ASPxPopupControl ForgotPopupControl = (ASPxPopupControl)LoginControl.FindControl("ForgotPopupControl");
            ForgotPopupControl.HeaderText = Utilities.GetResourceString("Headers", "RemindPassword");

            ASPxTextBox EmailTextBox = (ASPxTextBox)LoginControl.FindControl("ForgotPopupControl").FindControl("ForgotCallbackPanel").FindControl("EmailTextBox");
            EmailTextBox.ValidationSettings.RequiredField.ErrorText = Utilities.GetResourceString("Validators", "EmailRequiredValidator");
            EmailTextBox.ValidationSettings.RegularExpression.ErrorText = Utilities.GetResourceString("Validators", "EmailFormatValidator");
        }
        #endregion


        #region LoginControl_LoggedIn
        /// <summary>
        /// Save cookies and set current language.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LoginControl_LoggedIn(object sender, EventArgs e)
        {
            // Set cookies
            bool isRememberMeChecked = ((ASPxCheckBox)LoginControl.FindControl("RememberMeCheckBox")).Checked;
            string username = ((ASPxTextBox)LoginControl.FindControl("UserName")).Text;

            if (isRememberMeChecked)
            {
                HttpCookie cookie = new HttpCookie("username", username);
                cookie.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Add(cookie);
            }
            else
            {
                // Set cookies expired
                if (Request.Cookies["username"] != null)
                {
                    Response.Cookies["username"].Expires = DateTime.Now;
                }
            }
        }
        #endregion

        #region LoginControl_LoginError
        /// <summary>
        /// Show error lable and clear password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LoginControl_LoginError(object sender, EventArgs e)
        {
            LoginErrorLabel.Visible = true;
            ((ASPxTextBox)LoginControl.FindControl("Password")).Text = String.Empty;
        }
        #endregion

        #region ForgotCallbackPanel_Callback
        /// <summary>
        /// Show success or error messages depends on password reminding.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void ForgotCallbackPanel_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                //FIXME: this is testing code for feed import, you could remove it when app in live
                //CommonParser<CanonChannelMonitor> export = ImportFactory.GetParser(
                //                   Canon.Data.Enums.ChannelTypeEnum.XML,
                //                   23,
                //                   @"d:\Memos\Projects\Canon\CanonMonitor\CanonWebApp\Uploaded\21_2009-08-19.xml",
                //                   Server.MapPath("Uploaded"), string.Empty);
                //export.Logger = WebVariables.Logger;
                //if (!export.ExportToDb()) return;
                
                //SimpleXmlParser parser = new SimpleXmlParser(new System.IO.FileInfo(@"d:\Memos\Projects\Canon\CanonMonitor\CanonWebApp\Uploaded\21_2009-08-19.xml"));
                //List<Dictionary<string, string>> parsed = parser.Parse();
                //return;

                if (String.Equals(e.Parameter, "Remind", StringComparison.CurrentCultureIgnoreCase))
                {
                    ASPxTextBox EmailTextBox = (ASPxTextBox)LoginControl.FindControl("ForgotPopupControl").FindControl("ForgotCallbackPanel").FindControl("EmailTextBox");
                    HtmlTableRow SuccessMessageDiv = (HtmlTableRow)LoginControl.FindControl("ForgotPopupControl").FindControl("ForgotCallbackPanel").FindControl("SuccessMessageDiv");
                    HtmlTableRow ErrorMessageDiv = (HtmlTableRow)LoginControl.FindControl("ForgotPopupControl").FindControl("ForgotCallbackPanel").FindControl("ErrorMessageDiv");

                    bool isSent = Business.User.SendRemindPasswordEmail(EmailTextBox.Text);

                    SuccessMessageDiv.Visible = isSent;
                    ErrorMessageDiv.Visible = !isSent;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), LogLevel.Fatal);
            }
        }
        #endregion
    }
}
