using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Memos.Framework.Logging;
using Canon.Data;
using DevExpress.Web.ASPxEditors;

namespace CanonWebApp.Controls
{
    public partial class AdminPricesImport : CanonPageControl
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
            gridPrices.DataSource = db.Products.Where(p => p.IsActive == true);
            gridPrices.DataBind();
        }

        protected void clbPanelPricesImport_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
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