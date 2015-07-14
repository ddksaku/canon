using System;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Canon.Data;
using Canon.Data.Enums;
using CanonWebApp;
using CanonWebApp.Code;
using CanonWebApp.Business;
using Memos.Framework.Logging;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;
using BuUser = CanonWebApp.Business.User;
using User = Canon.Data.User;
using DevExpress.Web.ASPxPopupControl;

namespace CanonWebApp.Controls
{
    public partial class AdminUsers : CanonPageControl
    {
        protected bool EditFormHasErrors = false;
        protected string SelectedCategories = string.Empty;
        protected string SelectedRights = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;
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
            gridUsers.Columns["colUsername"].Caption = Utilities.GetResourceString("Common", "Username");
            gridUsers.Columns["colFullName"].Caption = Utilities.GetResourceString("Common", "FullName");
            gridUsers.Columns["colEmail"].Caption = Utilities.GetResourceString("Common", "Email");
            (gridUsers.Columns["colCommands"] as GridViewCommandColumn).EditButton.Text =
                Utilities.GetResourceString("Common", "ChannelEditCommand");
            (gridUsers.Columns["colCommands"] as GridViewCommandColumn).DeleteButton.Text =
                Utilities.GetResourceString("Common", "ChannelDeleteCommand");

            gridUsers.SettingsText.CommandCancel = Utilities.GetResourceString("Common", "EditFormCancel");
            gridUsers.SettingsText.CommandUpdate = Utilities.GetResourceString("Common", "EditFormUpdate");
            gridUsers.SettingsText.CommandEdit = Utilities.GetResourceString("Common", "EditFormEdit");
            gridUsers.SettingsText.ConfirmDelete = Utilities.GetResourceString("Common", "EditFormConfirm");
        }

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
                                case "lblEfGeneralInfo":
                                    ASPxLabel label1 = (ASPxLabel)ctrl2;
                                    label1.Text = Utilities.GetResourceString("Common", "UsersEfGeneralInfo");
                                    break;
                                case "lblEfCategories":
                                    ASPxLabel label2 = (ASPxLabel)ctrl2;
                                    label2.Text = Utilities.GetResourceString("Common", "UsersEfCategories");
                                    break;
                                case "lblEfRights":
                                    ASPxLabel label3 = (ASPxLabel)ctrl2;
                                    label3.Text = Utilities.GetResourceString("Common", "UsersEfRights");
                                    break;
                                case "lblEfUsername":
                                    ASPxLabel label4 = (ASPxLabel)ctrl2;
                                    label4.Text = Utilities.GetResourceString("Common", "UsersEfUsername");
                                    break;
                                case "chbEfStatusLogRights":
                                    ASPxCheckBox chkbox1 = (ASPxCheckBox)ctrl2;
                                    chkbox1.Text = Utilities.GetResourceString("Common", "UsersEfStatusLogRights");
                                    break;
                                case "lblEfFullName":
                                    ASPxLabel label5 = (ASPxLabel)ctrl2;
                                    label5.Text = Utilities.GetResourceString("Common", "UsersEfFullName");
                                    break;
                                case "chbEfChannelsRights":
                                    ASPxCheckBox chkbox2 = (ASPxCheckBox)ctrl2;
                                    chkbox2.Text = Utilities.GetResourceString("Common", "UsersEfChannelsRights");
                                    break;
                                case "lblEfEmail":
                                    ASPxLabel label6 = (ASPxLabel)ctrl2;
                                    label6.Text = Utilities.GetResourceString("Common", "UsersEfEmail");
                                    break;
                                case "chbEfProductsRights":
                                    ASPxCheckBox chkbox3 = (ASPxCheckBox)ctrl2;
                                    chkbox3.Text = Utilities.GetResourceString("Common", "UsersEfProductsRights");
                                    break;
                                case "lblEfPassword":
                                    ASPxLabel label7 = (ASPxLabel)ctrl2;
                                    label7.Text = Utilities.GetResourceString("Common", "UsersEfPassword");
                                    break;
                                case "chbEfCategoriesRights":
                                    ASPxCheckBox chkbox4 = (ASPxCheckBox)ctrl2;
                                    chkbox4.Text = Utilities.GetResourceString("Common", "UsersEfCategoriesRights");
                                    break;
                                case "chbEfSendDailyLog":
                                    ASPxCheckBox chkbox5 = (ASPxCheckBox)ctrl2;
                                    chkbox5.Text = Utilities.GetResourceString("Common", "UsersEfDailyLog");
                                    break;
                                case "chbEfUsersRights":
                                    ASPxCheckBox chkbox6 = (ASPxCheckBox)ctrl2;
                                    chkbox6.Text = Utilities.GetResourceString("Common", "UsersEfUsersRights");
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
        #endregion

        #region BindData
        /// <summary>
        /// Binding main grid based on current conditions
        /// </summary>
        protected override void BindData()
        {
            CanonDataContext db = Cdb.Instance;
            if (!this.IsFilterMode)
                gridUsers.DataSource = db.Users.Where(u=> u.IsForbidden != true);
            else
            {
                var newSource = db.Users.Where(c => (c.FullName.Contains(this.FilterText) ||
                                                       c.UserName.Contains(this.FilterText)||
                                                       c.Email.Contains(this.FilterText)) &&
                                                       c.IsForbidden != true);
                gridUsers.DataSource = newSource;
            }
            gridUsers.DataBind();
        }
        #endregion

        #region CallBack Panel
        /// <summary>
        /// Handler for general callback event (filtering, bulk deleting)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void clbPanelUsers_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.Parameter))
                {
                    List<object> keyValues = gridUsers.GetSelectedFieldValues("UserId");
                    foreach (object key in keyValues)
                        BuUser.DeleteUserById(int.Parse(key.ToString()));
                }
                
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }
        #endregion

        protected Canon.Data.User CollectNewDataFromForm(int key, OrderedDictionary oldValues, 
                                              OrderedDictionary newValues, bool isNew)
        {
            Canon.Data.User cc = new Canon.Data.User();
            if (!isNew)
                cc.UserId = key;
            cc.UserName = newValues["UserName"].ToString();
            cc.FullName = newValues["FullName"].ToString();
            cc.Email = newValues["Email"].ToString();
            if (!isNew)
            {
                string oldPassword = oldValues["Password"].ToString();
                string newPassword = newValues["Password"].ToString();
                if (oldPassword != newPassword)
                    cc.Password = newValues["Password"].ToString();
            }
            else
            {
                cc.Password = newValues["Password"].ToString();
            }

            // is active
            if (newValues["IsActive"] != null)
                cc.IsActive = (bool)newValues["IsActive"];
            else
                cc.IsActive = false;

            return cc;
        }

        #region DevExpress handlers
        protected void gridUsers_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;
                int keyToUpdate = int.Parse(e.Keys[0].ToString());
                //update user
                User cc = this.CollectNewDataFromForm(keyToUpdate, e.OldValues, e.NewValues, false);
                BuUser.UpdateUser(cc, e.OldValues["UserName"].ToString());

                //update rights
                List<int> selectedRights = GetRightsFromList();
                foreach(KeyValuePair<int,int> pair in SessionManager.CheckBoxListHelper)
                    BuUser.UpdateRightsRelation(keyToUpdate, pair.Value, selectedRights.Contains(pair.Key));

                e.Cancel = true;
                gridUsers.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridUsers_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;
                //insert user
                User cc = this.CollectNewDataFromForm(0, null, e.NewValues, true);
                int insertedId = BuUser.InsertUser(cc);

                //update rights
                List<int> selectedRights = GetRightsFromList();
                foreach (KeyValuePair<int, int> pair in SessionManager.CheckBoxListHelper)
                    BuUser.UpdateRightsRelation(insertedId, pair.Value, selectedRights.Contains(pair.Key));

                e.Cancel = true;
                gridUsers.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridUsers_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                BuUser.DeleteUserById(int.Parse(e.Keys[0].ToString()));
                e.Cancel = true;
                gridUsers.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridUsers_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {

            try
            {
                ASPxTextBox txtb = (ASPxTextBox)gridUsers.FindEditFormTemplateControl("txtHidden");
                if (txtb != null)
                    this.SelectedCategories = txtb.Text;

                //username is set
                if (string.IsNullOrEmpty(e.NewValues["UserName"].ToString().Trim()))
                {
                    e.RowError = Utilities.GetResourceString("Validators", "UserNameMustBeSet");
                    EditFormHasErrors = true;
                    return;
                }
                //full name is set
                if (string.IsNullOrEmpty(e.NewValues["FullName"].ToString().Trim()))
                {
                    e.RowError = Utilities.GetResourceString("Validators", "UserFullNameMustBeSet");
                    EditFormHasErrors = true;
                    return;
                }
                //email is set
                //email is optional now, but if IsDailyEmail set, email must be defined
                if ((e.NewValues.Contains("IsDailyEmail"))&&(e.NewValues["IsDailyEmail"] != null))
                  if ((bool)e.NewValues["IsDailyEmail"])
                    if (string.IsNullOrEmpty(e.NewValues["Email"].ToString().Trim()))
                    {
                        e.RowError = Utilities.GetResourceString("Validators", "UserEmailMustBeSet");
                        EditFormHasErrors = true;
                        return;
                    }
                //password is set
                if (string.IsNullOrEmpty(e.NewValues["Password"].ToString().Trim()))
                {
                    e.RowError = Utilities.GetResourceString("Validators", "UserPasswordMustBeSet");
                    EditFormHasErrors = true;
                    return;
                }
                //username is unique
                //email is unique
                string newUsername = e.NewValues["UserName"].ToString();
                string newEmail = (e.NewValues.Contains("Email")) ? e.NewValues["Email"].ToString() : string.Empty;
                if (gridUsers.IsNewRowEditing)
                {
                    CanonDataContext db = Cdb.Instance;
                    int userFound = db.Users.Count(u => (u.UserName == newUsername)&&
                                                        (u.IsForbidden != true));
                    if (!string.IsNullOrEmpty(newEmail))
                        userFound += db.Users.Count(u => (u.Email == newEmail)&&(u.IsForbidden != true));

                    if (userFound > 0)
                    {
                        e.RowError = Utilities.GetResourceString("Validators", "UserNameAndEmailMustBeUnique");
                        EditFormHasErrors = true;
                        return;
                    }
                }
                //email is in correct format
                if (!string.IsNullOrEmpty(e.NewValues["Email"].ToString().Trim()))
                    if (!Regex.IsMatch(newEmail, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}"))
                    {
                        e.RowError = Utilities.GetResourceString("Validators", "UserEmailMustBeInCorrectFormat");
                        EditFormHasErrors = true;
                        return;
                    }
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridUsers_BeforeGetCallbackResult(object sender, EventArgs e)
        {
            //Translate EditForm Header only in this place! (Devexpress issue)
            //http://devexpress.com/Support/Center/p/B96940.aspx?searchtext=IsNewRowEditing+PopupEditFormCaption&p=T4|P5|57
            //Localize New/Edit Form
          
            if (gridUsers.IsNewRowEditing)
            {
                gridUsers.SettingsText.PopupEditFormCaption = "Nový uživatel";

                // active checkbox
                Control ctrl = gridUsers.FindEditFormTemplateControl("chkIsActive");
                if (ctrl != null && (ctrl as ASPxCheckBox) != null)
                {
                    (ctrl as ASPxCheckBox).Checked = true;
                }
            }
            else
                gridUsers.SettingsText.PopupEditFormCaption = "Editace uživatele";

            SetChangePasswordVisibility(gridUsers.IsNewRowEditing);
        }

        private void SetChangePasswordVisibility(bool IsVisible)
        {
            try
            {
                ASPxButton btnChange = gridUsers.FindEditFormTemplateControl("btnChangePassword") as ASPxButton;
                ASPxTextBox txtPassword = gridUsers.FindEditFormTemplateControl("txtEfPassword") as ASPxTextBox;
                ASPxLabel lblPassword = gridUsers.FindEditFormTemplateControl("lblEfPassword") as ASPxLabel;

                if(btnChange != null)
                    btnChange.Visible = !IsVisible;
                if(txtPassword != null)
                    txtPassword.Visible = IsVisible;
                if (lblPassword != null)
                    lblPassword.Visible = IsVisible;
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridUsers_CancelRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            SessionManager.IsEditFormCreated = false;
        }

        protected void gridUsers_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            // this.LocalizeEditForm(e.EditForm);

            ASPxTextBox username = (ASPxTextBox)gridUsers.FindEditFormTemplateControl("txtEfUsername");
            if (username != null)
                username.Focus();

            if ((!SessionManager.IsEditFormCreated) || (EditFormHasErrors))
            {
                CanonDataContext db = Cdb.Instance;

                //init user rights checkbox list
                CheckBoxList cbl = (CheckBoxList)gridUsers.FindEditFormTemplateControl("chblUserRights");
                if (cbl != null)
                {
                    int counter = 0;
                    cbl.Items.Clear();
                    SessionManager.CheckBoxListHelper = new Dictionary<int, int>(5);
                    Dictionary<UserRightsEnum, string> list = BuUser.GetAllRightsList();
                    foreach (KeyValuePair<UserRightsEnum, string> pair in list)
                    {
                        int pairKey = (int)pair.Key;
                        cbl.Items.Add(new ListItem(pair.Value, pairKey.ToString()));
                        SessionManager.CheckBoxListHelper.Add(counter, pairKey);
                        counter++;
                    }
                }

                //initialize listbox and checkboxlist if anything exists
                if ((!gridUsers.IsNewRowEditing) && (!EditFormHasErrors))
                {
                    //combo/list initialization if needed
                    object value = gridUsers.GetRowValues(gridUsers.EditingRowVisibleIndex, new String[] { "UserId" });
                    int idToEdit = int.Parse(value.ToString());

                    //checkbox list init
                    if (cbl != null)
                    {
                        Dictionary<UserRightsEnum, string> list = BuUser.GetRightsList(idToEdit);
                        foreach (ListItem li in cbl.Items)
                            foreach (KeyValuePair<UserRightsEnum, string> pair in list)
                                if (li.Value == ((int)pair.Key).ToString())
                                    li.Selected = true;
                    }
                }
                if (EditFormHasErrors)
                {
                    //rights
                    if (cbl != null)
                    {
                        string[] values = SelectedRights.Split(';');
                        foreach (string val in values)
                        {
                            string[] item = val.Split('=');
                            if (item.Length != 2) continue;
                            foreach (ListItem li in cbl.Items)
                                if ((li.Value == item[0]) && (item[1] == "1"))
                                    li.Selected = true;
                        }
                    }
                }
                
                Control popupCtrl = gridUsers.FindEditFormTemplateControl("popupChangePassword");
                if (popupCtrl != null)
                {
                    Control changeCtrl = popupCtrl.FindControl("changePasswordCtrl");
                    if (changeCtrl != null && changeCtrl is AdminChangePassword)
                    {
                        object row = gridUsers.GetRow(gridUsers.EditingRowVisibleIndex);
                        if(row != null && row is User)
                        {
                            int userID = (row as User).UserId;

                            (changeCtrl as AdminChangePassword).SetUserId(userID);
                        }
                    }
                }

                SessionManager.IsEditFormCreated = true;
            }
        }
        #endregion

        protected void chblUserRights_Init(object sender, EventArgs e)
        {
            (sender as CheckBoxList).Attributes.Add("onchange", "UpdateRightsList(this);");
        }

        private List<int> GetRightsFromList()
        {
            List<int> selectedRights = new List<int>(5);
            CheckBoxList list = (CheckBoxList)gridUsers.FindEditFormTemplateControl("chblUserRights");
            if (!gridUsers.IsEditing) return selectedRights;
            foreach (string key in Request.Params.AllKeys)
            {
                if (key.StartsWith(list.UniqueID))
                {
                    string strSel = key.Replace(list.UniqueID, string.Empty).Replace("$", string.Empty);
                    selectedRights.Add(int.Parse(strSel));
                }
            }
            return selectedRights;
        }

        protected void gridUsers_StartRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            SessionManager.IsEditFormCreated = false;
        }

        protected void gridUsers_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
        }


    }
}