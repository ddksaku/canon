using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;

namespace CanonWebApp.Controls
{
    public class CommandColumnHeaderTemplate : ITemplate {
        ASPxGridView gridView = null;
        string ClientName = string.Empty;
        string Id = string.Empty;
        public CommandColumnHeaderTemplate(ASPxGridView gridView, string id, string clientName) {
            this.gridView = gridView;
            this.ClientName = clientName;
            this.Id = id;
        }

        #region ITemplate Members

        public void InstantiateIn(Control container) {
            ASPxCheckBox checkbox = new ASPxCheckBox();
            container.Controls.Add(checkbox);
            checkbox.ID = this.Id;
            checkbox.ClientInstanceName = this.ClientName;
            checkbox.ClientSideEvents.CheckedChanged = 
                "function(s, e) {"+gridView.ClientInstanceName+".PerformCallback('SelectAll'+s.GetChecked());}";
        }

        #endregion
    }
}


