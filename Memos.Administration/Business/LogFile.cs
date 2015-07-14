using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using Memos.Framework;
using Memos.Framework.Logging;

namespace Memos.Administration.Business
{
    /// <summary>
    /// Log file business object class.
    /// </summary>
    public class LogFile
    {
        // Delete logs methods

        #region DeleteAllLogFiles
        /// <summary>
        /// Delete all log files.
        /// </summary>
        public static void DeleteAllLogFiles()
        {
            FileInfo[] filesList = LoggerUtilities.GetAllLogFiles();
            LoggerUtilities.DeleteLogFiles(filesList);
        }
        #endregion

        #region DeleteSelectedLogFiles
        /// <summary>
        /// Delete selected log files.
        /// </summary>
        /// <param name="selectedFiles"></param>
        public static void DeleteSelectedLogFiles(List<object> selectedFiles)
        {
            FileInfo[] filesList = LoggerUtilities.GetSelectedLogFiles(selectedFiles);
            LoggerUtilities.DeleteLogFiles(filesList);
        }
        #endregion

        #region DeleteOlderLogFiles
        /// <summary>
        /// Delete log files, older than specified date.
        /// </summary>
        /// <param name="olderThanDate"></param>
        public static void DeleteOlderLogFiles(DateTime olderThanDate)
        {
            FileInfo[] filesList = LoggerUtilities.GetOlderLogFiles(olderThanDate);
            LoggerUtilities.DeleteLogFiles(filesList);
        }
        #endregion

        // Send logs methods

        #region SendAllLogFiles
        /// <summary>
        /// Send message for all log files.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        public static void SendAllLogFiles(string email, string subject, string text)
        {
            FileInfo[] filesList = LoggerUtilities.GetAllLogFiles();
            SendLogFiles(email, subject, text, filesList);
        }
        #endregion

        #region SendSelectedLogFiles
        /// <summary>
        /// Send message for selected log files.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <param name="selectedFiles"></param>
        public static void SendSelectedLogFiles(string email, string subject, string text, List<object> selectedFiles)
        {
            FileInfo[] filesList = LoggerUtilities.GetSelectedLogFiles(selectedFiles);
            SendLogFiles(email, subject, text, filesList);
        } 
        #endregion

        #region SendOlderLogFiles
        /// <summary>
        /// Send message for log files, older than specified date.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <param name="olderThanDate"></param>
        public static void SendOlderLogFiles(string email, string subject, string text, DateTime olderThanDate)
        {
            FileInfo[] filesList = LoggerUtilities.GetOlderLogFiles(olderThanDate);
            SendLogFiles(email, subject, text, filesList);
        }
        #endregion

        #region SendLogFiles
        /// <summary>
        /// Send message with generated zip file (with selected log files) in attach.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <param name="selectedFiles"></param>
        public static void SendLogFiles(string email, string subject, string text, FileInfo[] selectedFiles)
        {
            string zipFile = GenerateZipFile(selectedFiles);

            try
            {
                List<Attachment> attachments = new List<Attachment>();
                using (FileStream stream = File.OpenRead(zipFile))
                {
                    attachments.Add(new Attachment(stream, "Logs.zip"));

                    EmailGateway.Send(
                        String.Empty,
                        email,
                        subject,
                        text,
                        attachments,
                        true);

                    stream.Close();
                }
            }
            finally
            {
                File.Delete(zipFile);
            }
        } 
        #endregion

        #region GenerateZipFile
        /// <summary>
        /// Generate zip file with selected log files.
        /// </summary>
        /// <param name="selectedFiles"></param>
        /// <returns>Path to the generated file.</returns>
        protected static string GenerateZipFile(FileInfo[] selectedFiles)
        {
            string zipFile = ZipManager.GenerateZipFile(selectedFiles, ZipManager.CreateZipFile());

            return zipFile;
        }
        #endregion
    }
}