using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxGridView;
using CanonWebApp.Code;
using Memos.Framework.Logging;
using Canon.Data;
using System.Data.Linq;
using DevExpress.Web.ASPxEditors;
using Canon.Data.Enums;

namespace CanonWebApp.Controls
{
    public partial class AdminLog : CanonPageControl
    {
        /// <summary>
        /// Main Page_Load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //initialize controls
                if ((!this.Page.IsCallback) && (!this.Page.IsPostBack))
                {
                    deLogDate.Date = DateTime.Now.Date;
                    deLogDate.MaxDate = DateTime.Now;
                    BindCombo();
                }

                (gridMainLog.Columns["colChannelType"] as GridViewDataTextColumn).FieldName =
                                 string.Format("ChannelType{0}", SessionManager.CurrentShortLanguage);
                (gridMainLog.Columns["colLogState"] as GridViewDataTextColumn).FieldName =
                                 string.Format("Name{0}", SessionManager.CurrentShortLanguage);

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
            gridMainLog.Columns["colChannelName"].Caption = Utilities.GetResourceString("Common", "ChannelName");
            gridMainLog.Columns["colChannelType"].Caption = Utilities.GetResourceString("Common", "ChannelType");
            gridMainLog.Columns["colLogState"].Caption = Utilities.GetResourceString("Common", "ChannelLogState");
            deLogDate.CalendarProperties.TodayButtonText = Utilities.GetResourceString("Common", "Today");
        }
        #endregion

        #region DataBind
        /// <summary>
        /// Binding main grid based on current conditions
        /// </summary>
        protected override void BindData()
        {
            CanonDataContext db = Cdb.Instance;
            int selectedValue = int.Parse(cbState.SelectedItem.Value.ToString());
            if ( selectedValue == 0)
            {
                ISingleResult<GetMainLogByDateResult> objs = (ISingleResult<GetMainLogByDateResult>)
                                                                db.GetMainLogByDate(deLogDate.Date.Date, 0);
                List<GetMainLogByDateResult> list = objs.ToList();
                gridMainLog.DataSource = list;
            }
            else
            {
                ISingleResult<GetMainLogByDateResult> objs = (ISingleResult<GetMainLogByDateResult>)
                                                                db.GetMainLogByDate(deLogDate.Date.Date,
                                                                selectedValue);
                List<GetMainLogByDateResult> list = objs.ToList();
                gridMainLog.DataSource = list;
            }
            gridMainLog.DataBind();
        }

        protected void BindCombo()
        {
            int logStatusEnumId = 5;
            CanonDataContext db = Cdb.Instance;
            var enums = db.Enums.Where(e => e.EnumType == logStatusEnumId);
            string defaultValue = Utilities.GetResourceString("Common", "ImportLogComboDefault");
            cbState.Items.Clear();
            ListEditItem def = new ListEditItem(defaultValue, 0);
            cbState.Items.Add(def);
            foreach(Canon.Data.Enum en in enums)
                cbState.Items.Add(new ListEditItem(
                            (SessionManager.CurrentShortLanguage=="En")?en.NameEn:en.NameCz, en.EnumId));
            cbState.SelectedItem = def;
        }
        #endregion

        #region Callback Panel
        /// <summary>
        /// Handler for general callback event (filtering, bulk deleting)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void clbPanelLog_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                if (e.Parameter.StartsWith("MainLogErrors"))
                {
                    MainLogErrorsCtrl.Visible = true;
                    string idValue = e.Parameter.Replace("MainLogErrors", "");
                    int logId = int.Parse(idValue);
                    ImportLog il = Cdb.Instance.ImportLogs.FirstOrDefault(l=> l.LogId==logId);
                    if (il != null)
                    {
                        Channel channel = Cdb.Instance.Channels.FirstOrDefault(p => p.ChannelId == il.ChannelId);
                        string channelName = channel.ChannelName;
                        popupLogStatus.HeaderText =
                            string.Format(Utilities.GetResourceString("Headers", "MainLogErrorPopupForm"), 
                                                channelName, il.LogDate.ToString("dd.MM.yyyy"));
                    }
                    MainLogErrorsCtrl.Parameters = new int[] { logId };
                    int returned = MainLogErrorsCtrl.Bind();
                    if (returned == 0)
                        MainLogErrorsCtrl.Visible = false;

                    //labels update
                    lblTryedAmount.Text = il.Tryed.ToString();
                    lblSuccessAmount.Text = il.Success.ToString();
                }
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }
        #endregion

        protected void gridMainLog_HtmlRowPrepared(object sender, DevExpress.Web.ASPxGridView.ASPxGridViewTableRowEventArgs e)
        {
            if (e.RowType != DevExpress.Web.ASPxGridView.GridViewRowType.Data) return;
            ChannelLogEnum value = (ChannelLogEnum)e.GetValue("LogType");
            if (value == ChannelLogEnum.ChannelError)
                e.Row.BackColor = System.Drawing.Color.Red;
            else if (value == ChannelLogEnum.ProductError)
                e.Row.BackColor = System.Drawing.Color.Orange;
        }

        protected void gridMainLog_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {
            ASPxHyperLink link1 = (ASPxHyperLink)
                gridMainLog.FindRowCellTemplateControl(e.VisibleIndex,
                                                        gridMainLog.Columns["colHistoryLink"] as GridViewDataColumn,
                                                        "hlLogHistory");
            if (link1 != null)
            {
                link1.Text = Utilities.GetResourceString("Common", "DetailsButton");
                link1.ClientSideEvents.Click = string.Format("function(s,e){{OnMainHistoryErrorsClick(this, {0})}}", e.KeyValue);
            }

            ASPxHyperLink link2 = (ASPxHyperLink)
                gridMainLog.FindRowCellTemplateControl(e.VisibleIndex,
                                                        gridMainLog.Columns["colToChannelLink"] as GridViewDataColumn,
                                                        "hlToChannelLink");
            if (link2 != null)
            {
                link2.Text = Utilities.GetResourceString("Common", "GoToChannel");
                link2.NavigateUrl = string.Format("~/ChannelMapping.aspx?channel={0}", 
                    gridMainLog.GetRowValues(e.VisibleIndex, new String[] {"ChannelId"}));
            }
        }

    }
}