using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Memos.Framework.Logging;
using Canon.Data;
using DevExpress.Web.ASPxGridView.Localization;
using DevExpress.Utils.Localization.Internal;
using Canon.Data.Business;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxGridView;

namespace CanonWebApp.Controls
{
    public partial class AdminDistributorsImport : CanonPageControl
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
            gridDistributors.DataSource = db.ImportDistributors.Where(d => d.Succeeded == true);
            gridDistributors.DataBind();
        }

        protected void btnRemove_Init(object sender, EventArgs e)
        {
            ((LinkButton)sender).Attributes.Add("onClick", "return confirm('Are you sure you want to do this?','a','b');");
        }

        protected void gridDistributors_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            if (e.Parameters != null && e.Parameters.StartsWith("RemoveDistributor"))
            {
                string value = e.Parameters.Replace("RemoveDistributor","");
                int ID;
                if (int.TryParse(value, out ID) == true)
                {
                    CanonImportDistributor.RemoveImportDistributor(ID);
                    this.BindData();
                }
            }
        }

        protected void gridDistributors_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {
            ASPxHyperLink link = (ASPxHyperLink)
                gridDistributors.FindRowCellTemplateControl(e.VisibleIndex, 
                    gridDistributors.Columns["colRemove"] as GridViewDataColumn,
                    "hlRemoveDistributor");

            if (link != null)
            {
                link.ClientSideEvents.Click = string.Format("function(s,e){{OnRemoveDistributorClick(this,{0})}}", e.KeyValue);
            }                            
        }

        protected void clbPanelDistributorsImport_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.Parameter) == false)
                {
                    if (e.Parameter == "RefreshDistributors")
                    {
                        importControl.RefreshComboBoxes();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void clbPanelDistributorsGrid_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.Parameter) == false)
                {
                    if (e.Parameter == "RefreshAfterImport")
                    {
                        BindData();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }
    }

}