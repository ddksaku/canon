using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Canon.Data.Enums;

namespace Canon.Data.Business
{
    public class CanonProductsLog : ProductsLog
    {
        public static void Add(ProductsLogEnum type, int productId, string parameter, int userId)
        {
            ProductsLog record = new ProductsLog();
            record.LogParameter = parameter;
            record.LogType = (int)type;
            record.ProductId = productId;
            record.LogDate = DateTime.Now;
            record.UserId = userId;
            CanonDataContext db = Cdb.Instance;
            db.ProductsLogs.InsertOnSubmit(record);
            db.SubmitChanges();
        }

        public static void Add(int channelId, int productId, MappingLogEnum type)
        {
            ProductsLog pl = new ProductsLog();
            pl.ChannelId = channelId;
            pl.ProductId = productId;
            pl.LogType = (int)type;
            pl.LogDate = DateTime.Now;
            if (WebVariables.LoggedUserId != 0)
                pl.UserId = WebVariables.LoggedUserId;
            CanonDataContext db = Cdb.Instance;
            db.ProductsLogs.InsertOnSubmit(pl);
            db.SubmitChanges();
        }

        public static void Add(int channelId, int productId, MappingLogEnum type, string param)
        {
            ProductsLog pl = new ProductsLog();
            pl.ChannelId = channelId;
            pl.ProductId = productId;
            pl.LogType = (int)type;
            pl.LogParameter = param;
            pl.LogDate = DateTime.Now;
            if (WebVariables.LoggedUserId != 0)
                pl.UserId = WebVariables.LoggedUserId;
            CanonDataContext db = Cdb.Instance;
            db.ProductsLogs.InsertOnSubmit(pl);
            db.SubmitChanges();
        }

        public static void AddRecommendedLog(int userId, int productId, RecommendedChangeSourceEnum type, decimal price, DateTime recomDate)
        {
            RecommendedLog rl = new RecommendedLog();
            rl.LogDate = DateTime.Now;
            rl.LogType = (int) type;
            rl.Price = price;
            rl.UserId = userId;
            rl.ProductId = productId;
            rl.RecomDate = recomDate;
            CanonDataContext db = Cdb.Instance;
            db.RecommendedLogs.InsertOnSubmit(rl);
            db.SubmitChanges();
        }

        public static int AddImportLog(ChannelLogEnum type, int channelId, int tryed, int success)
        {
            ImportLog log = new ImportLog();
            log.ChannelId = channelId;
            log.LogDate = DateTime.Now;
            log.LogType = (int)type;
            log.Tryed = tryed;
            log.Success = success;
            CanonDataContext db = Cdb.Instance;
            db.ImportLogs.InsertOnSubmit(log);
            db.SubmitChanges();
            return log.LogId;
        }

        public static void AddImportErrorsLog(int logId, ChannelErrorEnum errorType,
                                              string productName)
        {
            ImportLogError le = new ImportLogError();
            le.ErrorType = (int)errorType;
            le.MainLogId = logId;
            if (productName != string.Empty)
                le.ProductName = productName;
            CanonDataContext db = Cdb.Instance;
            db.ImportLogErrors.InsertOnSubmit(le);
            db.SubmitChanges();
        }

    }
}
