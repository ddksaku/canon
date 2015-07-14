using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
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
using DevExpress.Web.Data;
using System.Text.RegularExpressions;

namespace CanonWebApp.Controls
{
    public partial class AdminChannels : CanonPageControl
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
                (gridChannels.Columns["colChannelType"] as GridViewDataTextColumn).FieldName =
                                 string.Format("Enum.Name{0}", SessionManager.CurrentShortLanguage);
                base.PageLoadEvent(sender, e);
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        #region Localization
        protected void LocalizeEditForm(WebControl editForm)
        {
            try
            {
                //In the following place FindEditFormTemplateControl returns always null. devexpress bug?
                foreach (Control ctrl in editForm.Controls)
                {
                    if (ctrl is GridViewEditFormTemplateContainer)
                    {
                        foreach (Control ctrl2 in ctrl.Controls)
                        {
                            switch (ctrl2.ID)
                            {
                                case "lblEfChannelInfo":
                                    ASPxLabel label2 = (ASPxLabel)ctrl2;
                                    label2.Text = Utilities.GetResourceString("Common", "ChannelEfHeader");
                                    break;
                                case "lblEfName":
                                    ASPxLabel label3 = (ASPxLabel)ctrl2;
                                    label3.Text = Utilities.GetResourceString("Common", "ChannelName");
                                    break;
                                case "lblEfType":
                                    ASPxLabel label4 = (ASPxLabel)ctrl2;
                                    label4.Text = Utilities.GetResourceString("Common", "ChannelType");
                                    break;
                                case "lblEfInfoType":
                                    ASPxLabel label5 = (ASPxLabel)ctrl2;
                                    label5.Text = Utilities.GetResourceString("Common", "ChannelInfoType");
                                    break;
                                case "lblEfUrl":
                                    ASPxLabel label6 = (ASPxLabel)ctrl2;
                                    label6.Text = Utilities.GetResourceString("Common", "ChannelUrl");
                                    break;
                                case "lblReportingTo":
                                    ASPxLabel label7 = (ASPxLabel)ctrl2;
                                    label7.Text = Utilities.GetResourceString("Common", "ChannelEfReportingTo");
                                    break;
                                case "chbEfActive":
                                    ASPxCheckBox checkbox1 = (ASPxCheckBox)ctrl2;
                                    checkbox1.Text = Utilities.GetResourceString("Common", "ChannelEfActive");
                                    break;
                                default:
                                    continue;
                            }//--switch
                        }//--foreach search controls
                    }//--if template container
                }//--foreach, search template
            }
            catch (Exception)
            {
            }
        }

        protected override void Localize()
        {
            popupChannelHistory.HeaderText = string.Empty;

            gridChannels.Columns["colChannelName"].Caption = Utilities.GetResourceString("Common", "ChannelName");
            gridChannels.Columns["colChannelType"].Caption = Utilities.GetResourceString("Common", "ChannelType");
            gridChannels.Columns["colChannelUrl"].Caption = Utilities.GetResourceString("Common", "ChannelUrl");
            gridChannels.Columns["colChannelReportingTo"].Caption = Utilities.GetResourceString("Common", "ChannelReportingTo");
            gridChannels.Columns["colChannelActive"].Caption = Utilities.GetResourceString("Common", "ChannelIsActive");
            (gridChannels.Columns["colCommands"] as GridViewCommandColumn).EditButton.Text =
                Utilities.GetResourceString("Common", "ChannelEditCommand");
            (gridChannels.Columns["colCommands"] as GridViewCommandColumn).DeleteButton.Text =
                Utilities.GetResourceString("Common", "ChannelDeleteCommand");
            (gridChannels.Columns["colCommands"] as GridViewCommandColumn).CustomButtons["btnCustomMap"].Text =
                Utilities.GetResourceString("Common", "ChannelMapCommand");

            gridChannels.SettingsText.CommandCancel = Utilities.GetResourceString("Common", "EditFormCancel");
            gridChannels.SettingsText.CommandUpdate = Utilities.GetResourceString("Common", "EditFormUpdate");
            gridChannels.SettingsText.CommandEdit = Utilities.GetResourceString("Common", "EditFormEdit");
            gridChannels.SettingsText.ConfirmDelete = Utilities.GetResourceString("Common", "EditFormConfirm");
        }
        #endregion

        #region DataBind
        /// <summary>
        /// Binding main grid based on current conditions
        /// </summary>
        protected override void BindData()
        {
            CanonDataContext db = Cdb.Instance;
            if (!this.IsFilterMode)
                gridChannels.DataSource = db.Channels;
            else
            {
                var newSource = db.Channels.Where(c => c.ChannelName.Contains(this.FilterText) ||
                                                       c.Url.Contains(this.FilterText));
                gridChannels.DataSource = newSource;
            }
            gridChannels.DataBind();
        }
        #endregion

        #region Callback Panel
        /// <summary>
        /// Handler for general callback event (filtering, bulk deleting)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void clbPanelChannels_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.Parameter))
                {
                    List<object> keyValues = gridChannels.GetSelectedFieldValues("ChannelId");
                    foreach (object key in keyValues)
                        CanonChannel.DeleteChannelById(int.Parse(key.ToString()));
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
                else if (e.Parameter.StartsWith("ChannelHistory"))
                {
                    string idValue = e.Parameter.Replace("ChannelHistory", "");
                    int channelId = int.Parse(idValue);
                    Channel channel = Cdb.Instance.Channels.FirstOrDefault(p => p.ChannelId == channelId);
                    if (channel != null)
                    {
                        string channelName = channel.ChannelName;
                        popupChannelHistory.HeaderText =
                            string.Format(Utilities.GetResourceString("Headers", "ChannelsHistoryPopupForm"), channelName);
                    }
                    ChannelHistoryCtrl.Parameters = new int[] { channelId };
                    SessionManager.PopupGridCurrentIds = ChannelHistoryCtrl.Parameters;
                    ChannelHistoryCtrl.Bind();
                }
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }
        #endregion

        protected void cmbEfType_Init(object sender, EventArgs e) 
        {
            ASPxComboBox cbo = sender as ASPxComboBox;
            this.FillComboWithEnumValues(cbo, 3);
        }

        protected void cmbEfInfoType_Init(object sender, EventArgs e)
        {
            ASPxComboBox cbo = sender as ASPxComboBox;
            this.FillComboWithEnumValues(cbo, 2);
        }

        protected void FillComboWithEnumValues(ASPxComboBox cbo, int enumType)
        {
            CanonDataContext db = Cdb.Instance;
            if (cbo != null)
            {
                cbo.Items.Clear();
                List<Enum> list = db.Enums.Where(e => e.EnumType == enumType).ToList();
                foreach (Enum en in list)
                    cbo.Items.Add(((SessionManager.CurrentShortLanguage == "En") ? en.NameEn : en.NameCz), en.EnumId);
            }
        }

        #region DevExpress handlers
        protected void gridChannels_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;
                int keyToUpdate = int.Parse(e.Keys[0].ToString());
                int newChannelType = int.Parse(Utilities.GetEditFormComboValue(gridChannels, "cmbEfType").ToString());
                int newInfoType = int.Parse(Utilities.GetEditFormComboValue(gridChannels, "cmbEfInfoType").ToString());
                if ((newChannelType != 0) && (newInfoType != 0))
                {
                    //update channel
                    CanonChannel cc = new CanonChannel();
                    cc.ChannelName = e.NewValues["ChannelName"].ToString();
                    cc.ChannelType = newChannelType;
                    cc.InfoType = newInfoType;
                    if (e.NewValues["IsActive"] != null)
                        cc.IsActive = (bool)e.NewValues["IsActive"];
                    else
                        cc.IsActive = false;
                    cc.Url = e.NewValues["Url"].ToString();
                    cc.ReportingTo = e.NewValues["ReportingTo"].ToString();
                    CanonChannel.UpdateChannelById(keyToUpdate, cc);
                }

                e.Cancel = true;
                gridChannels.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridChannels_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;
                int newChannelType = int.Parse(Utilities.GetEditFormComboValue(gridChannels, "cmbEfType").ToString());
                int newInfoType = int.Parse(Utilities.GetEditFormComboValue(gridChannels, "cmbEfInfoType").ToString());
                if ((newChannelType != 0) && (newInfoType != 0))
                {
                    //update channel
                    CanonChannel cc = new CanonChannel();
                    cc.ChannelName = e.NewValues["ChannelName"].ToString();
                    cc.ChannelType = newChannelType;
                    cc.InfoType = newInfoType;
                    if (e.NewValues["IsActive"] != null)
                        cc.IsActive = (bool)e.NewValues["IsActive"];
                    else
                        cc.IsActive = false;
                    cc.Url = e.NewValues["Url"].ToString();
                    cc.ReportingTo = e.NewValues["ReportingTo"].ToString();
                    CanonChannel.InsertChannel(cc);
                }

                e.Cancel = true;
                gridChannels.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridChannels_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                CanonChannel.DeleteChannelById(int.Parse(e.Keys[0].ToString()));
                e.Cancel = true;
                gridChannels.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridChannels_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {

            try
            {
                if (string.IsNullOrEmpty(e.NewValues["ChannelName"].ToString().Trim()))
                {
                    e.RowError = Utilities.GetResourceString("Validators", "ChannelNameMustBeSet");
                    return;
                }
                if (string.IsNullOrEmpty(e.NewValues["Url"].ToString().Trim()))
                {
                    e.RowError = Utilities.GetResourceString("Validators", "ChannelUrlMustBeSet");
                    return;
                }
                if (Utilities.GetEditFormComboValue(gridChannels, "cmbEfType") == null)
                {
                    e.RowError = Utilities.GetResourceString("Validators", "ChannelTypeMustBeSet");
                    return;
                }
                if (Utilities.GetEditFormComboValue(gridChannels, "cmbEfInfoType") == null)
                {
                    e.RowError = Utilities.GetResourceString("Validators", "ChannelInfoTypeMustBeSet");
                    return;
                }
                string allEmails = e.NewValues["ReportingTo"].ToString().Trim();
                string[] emails = allEmails.Split(';');
                foreach (string email in emails)
                {
                    if (string.IsNullOrEmpty(email))
                        continue;
                    if (!Regex.IsMatch(email, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}"))
                    {
                        e.RowError = Utilities.GetResourceString("Validators", "ChannelEmailsAreNotValid");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridChannels_BeforeGetCallbackResult(object sender, EventArgs e)
        {
            //Translate EditForm Header only in this place! (Devexpress issue)
            //http://devexpress.com/Support/Center/p/B96940.aspx?searchtext=IsNewRowEditing+PopupEditFormCaption&p=T4|P5|57
            //Localize New/Edit Form
            if (gridChannels.IsNewRowEditing)
            {
                gridChannels.SettingsText.PopupEditFormCaption =
                        Utilities.GetResourceString("Common", "ChannelEfTitleCreate");
            }
            else
                gridChannels.SettingsText.PopupEditFormCaption =
                        Utilities.GetResourceString("Common", "ChannelEfTitleEdit");
        }

        protected void gridChannels_CancelRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            SessionManager.IsEditFormCreated = false;
        }
        
        protected void gridChannels_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            this.LocalizeEditForm(e.EditForm);

            if (!SessionManager.IsEditFormCreated)
            {
                if (!gridChannels.IsNewRowEditing)
                {
                    //initialize channel type combobox
                    ASPxComboBox cbo = (ASPxComboBox)gridChannels.FindEditFormTemplateControl("cmbEfType");
                    if (cbo != null)
                    {
                        object value = gridChannels.GetRowValues(gridChannels.EditingRowVisibleIndex, new String[] { "ChannelType" });
                        foreach (ListEditItem lei in cbo.Items)
                            if (lei.Value.ToString() == value.ToString())
                                cbo.SelectedItem = lei;
                    }
                    //initialize info type combobox
                    ASPxComboBox cbo2 = (ASPxComboBox)gridChannels.FindEditFormTemplateControl("cmbEfInfoType");
                    if (cbo2 != null)
                    {
                        object value = gridChannels.GetRowValues(gridChannels.EditingRowVisibleIndex, new String[] { "InfoType" });
                        foreach (ListEditItem lei in cbo2.Items)
                            if (lei.Value.ToString() == value.ToString())
                                cbo2.SelectedItem = lei;
                    }
                }
                SessionManager.IsEditFormCreated = true;
            }
        }

        protected void gridChannels_StartRowEditing(object sender, ASPxStartRowEditingEventArgs e)
        {
            SessionManager.IsEditFormCreated = false;
        }

        protected void gridChannels_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
        {
            int channelId = (int)gridChannels.GetRowValues(e.VisibleIndex, new String[] { "ChannelId" });
            switch (e.ButtonID)
            {
                case "btnCustomMap":
                    ASPxWebControl.RedirectOnCallback(string.Format("~/ChannelMapping.aspx?channel={0}", channelId));
                    break;
            }
        }

        protected void gridChannels_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {
            ASPxHyperLink link = (ASPxHyperLink)
                gridChannels.FindRowCellTemplateControl(e.VisibleIndex,
                                                        gridChannels.Columns["colHistoryLink"] as GridViewDataColumn,
                                                        "hlChannelHistory");
            if (link != null)
            {
                link.Text = Utilities.GetResourceString("Common", "History");
                link.ClientSideEvents.Click = string.Format("function(s,e){{OnChannelHistoryClick(this, {0})}}", e.KeyValue);
            }
        }

        #endregion

    }
}