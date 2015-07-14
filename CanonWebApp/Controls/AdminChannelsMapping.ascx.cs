using System;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CanonWebApp.Code;
using Memos.Framework.Logging;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;
using Canon.Data;
using Canon.Data.Business;
using Enum = Canon.Data.Enum;
using System.Web.SessionState;
using CanonWebApp.Enums;
using Canon.Data.Import;
using Canon.Data.Enums;
using System.Data;

namespace CanonWebApp.Controls
{
    public partial class AdminChannelsMapping : CanonPageControl
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

        protected int? ManualImportReference
        {
            get
            {
                if (this.Request.Params["miq"] == null)
                    return null;
                string channel = this.Request.Params["miq"].ToString();
                int id;
                if (!int.TryParse(channel, out id))
                    return null;
                return id;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //bind states
            if ((!Page.IsCallback) && (!Page.IsPostBack))
                BindStates();

            if (this.CurrentChannel == null)
                Response.Redirect("~/Administration.aspx");

            base.PageLoadEvent(sender, e);

            //set actualization label
            lblLastActualizationValue.Text = CanonChannelMonitor.GetLastActualizationByChannel((int)this.CurrentChannel).ToString("dd.MM.yyyy HH:mm");

            //if channel in progress - disable Actualize button
            ManualImportStatusEnum currentStatus = CanonManualImport.GetChannelImportStatus((int)this.CurrentChannel);
            if ((currentStatus == ManualImportStatusEnum.InProgress)||
                (currentStatus == ManualImportStatusEnum.WaitingInQueue))
                btnActualize.Enabled = false;
            if (this.ManualImportReference != null)
                CanonManualImport.RemoveSubscriber(SessionManager.LoggedUser.UserId, (int)this.ManualImportReference);
        }

        #region Localization

        protected override void Localize()
        {
            popupImport.HeaderText = string.Empty;
            popupActualizeStatus.HeaderText = string.Empty;
            gridMapping.Columns["colProductState"].Caption = Utilities.GetResourceString("Common", "MappingState");
            gridMapping.Columns["colProductEan"].Caption = Utilities.GetResourceString("Common", "MappingEan");
            gridMapping.Columns["colProductName"].Caption = Utilities.GetResourceString("Common", "MappingName");
            gridMapping.Columns["colActual"].Caption = Utilities.GetResourceString("Common", "MappingActual");
            gridMapping.Columns["colRecommended"].Caption = Utilities.GetResourceString("Common", "MappingRecommended");

            (gridMapping.Columns["colCommands"] as GridViewCommandColumn).CustomButtons["btnCustomMapManual"].Text =
                Utilities.GetResourceString("Common", "MappingCustomMapManual");
        }
        #endregion

        #region DataBind
        /// <summary>
        /// Binding main grid based on current conditions
        /// </summary>
        protected override void BindData()
        {
            if (!this.IsFilterMode)
                gridMapping.DataSource = this.aspxGetMappingByChannel(string.Empty);
            else
            {
                gridMapping.DataSource = this.aspxGetMappingByChannel(this.FilterText);
            }
            gridMapping.DataBind();
            gridMapping.JSProperties["cpRowsCount"] = gridMapping.VisibleRowCount;
        }

        public List<GetMappingByChannelResult> aspxGetMappingByChannel(string filter)
        {
            ISingleResult<GetMappingByChannelResult> objresults = (ISingleResult<GetMappingByChannelResult>)
                                                        Cdb.Instance.GetMappingByChannel(this.CurrentChannel);
            List<GetMappingByChannelResult> list = objresults.ToList();
            //by text filter
            if (!string.IsNullOrEmpty(filter))
            {
                for (int i = list.Count - 1; i >= 0; i--)
                    if ((!list[i].ProductEan.Contains(filter)) &&
                       (!list[i].ProductName.Contains(filter)) &&
                       ((list[i].Actual != null) ? !list[i].Actual.Contains(filter) : true) &&
                       ((list[i].Recommended != null) ? !list[i].Recommended.Contains(filter) : true))
                        list.RemoveAt(i);
            }
            //by state
            if (cbStates.SelectedIndex > 0)
            {
                int selValue = int.Parse(cbStates.SelectedItem.Value.ToString());
                for (int i = list.Count - 1; i >= 0; i--)
                    if (list[i].ProductState != selValue)
                        list.RemoveAt(i);
            }
            return list;
        }

        protected void BindStates()
        {
            cbStates.Items.Clear();
            cbStates.Items.Add(Utilities.GetResourceString("Common", "All"), 0);
            cbStates.Items.Add(Utilities.GetResourceString("Common", "ActiveState"), 1);
            cbStates.Items.Add(Utilities.GetResourceString("Common", "UnMappedState"), 2);
            cbStates.Items.Add(Utilities.GetResourceString("Common", "ExcludedState"), 3);
            cbStates.SelectedIndex = 0;
        }
        #endregion

        protected void btnBackToChannels_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administration.aspx");
        }

        #region Callback Panel
        /// <summary>
        /// Handler for general callback event (filtering, bulk mapping)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void clbPanelMapping_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                if (e.Parameter == "UnMapSelected")
                {
                    List<object> keyValues = gridMapping.GetSelectedFieldValues("ProductId");
                    foreach (object key in keyValues)
                        CanonMapping.DeleteMapping((int)this.CurrentChannel, int.Parse(key.ToString()));
                }
                else if (e.Parameter == "MapSelected")
                {
                    List<object> keyValues = gridMapping.GetSelectedFieldValues("ProductId");
                    foreach (object key in keyValues)
                        CanonMapping.AddRecommendedMapping((int)this.CurrentChannel, int.Parse(key.ToString()));
                }
                else if (e.Parameter == "Search")
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
                else if (e.Parameter == "StateChanged")
                {
                }
                else if (e.Parameter == "DoActualization")
                {
                    //Actualize button click
                    btnActualize.Enabled = false;
                    ManualImportStatusEnum status = CanonManualImport.GetChannelImportStatus((int)this.CurrentChannel);
                    if ((status == ManualImportStatusEnum.NotInQueue) ||
                        (status == ManualImportStatusEnum.ImportComplete))
                    {
                        CanonManualImport.AddChannelToQueue(SessionManager.LoggedUser.UserId,
                                                            (int)this.CurrentChannel);
                        lblActualizeMessage.Text = Utilities.GetResourceString("Common", "ActualizeMessagePostedIntoQueue");
                    }
                    else
                    {
                        ManualImportQueue elem = CanonManualImport.GetLatestQueueElement((int)this.CurrentChannel);
                        int queueElemId = elem.RecordId;
                        CanonManualImport.AddNewSubscriber(SessionManager.LoggedUser.UserId, queueElemId);
                        lblActualizeMessage.Text = string.Format(Utilities.GetResourceString("Common", "ActualizeMessageAlreadyInQueue"),
                                elem.User.FullName,
                                (SessionManager.CurrentShortLanguage == "En") ? elem.Enum.NameEn : elem.Enum.NameCz);
                    }
                    clbPanelMapping.JSProperties["cpResult"] = "OK";
                }
                else if (e.Parameter == "ExcludeSelected")
                {
                    List<object> keyValues = gridMapping.GetSelectedFieldValues("ProductId");
                    foreach (object key in keyValues)
                        CanonMapping.AddToExceptions((int)this.CurrentChannel, int.Parse(key.ToString()));
                }
                else if (e.Parameter == "IncludeSelected")
                {
                    List<object> keyValues = gridMapping.GetSelectedFieldValues("ProductId");
                    foreach (object key in keyValues)
                        CanonMapping.RemoveFromExceptions((int)this.CurrentChannel, int.Parse(key.ToString()));
                }
                else if (e.Parameter.StartsWith("LogHistory"))
                {
                    string idValue = e.Parameter.Replace("LogHistory", "");
                    int productId = int.Parse(idValue);
                    Product product = Cdb.Instance.Products.FirstOrDefault(p => p.ProductId == productId);
                    if (product != null)
                    {
                        string productName = product.ProductName;
                        popupImport.HeaderText =
                            string.Format(Utilities.GetResourceString("Headers", "MappingHistoryPopupForm"), productName);
                    }
                    MapHistoryCtrl.Parameters = new int[] { (int)this.CurrentChannel, productId };
                    MapHistoryCtrl.Bind();
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
        protected void gridMapping_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
        {
            int productId = (int)gridMapping.GetRowValues(e.VisibleIndex, new String[] { "ProductId" });
            switch (e.ButtonID)
            {
                case "btnCustomMapManual":
                    ASPxWebControl.RedirectOnCallback(string.Format("~/ProductMapping.aspx?channel={0}&product={1}", 
                                                        this.CurrentChannel, productId));
                    break;
            }
        }

        protected void gridMapping_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e)
        {
            if (e.Column.Name == "colProductState")
            {
                switch ((ProductStateEnum)int.Parse(e.Value.ToString()))
                {
                    case ProductStateEnum.Active:
                        e.DisplayText = "<img src='App_Themes/Default/Images/state-ok.png' alt='Active'>";
                        break;
                    case ProductStateEnum.Excluded:
                        e.DisplayText = "<img src='App_Themes/Default/Images/state-excluded.png' alt='Excluded from monitoring'>";
                        break;
                    case ProductStateEnum.UnMapped:
                        e.DisplayText = "<img src='App_Themes/Default/Images/state-unmapped.png' alt='Not Mapped'>";
                        break;
                    default:
                        break;
                }
            }
        }

        protected void gridMapping_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                //select all functionality
                if (e.Parameters.StartsWith("SelectAll"))
                {
                    string value = e.Parameters.Replace("SelectAll","");
                    int count = gridMapping.VisibleRowCount;
                    for (int i=0; i<count; i++)
                        if (value.ToLower() == "true")
                            gridMapping.Selection.SelectRow(i);
                        else
                            gridMapping.Selection.UnselectRow(i);
                    //check header's checkbox
                    if (value.ToLower() == "true")
                    {
                        ASPxCheckBox checkbox = (ASPxCheckBox)gridMapping.FindHeaderTemplateControl(gridMapping.Columns[0],
                                                "selCheckbox");
                        if (checkbox != null)
                            checkbox.Checked = true;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridMapping_PageIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int count = gridMapping.VisibleRowCount;
                bool allSelected = true;
                for (int i = 0; i < count; i++)
                    allSelected = allSelected && gridMapping.Selection.IsRowSelected(i);
                //check header's checkbox
                if (allSelected)
                {
                    ASPxCheckBox checkbox = (ASPxCheckBox)gridMapping.FindHeaderTemplateControl(gridMapping.Columns[0],
                                            "selCheckbox");
                    if (checkbox != null)
                        checkbox.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridMapping_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {
            ASPxHyperLink link = (ASPxHyperLink)
                gridMapping.FindRowCellTemplateControl(e.VisibleIndex, 
                                                        gridMapping.Columns["colHistoryLink"] as GridViewDataColumn, 
                                                        "hlChannelMapHistory");
            if (link != null)
            {
                link.Text = Utilities.GetResourceString("Common", "History");
                link.ClientSideEvents.Click = string.Format("function(s,e){{OnMoreInfoClick(this, {0})}}", e.KeyValue);
            }

            //Disable Manual mapping link if product state is Disabled

        }

        protected void gridMapping_HtmlCommandCellPrepared(object sender, ASPxGridViewTableCommandCellEventArgs e)
        {
            try
            {
                if ((e.CommandColumn.Name == "colCommands") &&
                    (e.CommandCellType == GridViewTableCommandCellType.Data))
                {
                    ProductStateEnum state = (ProductStateEnum)gridMapping.GetRowValues(e.VisibleIndex, new String[] { "ProductState" });
                    if (state == ProductStateEnum.Excluded)
                    {
                        //disable button
                        e.Cell.Controls[0].Visible = false;
                        Label lbl = new Label();
                        lbl.Text = "&nbsp;";
                        e.Cell.Controls.Add(lbl);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), LogLevel.Error);
            }
        }
        #endregion

    }
}