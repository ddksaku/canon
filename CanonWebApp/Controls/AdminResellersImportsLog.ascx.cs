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
    public partial class AdminResellersImportsLog : CanonPageControl
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

            // logss for the last 30 days
            DateTime date = DateTime.Now.AddDays(-30);
            gridResellersImportsLog.DataSource = db.ImportResellers.Where(r => r.Succeeded == true && r.DateImported >= date).OrderByDescending(r=>r.DateImported).ToList();
            gridResellersImportsLog.DataBind();
        }
    }
}