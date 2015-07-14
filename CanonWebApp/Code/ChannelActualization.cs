using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CanonWebApp.Enums;
using Canon.Data;

namespace CanonWebApp.Code
{
    public class ChannelActualization
    {
        public int ChannelId { get; set; }
        public ChannelStateEnum State { get; set; }
        public DateTime? FinishTime { get; set; }

        public string ChannelName
        {
            get
            {
                Channel channel = Cdb.Instance.Channels.Where(c=> c.ChannelId==this.ChannelId).FirstOrDefault();
                if (channel == null) return string.Empty;
                return channel.ChannelName;
            }
        }

        public ChannelActualization(int channelId, ChannelStateEnum state)
        {
            FinishTime = null;
            ChannelId = channelId;
            State = state;
        }
    }
}
