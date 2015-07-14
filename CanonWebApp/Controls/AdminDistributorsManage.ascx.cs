using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Memos.Framework.Logging;
using Canon.Data;
using CanonWebApp.Code;
using Canon.Data.Business;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxGridView;
using Canon.Data.Exceptions;

namespace CanonWebApp.Controls
{
    public partial class AdminDistributorsManage : CanonPageControl
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

        #region BindData

        protected override void BindData()
        {
            CanonDataContext db = Cdb.Instance;
            gridDistributorsManage.DataSource = db.Distributors.Where(d => d.IsDeleted == false).ToList();
            gridDistributorsManage.DataBind();
        }

        #endregion

        #region DevExpress handlers

        protected void clbPanelDistributorsManage_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.Parameter))
                {
                    // remove selected distributors
                    List<object> keyValues = gridDistributorsManage.GetSelectedFieldValues("ID");
                    foreach (object key in keyValues)
                        CanonDistributor.DeleteDistributorById(int.Parse(key.ToString()));

                }

                this.BindData();
            }
            catch (ImportAssignedException iex)
            {
                clbPanelDistributorsManage.JSProperties["cpDisDeleteError"] = iex.Message;
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
            finally
            {
                gridDistributorsManage.Selection.UnselectAll();
            }
        }

        protected void cboType_Init(object sender, EventArgs e)
        {
            ASPxComboBox cbo = sender as ASPxComboBox;

            CanonDataContext db = Cdb.Instance;
            if (cbo != null)
            {
                cbo.DataSource = db.DistributorTypes.ToList();
                cbo.DataBind();
            }
        }

        protected void gridDistributorsManage_HtmlEditFormCreated(object sender, DevExpress.Web.ASPxGridView.ASPxGridViewEditFormEventArgs e)
        {
            if (!SessionManager.IsEditFormCreated)
            {
                if (!gridDistributorsManage.IsNewRowEditing)
                {
                    // initialize distributor type combo box
                    ASPxComboBox combo = (ASPxComboBox)gridDistributorsManage.FindEditFormTemplateControl("cboType");
                    if (combo != null)
                    {
                        object value = gridDistributorsManage.GetRowValues(gridDistributorsManage.EditingRowVisibleIndex, "IDDistributorType");
                        
                        foreach (ListEditItem lei in combo.Items)
                        {
                            if (lei.Value.ToString() == value.ToString())
                                combo.SelectedItem = lei;
                        }
                    }
                }

                SessionManager.IsEditFormCreated = true;
            }
        }

        protected void gridDistributorsManage_BeforeGetCallbackResult(object sender, EventArgs e)
        {
            if (gridDistributorsManage.IsNewRowEditing)
            {
                gridDistributorsManage.SettingsText.PopupEditFormCaption = "Přidání nového distributora";
                gridDistributorsManage.SettingsText.CommandUpdate = "Přidat distributora";
            }
            else
            {
                gridDistributorsManage.SettingsText.PopupEditFormCaption = "Editace distributora";
                gridDistributorsManage.SettingsText.CommandUpdate = "Uložit distributora";
            }

            SetCheckBoxVisibility("chkShowInImports", !gridDistributorsManage.IsNewRowEditing);
            SetCheckBoxVisibility("chkShowInReports", !gridDistributorsManage.IsNewRowEditing);
        }

        private void SetCheckBoxVisibility(string id, bool IsVisible)
        {
            ASPxCheckBox chk = (ASPxCheckBox)gridDistributorsManage.FindEditFormTemplateControl(id);
            if (chk != null)
            {
                chk.Visible = IsVisible;
            }
        }

        protected void gridDistributorsManage_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;

                CanonDistributor cd = new CanonDistributor();
                cd.ID = int.Parse(e.Keys[0].ToString());
                cd.FileAs = e.NewValues["FileAs"].ToString();
                cd.IDDistributorType = int.Parse(Utilities.GetEditFormComboValue(gridDistributorsManage, "cboType").ToString());
                cd.Note = e.NewValues["Note"].ToString();
                cd.ShowInImports = bool.Parse(e.NewValues["ShowInImports"].ToString());
                cd.ShowInReports = bool.Parse(e.NewValues["ShowInReports"].ToString());
                CanonDistributor.InsertOrUpdateDistributor(cd);

                e.Cancel = true;
                gridDistributorsManage.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridDistributorsManage_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;

                CanonDistributor cd = new CanonDistributor();
                cd.ID = -1;
                cd.FileAs = e.NewValues["FileAs"].ToString();
                cd.Note = e.NewValues["Note"].ToString();
                cd.IDDistributorType = int.Parse(Utilities.GetEditFormComboValue(gridDistributorsManage, "cboType").ToString());
                cd.ShowInImports = true;
                cd.ShowInReports = true;
                CanonDistributor.InsertOrUpdateDistributor(cd);

                e.Cancel = true;
                gridDistributorsManage.CancelEdit();
                this.BindData();

            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridDistributorsManage_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                CanonDistributor.DeleteDistributorById(int.Parse(e.Keys[0].ToString()));
            }
            catch (ImportAssignedException iex)
            {
                gridDistributorsManage.JSProperties["cpDisDeleteError"] = iex.Message;
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
           
            e.Cancel = true;
            gridDistributorsManage.CancelEdit();
            this.BindData();
        }

        protected void gridDistributorsManage_CancelRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            SessionManager.IsEditFormCreated = false;
        }

        protected void gridDistributorsManage_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.NewValues["FileAs"].ToString().Trim()))
                {
                    e.RowError = "Název nesmí být prázdný.";
                    return;
                }
                
                if (Utilities.GetEditFormComboValue(gridDistributorsManage, "cboType") == null)
                {
                    e.RowError = "Typ nesmí být prázdný.";
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridDistributorsManage_StartRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            SessionManager.IsEditFormCreated = false;
        }

        protected void gridDistributorsManage_HtmlRowCreated(object sender, DevExpress.Web.ASPxGridView.ASPxGridViewTableRowEventArgs e)
        {
            // edit link
            //ASPxHyperLink linkEdit = (ASPxHyperLink)
            //    gridDistributorsManage.FindRowCellTemplateControl(e.VisibleIndex,
            //        gridDistributorsManage.Columns["colCommands"] as GridViewDataColumn,
            //        "btnEdit");

            //if (linkEdit != null)
            //{
            //    linkEdit.ClientSideEvents.Click = string.Format("function(s,e){{OnEditClick({0})}}", e.VisibleIndex);
            //}

            //// delete link
            //ASPxHyperLink linkDelete = (ASPxHyperLink)
            //    gridDistributorsManage.FindRowCellTemplateControl(e.VisibleIndex,
            //        gridDistributorsManage.Columns["colCommands"] as GridViewDataColumn,
            //        "btnDelete");

            //if (linkDelete != null)
            //{
            //    object row = gridDistributorsManage.GetRow(e.VisibleIndex);
            //    if(row != null)
            //    {
            //        object name = (row as Distributor).FileAs;
            //        linkDelete.ClientSideEvents.Click = string.Format("function(s,e){{OnDeleteClick({0},{1})}}", name, e.VisibleIndex);
            //    }
            //}
        }

        #endregion
       
       
    }
}