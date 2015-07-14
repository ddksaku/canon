using System;
using System.Configuration;

namespace Memos.Framework
{
    /// <summary>
    /// Config values class of Memos framework.
    /// </summary>
    public static class ConfigSettings
    {
        // Common methods of getting config data

        #region GetValueFromConfig
        /// <summary>
        /// Get value from config file.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueFromConfig(string key)
        {
            return GetValueFromConfig(key, "");
        }
        #endregion

        #region GetValueFromConfig
        /// <summary>
        /// Get value from config file.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetValueFromConfig(string key, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];

            if (!String.IsNullOrEmpty(value))
            {
                return value;
            }

            return defaultValue;
        }
        #endregion

        // Application info

        #region ApplicationLogFolder
        /// <summary>
        /// Log folder of log files.
        /// </summary>
        public static string ApplicationLogFolder
        {
            get
            {
                return GetValueFromConfig("ApplicationLogFolder");
            }
        }
        #endregion

        #region ApplicationLogPattern
        /// <summary>
        /// Pattern for log files searching in the application.
        /// </summary>
        public static string ApplicationLogPattern
        {
            get
            {
                return GetValueFromConfig("ApplicationLogPattern");
            }
        }
        #endregion

        // E-mail gateway data

        #region MailFromName
        /// <summary>
        /// Mail from name.
        /// </summary>
        public static string MailFromName
        {
            get
            {
                return GetValueFromConfig("MailFromName");
            }
        }
        #endregion

        #region MailFromAddress
        /// <summary>
        /// Mail from address.
        /// </summary>
        public static string MailFromAddress
        {
            get
            {
                return GetValueFromConfig("MailFromAddress");
            }
        }
        #endregion

        #region SmtpServer
        /// <summary>
        /// SMTP server.
        /// </summary>
        public static string SmtpServer
        {
            get
            {
                return GetValueFromConfig("SmtpServer");
            }
        }
        #endregion

        #region SmtpPort
        /// <summary>
        /// SMTP port.
        /// </summary>
        public static string SmtpPort
        {
            get
            {
                return GetValueFromConfig("SmtpPort");
            }
        }
        #endregion

        #region SmtpUserName
        /// <summary>
        /// SMTP user name.
        /// </summary>
        public static string SmtpUserName
        {
            get
            {
                return GetValueFromConfig("SmtpUserName");
            }
        }
        #endregion

        #region SmtpPassword
        /// <summary>
        /// SMTP password.
        /// </summary>
        public static string SmtpPassword
        {
            get
            {
                return GetValueFromConfig("SmtpPassword");
            }
        }
        #endregion
    }
}