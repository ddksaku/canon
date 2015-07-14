using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CanonWebApp.Code;
using Canon.Data;
using Canon.Data.Business;
using Memos.Framework.Logging;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;
using Enum = Canon.Data.Enum;
using System.Web.SessionState;
using CanonWebApp.Enums;
using Canon.Data.Import;
using System.Data.SqlClient;
using System.Data;

namespace CanonWebApp.Controls
{
    public partial class AdminProductMapping : CanonPageControl
    {
        protected int? CurrentChannel
        {
            get
            {
                if (this.Request.Params["channel"] == null)
                    return null;
                string channel = this.Request.Params["channel"].ToString();
                int id;
                if (!int.TryParse(channel, out id))
                    return null;
                return id;
            }
        }

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
            if ((this.CurrentChannel == null)||(this.CurrentProduct == null))
                Response.Redirect("~/Administration.aspx");
            base.PageLoadEvent(sender, e);
        }

        #region Localization
        protected override void Localize()
        {
            gridCurrentMapping.Columns["colProductEan"].Caption = Utilities.GetResourceString("Common", "MappingEan");
            gridCurrentMapping.Columns["colProductName"].Caption = Utilities.GetResourceString("Common", "MappingName");
            gridCurrentMapping.Columns["colActual"].Caption = Utilities.GetResourceString("Common", "MappingActual");
            gridCurrentMapping.Columns["colRecommended"].Caption = Utilities.GetResourceString("Common", "MappingRecommended");

            gridPMapping.Columns["colProductName"].Caption = Utilities.GetResourceString("Common", "MappingName");
            gridPMapping.Columns["colRelevancePercent"].Caption = Utilities.GetResourceString("Common", "RelevancePercent");
            gridPMapping.Columns["colDescription"].Caption = Utilities.GetResourceString("Common", "Description");
            gridPMapping.Columns["colPriceVat"].Caption = Utilities.GetResourceString("Common", "PriceVat");

            (gridPMapping.Columns["colCommands"] as GridViewCommandColumn).CustomButtons["btnCustomChooseAndSave"].Text =
                Utilities.GetResourceString("Common", "MapAndSave");

            //get product name and channel name
            Product product = Cdb.Instance.Products.Where(p=> p.ProductId == (int)this.CurrentProduct).FirstOrDefault();
            Channel channel = Cdb.Instance.Channels.Where(p=> p.ChannelId == (int)this.CurrentChannel).FirstOrDefault();
            lblPageTitle.Text = string.Format(lblPageTitle.Text,product.ProductName, channel.ChannelName);
        }
        #endregion

        #region DataBind
        /// <summary>
        /// Binding main grid based on current conditions
        /// </summary>
        protected override void BindData()
        {
            if (!this.IsFilterMode)
                gridPMapping.DataSource = this.aspxGetAvailableForMapping(string.Empty);
            else
            {
                string filt = this.FilterText;
                gridPMapping.DataSource = this.aspxGetAvailableForMapping(filt);
            }
            gridPMapping.DataBind();
            gridPMapping.SortBy(gridPMapping.Columns[1], DevExpress.Data.ColumnSortOrder.Descending);
            //current mapping
            gridCurrentMapping.DataSource = this.aspxGetMappingByChannel();
            gridCurrentMapping.DataBind();
        }

        public DataTable aspxGetAvailableForMapping(string filter)
        {
            SqlParameter param1 = new SqlParameter();
            param1.ParameterName = "@channelId";
            param1.Value = (int)this.CurrentChannel;
            SqlParameter param2 = new SqlParameter();
            param2.ParameterName = "@productId";
            param2.Value = (int)this.CurrentProduct;

            DataSet ds = this.GetSqlDataSetByStoredProcedure("GetManualMappingProducts",
                                            param1, param2, null);

            if (ds == null)
                return null;

            //by text filter
            if ((!string.IsNullOrEmpty(filter)) && (ds.Tables.Count > 0))
            {
                DataTable dt = ds.Tables[0];
                int rowCount = dt.Rows.Count;
                for (int i = rowCount - 1; i >= 0; i--)
                {
                    string productName = dt.Rows[i]["ProductName"].ToString();
                    string productDesc = dt.Rows[i]["ProductDesc"].ToString();

                    if ((!productName.Contains(filter)) &&
                       (!productDesc.Contains(filter)))
                        dt.Rows.RemoveAt(i);
                }
                return dt;
            }
            else if (ds.Tables.Count > 0)
                return ds.Tables[0];
            return null;
        }

        private DataSet GetSqlDataSetByStoredProcedure(string storedProcedureName, SqlParameter param,
                                                        SqlParameter param2, SqlParameter param3)
        {
            try
            {
                string connectionString = Cdb.ConnectionString;
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                SqlCommand sqlCommand = new SqlCommand(storedProcedureName, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandTimeout = 30000;
                if (param != null)
                    sqlCommand.Parameters.Add(param);
                if (param2 != null)
                    sqlCommand.Parameters.Add(param2);
                if (param3 != null)
                    sqlCommand.Parameters.Add(param3);
                sqlConnection.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCommand);
                DataSet ds = new DataSet();
                sqlAdapter.Fill(ds);
                sqlConnection.Close();
                return ds;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), LogLevel.Fatal);
                return null;
            }
        }

        public List<GetMappingByChannelResult> aspxGetMappingByChannel()
        {
            ISingleResult<GetMappingByChannelResult> objresults = (ISingleResult<GetMappingByChannelResult>)
                                                        Cdb.Instance.GetMappingByChannel(this.CurrentChannel);
            List<GetMappingByChannelResult> list = objresults.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
                if (list[i].ProductId != this.CurrentProduct)
                  list.RemoveAt(i);
            return list;
        }
        #endregion

        protected void btnBackToMapping_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("~/ChannelMapping.aspx?channel={0}", this.CurrentChannel));
        }

        protected void gridPMapping_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
        {
            int relId = (int)gridPMapping.GetRowValues(e.VisibleIndex, new String[] { "RelId" });
            switch (e.ButtonID)
            {
                case "btnCustomChooseAndSave":
                    //save new mapping into DB
                    CanonMapping.AddMapping(relId);
                    ASPxWebControl.RedirectOnCallback(string.Format("~/ChannelMapping.aspx?channel={0}",
                                                        this.CurrentChannel));
                    break;
            }
        }

        #region Callback Panel
        /// <summary>
        /// Handler for general callback event (filtering, bulk mapping)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void clbPanelProductMapping_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
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

    }
}