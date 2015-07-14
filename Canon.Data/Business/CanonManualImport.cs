using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Canon.Data.Enums;

namespace Canon.Data.Business
{
    public class CanonManualImport
    {
        public static ManualImportStatusEnum GetChannelImportStatus(int channelId)
        {
            CanonDataContext db = Cdb.Instance;
            ManualImportQueue queue = 
                db.ManualImportQueues.OrderByDescending(q => q.PostDate).FirstOrDefault(p => p.ChannelId == channelId);
            if (queue == null)
                return ManualImportStatusEnum.NotInQueue;
            return (ManualImportStatusEnum)queue.Enum.EnumId;
        }

        public static ManualImportQueue GetLatestQueueElement(int channelId)
        {
            CanonDataContext db = Cdb.Instance;
            ManualImportQueue queue =
                db.ManualImportQueues.OrderByDescending(q => q.PostDate).FirstOrDefault(p => p.ChannelId == channelId);
            return queue;
        }

        public static void AddChannelToQueue(int userId, int channelId)
        {
            CanonDataContext db = Cdb.Instance;
            ManualImportQueue newElem = new ManualImportQueue();
            newElem.ChannelId = channelId;
            newElem.UserId = userId;
            newElem.Status = (int)ManualImportStatusEnum.WaitingInQueue;
            newElem.PostDate = DateTime.Now;
            db.ManualImportQueues.InsertOnSubmit(newElem);
            db.SubmitChanges();

            CanonManualImport.AddNewSubscriber(userId, newElem.RecordId);
        }

        public static void AddNewSubscriber(int userId, int queueElemId)
        {
            CanonDataContext db = Cdb.Instance;
            ManualImportSubscriber subs = new ManualImportSubscriber();
            subs.ManualImportId = queueElemId;
            subs.SubscriptDate = DateTime.Now;
            subs.UserId = userId;
            db.ManualImportSubscribers.InsertOnSubmit(subs);
            db.SubmitChanges();
        }

        public static void RemoveSubscriber(int userId, int queueElemId)
        {
            CanonDataContext db = Cdb.Instance;
            ManualImportSubscriber subs = db.ManualImportSubscribers.Where(m=> m.ManualImportId==queueElemId &&
                                                                               m.UserId == userId).FirstOrDefault();

            if (subs != null)
            {
                db.ManualImportSubscribers.DeleteOnSubmit(subs);
                db.SubmitChanges();
            }
        }

        public static ManualImportQueue GetCompleteChannelByUser(int userId)
        {
            CanonDataContext db = Cdb.Instance;
            ManualImportSubscriber ms = db.ManualImportSubscribers.Where(m=> m.UserId == userId &&
                         m.ManualImportQueue.Status == (int)ManualImportStatusEnum.ImportComplete).FirstOrDefault();
            if (ms != null)
                return ms.ManualImportQueue;

            return null;
        }
    }
}
