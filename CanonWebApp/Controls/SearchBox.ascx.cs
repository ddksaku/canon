using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.IO;

namespace CanonWebApp.Controls
{
    public partial class SearchBox : System.Web.UI.UserControl
    {

        private DataTable _results = new DataTable();
        public bool Display = true;
        public string Class = "";

        /// <summary>
        /// Full list of potencial values.
        /// </summary>
        public DataTable Results
        {
            get
            {
                return _results;
            }
        }

        public void AddResults(DataTable dt)
        {
            _results = dt.Copy();
            //foreach(DataRow dr in dt.Rows)
            //{
            //    _results.ImportRow(dr);
            //}
        }

        string values;

        public List<string> Value
        {
            get
            {
                if (hf.Value.Length < 2)
                {
                    return new List<string>();
                }
                return hf.Value.Substring(1).Substring(0, hf.Value.Length - 2).Split('|').ToList<string>();
            }
        }

        string arrayname;
        string arrayname2;
        string arr;



        protected override void OnPreRender(EventArgs e)
        {
            values = "";
            //foreach (string ss in Value)
            //{
            //    values = values + ss + "|";
            //}
            values = values.Length == 0 ? "" : values.Substring(0, values.Length - 1);

            txtKeyword.Value = "";

            if (!((VictoryPage)Page).IsAjax)
            {
                arrayname = this.ClientID + "Texts";
                arrayname2 = this.ClientID + "Values";
                arr = "arr_" + canvas.ClientID;
                txtKeyword.Attributes.Add("onkeyup", string.Format("SearchArray({0}, {1}, this, '{2}', {3})", arrayname, arrayname2, canvas.ClientID, arr));

                StringBuilder js = new StringBuilder();
                js.AppendLine("<script language=\"javascript\">");
                js.AppendLine("var " + arrayname + ";");
                js.AppendLine("var " + arrayname2 + ";");
                js.AppendLine(string.Format("var " + arr + " = '|{0}';", values == "" ? "" : values + "|"));
                js.AppendLine("XBrowserAddHandler(window, 'load', function() {");


                string path = Request.PhysicalApplicationPath + "ListBoxData.js";
               // if (!File.Exists(path))
                if(Application["JBR"] != "1")
                {
                    StreamWriter sw;
                    if (!File.Exists(path))
                    {
                        sw = File.CreateText(path);
                    }
                    else
                    {
                        sw = File.AppendText(path);
                    }

                    string s = "";
                    foreach (DataRow dr in _results.Rows)
                    {
                        s += "'" + dr["Text"].ToString().Replace("'", "_") + "'";
                        s += ",";
                    }
                    s = s.Length > 0 ? s.Substring(0, s.Length - 1) : "";

                    sw.WriteLine(arrayname + " = new Array(" + s + ");");

                    string s2 = "";
                    foreach (DataRow dr in _results.Rows)
                    {
                        s2 += "'" + dr["Value"].ToString().Replace("'", "_") + "'";
                        s2 += ",";
                    }
                    s2 = s2.Length > 0 ? s2.Substring(0, s2.Length - 1) : "";
                    sw.WriteLine(arrayname2 + " = new Array(" + s2 + ");");

                    sw.Flush();
                    sw.Close();
                }

                js.AppendLine(string.Format("InitSearchBox({0}, {1}, {4}, '{2}', {3});", arrayname, arrayname2, canvas.ClientID, arr, arr));

                js.AppendLine("});");
                js.AppendLine("</script>");

                ltrJS.Text = js.ToString();
            }

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write(string.Format("<span id='{0}' {1} class='{2}'>", ClientID, Display ? "" : "style='display: none;'", Class));
            base.Render(writer);
            writer.Write("</span>");
        }

    }
}