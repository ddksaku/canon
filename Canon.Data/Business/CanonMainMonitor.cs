using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;

namespace Canon.Data.Business
{
    public class CanonMainMonitor
    {
        private string querySkeleton = @"SELECT ProductId, ProductCode, ProductName, RecommendedPrice {0}
                                FROM 
                                (SELECT t2.ProductId, t2.ProductCode, t2.ProductName, t1.RecommendedPrice, t1.ChannelId, t1.ChannelPrice
                                FROM MainMonitor t1 
                                INNER JOIN Products t2 ON t1.ProductId=t2.ProductId
                                WHERE dbo.GetDateOnly(CalcDate)='{1}' {2} {5} {3} {6} {7}
                                ) p
                                PIVOT
                                (
                                AVG(channelprice)
                                FOR channelid IN
                                ( {4} )
                                ) AS pvt
                                ORDER BY pvt.ProductName";
        public string ConnectionString { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<int> ChannelIds { get; set; }
        public List<int> ProductIds { get; set; }
        public DateTime Date { get; set; }
        public string ProductName { get; set; }
        public PriceConditions PriceCondition { get; set; }

        public CanonMainMonitor(string conn)
        {
            this.ConnectionString = conn;
        }

        public DataSet GetMainMonitorValues()
        {
            string query = this.BuildQuery();
            if (WebVariables.Logger != null) 
                WebVariables.Logger.Debug(query);
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds);
            connection.Close();
            return ds;
        }

        public List<MappingRule> GetMappingRules()
        {
            if ((ChannelIds == null) || (ChannelIds.Count == 0))
                return null;
            CanonDataContext db = Cdb.Instance;
            List<MappingRule> maps = db.MappingRules.Where(m => ChannelIds.Contains(m.ChannelId)).ToList();
            return maps;
        }

        public string BuildQuery()
        {
            string query = string.Format(this.querySkeleton, this.GetFullChannelsList(), 
                                                             this.Date.ToString("yyyy-MM-dd"),
                                                             this.GetCategoryPredicate(),
                                                             this.GetProductCondition(),
                                                             this.GetChannelsList(),
                                                             this.GetProductListCondition(),
                                                             this.GetChannelCondition(),
                                                             this.GetPriceCondition());
            return query;
        }

        private string GetFullChannelsList()
        {
            if ((this.ChannelIds == null) || (this.ChannelIds.Count == 0))
                return string.Empty;
            string result = string.Empty;
            CanonDataContext db = Cdb.Instance;
            List<Channel> channels = db.Channels.Where(c => this.ChannelIds.Contains(c.ChannelId)).ToList();
            foreach(Channel channel in channels)
                result += string.Format(",[{0}] AS [{1}]", channel.ChannelId, channel.ChannelName);
            return string.Format(" {0} ", result);
        }

        private string GetCategoryPredicate()
        {
            if ((this.CategoryIds == null)||(this.CategoryIds.Count == 0)) 
                return string.Empty;
            string result = string.Empty;
            foreach (int cat in this.CategoryIds)
                result += string.Format("{0},",cat);
            result = result.Substring(0, result.Length - 1);
            return string.Format(" AND t2.CategoryId IN ({0}) ", result);
        }

        private string GetPriceCondition()
        {
            if (this.PriceCondition == PriceConditions.All)
                return string.Empty;
            string format = " AND t1.RecommendedPrice {0} t1.ChannelPrice";
            switch (this.PriceCondition)
            {
                case PriceConditions.RecommendedLess:
                    return string.Format(format, "<");
                case PriceConditions.RecommendedMore:
                    return string.Format(format, ">");
                case PriceConditions.RecommendedNotEqual:
                    return string.Format(format, "<>");
            }
            return string.Empty;
        }

        private string GetProductListCondition()
        {
            if ((this.ProductIds == null) || (this.ProductIds.Count == 0))
                return string.Empty;
            string result = string.Empty;
            foreach (int pro in this.ProductIds)
                result += string.Format("{0},", pro);
            result = result.Substring(0, result.Length - 1);
            return string.Format(" AND t2.ProductId IN ({0}) ", result);
        }

        private string GetChannelCondition()
        {
            if ((this.ChannelIds == null) || (this.ChannelIds.Count == 0))
                return string.Empty;
            string result = string.Empty;
            foreach (int cat in this.ChannelIds)
                result += string.Format("{0},", cat);
            result = result.Substring(0, result.Length - 1);
            return string.Format(" AND t1.ChannelId IN ({0}) ", result);
        }

        private string GetChannelsList()
        {
            if ((this.ChannelIds == null) || (this.ChannelIds.Count == 0))
                return string.Empty;
            string result = string.Empty;
            foreach (int cat in this.ChannelIds)
                result += string.Format("[{0}],", cat);
            result = result.Substring(0, result.Length - 1);
            return string.Format(" {0} ", result);
        }

        private string GetProductCondition()
        {
            if (string.IsNullOrEmpty(this.ProductName))
                return string.Empty;
            else return string.Format(" AND t2.ProductName like '%{0}%' ", this.ProductName);
        }

        #region ChartValues
        public static List<MainMonitor> GetValuesForChart(DateTime date1, DateTime date2, 
                                                          List<int> channels, List<int> products)
        {
            CanonDataContext db = Cdb.Instance;
            List<MainMonitor> list = db.MainMonitors.OrderByDescending(m => m.CalcDate).Where(
                                            w => w.CalcDate.Date >= date1.Date && w.CalcDate.Date <= date2.Date &&
                                            products.Contains(w.ProductId) && channels.Contains(w.ChannelId)).ToList();
            return list;
        }
        #endregion
    }

    public enum PriceConditions
    {
        RecommendedMore,
        RecommendedLess,
        RecommendedNotEqual,
        All
    }

}
