using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses;
using CanonWebApp.Code;
using Canon.Data;
using System.Data;
using Canon.Data.Business;
using DevExpress.Web.ASPxGridView;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using DevExpress.Web.ASPxEditors;
using Memos.Framework.Logging;
using CanonWebApp.Controls;

namespace CanonWebApp
{
    public partial class _Default : BasePage
    {
        private const string COOKIE_FILTER_NAME = "MAIN_MONITOR_FILTER_VALUE";
        private MainMonitorFilter Filter = null;
        private List<MappingRule> mappingRules = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("Reporter.aspx");

            Localize();
            if ((!this.IsCallback) && (!this.IsPostBack))
            {
                deDate.Date = DateTime.Now;
                deDate.MaxDate = DateTime.Now;
                deExportStartDate.Date = DateTime.Now.AddDays(-7);
                deExportFinishDate.Date = DateTime.Now;
                deExportStartDate.MaxDate = DateTime.Now;
                deExportFinishDate.MaxDate = DateTime.Now;
            }
            Bind();
        }

        #region Localize
        /// <summary>
        /// Localizes text depending on selected language.
        /// </summary>
        private void Localize()
        {
            popupExcelExport.HeaderText = Utilities.GetResourceString("Headers", "ExportToExcelHeader");
            deDate.CalendarProperties.TodayButtonText = Utilities.GetResourceString("Common", "Today");
            deExportFinishDate.CalendarProperties.TodayButtonText = Utilities.GetResourceString("Common", "Today");
            deExportStartDate.CalendarProperties.TodayButtonText = Utilities.GetResourceString("Common", "Today");
        }

        public string GetEmptySelectionMessage()
        {
            return Utilities.GetResourceString("Validators", "EmptySelectionError"); ;
        }

        protected string LocalizeColumnName(string fieldName)
        {
            switch (fieldName)
            {
                case "ProductCode":
                    return Utilities.GetResourceString("Common", "ProductEan");
                case "ProductName":
                    return Utilities.GetResourceString("Common", "ProductName");
                case "RecommendedPrice":
                    return Utilities.GetResourceString("Common", "RecommendedPrice");
                default:
                    return fieldName;
            }
        }
        #endregion

        #region Binding
        protected void Bind()
        {
            if ((!this.IsCallback) && (!this.IsPostBack))
            {
                //Restore filter from cookies
                this.LoadFilterFromCookies();
                this.InitialFilterBind();
                SessionManager.CurrentCategories = this.GetListOfCategories();
            }
            //Get main results
            try
            {
                List<int> channelIds = (SessionManager.CurrentChannels.Count==0)?this.GetListOfChannels()
                                                                                :SessionManager.CurrentChannels;
                List<int> categoryIds = (SessionManager.CurrentCategories.Count==0)?this.GetListOfCategories()
                                                                                :SessionManager.CurrentCategories;
                SessionManager.CurrentChannels = channelIds;
                SessionManager.CurrentCategories = categoryIds;

                CanonMainMonitor monitor = new CanonMainMonitor(Cdb.ConnectionString);
                monitor.Date = deDate.Date;
                monitor.ChannelIds = channelIds;
                monitor.PriceCondition = this.GetPriceCondition();
                if (categoryIds != null)
                    monitor.CategoryIds = categoryIds;
                if (!string.IsNullOrEmpty(txtProduct.Text.Trim()))
                    monitor.ProductName = Memos.Framework.Utilities.TruncateString(txtProduct.Text.Trim(), 300);
                mappingRules = monitor.GetMappingRules();
                DataSet ds = monitor.GetMainMonitorValues();
                //get sorting settings before columns clearing
                DevExpress.Data.ColumnSortOrder sortOrder = DevExpress.Data.ColumnSortOrder.None;
                string sortColumn = string.Empty;
                foreach (GridViewColumn gvc in gridMainMonitor.Columns)
                {
                    GridViewDataColumn gvdc = gvc as GridViewDataColumn;
                    if (gvdc != null)
                        if (gvdc.SortOrder != DevExpress.Data.ColumnSortOrder.None)
                        {
                            sortColumn = gvdc.FieldName;
                            sortOrder = gvdc.SortOrder;
                        }
                }
                gridMainMonitor.Columns.Clear();
                GridViewCommandColumn col0 = new GridViewCommandColumn();
                col0.ShowSelectCheckbox = true;
                col0.VisibleIndex = 0;
                col0.HeaderTemplate = new CommandColumnHeaderTemplate(gridMainMonitor, "selCheckbox", "gridMainMonitorSelectionBox");
                col0.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                col0.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                gridMainMonitor.Columns.Add(col0);
                int visIndex = 1;
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    GridViewDataTextColumn col1 = new GridViewDataTextColumn();
                    col1.Caption = this.LocalizeColumnName(dc.ColumnName);
                    col1.FieldName = dc.ColumnName;
                    col1.VisibleIndex = visIndex++;
                    if (dc.ColumnName == "ProductId")
                        col1.Visible = false;
                    if ((!string.IsNullOrEmpty(sortColumn)) && (col1.FieldName == sortColumn))
                        col1.SortOrder = sortOrder;
                    col1.Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
                    gridMainMonitor.Columns.Add(col1);
                }
                gridMainMonitor.SettingsPager.PageSize = int.Parse(cbPageSize.SelectedItem.Value.ToString());
                gridMainMonitor.DataSource = ds.Tables[0];
                gridMainMonitor.DataBind();
                gridMainMonitor.JSProperties["cpRowsCount"] = gridMainMonitor.VisibleRowCount;
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
                gridMainMonitor.Columns.Clear();
                gridMainMonitor.DataSource = null;
                gridMainMonitor.DataBind(); 
            }
        }

        protected PriceConditions GetPriceCondition()
        {
            if ((rblShopTypes.SelectedIndex == 1) && (rblPriceTypes.SelectedIndex == 1))
            {
                switch (cbConditions.SelectedIndex)
                {
                    case 1:
                        return PriceConditions.RecommendedLess;
                    case 2:
                        return PriceConditions.RecommendedMore;
                    case 3:
                        return PriceConditions.RecommendedNotEqual;
                }
            }
            else
            {
                switch (cbConditions.SelectedIndex)
                {
                    case 1:
                        return PriceConditions.RecommendedMore;
                    case 2:
                        return PriceConditions.RecommendedLess;
                    case 3:
                        return PriceConditions.RecommendedNotEqual;
                }
            }
            return PriceConditions.All;
        }

        protected List<int> GetListOfChannels()
        {
            CanonDataContext db = Cdb.Instance;
            List<int> list = new List<int>();
            foreach (ListItem li in lstShops.Items)
                if ((li.Value == "0") && (li.Selected))
                {
                    //put all ids of defined channel type
                    List<Channel> channels = db.Channels.Where(c => c.IsActive == true &&
                                c.ChannelType == int.Parse(rblShopTypes.SelectedItem.Value.ToString())).ToList();
                    foreach (Channel channel in channels)
                        list.Add(channel.ChannelId);
                    return list;
                }
                else if (li.Selected)
                    list.Add(int.Parse(li.Value));
            if (list.Count == 0)
            {
                //put all ids of defined channel type
                List<Channel> channels = db.Channels.Where(c => c.IsActive == true &&
                            c.ChannelType == int.Parse(rblShopTypes.SelectedItem.Value.ToString())).ToList();
                foreach (Channel channel in channels)
                    list.Add(channel.ChannelId);
            }
            return list;
        }

        protected List<int> GetListOfCategories()
        {
            List<int> list = new List<int>();
            List<int> flist = new List<int>();
            bool areAll = false;
            foreach (ListItem li in lstCategories.Items)
            {
                if ((li.Value == "0") && (li.Selected))
                    areAll = true;
                else if (li.Selected)
                    list.Add(int.Parse(li.Value));
                flist.Add(int.Parse(li.Value));
            }
            if (areAll)
                return flist;
            if (list.Count == 0)
                return null;
            return list;
        }

        protected void BindPrices()
        {
            rblPriceTypes.Items.Clear();
            if (rblShopTypes.SelectedIndex <= 0) //shops
            {
                rblPriceTypes.Items.Add(Utilities.GetResourceString("Common", "AbsolutePrices"), 0);
                rblPriceTypes.Items.Add(Utilities.GetResourceString("Common", "RelativePrices"), 1);
            }
            else //distributors
            {
                rblPriceTypes.Items.Add(Utilities.GetResourceString("Common", "PriceWithVat"), 0);
                rblPriceTypes.Items.Add(Utilities.GetResourceString("Common", "Margin"), 1);
            }
            rblPriceTypes.SelectedIndex = 0;
        }

        protected void BindChannelTypes()
        {
            CanonDataContext db = Cdb.Instance;
            rblShopTypes.Items.Clear();
            List<Canon.Data.Enum> ens = db.Enums.Where(e=> e.EnumType==3).OrderByDescending(e=> e.EnumId).ToList();
            foreach(Canon.Data.Enum en in ens)
                rblShopTypes.Items.Add((SessionManager.CurrentShortLanguage=="En")?en.NameEn:en.NameCz, en.EnumId);
            rblShopTypes.SelectedIndex = 0;
        }

        protected void BindChannels()
        {
            lstShops.Items.Clear();
            CanonDataContext db = Cdb.Instance;
            List<Channel> channels = db.Channels.Where(c => c.IsActive==true && 
                c.ChannelType == int.Parse(rblShopTypes.SelectedItem.Value.ToString())).ToList();
            lstShops.Items.Add(new ListItem(Utilities.GetResourceString("Common", "All"), "0"));
            foreach (Channel channel in channels)
                lstShops.Items.Add(new ListItem(channel.ChannelName, channel.ChannelId.ToString()));
            lstShops.SelectedIndex = 0;
        }

        protected void BindConditions()
        {
            cbConditions.Items.Clear();
            cbConditions.Items.Add(Utilities.GetResourceString("Common", "All"), 0);
            cbConditions.Items.Add(Utilities.GetResourceString("Common", "NegativeDifferencesOnly"), 1);
            cbConditions.Items.Add(Utilities.GetResourceString("Common", "PositiveDifferencesOnly"), 2);
            cbConditions.Items.Add(Utilities.GetResourceString("Common", "DefferencesOnly"), 3);
            cbConditions.SelectedIndex = 0;
        }

        protected void BindPageSizes()
        {
            cbPageSize.Items.Clear();
            cbPageSize.ValueType = Type.GetType("int");
            cbPageSize.Items.Add(Utilities.GetResourceString("Common", "Rows10"), 10);
            cbPageSize.Items.Add(Utilities.GetResourceString("Common", "Rows50"), 50);
            cbPageSize.Items.Add(Utilities.GetResourceString("Common", "Rows100"), 100);
            cbPageSize.Items.Add(Utilities.GetResourceString("Common", "Rows500"), 500);
            cbPageSize.Items.Add(Utilities.GetResourceString("Common", "Rows1000"), 1000);
            cbPageSize.SelectedIndex = 0;
        }

        protected void BindCategories()
        {
            CanonDataContext db = Cdb.Instance;
            lstCategories.Items.Clear();
            lstCategories.Items.Add(new ListItem(Utilities.GetResourceString("Common", "All"), "0"));
            List<UsersCategory> lst = db.UsersCategories.Where(c=> c.UserId==SessionManager.LoggedUser.UserId).ToList();
            foreach (UsersCategory ctg in lst)
                lstCategories.Items.Add(new ListItem(ctg.Category.CategoryName, ctg.CategoryId.ToString()));
            lstCategories.SelectedIndex = 0;
        }

        protected void BindShopsLabel()
        {
            if (rblShopTypes.SelectedIndex == 0)
                lblShops.Text = Utilities.GetResourceString("Common", "ShopsLabel");
            else
                lblShops.Text = Utilities.GetResourceString("Common", "DistributorsLabel");
        }
        #endregion

        #region Panel Callback
        /// <summary>
        /// Panel callback processing
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void DefaultPageCallbackPanel_Callback(object source, CallbackEventArgsBase e)
        {
            if ((!e.Parameter.StartsWith("ShowChart")) && (!e.Parameter.StartsWith("ExportToExcel")))
                return;
            List<int> channels = (SessionManager.CurrentChannels.Count == 0) ? this.GetListOfChannels()
                                                                : SessionManager.CurrentChannels;
            List<int> products = new List<int>();
            List<object> keyValues = gridMainMonitor.GetSelectedFieldValues("ProductId");
            foreach (object key in keyValues)
                products.Add(int.Parse(key.ToString()));
            if ((channels.Count == 0) || (products.Count == 0))
                return;
            if (e.Parameter.StartsWith("ShowChart"))
            {
                string productsParam = string.Format("products={0}", 
                                                     Memos.Framework.Utilities.ListToString(products));
                string channelsParam = string.Format("channels={0}",
                                                     Memos.Framework.Utilities.ListToString(channels));
                ASPxWebControl.RedirectOnCallback(string.Format("~/Charts.aspx?{0}&{1}", 
                                                                productsParam, channelsParam));
            }
            else if (e.Parameter.StartsWith("ExportToExcel"))
            {
                string filename = this.GenerateExcelFile(channels, products);
                string fileUrl = string.Format("{0}/{1}", ConfigSettings.UploadDirectory, filename);
                DefaultPageCallbackPanel.JSProperties["cpResult"] = fileUrl;
            }
        }
        #endregion

        #region Control event handlers
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            List<int> channelList = this.GetListOfChannels();
            List<int> categoryList = this.GetListOfCategories();
            SessionManager.CurrentChannels = channelList;
            SessionManager.CurrentCategories = categoryList;
            //Save filter into cookies
            this.SaveFilterToCookies();
            //Bind grid
            this.Bind();
        }

        protected void rblShopTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ShopTypeIsChanged();
            Bind();
        }

        protected void ShopTypeIsChanged()
        {
            BindChannels();
            BindPrices();
            List<int> channelList = this.GetListOfChannels();
            List<int> categoryList = this.GetListOfCategories();
            SessionManager.CurrentChannels = channelList;
            SessionManager.CurrentCategories = categoryList;
            BindShopsLabel();
        }
        #endregion

        #region Grid events
        protected void gridMainMonitor_HtmlDataCellPrepared(object sender, DevExpress.Web.ASPxGridView.ASPxGridViewTableDataCellEventArgs e)
        {
            int recommendedIndex = 4;
            string currentCulture = SessionManager.CurrentLanguage;
            if (e.DataColumn.Index == recommendedIndex)
            {
                decimal recommended = 0;
                decimal.TryParse(gridMainMonitor.GetDataRow(e.VisibleIndex)[e.DataColumn.Index - 1].ToString(),
                    NumberStyles.Any, CultureInfo.CreateSpecificCulture(currentCulture), out recommended);
                if (recommended > 0)
                    e.Cell.Text = string.Format("{0}", recommended.ToString("N2",
                        CultureInfo.CreateSpecificCulture(SessionManager.CurrentLanguage)));
                else
                    e.Cell.Text = "&nbsp;";
            }
            else if (e.DataColumn.Index >= recommendedIndex)
            {
                decimal recommended = 0;
                decimal current = 0;

                decimal.TryParse(gridMainMonitor.GetDataRow(e.VisibleIndex)[3].ToString(),
                    NumberStyles.Any, CultureInfo.CreateSpecificCulture(currentCulture), out recommended);
                decimal.TryParse(gridMainMonitor.GetDataRow(e.VisibleIndex)[e.DataColumn.Index - 1].ToString(),
                    NumberStyles.Any, CultureInfo.CreateSpecificCulture(currentCulture), out current);

                if ((recommended > 0) && (current != 0))
                {
                    decimal result = 0;
                    decimal percent = 0;

                    this.CalculateMonitorValues(current, recommended, out result, out percent);

                    //change colors
                    int cc = this.GetColorCombination(current, recommended);
                    int selCond = cbConditions.SelectedIndex;

                    if ((cc == 1) || (cc == 11) || (cc == 101) || (cc == 110))
                        if (selCond == 1)
                            e.Cell.ForeColor = Color.Black;
                        else
                            e.Cell.ForeColor = Color.Green;
                    else if ((cc == 0) || (cc == 10) || (cc == 100) || (cc == 111))
                        if (selCond == 2)
                            e.Cell.ForeColor = Color.Black;
                        else
                            e.Cell.ForeColor = Color.Red;
                    else
                        e.Cell.ForeColor = Color.Black;

                    //make links
                    if ((current-recommended)==0)
                        e.Cell.Text = this.GetUrlForValue(string.Format("{0}", result.ToString("N2",
                            CultureInfo.CreateSpecificCulture(SessionManager.CurrentLanguage))),
                            e.DataColumn.Caption, (int)e.KeyValue, e.Cell.ForeColor);
                    else
                        e.Cell.Text = this.GetUrlForValue(string.Format("{0} ({1}%)", result.ToString("N2",
                            CultureInfo.CreateSpecificCulture(SessionManager.CurrentLanguage)),
                            percent.ToString("N2")),
                            e.DataColumn.Caption, (int)e.KeyValue, e.Cell.ForeColor);
                }
                else if ((recommended == 0) || (current == 0))
                    e.Cell.Text = "&nbsp;";
            }
        }

        protected int GetColorCombination(decimal feed, decimal recom)
        {
            int result = 0;
            if (rblShopTypes.SelectedIndex == 1) result += 100;
            if (rblPriceTypes.SelectedIndex == 1) result += 10;
            if (feed > recom) result += 1;
            else if (feed == recom) result += 2;
            return result;
        }

        protected string GetUrlForValue(string displayText, string columnCaption, int productId, Color color)
        {
            if ((this.mappingRules == null) || (this.mappingRules.Count == 0))
                return displayText;
            foreach (MappingRule mr in this.mappingRules)
                if ((mr.ProductId == productId) && (mr.Channel.ChannelName == columnCaption))
                    return string.Format("<a href='{0}' target='_blank' style='color:{2};'>{1}</a>",
                        mr.MonitoredUrl, displayText, System.Drawing.ColorTranslator.ToHtml(color));
            return displayText;
        }

        protected void gridMainMonitor_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            try
            {
                if (e.Column.VisibleIndex >= 4)
                {
                    if ((e.Value1 == null) && (e.Value2 == null)) { e.Result = 0; e.Handled = true; return; }
                    else if (e.Value1 == null) { e.Result = -1; e.Handled = true; return; }
                    else if (e.Value2 == null) { e.Result = 1; e.Handled = true; return; }
                    decimal res1 = 0;
                    decimal perc1 = 0;
                    decimal res2 = 0;
                    decimal perc2 = 0;
                    this.CalculateMonitorValues(
                         (decimal)e.Value1, (decimal)e.GetRow1Value("RecommendedPrice"),
                         out res1, out perc1);
                    this.CalculateMonitorValues(
                         (decimal)e.Value2, (decimal)e.GetRow2Value("RecommendedPrice"),
                         out res2, out perc2);
                    if (res1 > res2) e.Result = 1;
                    else if (res1 < res2) e.Result = -1;
                    else e.Result = 0;
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void CalculateMonitorValues(decimal current, decimal recommended,
                                              out decimal result, out decimal percent)
        {
            if (rblPriceTypes.SelectedIndex == 0)
            {
                if (current == 0)
                {
                    result = 0;
                    percent = 0;
                    return;
                }
                result = current;
                percent = (recommended != 0) ? result / recommended * 100 : 0;
            }
            else if ((rblShopTypes.SelectedIndex == 0) && (rblPriceTypes.SelectedIndex == 1))
            {
                if (recommended == 0)
                {
                    result = 0;
                    percent = 0;
                    return;
                }
                result = current - recommended;
                percent = (recommended != 0) ? result / recommended * 100 : 0;
            }
            else
            {
                if (recommended == 0)
                {
                    result = 0;
                    percent = 0;
                    return;
                }
                result = recommended - current;
                percent = (recommended != 0) ? result / recommended * 100 : 0;
            }
        }

        protected void gridMainMonitor_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                //select all functionality
                if (e.Parameters.StartsWith("SelectAll"))
                {
                    string value = e.Parameters.Replace("SelectAll", "");
                    int count = gridMainMonitor.VisibleRowCount;
                    for (int i = 0; i < count; i++)
                        if (value.ToLower() == "true")
                            gridMainMonitor.Selection.SelectRow(i);
                        else
                            gridMainMonitor.Selection.UnselectRow(i);
                    //check header's checkbox
                    if (value.ToLower() == "true")
                    {
                        ASPxCheckBox checkbox = (ASPxCheckBox)gridMainMonitor.FindHeaderTemplateControl(gridMainMonitor.Columns[0],
                                                "selCheckbox");
                        if (checkbox != null)
                            checkbox.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }

        protected void gridMainMonitor_PageIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int count = gridMainMonitor.VisibleRowCount;
                bool allSelected = true;
                for (int i = 0; i < count; i++)
                    allSelected = allSelected && gridMainMonitor.Selection.IsRowSelected(i);
                //check header's checkbox
                if (allSelected)
                {
                    ASPxCheckBox checkbox = (ASPxCheckBox)gridMainMonitor.FindHeaderTemplateControl(gridMainMonitor.Columns[0],
                                            "selCheckbox");
                    if (checkbox != null)
                        checkbox.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
            }
        }
        #endregion

        #region Excel generation
        protected string GenerateExcelFile(List<int> channels, List<int> products)
        {
            string filename = string.Format("Canon-Export-{0}-{1}-{2}.xls", 
                                            deExportStartDate.Date.ToString("dd.MM.yyyy"),
                                            deExportFinishDate.Date.ToString("dd.MM.yyyy"),
                                            SessionManager.LoggedUser.UserId);
            string fullname = Path.Combine(Server.MapPath(ConfigSettings.UploadDirectory), filename);
            //Save records to Excel file
            using (MemXlsWriter xlsWriter = new MemXlsWriter(fullname, false))
            {
                xlsWriter.CultureToSaveDecimals = "en-GB";//SessionManager.CurrentLanguage;
                xlsWriter.XlsWriteHeaderManySheets();
                DateTime finish = deExportFinishDate.Date.Date;
                for (DateTime i = deExportStartDate.Date.Date; i <= finish; i=i.AddDays(1))
                {
                    DataTable dayResults = this.GenerateExcelData(channels, products, i);
                    dayResults = this.ModifyExcelResults(dayResults);
                    if (dayResults == null) continue;
                    xlsWriter.XlsWriteDataTable(dayResults, 1, " ", i.ToString("dd.MM.yyyy"));
                }
                xlsWriter.XlsWriteFooterManySheets();
            }
            return filename;
        }

        protected DataTable GenerateExcelData(List<int> channels, List<int> products, DateTime date)
        {
            CanonMainMonitor monitor = new CanonMainMonitor(Cdb.ConnectionString);
            monitor.Date = date;
            monitor.ChannelIds = channels;
            monitor.ProductIds = products;
            monitor.PriceCondition = this.GetPriceCondition();
            DataSet ds = monitor.GetMainMonitorValues();
            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            return null;
        }

        protected DataTable ModifyExcelResults(DataTable dt)
        {
            string currentCulture = SessionManager.CurrentLanguage;
            DataTable newDt = new DataTable();
            dt.Columns.RemoveAt(1);
            dt.Columns.RemoveAt(0);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                newDt.Columns.Add(new DataColumn(dt.Columns[j].ColumnName, typeof(string)));
                if (j >= 2)
                    newDt.Columns.Add(new DataColumn(string.Format("{0}, %", dt.Columns[j].ColumnName), 
                                                     typeof(string)));
            }
            foreach (DataRow dr in dt.Rows)
            {
                DataRow ndr = newDt.NewRow();
                decimal recommended = 0;
                decimal.TryParse(dr[1].ToString(),
                    NumberStyles.Any, CultureInfo.CreateSpecificCulture(currentCulture), out recommended);
                ndr[0] = dr[0].ToString();//product name
                ndr[1] = string.Format("{0}", recommended.ToString("N2",
                            CultureInfo.CreateSpecificCulture("cs-CZ")));//recommended price
                for (int i = 2; i < dt.Columns.Count; i++)
                {
                    decimal current = 0;
                    decimal.TryParse(dr[i].ToString(),
                        NumberStyles.Any, CultureInfo.CreateSpecificCulture(currentCulture), out current);

                    if ((recommended == 0) || (current == 0))
                    {
                        ndr[i] = string.Empty;
                        continue;
                    }
                    
                    decimal result = 0;
                    decimal percent = 0;
                    this.CalculateMonitorValues(current, recommended, out result, out percent);

                    if ((current - recommended) == 0)
                    {
                        ndr[2 * i - 2] = string.Format("{0}", result.ToString("N2",
                            CultureInfo.CreateSpecificCulture("cs-CZ")));
                    }
                    else
                    {
                        ndr[2 * i - 2] = string.Format("{0}", result.ToString("N2",
                            CultureInfo.CreateSpecificCulture("cs-CZ")));
                        ndr[2 * i - 2 + 1] = string.Format("{0}", percent.ToString("N2",
                            CultureInfo.CreateSpecificCulture("cs-CZ")));
                    }

                }
                newDt.Rows.Add(ndr);
            }
            return newDt;
        }
        #endregion

        #region Cookies
        protected void SaveFilterToCookies()
        {
            MainMonitorFilter filter = new MainMonitorFilter();
            filter.Categories = SessionManager.CurrentCategories;
            filter.Shops = SessionManager.CurrentChannels;
            filter.ChannelType = rblShopTypes.SelectedIndex;
            filter.Condition = cbConditions.SelectedIndex;
            filter.PriceType = rblPriceTypes.SelectedIndex;
            filter.ProductFilter = txtProduct.Text;
            filter.PageSize = cbPageSize.SelectedIndex;
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, filter);
            string str = System.Convert.ToBase64String(ms.ToArray());
            HttpCookie value = new HttpCookie(COOKIE_FILTER_NAME);
            value.Value = str;
            value.Expires = DateTime.Now.AddDays(90);
            HttpContext.Current.Response.Cookies.Add(value);
        }

        protected void LoadFilterFromCookies()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[COOKIE_FILTER_NAME];
            if (cookie == null) return;
            string str = cookie.Value;
            byte[] byteState = System.Convert.FromBase64CharArray(str.ToCharArray(),0,str.Length);
            MemoryStream ms = new MemoryStream(byteState);
            BinaryFormatter bf1 = new BinaryFormatter();
            this.Filter = (MainMonitorFilter)bf1.Deserialize(ms);
        }

        protected void InitialFilterBind()
        {
            //Bind shop types
            BindChannelTypes();
            if (this.Filter != null)
                rblShopTypes.SelectedIndex = this.Filter.ChannelType;
            //Bind Channels
            BindChannels();
            if (this.Filter != null)
                if ((this.Filter.Shops != null) && (this.Filter.Shops.Count > 0))
                {
                    for (int i = 0; i < lstShops.Items.Count; i++)
                        foreach (int cat in this.Filter.Shops)
                            if (lstShops.Items[i].Value == cat.ToString())
                                lstShops.Items[i].Selected = true;
                    lstShops.Items[0].Selected = false;
                }
            //Bind Prices
            BindPrices();
            if (this.Filter != null)
                rblPriceTypes.SelectedIndex = this.Filter.PriceType;
            //Bind others
            if (this.Filter != null)
                txtProduct.Text = this.Filter.ProductFilter;
            BindConditions();
            if (this.Filter != null)
                cbConditions.SelectedIndex = this.Filter.Condition;
            BindPageSizes();
            if (this.Filter != null)
                cbPageSize.SelectedIndex = this.Filter.PageSize;
            //Bind categories
            BindCategories();
            if (this.Filter != null)
                if ((this.Filter.Categories != null) && (this.Filter.Categories.Count > 0))
                {
                    for (int i = 0; i < lstCategories.Items.Count; i++)
                        foreach (int cat in this.Filter.Categories)
                            if (lstCategories.Items[i].Value == cat.ToString())
                                lstCategories.Items[i].Selected = true;
                    lstCategories.Items[0].Selected = false;
                }
            BindShopsLabel();
        }
        #endregion
    }
}
