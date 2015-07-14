using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;
using Memos.Framework.Logging;
using CanonWebApp.Code;
using Canon.Data.Business;
using Canon.Data;
using Canon.Data.Exceptions;

namespace CanonWebApp.Controls
{
    public partial class AdminCategories : CanonPageControl
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
            gridCategories.Columns["colCategoryName"].Caption = Utilities.GetResourceString("Common", "CategoryName");
            gridCategories.Columns["colInternalId"].Caption = Utilities.GetResourceString("Common", "InternalId");
            (gridCategories.Columns["colCommands"] as GridViewCommandColumn).EditButton.Text =
                Utilities.GetResourceString("Common", "ChannelEditCommand");
            (gridCategories.Columns["colCommands"] as GridViewCommandColumn).DeleteButton.Text =
                Utilities.GetResourceString("Common", "ChannelDeleteCommand");

            gridCategories.SettingsText.CommandCancel = Utilities.GetResourceString("Common", "EditFormCancel");
            gridCategories.SettingsText.CommandUpdate = Utilities.GetResourceString("Common", "EditFormUpdate");
            gridCategories.SettingsText.CommandEdit = Utilities.GetResourceString("Common", "EditFormEdit");
            gridCategories.SettingsText.ConfirmDelete = Utilities.GetResourceString("Common", "EditFormConfirm");

            lblProductsAssignedExist.Text = Utilities.GetResourceString("Validators", "ProductsExistDuringCategoryDelete");
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
                                case "lblEfCategoryName":
                                    ASPxLabel label2 = (ASPxLabel)ctrl2;
                                    label2.Text = Utilities.GetResourceString("Common", "CategoryEfName");
                                    break;
                                case "lblEfInternalId":
                                    ASPxLabel label3 = (ASPxLabel)ctrl2;
                                    label3.Text = Utilities.GetResourceString("Common", "CategoryEfInternalId");
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
                gridCategories.DataSource = db.Categories;
            else
            {
                var newSource = db.Categories.Where(c => c.CategoryName.Contains(this.FilterText) ||
                                                       c.InternalId.Contains(this.FilterText));
                gridCategories.DataSource = newSource;
            }
            gridCategories.DataBind();
        }
        #endregion

        #region CallBack Panel
        /// <summary>
        /// Handler for general callback event (filtering, bulk deleting)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void clbPanelCategories_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.Parameter))
                {
                    List<object> keyValues = gridCategories.GetSelectedFieldValues("CategoryId");
                    foreach (object key in keyValues)
                        CanonCategory.DeleteCategoryById(int.Parse(key.ToString()));
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
                this.BindData();
            }
            catch (ProductAssignedException pex)
            {
                clbPanelCategories.JSProperties["cpDeleteError"] = pex.ProductGroupName;
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }
        #endregion

        #region DevExpress handlers
        protected void gridCategories_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;
                int keyToUpdate = int.Parse(e.Keys[0].ToString());
                //update category
                CanonCategory cc = new CanonCategory();
                cc.CategoryName = e.NewValues["CategoryName"].ToString();
                cc.InternalId = e.NewValues["InternalId"].ToString();
                CanonCategory.UpdateCategoryById(keyToUpdate, cc);

                e.Cancel = true;
                gridCategories.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridCategories_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;
                //insert category
                CanonCategory cc = new CanonCategory();
                cc.CategoryName = e.NewValues["CategoryName"].ToString();
                cc.InternalId = e.NewValues["InternalId"].ToString();
                CanonCategory.InsertCategory(cc);

                e.Cancel = true;
                gridCategories.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridCategories_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                try
                {
                    CanonCategory.DeleteCategoryById(int.Parse(e.Keys[0].ToString()));
                }
                catch (ProductAssignedException pex)
                {
                    gridCategories.JSProperties["cpDeleteError"] = pex.ProductGroupName;
                }
                e.Cancel = true;
                gridCategories.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridCategories_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {

            try
            {
                if (string.IsNullOrEmpty(e.NewValues["CategoryName"].ToString().Trim()))
                {
                    e.RowError = Utilities.GetResourceString("Validators", "CategoryNameMustBeSet");
                    return;
                }
                if (string.IsNullOrEmpty(e.NewValues["InternalId"].ToString().Trim()))
                {
                    e.RowError = Utilities.GetResourceString("Validators", "CategoryIdMustBeSet");
                    return;
                }
                if (CanonCategory.IsCategoryNameExist(e.NewValues["CategoryName"].ToString().Trim()))
                {
                    e.RowError = Utilities.GetResourceString("Validators", "CategoryNameAlreadyExist");
                    if (string.IsNullOrEmpty(e.RowError))
                        e.RowError = "Category name is already exist.";
                    return;
                }
                if (CanonCategory.IsCategoryCodeExist(e.NewValues["InternalId"].ToString().Trim()))
                {
                    e.RowError = Utilities.GetResourceString("Validators", "CategoryCodeAlreadyExist");
                    if (string.IsNullOrEmpty(e.RowError))
                        e.RowError = "Category code is already exist.";
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridCategories_BeforeGetCallbackResult(object sender, EventArgs e)
        {
            //Translate EditForm Header only in this place! (Devexpress issue)
            //http://devexpress.com/Support/Center/p/B96940.aspx?searchtext=IsNewRowEditing+PopupEditFormCaption&p=T4|P5|57
            //Localize New/Edit Form
            if (gridCategories.IsNewRowEditing)
                gridCategories.SettingsText.PopupEditFormCaption =
                        Utilities.GetResourceString("Common", "CategoriesEfTitleCreate");
            else
                gridCategories.SettingsText.PopupEditFormCaption =
                        Utilities.GetResourceString("Common", "CategoriesEfTitleEdit");
        }

        protected void gridCategories_CancelRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            SessionManager.IsEditFormCreated = false;
        }

        protected void gridCategories_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            this.LocalizeEditForm(e.EditForm);

            if (!SessionManager.IsEditFormCreated)
            {
                if (!gridCategories.IsNewRowEditing)
                {
                    //PLACE combo initialization if needed
                }
                SessionManager.IsEditFormCreated = true;
            }
        }
        #endregion

    }
}