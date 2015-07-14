using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CanonWebApp.Code;
using CanonWebApp.Business;
using Canon.Data.Import;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxClasses;
using Memos.Framework.Logging;
using Canon.Data.Business;
using Canon.Data;

namespace CanonWebApp.Controls
{
    public partial class AdminProducts : CanonPageControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.PageLoadEvent(sender, e);
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        #region Localization
        protected override void Localize()
        {
            popupImport.HeaderText = Utilities.GetResourceString("Headers", "UploadImportPopupForm");
            popupProductImportHistory.HeaderText = Utilities.GetResourceString("Headers", "ProductImportPopupForm");

            gridProducts.Columns["colProductName"].Caption = Utilities.GetResourceString("Common", "ProductName");
            gridProducts.Columns["colProductCode"].Caption = Utilities.GetResourceString("Common", "ProductEan");
            gridProducts.Columns["colCategory"].Caption = Utilities.GetResourceString("Common", "ProductCategorie");
            gridProducts.Columns["colCurrentPrice"].Caption = Utilities.GetResourceString("Common", "RecommendedPrice");

            (gridProducts.Columns["colCommands"] as GridViewCommandColumn).CustomButtons["btnCustomDetails"].Text =
                Utilities.GetResourceString("Common", "DetailsButton");
        }
        #endregion

        #region BindData
        /// <summary>
        /// Binding main grid based on current conditions
        /// </summary>
        protected override void BindData()
        {
            CanonDataContext db = Cdb.Instance;
            if (!this.IsFilterMode)
                gridProducts.DataSource = db.Products.Where(p=> p.IsActive == true);
            else
            {
                var newSource = db.Products.Where(c => (c.ProductName.Contains(this.FilterText) ||
                                                       c.Category.CategoryName.Contains(this.FilterText))&&
                                                       (c.IsActive == true));
                gridProducts.DataSource = newSource;
            }
            gridProducts.DataBind();
            ProductImportHistoryCtrl.Bind();
        }
        #endregion

        #region CallBack Panel
        /// <summary>
        /// Handler for general callback event (filtering, bulk deleting)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void clbPanelProducts_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                if (e.Parameter == "Search")
                {
                    if (!string.IsNullOrEmpty(txtSearchParam.Text.Trim()))
                        this.FilterText = txtSearchParam.Text;
                    this.IsFilterMode = true;
                }
                else if (e.Parameter == "ShowAll")
                {
                    this.IsFilterMode = false;
                    this.FilterText = string.Empty;
                }
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }
        #endregion

        #region Grid events
        protected void gridProducts_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
        {
            int prodId = (int)gridProducts.GetRowValues(e.VisibleIndex, new String[] { "ProductId" });
            switch (e.ButtonID)
            {
                case "btnCustomDetails":
                    ASPxWebControl.RedirectOnCallback(string.Format("~/ProductDetails.aspx?product={0}",
                                                        prodId));
                    break;
            }
        }
        #endregion

    }
}