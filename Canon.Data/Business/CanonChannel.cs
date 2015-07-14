using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Canon.Data.Business
{
    public class CanonChannel : Channel
    {
        public static void DeleteChannelById(int id)
        {
            CanonDataContext db = Cdb.Instance;
            List<MainMonitor> mms = db.MainMonitors.Where(u => u.ChannelId == id).ToList();
            Channel channel = db.Channels.First(u => u.ChannelId == id);
            db.MainMonitors.DeleteAllOnSubmit(mms);
            db.SubmitChanges();
            db.Channels.DeleteOnSubmit(channel);
            db.SubmitChanges();
        }

        public static void UpdateChannelById(int id, CanonChannel newValues)
        {
            CanonDataContext db = Cdb.Instance;
            Channel channel = db.Channels.First(u => u.ChannelId == id);
            channel.ChannelName = newValues.ChannelName;
            channel.ChannelType = newValues.ChannelType;
            channel.InfoType = newValues.InfoType;
            channel.IsActive = newValues.IsActive;
            channel.Url = newValues.Url;
            channel.ReportingTo = newValues.ReportingTo;
            db.SubmitChanges();
        }

        public static void InsertChannel(CanonChannel newValues)
        {
            CanonDataContext db = Cdb.Instance;
            Channel channel = new Channel();
            channel.ChannelName = newValues.ChannelName;
            channel.ChannelType = newValues.ChannelType;
            channel.InfoType = newValues.InfoType;
            channel.IsActive = newValues.IsActive;
            channel.Url = newValues.Url;
            channel.ReportingTo = newValues.ReportingTo;
            db.Channels.InsertOnSubmit(channel);
            db.SubmitChanges();
        }
    }
}
