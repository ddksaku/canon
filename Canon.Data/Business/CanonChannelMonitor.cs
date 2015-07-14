using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using Memos.Framework;
using Canon.Data.Enums;

namespace Canon.Data.Business
{
    public class CanonChannelMonitor : ChannelMonitor
    {
        /// <summary>
        /// Inserts new record into db
        /// </summary>
        public bool InsertNewRecord()
        {
            CanonDataContext db = Cdb.Instance;

            //check if name is not started with stop words from FeedProductExceptions
            List<FeedProductException> flist = db.FeedProductExceptions.ToList();
            foreach(FeedProductException fpe in flist)
                if (this.ProductName.ToUpper().StartsWith(fpe.StopWord))
                    return false;

            ChannelMonitor rec = new ChannelMonitor();
            rec.ChannelId = this.ChannelId;
            rec.ImportDate = DateTime.Now;
            rec.Price = this.Price;
            rec.PriceVat = this.PriceVat;
            rec.ProductDesc = Utilities.TruncateString(this.ProductDesc, 1000);
            rec.ProductName = Utilities.TruncateString(this.ProductName, 1000);
            rec.ProductUrl = Utilities.TruncateString(this.ProductUrl, 1000);
            rec.Vat = this.Vat;

            //Check price is changed !!! BEFORE submit
            this.CheckIfPriceIsChanged(this.ChannelId, rec.ProductName, rec.ProductUrl, rec.PriceVat);

            db.ChannelMonitors.InsertOnSubmit(rec);
            db.SubmitChanges();
            return true;
        }

        public void CheckIfPriceIsChanged(int channelId, string productName, string productUrl, decimal price)
        {
            CanonDataContext db = Cdb.Instance;
            //try to find mapping rule
            MappingRule mr = db.MappingRules.Where(m => m.ChannelId == channelId
                                                    && m.MonitoredName == productName
                                                    && m.MonitoredUrl == productUrl).FirstOrDefault();
            if (mr == null)
                return;

            decimal? currentPrice = CanonMapping.GetCurrentMapPrice(channelId, productName, productUrl);
            if (currentPrice == null)
                return;

            if ((decimal)currentPrice == price)
                CanonProductsLog.Add(channelId, mr.ProductId, MappingLogEnum.PriceWithoutChanges);
            else
                CanonProductsLog.Add(channelId, mr.ProductId, MappingLogEnum.NewPrice, price.ToString());
        }

        /// <summary>
        /// Cleans today's data from ChannelMonitor table
        /// </summary>
        /// <param name="channelId"></param>
        public void CleanTodaysData(int channelId)
        {
            Cdb.Instance.CleanTodaysChannelMonitor(channelId);
        }

        public static DateTime GetLastActualizationByChannel(int channelId)
        {
            ISingleResult<GetLastActualizationByChannelResult> res = 
                                Cdb.Instance.GetLastActualizationByChannel(channelId);
            List<GetLastActualizationByChannelResult> list = res.ToList<GetLastActualizationByChannelResult>();
            if (list.Count > 0)
                return list[0].ImportDate;
            return DateTime.Now;
        }
    }
}
