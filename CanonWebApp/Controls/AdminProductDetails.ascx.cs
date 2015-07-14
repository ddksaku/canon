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
    public partial class AdminProductDetails : CanonPageControl
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
            if (this.CurrentProduct == null)
                Response.Redirect("~/Administration.aspx");

            (gridDetails.Columns["colChannelType"] as GridViewDataTextColumn).FieldName =
                 string.Format("Channel.Enum.Name{0}", SessionManager.CurrentShortLanguage);

            base.PageLoadEvent(sender, e);

            //get product name
            Product product = Cdb.Instance.Products.Where(p => p.ProductId == (int)this.CurrentProduct).FirstOrDefault();
            lblPageTitle.Text = string.Format(lblPageTitle.Text, product.ProductName);
        }

        protected void btnBackToProducts_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administration.aspx");
        }

        #region Localization
        protected override void Localize()
        {
            gridDetails.Columns["colChannelName"].Caption = Utilities.GetResourceString("Common", "ChannelName");
            gridDetails.Columns["colChannelType"].Caption = Utilities.GetResourceString("Common", "ChannelType");
            gridDetails.Columns["colProductName"].Caption = Utilities.GetResourceString("Common", "ProductName");
            gridDetails.Columns["colPriceVat"].Caption = Utilities.GetResourceString("Common", "PriceVat");
        }
        protected void LocalizeDataView(ASPxDataView view)
        {
            try
            {
                string[] idToTranslate = new string[] { "lblGeneralTitle", "lblProductName", "lblProductEan", 
                                                        "lblCategory", "lblRecommendedPrice" };
                DataViewItem item = view.Items[0];
                foreach (string id in idToTranslate)
                {
                    ASPxLabel ctrl = (ASPxLabel)view.FindItemControl(id, item);
                    if (ctrl != null)
                        ctrl.Text = Utilities.GetResourceString("Common", "Details" + id.Replace("lbl",""));
                }
            }
            catch (Exception)
            {
            }
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
                gridDetails.DataSource = db.MappingRules.Where(m=> m.ProductId==this.CurrentProduct);
            else
            {
                var newSource = db.MappingRules.Where(m => m.ProductId == this.CurrentProduct &&
                                                       (m.MonitoredName.Contains(this.FilterText)||
                                                       (m.Channel.ChannelName.Contains(this.FilterText))));
                gridDetails.DataSource = newSource;
            }
            gridDetails.DataBind();

            //bind general dataview
            var products = db.Products.Where(p => p.ProductId == this.CurrentProduct);
            dvGeneralInfo.DataSource = products;
            dvGeneralInfo.DataBind();
            this.LocalizeDataView(dvGeneralInfo);
        }
        #endregion

        #region CallBack Panel
        /// <summary>
        /// Handler for general callback event (filtering, bulk deleting)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void clbPanelDetails_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
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
        protected void gridDetails_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "CurrentPrice")
            {
                int id = e.ListSourceRowIndex;
                int channelId = Convert.ToInt32(e.GetListSourceFieldValue("ChannelId"));
                string monitoredName = e.GetListSourceFieldValue("MonitoredName").ToString();
                string monitoredUrl = e.GetListSourceFieldValue("MonitoredUrl").ToString();
                e.Value = CanonMapping.GetCurrentMapPrice(channelId, monitoredName, monitoredUrl);
            }
        }
        #endregion
    }
}