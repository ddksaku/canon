using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;

namespace Canon.Data.Import
{
    public class CanonChannelMapping
    {
        protected int _channelId;

        public CanonChannelMapping(int channelId)
        {
            this._channelId = channelId;
        }

        public void UpdateMonitor()
        {
            CanonDataContext db = Cdb.Instance;
            db.UpdateMonitorValues(this._channelId);
        }

        public void CreateNewMappingRules()
        {
            CanonDataContext db = Cdb.Instance;
            db.CreateNewMappingRulesForAll();
        }

        public void UpdateRelevances()
        {
            CanonDataContext db = Cdb.Instance;
            db.RecreateCurrentRelevanceForChannel(this._channelId);
        }

        public void CleanNotExistingProducts()
        {
            CanonDataContext db = Cdb.Instance;
            List<MappingRule> rules = db.MappingRules.Where(c => c.ChannelId == this._channelId).ToList();
            foreach (MappingRule rule in rules)
            {
                ChannelMonitor cm = db.ChannelMonitors.FirstOrDefault(c=> c.ProductName == rule.MonitoredName &&
                                                                          c.ProductUrl == rule.MonitoredUrl);
                if (cm == null)
                {
                    db.MappingRules.DeleteOnSubmit(rule);
                    db.SubmitChanges();
                }
            }
        }

        public void MarkRecommendeedRelevances()
        {
            CanonDataContext db = Cdb.Instance;
            Dictionary<int,int> recommended = this.ChooseBestPairs();
            foreach(KeyValuePair<int,int> pair in recommended)
            {
                CurrentRelevance relevance = db.CurrentRelevances.First(r => r.ChannelMonitorId == pair.Value
                                                                            && r.ProductId == pair.Key);
                relevance.BestForMapping = true;
            }
            db.SubmitChanges();
        }

        protected Dictionary<int, int> ChooseBestPairs()
        {
            CanonDataContext db = Cdb.Instance;
            Dictionary<int, int> result = new Dictionary<int, int>(500);
            var bestMap = db.GetBestRelevanceForMapping(this._channelId).OrderByDescending(p=> p.RelevancePercent);
            List<GetBestRelevanceForMappingResult> list = bestMap.ToList();
            var grouped = db.GetBestRelevanceForMapping(this._channelId).GroupBy(g=> g.ProductId);
            List<int> groupedList = new List<int>(100);
            List<MappingRule> currentRules = db.MappingRules.Where(c => c.ChannelId == this._channelId).ToList();
            foreach (var pc in grouped)
                if (pc.Count() == 1) groupedList.Add(pc.Key);
            //first add items which have one2one pair
            foreach (GetBestRelevanceForMappingResult mr in list)
            {
                if (!groupedList.Contains(mr.ProductId)) continue;
                if (result.ContainsValue(mr.ChannelMonitorId)) continue;
                if (this.IsForbiddenToRecommend(currentRules, mr.ProductId, mr.ChannelMonitorId)) continue;
                result.Add(mr.ProductId, mr.ChannelMonitorId);
            }
            //now add the rest
            foreach (GetBestRelevanceForMappingResult mr in list)
            {
                if (result.ContainsKey(mr.ProductId)) continue;
                if (result.ContainsValue(mr.ChannelMonitorId)) continue;
                if (this.IsForbiddenToRecommend(currentRules, mr.ProductId, mr.ChannelMonitorId)) continue;
                result.Add(mr.ProductId, mr.ChannelMonitorId);
            }
            return result;
        }

        protected bool IsForbiddenToRecommend(List<MappingRule> rules, int productId, int channelMonId)
        {
            CanonDataContext db = Cdb.Instance;
            CurrentRelevance relevance = db.CurrentRelevances.First(r => r.ChannelMonitorId == channelMonId
                                                            && r.ProductId == productId);
            foreach (MappingRule rule in rules)
            {
                if ((rule.MonitoredName == relevance.ProductName) &&
                    (rule.MonitoredUrl == relevance.ProductUrl) &&
                    (rule.ProductId != productId))
                    return true;
            }
            return false;
        }
    }
}
