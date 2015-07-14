using System;
using System.IO;
using System.Web;
using log4net.Appender;
using log4net.Core;
using log4net;

namespace Memos.Framework.Logging
{
    /// <summary>
    /// Pack rolling file appender.
    /// </summary>
    public class PackRollingFileAppender : RollingFileAppender
    {
        #region Fields
        
        /// <summary>
        /// Log prefix.
        /// </summary>
        public const string LOG_PREFIX = "LOG_PREFIX";

        /// <summary>
        /// HTTP context.
        /// </summary>
        public const string HTTP_CONTEXT = "HTTP_CONTEXT";

        /// <summary>
        /// Session vars.
        /// </summary>
        public const string SESSION_VARS = "SESSION_VARS"; 
        
        #endregion

        #region Append
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            string filename = Path.GetDirectoryName(File) + "\\";

            OpenFile(filename, true);

            base.Append(loggingEvent);

            PackOldFiles();
        } 
        #endregion

        #region OpenFile
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="append"></param>
        protected override void OpenFile(string fileName, bool append)
        {
            LogFilePrefix prefix = LogFilePrefix.Debug;

            if (ThreadContext.Properties[LOG_PREFIX] != null)
            {
                prefix = (LogFilePrefix)ThreadContext.Properties[LOG_PREFIX];
            }

            fileName += prefix;

            base.OpenFile(fileName, append);
        }
        #endregion

        #region PackOldFiles
        /// <summary>
        /// Pack log files, older than first day of previous month.
        /// </summary>
        protected void PackOldFiles()
        {
            // Get log files older than first day of previous month
            DateTime olderDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1).Date;
            FileInfo[] oldFiles = LoggerUtilities.GetOlderLogFiles(olderDate);
            if (oldFiles.Length == 0)
            {
                return;
            }

            // Generate ZIP file from old log files
            string zipFile = ZipManager.GenerateZipFile(oldFiles, ZipManager.CreateZipFile());

            // Create name for ZIP file
            string logsPath = Path.Combine(
                HttpContext.Current.Server.MapPath("~/"),
                ConfigSettings.ApplicationLogFolder);

            string zipFileName = Path.Combine(logsPath, String.Concat("Logs", olderDate.ToString("yyyyMM"), ".zip"));

            #region Check if file exists and add appender

            bool isZipExists = System.IO.File.Exists(zipFileName);
            int appender = 1;
            while (isZipExists)
            {
                zipFileName = Path.Combine(logsPath, String.Concat("Logs", olderDate.ToString("yyyyMM"), "-", appender, ".zip"));
                isZipExists = System.IO.File.Exists(zipFileName);
                appender++;
            }

            #endregion

            // Move ZIP file into the log directory
            System.IO.File.Move(zipFile, zipFileName);

            // Delete old log files
            LoggerUtilities.DeleteLogFiles(oldFiles);
        }
        #endregion
    }
}