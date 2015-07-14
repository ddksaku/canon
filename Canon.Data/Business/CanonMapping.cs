using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Canon.Data.Enums;

namespace Canon.Data.Business
{
    public class CanonMapping
    {
        public static void DeleteMapping(int channelId, int productId)
        {
            CanonDataContext db = Cdb.Instance;
            try
            {
                MappingRule rule = db.MappingRules.Where(r => r.ChannelId == channelId &&
                                                     r.ProductId == productId).FirstOrDefault();
                if (rule != null)
                {
                    db.MappingRules.DeleteOnSubmit(rule);
                    db.SubmitChanges();
                    CanonProductsLog.Add(channelId, productId, MappingLogEnum.DeletedMappingRule);
                }
            }
            catch (Exception)
            {
            }
        }

        public static ProductStateEnum GetProductState(int channelId, int productId)
        {
            return (ProductStateEnum)Cdb.Instance.GetProductState(channelId, productId);
        }

        public static void AddRecommendedMapping(int channelId, int productId)
        {
            CanonDataContext db = Cdb.Instance;
            try
            {
                CurrentRelevance cr = db.CurrentRelevances.Where(c => c.BestForMapping == true &&
                                                                      c.ChannelId == channelId &&
                                                                      c.ProductId == productId).FirstOrDefault();
                if (cr != null)
                {
                    MappingRule rule = db.MappingRules.Where(r => r.ChannelId == channelId &&
                                                         r.ProductId == productId).FirstOrDefault();
                    if (rule == null)
                    {
                        //NEW rule
                        MappingRule mr = new MappingRule();
                        mr.ChannelId = channelId;
                        mr.ProductId = productId;
                        mr.MonitoredName = cr.ProductName;
                        mr.MonitoredUrl = cr.ProductUrl;
                        db.MappingRules.InsertOnSubmit(mr);
                        CanonProductsLog.Add(channelId, productId, MappingLogEnum.NewMappingRule);
                    }
                    else
                    {
                        //UPDATED rule
                        rule.MonitoredName = cr.ProductName;
                        rule.MonitoredUrl = cr.ProductUrl;
                        CanonProductsLog.Add(channelId, productId, MappingLogEnum.ChangedMappingRule);
                    }
                    db.SubmitChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public static void AddMapping(int relevanceId)
        {
            CanonDataContext db = Cdb.Instance;
            try
            {
                CurrentRelevance cr = db.CurrentRelevances.Where(c => c.RelId == relevanceId).FirstOrDefault();
                if (cr != null)
                {
                    MappingRule rule = db.MappingRules.Where(r => r.ChannelId == cr.ChannelId &&
                                                         r.ProductId == cr.ProductId).FirstOrDefault();
                    if (rule == null)
                    {
                        //NEW rule
                        MappingRule mr = new MappingRule();
                        mr.ChannelId = cr.ChannelId;
                        mr.ProductId = cr.ProductId;
                        mr.MonitoredName = cr.ProductName;
                        mr.MonitoredUrl = cr.ProductUrl;
                        db.MappingRules.InsertOnSubmit(mr);
                        CanonProductsLog.Add(cr.ChannelId, cr.ProductId, MappingLogEnum.NewMappingRule);
                    }
                    else
                    {
                        //UPDATED rule
                        rule.MonitoredName = cr.ProductName;
                        rule.MonitoredUrl = cr.ProductUrl;
                        CanonProductsLog.Add(cr.ChannelId, cr.ProductId, MappingLogEnum.ChangedMappingRule);
                    }
                    db.SubmitChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public static void AddToExceptions(int channelId, int productId)
        {
            CanonDataContext db = Cdb.Instance;
            MappingRule rule = db.MappingRules.Where(r => r.ChannelId == channelId &&
                                     r.ProductId == productId).FirstOrDefault();
            if (rule != null)
                db.MappingRules.DeleteOnSubmit(rule);

            Excluded old = db.Excludeds.FirstOrDefault(d=> d.ChannelId==channelId && d.ProductId==productId);
            if (old != null)
                return;
            Excluded exc = new Excluded();
            exc.ProductId = productId;
            exc.ChannelId = channelId;
            db.Excludeds.InsertOnSubmit(exc);
            db.SubmitChanges();
            CanonProductsLog.Add(channelId, productId, MappingLogEnum.AddedToExceptions);
        }

        public static void RemoveFromExceptions(int channelId, int productId)
        {
            CanonDataContext db = Cdb.Instance;
            Excluded prod = db.Excludeds.Where(r => r.ChannelId == channelId &&
                                     r.ProductId == productId).FirstOrDefault();
            if (prod != null)
                db.Excludeds.DeleteOnSubmit(prod);

            db.SubmitChanges();

            CanonProductsLog.Add(channelId, productId, MappingLogEnum.RemovedFromExceptions);
        }

        public static decimal? GetCurrentMapPrice(int channelId, string monitoredName, string monitoredUrl)
        {
            CanonDataContext db = Cdb.Instance;
            ChannelMonitor cm = db.ChannelMonitors.Where(c => c.ChannelId == channelId &&
                                                             c.ProductName == monitoredName &&
                                                             c.ProductUrl == monitoredUrl).OrderByDescending(
                                                                          m => m.ImportDate).FirstOrDefault();
            if (cm != null)
                return cm.PriceVat;
            return null;
        }
    }
}
