using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace CanonImportService
{
    public class CanonConfigManager
    {
        /// <summary>
        /// UploadDirectory
        /// </summary>
        public static string UploadDataFolder
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["UploadDataFolder"].ToString(); }
        }

        /// <summary>
        /// ServerLanguage
        /// </summary>
        public static string ServerLanguage
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["ServerLanguage"].ToString(); }
        }

        /// <summary>
        /// TimeToStart
        /// </summary>
        public static DateTime TimeToStart
        {
            get 
            {
                try
                {
                    string value = System.Configuration.ConfigurationManager.AppSettings["TimeToStart"].ToString();
                    string today = DateTime.Now.ToString("dd.MM.yyyy");
                    string formated = string.Format("{0} {1}:00", today, value);
                    DateTime parsed =
                            DateTime.ParseExact(formated, "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                    return parsed;
                }
                catch (Exception)
                {
                }
                return DateTime.Now.AddYears(1);
            }
        }

        /// <summary>
        /// ForceStart
        /// </summary>
        public static bool ForceStart
        {
            get { return bool.Parse(System.Configuration.ConfigurationManager.AppSettings["ForceStart"].ToString()); }
        }
    }
}
