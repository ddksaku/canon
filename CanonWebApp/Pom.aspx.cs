using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Web;
using CanonWebApp.Code;
using System.Collections.Generic;

namespace CanonWebApp
{
    public partial class Pom : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebChartControl chart = new WebChartControl();
            chart.EnableViewState = false;
            ChartTitle title = new ChartTitle();
            title.Text = string.Format(Utilities.GetResourceString("Common", "ChartHeader"),
                                       "Jmeno", "Start 10.12.2010","Finish 17.12.2010");
            title.Font = new System.Drawing.Font("Tahoma", 10);
            chart.Titles.Add(title);
            chart.FillStyle.FillMode = FillMode.Solid;

            Dictionary<int, Series> series = new Dictionary<int, Series>();

            Series s0 = new Series("Recommended price", ViewType.StackedBar);
            s0.ValueScaleType = ScaleType.Numerical;
            s0.ArgumentScaleType = ScaleType.DateTime;
            series.Add(0, s0);
            
            SetGraphPoint(series[0], "rnd");

            Series s1 = new Series("Actual price", ViewType.Line);
            s1.ValueScaleType = ScaleType.Numerical;
            s1.ArgumentScaleType = ScaleType.DateTime;
            series.Add(1, s1);

            SetGraphPoint(series[1], "fib");

            //Series s2 = new Series("Predicted price", ViewType.Bubble);
            //s2.ValueScaleType = ScaleType.Numerical;
            //s2.ArgumentScaleType = ScaleType.DateTime;
            //series.Add(2, s2);

            //SetGraphPoint(series[2], "byte");

            foreach (KeyValuePair<int, Series> serie in series)
                chart.Series.Add(serie.Value);

            //chart.SeriesTemplate.ShowInLegend = false;
            chart.Legend.Direction = LegendDirection.LeftToRight;
            chart.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.Center;
            chart.Legend.AlignmentVertical = LegendAlignmentVertical.Bottom;

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
            chart.Diagram = xyDiagram;
            chart.Width = 700;
            chart.Height = 500;
            //chart.DataBind();
            phMain.Controls.Add(chart);
        }

        public void SetGraphPoint(Series serie, string type)
        {
            for (int i = 0; i < 20; i++)
            {
                if (type == "rnd")
                {
                    serie.Points.Add(new SeriesPoint(DateTime.Now.AddDays(i), new double[] { (double)GetRandom(i) }));
                }
                else if (type == "fib")
                {
                    serie.Points.Add(new SeriesPoint(DateTime.Now.AddDays(i), new double[] { (double)Fibonacci(i) }));
                }
                else if (type == "byte")
                {
                    serie.Points.Add(new SeriesPoint(DateTime.Now.AddDays(i), new double[] { (double)GetRandomBytes(i) }));
                }
            }
        }

        public int GetRandomBytes(int index)
        {
            byte[] randBytes = new byte[108];

            Random randNum = new Random();

            randNum.NextBytes(randBytes);

            return randBytes[index] * 4;
        }

        public int GetRandom(int seed)
        {
            Random rnd = new Random(seed);
            return (rnd.Next(1000) % 7) * 531;
        }

        public int Fibonacci(int x)
        {
            if (x <= 1)
                return 1;
            return Fibonacci(x - 1) + Fibonacci(x - 2);
        }
    }
}
