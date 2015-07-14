using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CanonWebApp.Code;
using Memos.Framework.Logging;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;

namespace CanonWebApp.Controls
{
    public class CanonPageControl : System.Web.UI.UserControl
    {
        protected string FilterText
        {
            get
            {
                return SessionManager.FilterText;
            }
            set
            {
                SessionManager.FilterText = value;
            }
        }

        //protected string HiddenText
        //{
        //    get
        //    {
        //        return SessionManager.HiddenFormText;
        //    }
        //    set
        //    {
        //        SessionManager.HiddenFormText = value;
        //    }
        //}

        protected bool IsFilterMode
        {
            get
            {
                return SessionManager.IsFilteredPage;
            }
            set
            {
                SessionManager.IsFilteredPage = value;
            }
        }

        protected virtual void Localize(){return;}
        protected virtual void BindData(){return;}

        protected void PageLoadEvent(object sender, EventArgs e)
        {
            try
            {
                // we don't need localization in this project
                // Localize();

                if (!this.Page.IsPostBack && !this.Page.IsCallback)
                {
                    this.IsFilterMode = false;
                    this.FilterText = string.Empty;
                }
                BindData();
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }
    }
}
