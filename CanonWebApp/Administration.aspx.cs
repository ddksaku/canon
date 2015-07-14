using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses;
using CanonWebApp.Code;
using Canon.Data.Enums;

namespace CanonWebApp
{
    public partial class Administration : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // we don't need localization in this project
            // Localize();

            if ((!Page.IsCallback)&&(!Page.IsPostBack))
                ShowTabsBasedOnPermissions();
        }

        #region Localize
        /// <summary>
        /// Localizes text depending on selected language.
        /// </summary>
        private void Localize()
        {
            //AdministrationPageControl.TabPages[0].Text = Utilities.GetResourceString("Headers", "StatusLog");
            //AdministrationPageControl.TabPages[1].Text = Utilities.GetResourceString("Headers", "Channels");
            //AdministrationPageControl.TabPages[2].Text = Utilities.GetResourceString("Headers", "Products");
            //AdministrationPageControl.TabPages[3].Text = Utilities.GetResourceString("Headers", "Categories");
            //AdministrationPageControl.TabPages[4].Text = Utilities.GetResourceString("Headers", "Users");
        }
        #endregion

        #region AdministrationPageCallbackPanel_Callback
        /// <summary>
        /// Bind and show/hide controls in the active tab.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void AdministrationPageCallbackPanel_Callback(object source, CallbackEventArgsBase e)
        {
            BindTabControls(AdministrationPageControl.ActiveTabIndex);
            ShowHideTabControls(AdministrationPageControl.ActiveTabIndex);
        }
        #endregion

        #region BindTabControls
        /// <summary>
        /// Bind controls in the active tab.
        /// </summary>
        /// <param name="activeTab"></param>
        protected void BindTabControls(int activeTab)
        {
            switch (activeTab)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }
        #endregion

        #region ShowHideTabControls
        /// <summary>
        /// Show/hide controls in the active tab.
        /// </summary>
        /// <param name="activeTab"></param>
        protected void ShowHideTabControls(int activeTab)
        {
            this.AdminDistributorsCtrl.Visible = activeTab == 0;
            this.AdminPricesCtrl.Visible = activeTab == 1;
            this.AdminResellersCtrl.Visible = activeTab == 2;
            this.AdminDistributorsManageCtrl.Visible = activeTab == 3;
            this.AdminGroupsCtrl.Visible = activeTab == 4;
            this.AdminUsersCtrl.Visible = activeTab == 5;
        }
        #endregion

        #region ShowTabsBasedOnPermissions
        /// <summary>
        /// Hides tabs based on users permissions
        /// </summary>
        protected void ShowTabsBasedOnPermissions()
        {
            Dictionary<UserRightsEnum, string> rights = CanonWebApp.Business.User.GetRightsList(
                                                        SessionManager.LoggedUser.UserId);
            if (!rights.ContainsKey(UserRightsEnum.DistributorsImport))
                AdministrationPageControl.TabPages.FindByName("ImportDistributors").Visible = false;
            if (!rights.ContainsKey(UserRightsEnum.PricesImport))
                AdministrationPageControl.TabPages.FindByName("ImportPrices").Visible = false;
            if (!rights.ContainsKey(UserRightsEnum.ResellersImport))
                AdministrationPageControl.TabPages.FindByName("ImportResellerGroups").Visible = false;
            if (!rights.ContainsKey(UserRightsEnum.Distributors))
                AdministrationPageControl.TabPages.FindByName("DistributorsManage").Visible = false;
            if (!rights.ContainsKey(UserRightsEnum.Groups))
                AdministrationPageControl.TabPages.FindByName("Groups").Visible = false;
            if (!rights.ContainsKey(UserRightsEnum.Users))
                AdministrationPageControl.TabPages.FindByName("Users").Visible = false;
        }
        #endregion

    }
}
