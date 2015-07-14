using System;
using System.IO;
using System.Web.Security;
using System.Web.UI;
using DevExpress.Web.ASPxCallback;
using DevExpress.Web.ASPxClasses;
using CanonWebApp.Code;
using CanonWebApp.Enums;
using Canon.Data.Business;
using Canon.Data;

namespace CanonWebApp.Controls
{
    /// <summary>
    /// Pages header user control.
    /// </summary>
    public partial class MasterHeader : UserControl
    {
        #region Page_Load
        /// <summary>
        /// Set tooltips for user's role and company, company logo, open password changer popup control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            UserNameLabel.Text = SessionManager.LoggedUser.FullName;
            UserNameLabel.ToolTip = SessionManager.LoggedUser.UserName;
            UpdateActualizationStatus();

            //change
            if (CanonWebApp.Business.User.GetRightsList(SessionManager.LoggedUser.UserId).Count == 0)
            {
                AdministrationSeparatorRow.Visible = false;
                AdministrationLinkRow.Visible = false;
                LocalizationSeparatorRow.Visible = false;
                LocalizationLinkRow.Visible = false;
            }

            //if (!CanonWebApp.Business.User.GetRightsList(SessionManager.LoggedUser.UserId).ContainsKey(Canon.Data.Enums.UserRightsEnum.Localization))
            //{
            //    LocalizationSeparatorRow.Visible = false;
            //    LocalizationLinkRow.Visible = false;
            //}
        }
        #endregion

        #region OnPreRender
        /// <summary>
        /// Page permissions.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            //AdministrationSeparatorRow.Visible = AdministrationLinkRow.Visible =
            //    (SessionManager.LoggedUser.Role.InternalID == (int)RoleTypes.SystemAdministrator) ||
            //    (SessionManager.LoggedUser.Role.InternalID == (int)RoleTypes.CompanyAdministrator);

            //LocalizationSeparatorRow.Visible = LocalizationLinkRow.Visible =
            //    (SessionManager.LoggedUser.Role.InternalID == (int)RoleTypes.SystemAdministrator);

            SetSelectedMenuStyle();

            CurrentDateTimeLabel.Text = String.Format(
                "{0}, {1}",
                DateTime.Now.ToLongDateString(),
                DateTime.Now.ToShortTimeString());

            UpdateActualizationStatus();
        }
        #endregion

        #region SetSelectedMenuStyle
        /// <summary>
        /// Set selected menu style.
        /// </summary>
        private void SetSelectedMenuStyle()
        {
            string absolutePath = Request.Url.AbsolutePath;

            if (absolutePath.EndsWith("/Localization/Default.aspx", StringComparison.CurrentCultureIgnoreCase))
            {
                LocalizationLinkRow.Attributes["class"] = "menu-orange-selected";
                BorderRightDiv.Attributes["class"] = "menu-border-grey";
            }
            else if ((absolutePath.EndsWith("/Administration.aspx", StringComparison.CurrentCultureIgnoreCase))||
                (absolutePath.EndsWith("/ChannelMapping.aspx", StringComparison.CurrentCultureIgnoreCase)))
            {
                AdministrationLinkRow.Attributes["class"] = "menu-orange-selected";

                // if Administration menu is the last one
                if (!LocalizationSeparatorRow.Visible)
                {
                    BorderRightDiv.Attributes["class"] = "menu-border-grey";
                }
            }
            else
            {
                HomeLinkRow.Attributes["class"] = "menu-orange-selected";
                BorderLeftDiv.Attributes["class"] = "menu-border-grey";
            }
        } 
        #endregion


        #region LogoutLinkButton_Click
        /// <summary>
        /// Log out.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LogoutLinkButton_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            SessionManager.LoggedUser = null;
            Response.Redirect(FormsAuthentication.LoginUrl);
        }
        #endregion
        
        #region LanguageCallback_Callback
        /// <summary>
        /// Occurs when any language image was clicked.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void LanguageCallback_Callback(object source, CallbackEventArgs e)
        {
            if (String.Equals(e.Parameter, "LanguageEn", StringComparison.CurrentCultureIgnoreCase))
            {
                SessionManager.CurrentLanguage = Business.Language.SupportedLanguages[Languages.English];
            }
            else if (String.Equals(e.Parameter, "LanguageCz", StringComparison.CurrentCultureIgnoreCase))
            {
                SessionManager.CurrentLanguage = Business.Language.SupportedLanguages[Languages.Czech];
            }
            ASPxWebControl.RedirectOnCallback(Request.Url.OriginalString);
        } 
        #endregion

        #region Actualization info Callback
        protected void clbCheckIfReady_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            if (String.Equals(e.Parameter, "CheckActualization", StringComparison.CurrentCultureIgnoreCase))
                UpdateActualizationStatus();
            else if (String.Equals(e.Parameter, "GoToFinished", StringComparison.CurrentCultureIgnoreCase))
            {
                ManualImportQueue miq = CanonManualImport.GetCompleteChannelByUser(SessionManager.LoggedUser.UserId);
                if (miq != null)
                    ASPxWebControl.RedirectOnCallback(string.Format("~/ChannelMapping.aspx?channel={0}&miq={1}",
                                                      miq.ChannelId, miq.RecordId));
            }
        }
        #endregion

        protected void UpdateActualizationStatus()
        {
            ManualImportQueue miq = CanonManualImport.GetCompleteChannelByUser(SessionManager.LoggedUser.UserId);
            if (miq == null)
                imgActualizationState.Visible = false;
            else
                imgActualizationState.Visible = true;
        }
    }
}