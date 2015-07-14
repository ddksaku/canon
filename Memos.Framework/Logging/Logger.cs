using System;
using System.Net;
using System.Web;
using log4net;
using System.Reflection;

namespace Memos.Framework.Logging
{
    /// <summary>
    /// Class that encapsulates log4net logginng engine.
    /// </summary>
    public static class Logger
    {
        #region Fields

        /// <summary>
        /// Executing assembly.
        /// </summary>
        public static Assembly assembly; 

        /// <summary>
        /// ILog instance.
        /// </summary>
        private static ILog log;

        #endregion

        #region Log
        /// <summary>
        /// Logs messages into the log output
        /// </summary>
        /// <param name="message">Message that is sent to the log ouput</param>
        /// <param name="level">Level of the message</param>
        public static void Log(string message, LogLevel level)
        {
            Log(message, level, LogFilePrefix.Error);
        }

        /// <summary>
        /// Logs messages into the log output
        /// </summary>
        /// <param name="message">Message that is sent to the log ouput</param>
        /// <param name="level">Level of the message</param>
        /// <param name="prefix">Prefix of the log file. It is valid on for PackRollingFileAppender</param>
        public static void Log(string message, LogLevel level, LogFilePrefix prefix)
        {
            if (assembly == null)
            {
                return;
            }

            log = LogManager.GetLogger(assembly, assembly.GetTypes()[0]);

            ThreadContext.Properties[PackRollingFileAppender.HTTP_CONTEXT] = String.Empty;
            ThreadContext.Properties[PackRollingFileAppender.SESSION_VARS] = String.Empty;

            //Save additional information
            string logMessage = message + Environment.NewLine;
            if (level > LogLevel.Warn)
            {
                ThreadContext.Properties[PackRollingFileAppender.HTTP_CONTEXT] = AddHTTPContextInfo();
            }

            if (level > LogLevel.Debug)
            {
                ThreadContext.Properties[PackRollingFileAppender.SESSION_VARS] = SessionManager.ToString();
            }

            ThreadContext.Properties[PackRollingFileAppender.LOG_PREFIX] = prefix;

            switch (level)
            {
                case LogLevel.Info:
                    log.Info(logMessage);
                    break;
                case LogLevel.Debug:
                    log.Debug(logMessage);
                    break;
                case LogLevel.Warn:
                    log.Warn(logMessage);
                    break;
                case LogLevel.Error:
                    log.Error(logMessage);
                    break;
                case LogLevel.Fatal:
                    log.Fatal(logMessage);
                    break;
            }
        }
        #endregion

        #region AddHTTPContextInfo
        /// <summary>
        /// Returns string, that contains usefull information about HttpContext class
        /// </summary>
        /// <param name="context">HttpContext class, from which we get usefull information</param>
        /// <returns>Returns string, that contains usefull information about HttpContext class</returns>
        private static string AddHTTPContextInfo(HttpContext context)
        {
            string record = String.Empty;
            if (context == null)
            {
                return record;
            }

            try
            {
                if (context.AllErrors != null)
                {
                    foreach (Exception exc in context.AllErrors)
                    {
                        record += "Http request error: " + exc + Environment.NewLine;
                    }
                }

                IPHostEntry host = Dns.GetHostEntry(context.Request.UserHostAddress);

                //Saves usefull information from HTTPContext class
                record += "Path: " + context.Request.Path + Environment.NewLine;
                record += "Request type: " + context.Request.RequestType + Environment.NewLine;

                if (context.Request.UrlReferrer != null)
                {
                    record += "Url referrer: " + context.Request.UrlReferrer.AbsoluteUri + Environment.NewLine;
                }

                record += "User agent: " + context.Request.UserAgent + Environment.NewLine;
                record += "User host address: " + context.Request.UserHostAddress + Environment.NewLine;

                if (host != null)
                {
                    record += "User host name: " + host.HostName + Environment.NewLine;
                }

                record += "Absolute URI: " + context.Request.Url.AbsoluteUri + Environment.NewLine;
                record += "Total bytes: " + context.Request.TotalBytes + Environment.NewLine;

                foreach (string param in context.Request.Form.AllKeys)
                {
                    if (param == "__VIEWSTATE" || param == "__EVENTARGUMENT" || param == "__EVENTVALIDATION")
                    {
                        //Do not log viewstate and other useless information, otherwise .. log file will have big size
                        continue;
                    }

                    record += "Param: " + param + " Value: " + context.Request.Form[param] + Environment.NewLine;
                }

                foreach (string appKey in context.Application.AllKeys)
                {
                    record += "Application key: " + appKey + " Value: " + context.Application[appKey].ToString() + Environment.NewLine;
                }
            }
            catch
            {
                //If any exception occurs, do nothing. What you can do??
            }

            return record;
        }

        /// <summary>
        /// Returns string, that contains usefull information about current HttpContext class
        /// </summary>
        /// <returns>Returns string, that contains usefull information about HttpContext class</returns>
        private static string AddHTTPContextInfo()
        {
            return AddHTTPContextInfo(HttpContext.Current);
        }
        #endregion
    }
}