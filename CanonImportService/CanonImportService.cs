using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using log4net;
using log4net.Config;
using Memos.Framework;
using Memos.Import;
using Canon.Data;
using Canon.Data.Business;
using Canon.Data.Import;
using Canon.Data.Enums;
using System.Net.Mail;
using System.IO;

namespace CanonImportService
{
    public partial class CanonImportService : ServiceBase
    {
        public static readonly ILog logger = LogManager.GetLogger(typeof(CanonImportService));
        private readonly object padlock = new object();
        private bool IsAlreadyWork = false;
        public static DateTime LastEmailSent = DateTime.Now.AddDays(-10);

        public CanonImportService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            XmlConfigurator.Configure();
            logger.Info("Import service has been started.");
            logger.Debug(System.Configuration.ConfigurationManager.ConnectionStrings["Main"].ToString());
            Cdb.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Main"].ToString();
            logger.Debug("New connection: " + Cdb.Instance.Connection.ConnectionString);
            WebVariables.ServerLanguage = CanonConfigManager.ServerLanguage;
            WebVariables.Logger = logger;
            WebVariables.LoggedUserId = 0;
        }

        protected override void OnStop()
        {
            logger.Info("Import service has been stopped.");
        }

        private void MainTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (padlock)
            {
                if (IsAlreadyWork) return;
                IsAlreadyWork = true;
            }
            try
            {
                logger.Debug(string.Format("New elapsation started-{0}", DateTime.Now));

                //log maintainance
                this.DeleteOldLogs();

                //go through items in Manual Mapping Import Queue
                this.ProcessImportQueue();

                //check current time against defined in config
                if (!CanonConfigManager.ForceStart)
                {
                    DateTime start = CanonConfigManager.TimeToStart;
                    DateTime now = DateTime.Now;

                    logger.Debug(string.Format("now-{0}, start-{1}", now, start));

                    if (start > now) return;
                    else if ((now - start).TotalMinutes > 30) return;
                }

                //Clear manual import queue and subsribers
                CanonDataContext db = Cdb.Instance;
                if (!CanonConfigManager.ForceStart)
                {
                    //delete all complete every night
                    var allManuals = db.ManualImportQueues.Where(p => p.Status == (int)ManualImportStatusEnum.ImportComplete
                                                                  || p.Status == (int)ManualImportStatusEnum.InProgress);
                    db.ManualImportQueues.DeleteAllOnSubmit(allManuals);
                    db.SubmitChanges();
                }
                else
                {
                    //delete all old
                    var allManualsA = db.ManualImportQueues.Where(p => p.PostDate.Date < DateTime.Now.Date);
                    db.ManualImportQueues.DeleteAllOnSubmit(allManualsA);
                    db.SubmitChanges();
                }

                //application maintainance
                db.MaintainDatabase();

                //go through all channels
                foreach (Channel channel in db.Channels)
                {
                    try
                    {
                        if (!channel.IsActive)
                        {
                            logger.Info(string.Format("Channel is disabled: {0}, {1}, {2}",
                                        channel.ChannelId, channel.ChannelName, channel.Url));
                            continue;
                        }

                        logger.Info(string.Format("Starting new export: {0}, {1}, {2}",
                                    channel.ChannelId, channel.ChannelName, channel.Url));

                        if (!ShouldRun(channel))
                        {
                            logger.Info(string.Format("Data already imported today or there was a error during import."));
                            continue;
                        }

                        this.ProcessChannel(channel);
                    }
                    catch (Exception ex)
                    {
                        logger.Warn(string.Format("Import for channel {0} failed", channel.ChannelName), ex);
                    }
                }
                //send daily emails
                if (CanonImportService.LastEmailSent.Date < DateTime.Now.Date)
                {
                    List<User> users = db.Users.Where(u => u.IsDailyEmail && u.Email.Length > 0).ToList();
                    string text = this.CreateEmailText(null).Trim();
                    logger.Info("Email text:" + text);
                    if (!string.IsNullOrEmpty(text))
                        foreach (User user in users)
                        {
                            try
                            {
                                EmailGateway email = new EmailGateway(user.Email, "Daily Log", text);
                                email.Send();
                                CanonImportService.LastEmailSent = DateTime.Now;
                            }
                            catch (Exception ex)
                            {
                                logger.Fatal(ex);
                            }
                        }
                    //send daily email for users in channels
                    List<Channel> repChannels = db.Channels.ToList();
                    foreach (Channel repChannel in repChannels)
                    {
                        if (!repChannel.IsActive)
                            continue;
                        if (!string.IsNullOrEmpty(repChannel.ReportingTo))
                        {
                            string[] emails = repChannel.ReportingTo.Split(';');
                            string emailText = this.CreateEmailText(repChannel).Trim();
                            foreach (string email in emails)
                            {
                                try
                                {
                                    EmailGateway emailG = new EmailGateway(email, "Průzkum trhu", emailText);
                                    logger.Info(string.Format("Sending: {0}, text={1}", email, emailText));
                                    emailG.Send();
                                    CanonImportService.LastEmailSent = DateTime.Now;
                                }
                                catch (Exception ex)
                                {
                                    logger.Fatal(ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
            finally
            {
                IsAlreadyWork = false;
            }
        }

        protected bool ShouldRun(Channel channel)
        {
            CanonDataContext db = Cdb.Instance;
            int count = db.MainMonitors.Count(c => c.ChannelId == channel.ChannelId 
                                                   && c.CalcDate.Date == DateTime.Now.Date);
            int logs = db.ImportLogs.Count(c => c.ChannelId == channel.ChannelId
                                                   && c.LogDate == DateTime.Now.Date);
            if (logs > 0)
                return false;
            if (count > 0)
                return false;
            return true;
        }

        protected void ProcessImportQueue()
        {
            CanonDataContext db = Cdb.Instance;
            List<ManualImportQueue> waiting = db.ManualImportQueues.Where(q=> q.Status == (int)ManualImportStatusEnum.WaitingInQueue).ToList();
            foreach (ManualImportQueue miq in waiting)
            {
                miq.Status = (int)ManualImportStatusEnum.InProgress;
                db.SubmitChanges();

                Channel channel = db.Channels.Where(c=> c.ChannelId == miq.ChannelId).FirstOrDefault();

                logger.Info(string.Format("Starting new export: {0}, {1}, {2}",
                    channel.ChannelId, channel.ChannelName, channel.Url));

                if (channel != null)
                    this.ProcessChannel(channel);

                miq.Status = (int)ManualImportStatusEnum.ImportComplete;
                db.SubmitChanges();
            }
        }

        protected void ProcessChannel(Channel channel)
        {
            //export data
            CommonParser<CanonChannelMonitor> export = ImportFactory.GetParser(
                                                               (Canon.Data.Enums.ChannelTypeEnum)channel.InfoType,
                                                               channel.ChannelId,
                                                               channel.Url,
                                                               CanonConfigManager.UploadDataFolder,
                                                               channel.AdditionalCommand);
            export.Logger = logger;
            if (!export.ExportToDb()) return;

            logger.Info(string.Format("Export to DB finished."));

            try
            {
                CanonChannelMapping map = new CanonChannelMapping(channel.ChannelId);

                map.CleanNotExistingProducts();

                map.UpdateRelevances();

                logger.Info(string.Format("Relevance points were updated."));

                map.MarkRecommendeedRelevances();

                logger.Info(string.Format("The best relevanced pairs were chosen."));

                //Create new mapping rules for all channels
                map.CreateNewMappingRules();
                logger.Info(string.Format("New mapping rules has been created."));

                //Update main monitor
                map.UpdateMonitor();
                logger.Info(string.Format("Main monitor has been updated."));
            }
            catch (Exception ex)
            {
                //into db log
                int id = CanonProductsLog.AddImportLog(ChannelLogEnum.ChannelError,
                                    channel.ChannelId, export.TryedRecords, export.SuccessRecords);
                CanonProductsLog.AddImportErrorsLog(id, ChannelErrorEnum.ChannelMappingError, string.Empty);

                logger.Fatal(ex);
            }

            //into db log
            CanonProductsLog.AddImportLog(ChannelLogEnum.ImportOk,
                                channel.ChannelId, export.TryedRecords, export.SuccessRecords);

            logger.Info(string.Format("Export finished: {0}, {1}, {2}",
                        channel.ChannelId, channel.ChannelName, channel.Url));
        }

        protected string CreateEmailText(Channel specific)
        {
            string[] txtHeaders = new string[] { "Canon Monitoring daily results", "Canon monitoring - průzkum trhu" };
            string[] txtImportErrors = new string[] { "Import errors:", "Chyby při importu:" };
            string[] txtResults = new string[] { "<h3 style='margin-bottom: 3px;'>Different prices:</h3><table cellspacing='0' cellpadding='5' border='1' bordercolor='#aaa'><thead align='center'><tr bgcolor='#FF7C24' style='background: #FF7C24; color: #fff;'><font size='3' face='Calibri,Arial' color='#fff'><th>Product Name</th><th>Recommended Price</th><th>Feed Price</th><th>Difference</th></font></tr></thead><tbody>",
                "<h3 style='margin-bottom: 3px;'>Výsledky průzkumu:</h3><table cellspacing='0' cellpadding='5' border='1' bordercolor='#aaa'><thead align='center'><tr bgcolor='#FF7C24' style='background: #FF7C24; color: #fff;'><font size='3' face='Calibri,Arial' color='#fff'><th>Název produktu</th><th>Doporučená cena</th><th>Vaše cena</th><th>Rozdíl</th></font></tr></thead><tbody>" };
            int langIndex = (specific == null) ? 0 : 1;
            StringBuilder emailText = new StringBuilder();
            CanonDataContext db = Cdb.Instance;
            List<MainMonitor> mm = db.MainMonitors.Where(m => m.CalcDate.Date == DateTime.Now.Date &&
                                                 m.ChannelPrice != m.RecommendedPrice).ToList();
            List<ImportLog> logs = db.ImportLogs.Where(i => i.LogType != (int)ChannelLogEnum.ImportOk
                                && i.LogDate.Date == DateTime.Now.Date).OrderBy(o => o.LogId).ToList();
            foreach (Channel c in db.Channels)
            {
                if ((specific != null) && (c.ChannelId != specific.ChannelId))
                    continue;
                StringBuilder importErrors = new StringBuilder(10);
                foreach (ImportLog il in logs)
                    if (il.ChannelId == c.ChannelId)
                    {
                        List<ImportLogError> errors = db.ImportLogErrors.Where(l => l.MainLogId == il.LogId).ToList();
                        bool isFirstError = true;
                        foreach (ImportLogError error in errors)
                        {
                            if (isFirstError)
                            {
                                importErrors.AppendLine(string.Format("<h3>{0}</h3>", txtImportErrors[langIndex]));
                                isFirstError = false;
                            }
                            importErrors.AppendLine(string.Format("{0} {1}<br>", error.Enum.NameEn,
                               string.IsNullOrEmpty(error.ProductName) ? string.Empty : error.ProductName));
                        }
                    }
                StringBuilder priceErrors = new StringBuilder(10);
                bool isFirstPrice = true;
                int counter = 0;
                foreach (MainMonitor m in mm)
                    if (m.ChannelId == c.ChannelId)
                    {
                        if (isFirstPrice)
                        {
                            priceErrors.AppendLine(txtResults[langIndex]);
                            isFirstPrice = false;
                        }
                        if (langIndex == 0)
                            priceErrors.AppendLine(string.Format("<tr><font size='3' face='Calibri,Arial' color='black'><td>{0}</td><td>{1}</td><td><a href='{3}'>{2}</a></td><td><font color='{5}'>{4}</font></td></font></tr>",
                                m.Product.ProductName, m.RecommendedPrice,
																m.ChannelPrice, m.ChannelMonitor.ProductUrl,
																m.ChannelPrice - m.RecommendedPrice,
																(((m.ChannelPrice - m.RecommendedPrice)>0)? "#009900" : "#cc0000"),
																(((counter%2)==0)? "#FFFFFF" : "#F2F2F2")));
                        else
                            priceErrors.AppendLine(string.Format("<tr bgcolor='{6}'><font size='3' face='Calibri,Arial' color='black'><td>{0}</td><td>{1}</td><td><a href='{3}'>{2}</a></td><td><font color='{5}'>{4}</font></td></font></tr>",
                                m.Product.ProductName, m.RecommendedPrice,
                                m.ChannelPrice, m.ChannelMonitor.ProductUrl, 
                                m.ChannelPrice - m.RecommendedPrice,
                                (((m.ChannelPrice - m.RecommendedPrice)>0)? "#009900" : "#cc0000"),
                                (((counter%2)==0)? "#FFFFFF" : "#F2F2F2")));
                        counter++;
                    }
                if (priceErrors.Length > 0)
                    priceErrors.AppendLine("</tbody></table>");

                if ((importErrors.Length > 0) || (priceErrors.Length > 0))
                {
                    emailText.AppendLine(string.Format("<h2 style='margin-bottom: 3px;'>{0}</h2>", c.ChannelName));
                    emailText.Append(importErrors.ToString());
                    emailText.Append(priceErrors.ToString());
                }
            }
            if (emailText.Length > 0)
            {
                emailText.Insert(0, string.Format("<html><body style='color: black; font: bold Calibri,Arial !important;'><basefont face='Calibri,Arial' color='black'><font size='12' face='Calibri,Arial' color='black'><h1 style='margin-bottom: 3px;'>{0}</h1>", txtHeaders[langIndex]));
                emailText.AppendLine("</font></body></html>");
            }
            return emailText.ToString();
        }

        private void DeleteOldLogs()
        {
            try
            {
                int delNum = Memos.Framework.Utilities.DeleteOldFiles(CanonConfigManager.UploadDataFolder,
                                                         "*.*",
                                                         DateTime.Now.AddDays(-2));
                DirectoryInfo di = new DirectoryInfo(CanonConfigManager.UploadDataFolder);
                if (delNum > 0)
                logger.Info(string.Format("Deleted {0} old files in {1}", delNum, 
                                                                    di.FullName));
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.ToString(), ex.InnerException);
            }
        }
    }
}
