using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using CanonWebApp.Code.Reporter.PivotTableTableAdapters;
using CanonWebApp.Code.Reporter;
using System.Data.SqlTypes;
using DevExpress.XtraCharts.Web;
using DevExpress.XtraCharts;

namespace CanonWebApp.Controls
{
    public partial class ReportPanel : UserControl
    {
        public enum QueryType
        {
            Select = 1,
        }

        public enum SelectList
        {
            Count = 1,
            All = 2,
            Expression = 3,
            TurnOver = 4
        }

        public string CssClass;

        #region Properties

        // Builded SQL query
        private string _sql;
        // Builded SQL query
        public string SQL
        {
            get { return _sql; }
            set { _sql = value; }
        }

        #endregion


        public class SqlQuery
        {
            StringBuilder _query = new StringBuilder();


            public override string ToString()
            {
                return _query.ToString();
            }

            string ListBoxToValues(ListBox ddl)
            {
                string ret = "";
                foreach (ListItem li in ddl.Items)
                {
                    if (li.Selected)
                    {
                        ret += "'" + li.Value + "'";
                        ret += ",";
                    }
                }
                ret = ret.Length > 0 ? ret.Substring(0, ret.Length - 1) : ret;
                return ret;
            }

            DateTime _start;
            DateTime _end;

            public void Start(QueryType type, SelectList list, string expression, DateTime start, DateTime end)
            {
                if (type == QueryType.Select)
                {
                    _query.Append("SELECT ");
                }
                switch (list)
                {
                    case SelectList.All:
                        _query.Append("*");
                        break;
                    case SelectList.Count:
                        _query.Append("Count(*) AS C");
                        break;
                    case SelectList.Expression:
                        _query.Append(expression);
                        break;
                }
                _query.Append(" ");
                _start = start;
                _end = end;
            }

            public void From(string expression)
            {
                _query.Append("FROM ");
                _query.Append(expression);
            }

            public void WhereEquals(string conditionField, string values)
            {
                _query.Append("WHERE ");
                _query.Append(conditionField);
                _query.AppendFormat(" IN ({0})", values);
                _query.Append(" ");
                //if (con)
            }

            public void WhereEquals(string conditionField, ListBox list)
            {
                WhereEquals(conditionField, ListBoxToValues(list));
            }

            public void HavingEquals(string conditionField, string values)
            {
                _query.Append("HAVING ");
                _query.Append(conditionField);
                _query.AppendFormat(" IN ({0})", values);
                _query.Append(" ");
            }

            public void HavingEquals(string conditionField, ListBox list)
            {
                _query.Append("HAVING ");
                _query.Append(conditionField);
                _query.AppendFormat(" IN ({0})", ListBoxToValues(list));
                _query.Append(" ");
            }

            public void GroupBy(string expression)
            {
                _query.Append("GROUP BY ");
                _query.Append(expression);
                _query.Append(" ");
            }

            public void OrderBy(string expression)
            {
                _query.Append("ORDER BY ");
                _query.Append(expression);
                _query.Append(" ");
            }

            public void OrderByCount()
            {
                _query.Append("ORDER BY C DESC");
            }
        }


        SqlQuery q;

        protected void Page_Load(object sender, EventArgs e)
        {
            q = new SqlQuery();


            // Y Axis value

            // We watch Obrat
            if (ddlYValueType.SelectedValue.Equals("Obrat"))
            {
                //q.Start(QueryType.Select, SelectList.Expression, "");
            }

            // We watch Prodanych kusu
            if (ddlYValueType.SelectedValue.Equals("ProdanychKusu"))
            {
                //q.Start(QueryType.Select, SelectList.Count, "");
            }

            if (ddlYFilterType.SelectedIndex > 0)
            {
                //q.WhereEquals(ddlYFilterType.SelectedValue, lbYFilter);
            }

            //q.GroupBy(ddlYGroupBy.SelectedValue);
            //q.OrderByCount();


            // jak budeme resit kresleni po dnech???
            //foreach ()
            //{
            //}

            WebChartControl.ProcessImageRequest(this.Page);
            pnlGraph.Controls.Clear();
        }

        enum IntervalType
        {
            Day = 1,
            Week = 7,
            Month = 3,
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



        void DrawChart(DataTable data, DateTime from, DateTime to, IntervalType interval, ValueRepresentation representation)
        {

            List<Interval> _intervals = new List<Interval>();

            if (interval == IntervalType.Day || interval == IntervalType.Week)
            {
                // Day by day or week by week
                for (int i = 0; i < (to.Subtract(from).TotalDays / (int)interval); i++)
                {
                    //Response.Write(i + 1);
                    //Response.Write(":   ");
                    //Response.Write((i * (int)interval) + 1);
                    //Response.Write(" - ");
                    //Response.Write((i * (int)interval) + 1 + 7);
                    //Response.Write("   |   ");
                    //Response.Write(from.AddDays((i * (int)interval) + 1 - 1));
                    //Response.Write(" - ");
                    //Response.Write(from.AddDays((i * (int)interval) + 1 - 1));
                    //Response.Write("<br>");

                    _intervals.Add(new Interval(from.AddDays((i * (int)interval) + 1 - 1), from.AddDays((i * (int)interval) + (int)interval + 1 - 1)));

                }
            }
            if (interval == IntervalType.Month)
            {
                for (int i = 0; i < to.Month - from.Month + ((to.Year - from.Year) * 12) + 1; i++)
                {
                    //Response.Write(i + 1);
                    //Response.Write(":   ");
                    //Response.Write(new DateTime(from.Year, from.Month, 1).AddMonths(i));
                    //Response.Write(" - ");
                    //Response.Write(new DateTime(from.Year, from.Month, 1).AddMonths(i+1).AddDays(-1));
                    //Response.Write("<br>");

                    _intervals.Add(new Interval(new DateTime(from.Year, from.Month, 1).AddMonths(i), new DateTime(from.Year, from.Month, 1).AddMonths(i + 1).AddDays(-1)));
                }
            }
            //List<>

            // Draw chart here item by item in _intervals

            WebChartControl chart = new WebChartControl();
            chart.EnableViewState = false;
            ChartTitle title = new ChartTitle();
            title.Text = "pokus";
            title.Font = new System.Drawing.Font("Tahoma", 10);
            chart.Titles.Add(title);
            chart.FillStyle.FillMode = FillMode.Solid;
            Dictionary<int, Series> series = new Dictionary<int, Series>();

            //foreach (int channelId in channels)
            //{
            //    Channel channel = db.Channels.Where(c => c.ChannelId == channelId).FirstOrDefault();
            //    if (channel == null) continue;
            //    Series s = new Series(channel.ChannelName, ViewType.Line);
            //    s.ValueScaleType = ScaleType.Numerical;
            //    s.ArgumentScaleType = ScaleType.DateTime;
            //    series.Add(channelId, s);
            //}




            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CanonConnectionStringMain"].ConnectionString))
            {
                conn.Open();

                List<string> seriesNames = new List<string>();

                int k = 0;
                // Create series
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("SELECT DISTINCT {0} FROM PivotTable WHERE DateSale >= @Start AND DateSale < @End", ddlYGroupBy.SelectedValue);
                    cmd.Parameters.Add(new SqlParameter("@Start", SqlDbType.DateTime) { Value = _intervals[0].Start });
                    cmd.Parameters.Add(new SqlParameter("@End", SqlDbType.DateTime) { Value = _intervals[_intervals.Count - 1].End });

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Series s = new Series(reader[0].ToString(), ViewType.Line);
                        s.ValueScaleType = ScaleType.Numerical;
                        s.ArgumentScaleType = ScaleType.DateTime;
                        seriesNames.Add(reader[0].ToString());
                        series.Add(k, s);
                        k++;
                    }
                    reader.Close();
                }

                // Draw series
                foreach (Interval i in _intervals)
                {
                    for (int j = 0; j < seriesNames.Count; j++ )
                    {
                        using (SqlCommand command = GenerateReportCommand(i.Start, i.End, seriesNames[j]))
                        {
                            command.Connection = conn;
                            SqlDataReader dr = command.ExecuteReader();

                            while (dr.Read())
                            {
                                // Here it comes
                                //Response.Write(dr[0]);
                                //Response.Write(" - ");
                                //Response.Write(dr[1]);
                                //Response.Write("<br>");
                                if (dr[0] is DBNull)
                                {
                                    series[j].Points.Add(new SeriesPoint(i.Start, new double[] { 0 }));
                                }
                                else
                                {
                                    series[j].Points.Add(new SeriesPoint(i.Start, new double[] { Convert.ToDouble(dr[0]) }));
                                }
                            }
                            dr.Close();
                        }
                    }
                }
            }

            foreach (KeyValuePair<int, Series> serie in series)
                chart.Series.Add(serie.Value);
            //diagram
            DevExpress.XtraCharts.XYDiagram xyDiagram = new XYDiagram();
            xyDiagram.AxisY.Title.Font = new System.Drawing.Font("Tahoma", 8);
            xyDiagram.AxisY.Title.Text = "CZK";
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
            }

            
            

            //xyDiagram.AxisX.DateTimeMeasureUnit = DateTimeMeasurementUnit.Month;
            xyDiagram.EnableZooming = true;

            chart.Diagram = xyDiagram;
            chart.Width = 800;
            chart.Height = 500;
            chart.DataBind();
            pnlGraph.Controls.Add(chart);
        }

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
                    //text.Append(", " + ddlYGroupBy.SelectedValue);
                    break;//text.Append("Sum(CAST(Turnover as Decimal)) ");
                case (int)SelectList.TurnOver:
                    text.Append("Sum(CAST(replace(Turnover, ',', '.') as Decimal)) ");
                    break;
                case (int)SelectList.Expression:
                    //text.Append(expression);
                    break;
            }
            text.Append(" FROM PivotTable");
            text.Append(" WHERE (DateSale >= @Start AND DateSale < @End) AND (" + ddlYGroupBy.SelectedValue + " = '" + serie + "') ");
            //text.Append(" FROM PivotTable");

            command.CommandText = text.ToString();
            command.Parameters.Add(new SqlParameter("@Start", SqlDbType.DateTime) { Value = start });
            command.Parameters.Add(new SqlParameter("@End", SqlDbType.DateTime) { Value = end });

            //Response.End();
            return command;
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            //Response.Write(q.ToString());

            //SqlCommand
            //SqlConnection
            //PivotTableTableAdapter ptta = new PivotTableTableAdapter();
            //PivotTable data = new PivotTable();
            //ptta.Fill(data._PivotTable);


            DrawChart(null, new DateTime(2010, 1, 1), new DateTime(2010, 12, 31), IntervalType.Month, ValueRepresentation.AbsoluteValues);
        }

        protected void ddlYFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlYFilterType.SelectedIndex == 1)
            {
                lbYFilter.Visible = false;
            }
            else
            {
                lbYFilter.Visible = true;
            }
            
        }
    }
}
