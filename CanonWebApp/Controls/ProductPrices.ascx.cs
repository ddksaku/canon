using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CanonWebApp.Code;
using Canon.Data;
using Canon.Data.Business;
using Memos.Framework.Logging;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxDataView;
using DevExpress.Web.ASPxEditors;
using Enum = Canon.Data.Enum;
using System.Web.SessionState;
using CanonWebApp.Enums;
using Canon.Data.Import;

namespace CanonWebApp.Controls
{
    public partial class ProductPrices : CanonPageControl
    {
        protected int? CurrentProduct
        {
            get
            {
                if (this.Request.Params["product"] == null)
                    return null;
                string channel = this.Request.Params["product"].ToString();
                int id;
                if (!int.TryParse(channel, out id))
                    return null;
                return id;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.PageLoadEvent(sender, e);
        }

        #region Localization
        protected override void Localize()
        {
            popupRecomHistory.HeaderText = Utilities.GetResourceString("Headers", "RecommendedHistoryPopupForm");
            gridPriceHistory.Columns["colPriceDate"].Caption = Utilities.GetResourceString("Common", "PriceDate");
            gridPriceHistory.Columns["colPriceValue"].Caption = Utilities.GetResourceString("Common", "PriceValue");
            gridPriceHistory.SettingsText.CommandCancel = Utilities.GetResourceString("Common", "EditFormCancel");
            gridPriceHistory.SettingsText.CommandUpdate = Utilities.GetResourceString("Common", "EditFormUpdate");
            gridPriceHistory.SettingsText.CommandEdit = Utilities.GetResourceString("Common", "EditFormEdit");
        }
        #endregion

        #region BindData
        /// <summary>
        /// Binding main grid based on current conditions
        /// </summary>
        protected override void BindData()
        {
            CanonDataContext db = Cdb.Instance;
            gridPriceHistory.DataSource = db.RecommendedPrices.OrderByDescending(p=> p.ChangeDate).Where(
                                                    m => m.ProductId == this.CurrentProduct);
            gridPriceHistory.DataBind();
            RecommendedHistoryCtrl.Parameters = new int[] { (int)this.CurrentProduct };
            RecommendedHistoryCtrl.Bind();
        }
        #endregion

        #region DevExpress handlers
        protected void gridPriceHistory_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;
                int keyToUpdate = int.Parse(e.Keys[0].ToString());
                decimal newPrice = 0;
                if (e.NewValues["Price"] != null)
                {
                    newPrice = (decimal)e.NewValues["Price"];
                    CanonProduct.UpdateRecommendedPrice(keyToUpdate, newPrice);
                }
                e.Cancel = true;
                gridPriceHistory.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }

        }

        protected void CallbackHistoryPanel_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            RecommendedHistoryCtrl.Bind();
        }
        #endregion
    }
}