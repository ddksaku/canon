using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Canon.Data;
using Canon.Data.Business;
using DevExpress.XtraCharts.Web;
using DevExpress.XtraCharts;
using System.IO;
using DevExpress.XtraCharts.Native;
using System.Drawing.Imaging;
using Memos.Framework;
using CanonWebApp.Code;
using Config=CanonWebApp.Code.ConfigSettings;
using Sess=CanonWebApp.Code.SessionManager;
using Utilities=CanonWebApp.Code.Utilities;

namespace CanonWebApp.Controls
{
    public partial class ChartExporter : System.Web.UI.UserControl
    {
        private List<string> _files = new List<string>();

        private List<string> FilesToSave
        {
            get { return _files; }
        }

        private List<int> Channels
        {
            get
            {
                List<int> res = new List<int>();
                if (this.Request.Params["channels"] == null)
                    return res;
                string[] channels = this.Request.Params["channels"].ToString().Split(',');
                foreach (string channel in channels)
                {
                    int id;
                    if (int.TryParse(channel, out id))
                        res.Add(id);
                }
                return res;
            }
        }

        private List<int> Products
        {
            get
            {
                List<int> res = new List<int>();
                if (this.Request.Params["products"] == null)
                    return res;
                string[] products = this.Request.Params["products"].ToString().Split(',');
                foreach (string product in products)
                {
                    int id;
                    if (int.TryParse(product, out id))
                        res.Add(id);
                }
                return res;
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            _files.Clear();
            Localize();
            WebChartControl.ProcessImageRequest(this.Page);
            if ((!this.Page.IsPostBack) && (!this.Page.IsCallback))
            {
                deStartDate.Date = DateTime.Now.AddDays(-7);
                deFinishDate.Date = DateTime.Now;
                deStartDate.MaxDate = DateTime.Now;
                deFinishDate.MaxDate = DateTime.Now;
            }
            panelPics.Controls.Clear();
            Bind();
        }

        protected void Localize()
        {
            deStartDate.CalendarProperties.TodayButtonText = Utilities.GetResourceString("Common", "Today");
            deFinishDate.CalendarProperties.TodayButtonText = Utilities.GetResourceString("Common", "Today");
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
        }

        protected void btnSavePictures_Click(object sender, EventArgs e)
        {
            //create zip package and redirect to it
            string zipShortFilename = string.Format("Charts-{0}-{1}.zip",
                deStartDate.Date.ToString("dd.MM.yyyy"),
                deFinishDate.Date.ToString("dd.MM.yyyy"));
            string zipFilename = Path.Combine(Server.MapPath(Config.UploadDirectory), zipShortFilename);
            FileInfo[] files = new FileInfo[this.FilesToSave.Count];
            for (int i = 0; i < files.Length; i++)
                files[i] = new FileInfo(this.FilesToSave[i]);
            ZipManager.GenerateZipFile(files, zipFilename);
            Response.Redirect(string.Format("~/{0}/{1}", Config.UploadDirectory, zipShortFilename));
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default.aspx");
        }

        public void Bind()
        {
            List<int> channels = this.Channels;
            List<int> products = this.Products;
            CanonDataContext db = Cdb.Instance;
            List<MainMonitor> list = CanonMainMonitor.GetValuesForChart(deStartDate.Date,
                                               deFinishDate.Date, channels, products);
            foreach (int productId in products)
            {
                Product product = db.Products.Where(p => p.ProductId == productId).FirstOrDefault();
                if (product == null) continue;
                WebChartControl chart = new WebChartControl();
                chart.EnableViewState = false;
                ChartTitle title = new ChartTitle();
                title.Text = string.Format(Utilities.GetResourceString("Common", "ChartHeader"),
                                           product.ProductName, deStartDate.Date.ToString("dd.MM.yyyy"),
                                           deFinishDate.Date.ToString("dd.MM.yyyy"));
                title.Font = new System.Drawing.Font("Tahoma", 10);
                chart.Titles.Add(title);
                chart.FillStyle.FillMode = FillMode.Solid;
                Dictionary<int, Series> series = new Dictionary<int, Series>();
                Series s0 = new Series("Recommended price", ViewType.Line);
                s0.ValueScaleType = ScaleType.Numerical;
                s0.ArgumentScaleType = ScaleType.DateTime;
                series.Add(0, s0);
                foreach (int channelId in channels)
                {
                    Channel channel = db.Channels.Where(c=> c.ChannelId==channelId).FirstOrDefault();
                    if (channel == null) continue;
                    Series s = new Series(channel.ChannelName, ViewType.Line);
                    s.ValueScaleType = ScaleType.Numerical;
                    s.ArgumentScaleType = ScaleType.DateTime;
                    series.Add(channelId, s);
                }
                foreach (MainMonitor mm in list)
                {
                    if (mm.ProductId != productId) continue;
                    series[mm.ChannelId].Points.Add(new SeriesPoint(mm.CalcDate, new double[] { (double)mm.ChannelPrice }));
                    series[0].Points.Add(new SeriesPoint(mm.CalcDate, new double[] { (double)mm.RecommendedPrice }));
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
                chart.Diagram = xyDiagram;
                chart.Width = 700;
                chart.Height = 500;
                chart.DataBind();
                panelPics.Controls.Add(chart);

                this.FilesToSave.Add(this.SaveChartIntoFile(product.ProductName, chart));
            }
        }

        protected string SaveChartIntoFile(string productName, WebChartControl chart)
        {
            string filename = string.Format("{0}_{1}.jpg", Sess.LoggedUser.UserId, CleanForbiddenSymbols(productName));
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

        protected string CleanForbiddenSymbols(string initial)
        {
            string result = initial;
            string[] forbidden = new string[] { @"\", @"/", @":", @"*", @"?", "\"", @"<", @">", @"|"};
            foreach (string forb in forbidden)
                result = result.Replace(forb, " ");
            return result.Trim();
        }
    }
}