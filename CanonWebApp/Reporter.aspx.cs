using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CanonWebApp.Code.Reporter.PivotTableTableAdapters;
using CanonWebApp.Code.Reporter;
using System.Text;
using System.Data.SqlClient;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Web;
using System.Data;
using System.Configuration;
using CanonWebApp.Controls;
using Memos.Framework;
using CanonWebApp.Code;
using Config = CanonWebApp.Code.ConfigSettings;
using Sess = CanonWebApp.Code.SessionManager;
using Utilities = CanonWebApp.Code.Utilities;
using System.IO;
using DevExpress.XtraCharts.Native;
using System.Drawing.Imaging;
using DevExpress.Web.ASPxGridView;
using System.Globalization;
using System.Drawing;

namespace CanonWebApp
{
    public partial class Reporter : VictoryPage
    {
        string allValues = "(vše)";


        // Is in Ajax Mode?
        private bool _isAjax;
        /// <summary>
        /// Is in Ajax Mode?
        /// </summary>
        private bool IsAjax
        {
            get { return _isAjax; }
            set { _isAjax = value; }
        }

        /// <summary>
        /// "Omezeni na"
        /// </summary>
        List<string> _filerYValues = new List<string>();

        /// <summary>
        /// Saves chart into image file.
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="chart"></param>
        /// <returns></returns>
        protected string SaveChartIntoFile(string productName, WebChartControl chart)
        {
            string filename = string.Format("{0}_{1}.jpg", Sess.LoggedUser.UserId, (productName));
            filename = Path.Combine(Server.MapPath(Config.UploadDirectory), filename);
            using (MemoryStream memoryImage = new MemoryStream())
            {
                ((IChartContainer)chart as IChartContainer).Chart.ExportToImage(memoryImage, ImageFormat.Jpeg);
                memoryImage.Seek(0, System.IO.SeekOrigin.Begin);
                FileStream fs = File.OpenWrite(filename);
                memoryImage.WriteTo(fs);
                fs.Close();
            }
            return filename;
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void Page_Load(object sender, EventArgs e)
        {
            //AjaxContentID = reportForm.ID;
            if (!string.IsNullOrEmpty(Request["ajax"]))
            {
                if (Request["ajax"].Equals("yes"))
                {
                    IsAjax = true;
                }
            }

            // AJAX - please extract this
            #region Dynamic form fields & JS settings

            if (!IsAjax)
            {
                lbFormAction.Attributes.Add("onclick", string.Format("return SendForm('{0}', 'ValidateFormAndSend', this);", reportForm.ClientID));
                lbFormAction.Attributes.Add("style", "display: none!important;");
                ddlYFilterType.Attributes.Add("onchange", "document.getElementById('" + lbFormAction.ClientID + "').click();");
                ddlXValueType.Attributes.Add("onchange", "document.getElementById('" + lbFormAction.ClientID + "').click();");
                ddlYGroupBy.Attributes.Add("onchange", "Lines(0, '" + lbYHaving.ClientID  + "');document.getElementById('" + lbFormAction.ClientID + "').click();");
                //ddlYGroupBy.Attributes.Add("onchange", string.Format("document.getElementById('{0}').style.visibility='hidden';", lbYHaving.ClientID));

                StringBuilder js = new StringBuilder();
                js.AppendLine("<script language=\"javascript\">");
                js.AppendLine(string.Format("ajax_url = '{0}';", Request.Url));
                js.AppendLine("XBrowserAddHandler(window, 'load', function() {");
                js.AppendLine(string.Format("ddlLines='{2}'; InitGraphSize('{0}', '{1}');", txtScreenWidth.ClientID, txtScreenHeight.ClientID, lbYHaving.ClientID));
                js.AppendLine(" });");
                js.AppendLine("XBrowserAddHandler(window, 'resize', function() {");
                js.AppendLine(string.Format("InitGraphSize('{0}', '{1}');", txtScreenWidth.ClientID, txtScreenHeight.ClientID));
                js.AppendLine(" });");
                js.AppendLine("</script>");

                ((Literal)Master.FindControl("ltrJS")).Text = js.ToString();
            }

            #endregion

            if (!IsPostBack)
            {
                //ccDateFrom.Date = DateTime.Now.AddMonths(-1);
                //ccDateTo.Date = DateTime.Now;
                //ccDateFrom.Date = new DateTime(2010, 1, 1);
                //ccDateTo.Date = new DateTime(2010, 1, 1).AddYears(1).AddDays(-1);

                ccDateFrom.Date = new DateTime(2011, 1, 1);
                ccDateTo.Date = new DateTime(2011, 1, 1).AddYears(1).AddDays(-1);

                Panel();
                DrawChart(null, ccDateFrom.Date, ccDateTo.Date, (IntervalType)Convert.ToInt32(ddlXGroupByTime.SelectedValue), ValueRepresentation.AbsoluteValues, true);
            }
            else {
                if (!string.IsNullOrEmpty(Request["ctl00$Content$ccDateFrom$deMainDate$I"]))
                {
                    ccDateFrom.Date = Convert.ToDateTime(Request["ctl00$Content$ccDateFrom$deMainDate$I"]);
                    ccDateTo.Date = Convert.ToDateTime(Request["ctl00$Content$ccDateTo$deMainDate$I"]);
                }
            }

            List<SearchBox> sbs = new List<SearchBox>();

            #region Y Filter

            if (Session["Controls"] == null)
            {
                // Omezeni na distributora
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CanonConnectionStringMain"].ConnectionString))
                {
                    conn.Open();

                    List<string> seriesNames = new List<string>();

                    int k = 0;
                    // Create series
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;

                        foreach (ListItem li in ddlYFilterType.Items)
                        {
                            if (k > 0 && li.Enabled)
                            {

                                SearchBox sb = (SearchBox)LoadControl("Controls/SearchBox.ascx");

                                sb.ID = "lb" + li.Value;
                                sb.Display = false;
                                sb.Class = "filter";
                                pnlYFilter.Controls.Add(sb);

                                // DataBind
                                if (!IsPostBack)
                                {
                                    if (ddlYGroupBy.SelectedValue == "ReselerGroupJoined")
                                    {
                                        cmd.CommandText = string.Format("SELECT DISTINCT IsNull(" + li.Value + ", 'NO-NAME') AS Text, " + li.Value + " AS Value FROM PivotTable ORDER BY " + li.Value, ddlYGroupBy.SelectedValue);
                                    }
                                    else
                                    {
                                        cmd.CommandText = string.Format("SELECT DISTINCT IsNull(" + li.Value + ", '(neuvedeno)') AS Text, " + li.Value + " AS Value FROM PivotTable ORDER BY " + li.Value, ddlYGroupBy.SelectedValue);
                                    }
                                    

                                    SqlDataAdapter sda = new SqlDataAdapter();
                                    sda.SelectCommand = cmd;
                                    DataTable dt = new DataTable();
                                    sda.Fill(dt);
                                    sb.AddResults(dt);
                                }

                                sbs.Add(sb);
                            }
                            k++;
                        }
                    }
                    conn.Close();
                    Session["Controls"] = sbs;
                }
            }

            else
            {
                List<SearchBox> sbs_session = (List<SearchBox>)Session["Controls"];
                foreach (SearchBox sb in sbs_session)
                {
                    pnlYFilter.Controls.Add(sb);
                }
            }

            #endregion


            if (!IsPostBack)
            {
                WebChartControl.ProcessImageRequest(this.Page);
                pnlGraph.Controls.Clear();
            }


            // Y
            foreach (Control c in pnlYFilter.Controls)
            {
                if (c is SearchBox)
                {
                    ((SearchBox)c).Display = false;
                }
            }
            if (ddlYFilterType.SelectedValue != "-")
            {
                Control c = pnlYFilter.FindControl("lb" + ddlYFilterType.SelectedValue);
                ((SearchBox)c).Display = true;

            }

            if (ddlXValueType.SelectedValue == "Time")
            {
                ddlXGroupByTime.Style.Add("display", "block");
                ddlXGroupByData.Style.Add("display", "none");
            }
            else 
            {
                ddlXGroupByTime.Style.Add("display", "none");
                ddlXGroupByData.Style.Add("display", "block");
            }

            base.Page_Load(sender, e);

        }

        #region Report panel

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            if (chbTop.Checked)
            {
                Validate("topCount");
            }
            if (IsValid)
            {
                Panel();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CanonConnectionStringMain"].ConnectionString))
                {
                    conn.Open();

                    // Create pivot
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;

                        // Clear pivot
                        cmd.CommandText = "delete from pivottable";
                        cmd.ExecuteNonQuery();

                        // Fill the Pivot
                        cmd.CommandText = @"
                                            insert into PivotTable(DateSale,Reseler,Mcode,Product,Turnover,Currency,VAD,TurnoverEUR,ReselerGroup,ProductGroup,Reseler2,ReselerGroupDesc,ProductGroupDesc,MonthSale,Cee,Ico,ItemCount,MCodeTrim,ProductJoined,ReselerGroupJoined,DistributorTypeJoined,ReselerCountryJoined,ReselerJoined) select DateSale,Reseler,Mcode,Product,Turnover,Currency,VAD,TurnoverEUR,ReselerGroup,ProductGroup,Reseler2,ReselerGroupDesc,ProductGroupDesc,MonthSale,Cee,Ico,ItemCount,MCodeTrim,ProductJoined,ReselerGroupJoined,DistributorTypeJoined,ReselerCountryJoined,ReselerJoined from Data2010
                                            
                                            insert into pivottable(VAD, DateSale, Ico, MCode, ItemCount, TurnoverEUR, Product, Reseler) 
                                            select Distributor.FileAs, [Date], ResellerIdentificationNumber, ProductCode, Quantity, UnitPrice , ProductName, ResellerName 
                                            from ImportDistributorRecord
	                                            join ImportDistributor on ImportDistributorRecord.IDImportDistributor = ImportDistributor.ID
	                                            join Distributor on ImportDistributor.IDDistributor = Distributor.ID where ImportDistributor.IsDeleted = 0                                            
";

                        cmd.ExecuteNonQuery();

                        // Postprodukce dat 2010
                        cmd.CommandText = @"
                                            update pivottable set ReselerCountryJoined = IsNull(IDCountry, '-1')  from
                                            PivotTable left join Reseller
                                            on reseller.IdentificationNumber = PivotTable.ico


                                            update pivottable set ReselerCountryJoined = 'CZ' where ReselerCountryJoined = '1'
                                            update pivottable set ReselerCountryJoined = 'SK' where ReselerCountryJoined = '2'
                                            update pivottable set ReselerCountryJoined = '[Neznámá]' where ReselerCountryJoined = '-1'


                                            update pivottable set ReselerGroupJoined = IsNull(ResellerGroup.FileAs, 'NO-NAME')  
                                            from
                                            PivotTable left join Reseller
                                            on reseller.IdentificationNumber = PivotTable.ico
                                            left join ResellerGroup on reseller.IDResellerGroup = ResellerGroup.ID


                                            update pivottable set DistributorTypeJoined = IsNull(DistributorType.FileAs, '[Neznámý]')  
                                            from
                                            PivotTable left join Distributor
                                            on Distributor.FileAs = PivotTable.VAD
                                            join DistributorType on Distributor.IDDistributorType = DistributorType.ID


                                            update pivottable set ReselerJoined = IsNull(Reseller.FileAs, '* ' + Reseler)  
                                            from
                                            PivotTable left join Reseller
                                            on reseller.IdentificationNumber = PivotTable.ico

                                            update products set ProductCodeTrim =  Left(ProductCode, Len(ProductCode) - 2)

                                            update pivottable set ProductJoined = Replace(Replace(Replace(Replace(Replace(IsNull(Products.ProductName, '* ' + Product), '(', '['), ')', ']'), '=', ' - '), '" + ";" + @"', ''), '" + "\"" + @"', ' ')
                                            from
                                            PivotTable left join Products
                                            on Products.ProductCodeTrim = PivotTable.Mcode COLLATE SQL_Latin1_General_CP1_CI_AS


                                            update pivottable set productgroupdesc = IsNull(FileAs, '[Neznámá]') from 
                                            PivotTable left join Products
                                            on Products.ProductCodeTrim = PivotTable.Mcode collate SQL_Latin1_General_CP1_CI_AS
                                            left join ProductGroup on ProductGroup.ID = Products.IDProductGroup
                                            where PivotTable.DateSale >= '2011-01-01 00:00:00.000'
";
                        cmd.ExecuteNonQuery();

                        // 
                        //update pivottable set TurnOverEUR = IsNull(Products.CurrentPrice, 0)
                        //from
                        //PivotTable left join Products
                        //on Products.ProductCodeTrim = PivotTable.Mcode COLLATE SQL_Latin1_General_CP1_CI_AS
                    }
                }

                DrawChart(null, ccDateFrom.Date, ccDateTo.Date, (IntervalType)Convert.ToInt32(ddlXGroupByTime.SelectedValue), ValueRepresentation.AbsoluteValues, false);

                pnlTable.Visible = true;
                gvGrid.Rows[gvGrid.Rows.Count - 1].BackColor = Color.LightGray;
                gvGrid.Rows[gvGrid.Rows.Count - 1].Cells[gvGrid.Rows[gvGrid.Rows.Count - 1].Cells.Count - 1].Style.Add("background", "white!important");
                gvGrid.Rows[gvGrid.Rows.Count - 1].Cells[0].Style.Add("background", "white!important");
            }
        }

        protected void btnGenerateExcel_Click(object sender, EventArgs e)
        {
            GenerateGrid(ccDateFrom.Date, ccDateTo.Date);
            gridExport.FileName = "Canon_Export_" + ccDateFrom.Date.ToString("yyyy-MM-dd") + "_" + ccDateTo.Date.ToString("yyyy-MM-dd");
            gridExport.WriteXlsToResponse();
        }

        protected void btnGenerateGrid_Click(object sender, EventArgs e)
        {
            GenerateGrid(ccDateFrom.Date, ccDateTo.Date);
        }

        #endregion

        #region Chart Data Layer

        public enum SelectList
        {
            Count = 1,
            All = 2,
            Expression = 3,
            TurnoverEUR = 4
        }

        enum IntervalType
        {
            Day = 1,
            Week = 7,
            Month = 3,
            Year = 4
        }

        enum ValueRepresentation
        {
            AbsoluteValues = 1,
            Percent = 2,
            Margins = 3
        }

        class Interval
        {

            DateTime _start;

            public DateTime Start
            {
                get
                {
                    return _start;
                }
                set
                {
                    _start = value;
                }
            }

            DateTime _end;

            public DateTime End
            {
                get
                {
                    return _end;
                }
                set
                {
                    _end = value;
                }
            }

            public Interval(DateTime start, DateTime end)
            {
                _start = start;
                _end = end;
            }
        }

        #endregion

        #region Chart and grid generating

        SqlCommand GenerateReportCommand(DateTime start, DateTime end, string serie)
        {
            SqlCommand command = new SqlCommand();

            StringBuilder text = new StringBuilder();

            text.Append("SELECT ");
            switch (Convert.ToInt32(ddlYValueType.SelectedValue))
            {
                case (int)SelectList.All:
                    text.Append("*");
                    break;
                case (int)SelectList.Count:
                    text.Append("Sum(CAST(ItemCount as Decimal)) ");
                    break;
                case (int)SelectList.TurnoverEUR:
                    text.Append("Sum(CAST(replace(TurnoverEUR, ',', '.') as Decimal)) ");
                    break;
                case (int)SelectList.Expression:
                    //text.Append(expression);
                    break;
            }
            text.Append(" FROM PivotTable");
            text.Append(" WHERE (DateSale >= @Start AND DateSale < @End) AND (" + ddlYGroupBy.SelectedValue + " = '" + serie + "') ");

#warning Extract to function
            if (ddlYFilterType.SelectedIndex > 0)
            {
                if (_filerYValues.Count > 0)
                {
                    text.Append(" AND (");
                    // Y Filter
                    int i = 0;
                    foreach (string s in _filerYValues)
                    {
                        text.Append(ddlYFilterType.SelectedValue);
                        text.Append(" = '");
                        text.Append(s);
                        text.Append("' ");
                        if (i < _filerYValues.Count - 1)
                        {
                            text.Append("OR ");
                        }
                        i++;
                    }
                    text.Append(") ");
                }
            }

            command.CommandText = text.ToString();
            command.Parameters.Add(new SqlParameter("@Start", SqlDbType.DateTime) { Value = start });
            command.Parameters.Add(new SqlParameter("@End", SqlDbType.DateTime) { Value = end });

            return command;
        }

        SqlCommand GenerateReportCommandByData(string data, string serie)
        {
            SqlCommand command = new SqlCommand();

            StringBuilder text = new StringBuilder();

            text.Append("SELECT ");
            switch (Convert.ToInt32(ddlYValueType.SelectedValue))
            {
                case (int)SelectList.All:
                    text.Append("*");
                    break;
                case (int)SelectList.Count:
                    text.Append("Sum(CAST(ItemCount as Decimal)) ");
                    break;
                case (int)SelectList.TurnoverEUR:
                    text.Append("Sum(CAST(replace(TurnoverEUR, ',', '.') as Decimal)) ");
                    break;
                case (int)SelectList.Expression:
                    //text.Append(expression);
                    break;
            }
            text.Append(" FROM PivotTable");
            text.Append(" WHERE (" + ddlXGroupByData.SelectedValue + " = '" + data + "') AND (" + ddlYGroupBy.SelectedValue + " = '" + serie + "') ");

#warning Extract to function
            if (ddlYFilterType.SelectedIndex > 0)
            {
                if (_filerYValues.Count > 0)
                {
                    text.Append(" AND (");
                    // Y Filter
                    int i = 0;
                    foreach (string s in _filerYValues)
                    {
                        text.Append(ddlYFilterType.SelectedValue);
                        text.Append(" = '");
                        text.Append(s);
                        text.Append("' ");
                        if (i < _filerYValues.Count - 1)
                        {
                            text.Append("OR ");
                        }
                        i++;
                    }
                    text.Append(") ");
                }
            }

            command.CommandText = text.ToString();

            return command;
        }

        void DrawChart(DataTable data, DateTime from, DateTime to, IntervalType interval, ValueRepresentation representation, bool ajax)
        {
                List<Interval> _intervals = new List<Interval>();

                if (interval == IntervalType.Day || interval == IntervalType.Week)
                {
                    // Day by day or week by week
                    for (int i = 0; i < (to.Subtract(from).TotalDays / (int)interval); i++)
                    {
                        _intervals.Add(new Interval(from.AddDays((i * (int)interval) + 1 - 1), from.AddDays((i * (int)interval) + (int)interval + 1 - 1)));

                    }
                }
                if (interval == IntervalType.Month)
                {
                    for (int i = 0; i < to.Month - from.Month + ((to.Year - from.Year) * 12) + 1; i++)
                    {
                        _intervals.Add(new Interval(new DateTime(from.Year, from.Month, 1).AddMonths(i), new DateTime(from.Year, from.Month, 1).AddMonths(i + 1).AddDays(-1)));
                    }
                }

                if (interval == IntervalType.Year)
                {
                    for (int i = 0; i < to.Year - from.Year + 1; i++)
                    {
                        _intervals.Add(new Interval(new DateTime(from.Year, from.Month, 1).AddYears(i), new DateTime(from.Year, from.Month, 1).AddYears(i + 1).AddDays(-1)));
                    }
                }
            //foreach (Interval ii in _intervals)
            //{
            //    Response.Write(ii.Start);
            //    Response.Write(" - ");
            //    Response.Write(ii.End);
            //    Response.Write("<br>");
            //}

            // Draw chart here item by item in _intervals

            WebChartControl chart = new WebChartControl();

            if (!ajax)
            {
                chart.EnableViewState = false;
                ChartTitle title = new ChartTitle();
                title.Text = "Graf dle " + ddlYGroupBy.SelectedItem.Text.ToLower() + ":";
                title.Font = new System.Drawing.Font("Tahoma", 10);
                chart.Titles.Add(title);
                chart.FillStyle.FillMode = FillMode.Solid;
            }
            Dictionary<int, Series> series = new Dictionary<int, Series>();

            List<bool> seriesVisibilities = new List<bool>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CanonConnectionStringMain"].ConnectionString))
            {
                conn.Open();

                List<string> seriesNames = new List<string>();
                List<string> seriesCriteria = new List<string>();

                int k = 0;
                Culture = "cs-CZ";
                // Create series
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    StringBuilder text = new StringBuilder("");

#warning extract to function
                    if (ddlYFilterType.SelectedIndex > 0)
                    {
                        if (_filerYValues.Count > 0)
                        {
                            text.Append(" AND (");
                            // Y Filter
                            int i = 0;
                            foreach (string s in _filerYValues)
                            {
                                text.Append(ddlYFilterType.SelectedValue);
                                text.Append(" = '");
                                text.Append(s);
                                text.Append("' ");
                                if (i < _filerYValues.Count - 1)
                                {
                                    text.Append("OR ");
                                }
                                i++;
                            }
                            text.Append(") ");
                        }
                    }
                    string top = "";
                    if (chbTop.Checked)
                    {
                        int tp = Convert.ToInt32(txtTop.Text);
                        if (!ajax)
                        {
                            tp += 1;
                        }
                        top = "TOP " + tp;
                    }
                    
                    cmd.CommandText = string.Format("SELECT {2} {0}, {0} AS Value FROM PivotTable WHERE DateSale >= @Start AND DateSale < @End {1} GROUP BY {0} ORDER BY Sum(CAST(replace(TurnoverEUR, ',', '.') as Decimal)) DESC", ddlYGroupBy.SelectedValue, text, top);

                    cmd.Parameters.Add(new SqlParameter("@Start", SqlDbType.DateTime) { Value = _intervals[0].Start });
                    cmd.Parameters.Add(new SqlParameter("@End", SqlDbType.DateTime) { Value = _intervals[_intervals.Count - 1].End });

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (ddlYGroupBy.SelectedValue == "ReselerGroupJoined")
                        {
                            seriesNames.Add(reader[1].ToString().Trim() == "" ? "NO-NAME" : reader[1].ToString());
                        }
                        else
                        {
                            seriesNames.Add(reader[1].ToString().Trim() == "" ? "(neuvedeno)" : reader[1].ToString());
                        }
                        
                        if (!ajax)
                        {
                            Series s;
                            if (rblValueDisplayMode.SelectedValue == "2" || ddlXValueType.SelectedValue == "Data")
                            {
                                s = new Series(reader[1].ToString(), ViewType.StackedBar);
                            }
                            else
                            {
                                s = new Series(reader[1].ToString(), ViewType.Line);
                            }
                            s.ValueScaleType = ScaleType.Numerical;
                            if (ddlXValueType.SelectedValue == "Time")
                            {
                                s.ArgumentScaleType = ScaleType.DateTime;
                            }
                            if (ddlXValueType.SelectedValue == "Data")
                            {
                                s.ArgumentScaleType = ScaleType.Qualitative;
                            }

                            if (ddlYValueType.SelectedValue == "4")
                            {
                                if (rblValueDisplayMode.SelectedValue != "2")
                                {
                                    s.PointOptions.ValueNumericOptions.Format = NumericFormat.Currency;
                                }
                                if (rblValueDisplayMode.SelectedValue == "2")
                                {
                                    s.PointOptions.ValueNumericOptions.Format = NumericFormat.Percent;
                                }
                            }
                            //s.PointOptions.ValueNumericOptions.Format = NumericFormat.Currency;
                            seriesCriteria.Add(reader[0].ToString());
                            seriesVisibilities.Add(false);
                            series.Add(k, s);
                            k++;
                        }
                    }
                    reader.Close();
                }

                // Draw series
                // By Time

                // Enumerate series here and send them via ajax
                if (ajax)
                {
                    lbYHaving.Items.Clear();
                    lbYHaving.Items.Add(allValues);
                    foreach (string s in seriesNames)
                    {
                        lbYHaving.Items.Add(s);
                    }
                    DataBind(lbYHaving);

                    return;
                }

                if (ddlXValueType.SelectedValue == "Time")
                {
                    foreach (Interval i in _intervals)
                    {
                        for (int j = 0; j < seriesNames.Count; j++)
                        {
                            using (SqlCommand command = GenerateReportCommand(i.Start, i.End, seriesCriteria[j]))
                            {
                                command.Connection = conn;
                                SqlDataReader dr = command.ExecuteReader();

                                while (dr.Read())
                                {
                                    // Here it comes
                                    if (dr[0] is DBNull)
                                    {
                                        series[j].Points.Add(new SeriesPoint(i.Start, new double[] { 0 }));
                                    }
                                    else
                                    {
                                        series[j].Points.Add(new SeriesPoint(i.Start, new double[] { Convert.ToDouble(dr[0]) }));
                                        seriesVisibilities[j] = true;
                                    }
                                }
                                dr.Close();
                            }
                        }
                    }
                }
                if (ddlXValueType.SelectedValue == "Data")
                {
                    using (SqlCommand command_data = new SqlCommand())
                    {
                        command_data.Connection = conn;
                        command_data.CommandText = "SELECT DISTINCT " + ddlXGroupByData.SelectedValue + " FROM PivotTable WHERE DateSale >= @Start AND DateSale < @End";
                        command_data.Parameters.Add(new SqlParameter("@Start", SqlDbType.DateTime) { Value = _intervals[0].Start });
                        command_data.Parameters.Add(new SqlParameter("@End", SqlDbType.DateTime) { Value = _intervals[_intervals.Count - 1].End });

                        SqlDataReader reader = command_data.ExecuteReader();
                        List<string> xData = new List<string>();
                        
                        while (reader.Read())
                        {
                            xData.Add(reader[0].ToString());
                        }
                        reader.Close();

                        foreach(string xd in xData)
                        {
                            for (int j = 0; j < seriesNames.Count; j++)
                            {
                                using (SqlCommand command = GenerateReportCommandByData(xd, seriesCriteria[j]))
                                {
                                    command.Connection = conn;
                                    SqlDataReader dr = command.ExecuteReader();

                                    while (dr.Read())
                                    {
                                        // Here it comes
                                        if (dr[0] is DBNull)
                                        {
                                            series[j].Points.Add(new SeriesPoint(xd, new double[] { 0 }));
                                        }
                                        else
                                        {
                                            series[j].Points.Add(new SeriesPoint(xd, new double[] { Convert.ToDouble(dr[0]) }));
                                            seriesVisibilities[j] = true;
                                        }
                                    }
                                    dr.Close();
                                }
                            }
                        }
                    }
                }
            }

            int l = 0;
            object yHaving = Request[lbYHaving.ClientID.Replace('_', '$')];
            

            if (yHaving == null)
            {
                yHaving = allValues;
            }

            lbYHaving.Items.Clear();

            if (yHaving.ToString() != allValues)
            {
                {
                    for (int i = 0; i < series.Count; i++)
                    {
                        if (seriesVisibilities[i])
                        {
                            lbYHaving.Items.Add(series[i].Name);
                            foreach (string s in yHaving.ToString().Split(','))
                            {
                                if (s == series[i].Name)
                                {
                                    lbYHaving.Items[l].Selected = true;
                                    seriesVisibilities[i] = true;
                                    break;
                                }
                                else
                                {
                                    seriesVisibilities[i] = false;
                                }
                            }
                            l++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < series.Count; i++)
                {
                    if (seriesVisibilities[i] && yHaving.ToString() != allValues)
                    {
                        lbYHaving.Items.Add(series[i].Name);
                    }
                }
            }

            lbYHaving.Items.Insert(0, allValues);
            if (yHaving.ToString() == allValues)
            {
                lbYHaving.Items[0].Selected = true;
            }

            // Other
            for (int i = 0; i < series.Count; i++)
            {
                if (seriesVisibilities[i])
                {

                    if (seriesVisibilities[i] && yHaving.ToString() == allValues)
                    {
                        lbYHaving.Items.Add(series[i].Name);
                    }
                    chart.Series.Add(series[i]);
                }
            }

            #region Compute sum

            Series suma = new Series("Suma", ViewType.Line);
            suma.ValueScaleType = ScaleType.Numerical;
            suma.ArgumentScaleType = ScaleType.DateTime;
            if (ddlYValueType.SelectedValue == "4")
            {
                if (rblValueDisplayMode.SelectedValue != "2")
                {
                    suma.PointOptions.ValueNumericOptions.Format = NumericFormat.Currency;
                }
                if (rblValueDisplayMode.SelectedValue == "2")
                {
                    suma.PointOptions.ValueNumericOptions.Format = NumericFormat.Percent;
                }
            }
            

            DataTable grid = new DataTable();
            string xArgumentName = ddlXValueType.SelectedItem.Text + " - dle ";

            if (ddlXValueType.SelectedValue == "Time")
            {
                xArgumentName += ddlXGroupByTime.SelectedItem.Text.ToLower();
            }
            else
            {
                xArgumentName += ddlXGroupByData.SelectedItem.Text.ToLower();
            }

            grid.Columns.Add(new DataColumn(xArgumentName));
            for (int i = 0; i < chart.Series.Count; i++)
            {
                grid.Columns.Add(new DataColumn(chart.Series[i].Name));
            }

            double sum;
            for (int j = 0; j < chart.Series[0].Points.Count; j++)
            {
                sum = 0;
                DataRow dr = grid.NewRow();
                if (ddlXValueType.SelectedValue == "Time")
                {
                    dr[0] = Convert.ToDateTime(chart.Series[0].Points[j].Argument).ToString("dd.MM.yyyy");
                }
                else
                {
                    dr[0] = chart.Series[0].Points[j].Argument;
                }
                for (int i = 0; i < chart.Series.Count; i++)
                {
                    dr[i+1] = chart.Series[i].Points[j].Values[0];
                    sum += chart.Series[i].Points[j].Values[0];
                }
                grid.Rows.Add(dr);
                suma.Points.Add(new SeriesPoint(chart.Series[0].Points[j].Argument, new double[] { sum }));

                // Percent
                if (rblValueDisplayMode.SelectedValue == "2")
                {
                    for (int i = 0; i < chart.Series.Count; i++)
                    {
                        if (sum == 0)
                        {
                            chart.Series[i].Points[j].Values[0] = 0;
                        }
                        else
                        {
                            chart.Series[i].Points[j].Values[0] = Math.Round(((chart.Series[i].Points[j].Values[0] / sum) * 1), 2);
                        }
                    }
                }

                if (rblValueDisplayMode.SelectedValue == "3")
                {
                    if (j > 0)
                    {
                        for (int i = 0; i < chart.Series.Count; i++)
                        {
                            chart.Series[i].Points[j].Values[0] = chart.Series[i].Points[j].Values[0] - chart.Series[i].Points[j - 1].Values[0];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < chart.Series.Count; i++)
                        {
                            chart.Series[i].Points[j].Values[0] = 0;
                        }
                    }

                }
            }
            suma.View.Color = System.Drawing.Color.Black;
            series.Add(series.Count, suma);
            if (rblValueDisplayMode.SelectedValue == "1")
            {
                chart.Series.Add(series[series.Count - 1]);
            }

            if (chbShowSum.Checked && rblValueDisplayMode.SelectedValue == "1" && ddlXValueType.SelectedValue == "Time")
            {
                int toBeRemoved = chart.Series.Count - 1;
                for (int j = 0; j < toBeRemoved; j++)
                {
                    chart.Series.RemoveAt(0);
                }
            }

            // Show grid

            DataRow drSum = grid.NewRow();
            for(int i = 1; i < grid.Columns.Count; i++)
            {
                double dSum = 0;
                for (int j = 0; j < grid.Rows.Count; j++)
                {
                    dSum += Convert.ToDouble(grid.Rows[j][i]);
                }
                drSum[i] = dSum;
            }
            grid.Columns.Add(new DataColumn("Součet za svislou osu"));
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                double dSum = 0;
                for (int j = 1; j < grid.Columns.Count - 1; j++)
                {
                    dSum += Convert.ToDouble(grid.Rows[i][j] == null ? "0" : grid.Rows[i][j]);
                }
                if (rblValueDisplayMode.SelectedValue == "2")
                {
                    for (int j = 1; j < grid.Columns.Count - 1; j++)
                    {
                        if (dSum == 0)
                        {
                            grid.Rows[i][j] = "0";
                        }
                        else
                        {
                            grid.Rows[i][j] = (Math.Round(Convert.ToDouble(grid.Rows[i][j] == null ? "0" : grid.Rows[i][j]) / dSum, 4) * 100).ToString() + " %";
                        }
                    }
                }
                grid.Rows[i][grid.Columns[grid.Columns.Count-1]] = dSum;
            }
            grid.Rows.Add(drSum);
            grid.Rows[grid.Rows.Count - 1][0] = "Součet za vodorovnou osu";
            gvGrid.DataSource = grid;
            gvGrid.DataBind();

            #endregion

            #region Customize chart

            DevExpress.XtraCharts.XYDiagram xyDiagram = new XYDiagram();

            if (ddlYValueType.SelectedIndex == 0)
            {
                xyDiagram.AxisY.Title.Text = "EUR";
            }
            if (ddlYValueType.SelectedIndex == 1)
            {
                xyDiagram.AxisY.Title.Text = "Kusů";
            }

            xyDiagram.AxisY.Title.Font = new System.Drawing.Font("Tahoma", 8);
            xyDiagram.AxisY.Label.Staggered = false;
            xyDiagram.AxisY.Title.Visible = true;
            xyDiagram.AxisY.Range.SideMarginsEnabled = true;
            xyDiagram.AxisY.Interlaced = true;

            xyDiagram.AxisX.Title.Font = new System.Drawing.Font("Tahoma", 8);
            xyDiagram.AxisX.Label.Staggered = true;
            xyDiagram.AxisX.Range.SideMarginsEnabled = true;
            xyDiagram.AxisX.Tickmarks.MinorVisible = false;

            xyDiagram.AxisX.Range.MinValue = from;
            xyDiagram.AxisX.Range.MaxValue = to;

            switch (interval)
            {
                case IntervalType.Day:
                    xyDiagram.AxisX.DateTimeMeasureUnit = DateTimeMeasurementUnit.Day;
                    break;
                case IntervalType.Week:
                    xyDiagram.AxisX.DateTimeMeasureUnit = DateTimeMeasurementUnit.Day;
                    xyDiagram.AxisX.GridSpacingAuto = false;
                    xyDiagram.AxisX.GridSpacing = 7;
                    break;
                case IntervalType.Month:
                    xyDiagram.AxisX.DateTimeMeasureUnit = DateTimeMeasurementUnit.Month;
                    break;
                case IntervalType.Year:
                    xyDiagram.AxisX.DateTimeMeasureUnit = DateTimeMeasurementUnit.Year;
                    break;
            }
            xyDiagram.EnableZooming = true;

            chart.Diagram = xyDiagram;
            chart.Width = Convert.ToInt32(txtScreenWidth.Text) - 50;
            chart.Height = 602;
            //chart.Height = Convert.ToInt32(txtScreenHeight.Text) - 230;

            #endregion

            chart.DataBind();
            pnlGraph.Controls.Add(chart);
        }

        void GenerateGrid(DateTime start, DateTime end)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CanonConnectionStringMain"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    StringBuilder text = new StringBuilder();

                    text.Append("SELECT DateSale AS [Datum prodeje], Mcode, VAD AS [Distributor], CAST(replace(TurnoverEUR, ',', '.') as Decimal) AS Obrat, ProductGroupDesc AS [Produktová Skupina], Ico AS [Ičo], ItemCount AS [Počet položek], ProductJoined AS [Název produktu], ReselerGroupJoined AS [Skupina reselerů], DistributorTypeJoined AS [Typ distributora], ReselerCountryJoined AS [Země reselera], ReselerJoined AS [Název reselera] FROM PivotTable WHERE (DateSale >= @Start AND DateSale < @End)");

                    cmd.CommandText = text.ToString();
                    cmd.Parameters.Add(new SqlParameter("@Start", SqlDbType.DateTime) { Value = start });
                    cmd.Parameters.Add(new SqlParameter("@End", SqlDbType.DateTime) { Value = end });

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvTable.DataSource = dt;
                    gvTable.DataBind();
                }
            }

        }

        #endregion

        /// <summary>
        /// This one controls the configuration panel.
        /// </summary>
        void Panel()
        {
            foreach (Control c in pnlYFilter.Controls)
            {
                if (c is SearchBox)
                {
                    ActionControl(c.ClientID, ControlActionType.Hide);
                }
            }
            if (ddlYFilterType.SelectedValue != "-")
            {
                //pnlYFilter.FindControl("lb" + ddlYFilterType.SelectedValue).Visible = true;
                ActionControl(pnlYFilter.FindControl("lb" + ddlYFilterType.SelectedValue).ClientID, ControlActionType.Show);
            }
            if (ddlYFilterType.SelectedIndex > 0)
            {
                _filerYValues = ((SearchBox)pnlYFilter.FindControl("lb" + ddlYFilterType.SelectedValue)).Value;
            }
            if (ddlXValueType.SelectedValue == "Time")
            {
                ActionControl(ddlXGroupByData.ClientID, ControlActionType.Hide);
                ActionControl(ddlXGroupByTime.ClientID, ControlActionType.Show);

                //ddlXGroupByData.Visible = false;
                //ddlXGroupByTime.Visible = true;
            }
            if(ddlXValueType.SelectedValue == "Data")
            {
                ActionControl(ddlXGroupByTime.ClientID, ControlActionType.Hide);
                ActionControl(ddlXGroupByData.ClientID, ControlActionType.Show);
                
                //ddlXGroupByTime.Visible = false;
                //ddlXGroupByData.Visible = true;
            }
            //lbFormAction.Visible = false;
            //btnGenerateGrid.Visible = false;
        }

        // Ajax hidden button
        protected void lbFormAction_Click(object sender, EventArgs e)
        {
            Panel();
            DrawChart(null, ccDateFrom.Date, ccDateTo.Date, (IntervalType)Convert.ToInt32(ddlXGroupByTime.SelectedValue), ValueRepresentation.AbsoluteValues, true);
        }

        // AJAX Core - exctract to base class
        protected override void Render(HtmlTextWriter writer)
        {
            //gvGrid.Columns[gvGrid.Columns.Count - 1].HeaderStyle.BackColor = Color.LightGray;
            for (int i = 0; i < gvGrid.Rows.Count; i++)
            {
                gvGrid.Rows[i].Cells[gvGrid.Rows[i].Cells.Count - 1].BackColor = Color.LightGray;
            }

            if (IsAjax)
            {
                Response.Write(ValidationString());
                Response.Write("|");
                Response.Write(ValueString2());
                Response.Write("|");
                Response.Write(ActionsString());
                Response.End();
            }
            else
            {
                base.Render(writer);
            }
        }

        protected void btnPokus_Click(object sender, EventArgs e)
        {
            
        }

    }
}
