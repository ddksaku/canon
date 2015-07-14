using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using log4net;

namespace Memos.Framework.Logging
{
    /// <summary>
    /// Logger utilities class.
    /// </summary>
    public class LoggerUtilities
    {
        #region GetAllLogFiles
        /// <summary>
        /// Get all log files.
        /// </summary>
        /// <returns></returns>
        public static FileInfo[] GetAllLogFiles()
        {
            string logsPath = Path.Combine(
                HttpContext.Current.Server.MapPath("~/"),
                ConfigSettings.ApplicationLogFolder);

            DirectoryInfo directoryInfo = new DirectoryInfo(logsPath);
            FileInfo[] allFilesList = directoryInfo.GetFiles(ConfigSettings.ApplicationLogPattern);

            return allFilesList;
        }
        #endregion

        #region GetSelectedLogFiles
        /// <summary>
        /// Get selected log files.
        /// </summary>
        /// <param name="selectedFiles"></param>
        /// <returns></returns>
        public static FileInfo[] GetSelectedLogFiles(List<object> selectedFiles)
        {
            List<FileInfo> selectedFilesList = new List<FileInfo>();
            foreach (object file in selectedFiles)
            {
                string fileName = Convert.ToString(file);
                FileInfo fileInfo = new FileInfo(fileName);
                selectedFilesList.Add(fileInfo);
            }

            return selectedFilesList.ToArray();
        }
        #endregion

        #region GetOlderLogFiles
        /// <summary>
        /// Get log files, older than specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static FileInfo[] GetOlderLogFiles(DateTime date)
        {
            FileInfo[] filesList = GetAllLogFiles();

            List<FileInfo> olderFilesList = new List<FileInfo>();
            foreach (FileInfo fileInfo in filesList)
            {
                if (fileInfo.CreationTime.Date < date.Date)
                {
                    olderFilesList.Add(fileInfo);
                }
            }

            return olderFilesList.ToArray();
        }
        #endregion

        #region DeleteLogFiles
        /// <summary>
        /// Delete log files.
        /// </summary>
        /// <param name="selectedFiles"></param>
        public static void DeleteLogFiles(FileInfo[] selectedFiles)
        {
            foreach (FileInfo fileInfo in selectedFiles)
            {
                if (File.Exists(fileInfo.FullName))
                {
                    File.Delete(fileInfo.FullName);
                }
            }
        }
        #endregion

        #region ResetNotification
        /// <summary>
        /// Application reset notification.
        /// </summary>
        public static void ResetNotification()
        {
            HttpRuntime runtime = (HttpRuntime)typeof(HttpRuntime).InvokeMember(
                "_theRuntime",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField,
                null,
                null,
                null);

            if (runtime == null)
            {
                return;
            }

            string shutDownMessage = (string)runtime.GetType().InvokeMember(
                "_shutDownMessage",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
                null,
                runtime,
                null);

            string shutDownStack = (string)runtime.GetType().InvokeMember(
                "_shutDownStack",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
                null,
                runtime,
                null);

            string allLog = "Shutdown message: " + Environment.NewLine + "\t" + shutDownMessage;
            allLog += "ShutdownStack:" + Environment.NewLine + "\t" + shutDownStack;

            Logger.Log(allLog, LogLevel.Info, LogFilePrefix.Restart);
        } 
        #endregion

        #region ShutDownLogger
        /// <summary>
        /// Shut down logger.
        /// </summary>
        public static void ShutDownLogger()
        {
            LogManager.Shutdown();
        }
        #endregion
    }
}