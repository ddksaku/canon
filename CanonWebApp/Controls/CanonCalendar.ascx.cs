using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxEditors;

namespace CanonWebApp.Controls
{
    public partial class CanonCalendar : System.Web.UI.UserControl
    {
        public string ParentPanel
        {
            get;set;
        }

        public DateTime Date
        {
            get { return deMainDate.Date; }
            set { deMainDate.Date = value; }
        }

        public DateTime MaxDate
        {
            get { return deMainDate.MaxDate; }
            set { deMainDate.MaxDate = value; }
        }

        public DateEditCalendarProperties CalendarProperties
        {
            get { return deMainDate.CalendarProperties; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            deMainDate.ClientInstanceName = string.Format("{0}_cin",deMainDate.ClientID);
            string callbackCommand = string.Empty;
            if (!string.IsNullOrEmpty(this.ParentPanel))
                callbackCommand = string.Format("{0}.PerformCallback('DateChanged' + dd.GetDate());", this.ParentPanel);

            if (!string.IsNullOrEmpty(callbackCommand))
                deMainDate.ClientSideEvents.DateChanged = string.Format("function(s,e){{ var dd=s; {0} }}",
                                                                        callbackCommand);
            imgNextDay.ClientSideEvents.Click = string.Format(
                "function(s,e){{ var tt = {0}.GetDate(); if (tt == null) return; var myDate = new Date(); myDate.setFullYear(tt.getFullYear(),tt.getMonth(),tt.getDate()); myDate.setDate(myDate.getDate() + 1); var tomorrow = new Date(); tomorrow.setDate(tomorrow.getDate() + 1); if ((myDate.getDate() >= tomorrow.getDate())&&(myDate.getMonth() >= tomorrow.getMonth())) return; {0}.SetDate(myDate); var dd={0}; {1} }}",
                deMainDate.ClientInstanceName, callbackCommand);
            imgPreviousDay.ClientSideEvents.Click = string.Format(
                "function(s,e){{ var tt = {0}.GetDate(); if (tt == null) return; var myDate2 = new Date(); myDate2.setFullYear(tt.getFullYear(),tt.getMonth(),tt.getDate()); myDate2.setDate(myDate2.getDate() - 1); {0}.SetDate(myDate2); var dd={0}; {1} }}",
                deMainDate.ClientInstanceName, callbackCommand);
        }
    }
}