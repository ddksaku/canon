using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Memos.Framework.Logging;
using Canon.Data;

namespace CanonWebApp.Controls
{
    public partial class AdminResellersImport : CanonPageControl
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

        protected override void OnInit(EventArgs e)
        {
            this.importControl.ParentPopupClientID = this.popupImport.ClientInstanceName;

            base.OnInit(e);
        }

        protected override void BindData()
        {
            CanonDataContext db = Cdb.Instance;
            gridResellers.DataSource = db.Resellers.Where(r=>r.ResellerGroup.IsDeleted == false).ToList();
            gridResellers.DataBind();
        }

        protected void clbPanelResellersImport_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            try
            {
                this.BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }
    }
}