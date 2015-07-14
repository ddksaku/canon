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
using Canon.Data.Exceptions;

namespace CanonWebApp.Controls
{
    public partial class AdminGroups : CanonPageControl
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

        protected override void BindData()
        {
            CanonDataContext db = Cdb.Instance;
            gridProductGroups.DataSource = db.ProductGroups.ToList();
            gridProductGroups.DataBind();
            
            gridResellerGroups.DataSource = db.ResellerGroups.Where(rg => rg.IsDeleted == false).ToList();
            gridResellerGroups.DataBind();

        }

        #region DevExpress Handler for ProductGroups

        protected void clbPanelGroups_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                if (e.Parameter == "RemoveSelectedProductGroups")
                {
                    List<object> keyValues = gridProductGroups.GetSelectedFieldValues("ID");
                    foreach (object key in keyValues)
                        CanonProductGroup.DeleteProductGroupById(int.Parse(key.ToString()));
                }
                else if (e.Parameter == "RemoveSelectedResellerGroups")
                {
                    List<object> keyValues = this.gridResellerGroups.GetSelectedFieldValues("ID");
                    foreach (object key in keyValues)
                        CanonResellerGroup.DeleteResellerGroupById(int.Parse(key.ToString()));
                }

                this.BindData();
            }
            catch (ProductAssignedException pex)
            {
                clbPanelGroups.JSProperties["cpPGDeleteError"] = pex.Message;
            }
            catch (ResellerAssignedException rex)
            {
                clbPanelGroups.JSProperties["cpRGDeleteError"] = rex.Message;
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
            finally
            {
                gridProductGroups.Selection.UnselectAll();
                gridResellerGroups.Selection.UnselectAll();
            }
        }

        protected void gridProductGroups_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                try
                {
                    CanonProductGroup.DeleteProductGroupById(int.Parse(e.Keys[0].ToString()));
                }
                catch (ProductAssignedException pex)
                {
                    gridProductGroups.JSProperties["cpPGDeleteError"] = pex.Message;
                }

                e.Cancel = true;
                this.gridProductGroups.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridProductGroups_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;

                CanonProductGroup cpg = new CanonProductGroup();
                cpg.ID = -1;
                cpg.FileAs = e.NewValues["FileAs"].ToString();
                cpg.Code = e.NewValues["Code"].ToString();
                CanonProductGroup.InsertOrUpdateProductGroup(cpg);

                e.Cancel = true;
                gridProductGroups.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridProductGroups_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;

                CanonProductGroup cpg = new CanonProductGroup();
                cpg.ID = int.Parse(e.Keys[0].ToString());
                cpg.FileAs = e.NewValues["FileAs"].ToString();
                cpg.Code = e.NewValues["Code"].ToString();
                CanonProductGroup.InsertOrUpdateProductGroup(cpg);

                e.Cancel = true;
                gridProductGroups.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridProductGroups_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.NewValues["FileAs"].ToString().Trim()))
                {
                    e.RowError = "Název nesmí být prázdný.";
                }

                if (string.IsNullOrEmpty(e.NewValues["Code"].ToString().Trim()))
                {
                    e.RowError = "Identifikátor nesmí být prázdný.";
                }

                if (e.NewValues["Code"] != null && CanonProductGroup.ExistAlreadyIdentifier(e.NewValues["Code"].ToString(), Convert.ToInt32(e.Keys[0])) == true)
                {
                    e.RowError = string.Format("Identifikátor '{0}' už existuje.", e.NewValues["Code"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridProductGroups_CancelRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            SessionManager.IsEditFormCreated = false;
        }

        protected void gridProductGroups_StartRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            SessionManager.IsEditFormCreated = false;
        }

        protected void gridProductGroups_HtmlRowCreated(object sender, DevExpress.Web.ASPxGridView.ASPxGridViewTableRowEventArgs e)
        {

        }

        protected void gridProductGroups_HtmlEditFormCreated(object sender, DevExpress.Web.ASPxGridView.ASPxGridViewEditFormEventArgs e)
        {
            
        }

        protected void gridProductGroups_BeforeGetCallbackResult(object sender, EventArgs e)
        {
            if (gridProductGroups.IsNewRowEditing)
            {
                gridProductGroups.SettingsText.PopupEditFormCaption = "Nová produktová skupina";
                gridProductGroups.SettingsText.CommandUpdate = "Vytvořit skupinu";
            }
            else
            {
                gridProductGroups.SettingsText.PopupEditFormCaption = "Editace produktové skupiny";
                gridProductGroups.SettingsText.CommandUpdate = "Uložit změny";
            }
        }

        protected void clbPanelGroups_CustomJSProperties(object sender, DevExpress.Web.ASPxClasses.CustomJSPropertiesEventArgs e)
        {

        }

        #endregion

        #region DevExpress handlers for Reseller Groups

        protected void gridResellerGroups_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                try
                {
                    CanonResellerGroup.DeleteResellerGroupById(int.Parse(e.Keys[0].ToString()));
                }
                catch (ResellerAssignedException rex)
                {
                    gridResellerGroups.JSProperties["cpRGDeleteError"] = rex.Message;
                }

                e.Cancel = true;
                gridResellerGroups.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridResellerGroups_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;

                CanonResellerGroup crg = new CanonResellerGroup();
                crg.ID = -1;
                crg.FileAs = e.NewValues["FileAs"].ToString();
                crg.Code = e.NewValues["Code"].ToString();
                CanonResellerGroup.InsertOrUpdateResellerGroup(crg);

                e.Cancel = true;
                gridResellerGroups.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridResellerGroups_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                SessionManager.IsEditFormCreated = false;

                CanonResellerGroup crg = new CanonResellerGroup();
                crg.ID = int.Parse(e.Keys[0].ToString());
                crg.FileAs = e.NewValues["FileAs"].ToString();
                crg.Code = e.NewValues["Code"].ToString();
                CanonResellerGroup.InsertOrUpdateResellerGroup(crg);

                e.Cancel = true;
                this.gridResellerGroups.CancelEdit();
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridResellerGroups_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.NewValues["FileAs"].ToString().Trim()))
                {
                    e.RowError = "Název nesmí být prázdný.";
                }

                if (string.IsNullOrEmpty(e.NewValues["Code"].ToString().Trim()))
                {
                    e.RowError = "Identifikátor nesmí být prázdný.";
                }

                if (e.NewValues["Code"] != null && CanonResellerGroup.ExistAlreadyIdentifier(e.NewValues["Code"].ToString(), Convert.ToInt32(e.Keys[0])))
                {
                    e.RowError = string.Format("Identifikátor '{0}' už existuje.", e.NewValues["Code"].ToString());
                }

            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridResellerGroups_StartRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            SessionManager.IsEditFormCreated = false;
        }

        protected void gridResellerGroups_CancelRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            SessionManager.IsEditFormCreated = false;
        }

        protected void gridResellerGroups_HtmlEditFormCreated(object sender, DevExpress.Web.ASPxGridView.ASPxGridViewEditFormEventArgs e)
        {

        }

        protected void gridResellerGroups_HtmlRowCreated(object sender, DevExpress.Web.ASPxGridView.ASPxGridViewTableRowEventArgs e)
        {

        }

        protected void gridResellerGroups_BeforeGetCallbackResult(object sender, EventArgs e)
        {
            if (gridResellerGroups.IsNewRowEditing)
            {
                gridResellerGroups.SettingsText.PopupEditFormCaption = "Nová skupina reselerů";
                gridResellerGroups.SettingsText.CommandUpdate = "Vytvořit skupinu";
            }
            else
            {
                gridResellerGroups.SettingsText.PopupEditFormCaption = "Editace skupiny reselerů";
                gridResellerGroups.SettingsText.CommandUpdate = "Uložit změny";
            }
        }

        #endregion
    }
}