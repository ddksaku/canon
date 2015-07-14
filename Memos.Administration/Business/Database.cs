using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using Memos.Framework;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Memos.Administration.Business
{
    /// <summary>
    /// Database business object class.
    /// </summary>
    public class Database
    {
        // Generate connection string

        #region DefaultConnectionString
        /// <summary>
        /// Default connection string.
        /// </summary>
        /// <returns></returns>
        public static string DefaultConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(ConfigSettings.SqlUserName) ||
                    String.IsNullOrEmpty(ConfigSettings.SqlPassword))
                {
                    return GenerateConnectionString(
                        ConfigSettings.SqlServerName,
                        ConfigSettings.SqlDatabaseName);
                }

                return GenerateConnectionString(
                    ConfigSettings.SqlServerName,
                    ConfigSettings.SqlDatabaseName,
                    ConfigSettings.SqlUserName,
                    ConfigSettings.SqlPassword);
            }
        }
        #endregion

        #region GenerateConnectionString
        /// <summary>
        /// Generate connection string.
        /// </summary>
        /// <returns></returns>
        public static string GenerateConnectionString(string serverName, string databaseName)
        {
            string connectionString = String.Format(
                "Data Source={0};Initial Catalog={1};Integrated Security=True;",
                serverName,
                databaseName);

            return connectionString;
        }
        #endregion

        #region GenerateConnectionString
        /// <summary>
        /// Generate connection string.
        /// </summary>
        /// <returns></returns>
        public static string GenerateConnectionString(string serverName, string databaseName, string userName, string password)
        {
            string connectionString = String.Format(
                "Data Source={0};Initial Catalog={1};Integrated Security=False;User ID={2};Password={3};",
                serverName,
                databaseName,
                userName,
                password);

            return connectionString;
        }
        #endregion

        // Backup database

        #region BackupDatabase
        /// <summary>
        /// Backup database.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="databaseName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>Name of the file.</returns>
        protected static string BackupDatabase(string serverName, string databaseName, string userName, string password)
        {
            Backup backup = new Backup();
            
            backup.Action = BackupActionType.Database;
            backup.BackupSetName = "Archive";
            backup.Database = databaseName;

            string fileName = Guid.NewGuid().ToString();
            BackupDeviceItem deviceItem = new BackupDeviceItem(fileName, DeviceType.File);
            ServerConnection connection = new ServerConnection(serverName, userName, password);
            Server sqlServer = new Server(connection);

            backup.Initialize = true;
            backup.Checksum = true;
            backup.ContinueAfterError = true;

            backup.Devices.Add(deviceItem);
            backup.Incremental = false;

            backup.ExpirationDate = DateTime.Now.AddDays(3);
            backup.LogTruncation = BackupTruncateLogType.Truncate;

            backup.FormatMedia = false;

            backup.SqlBackup(sqlServer);

            return fileName;
        }
        #endregion

        #region BackupDatabase
        /// <summary>
        /// Backup database.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <returns>Name of the file.</returns>
        protected static FileInfo BackupDatabase(string connectionString, string databaseName)
        {
            string fileName = Path.Combine(Utilities.TempDirectory, Guid.NewGuid().ToString());

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = String.Concat("BACKUP DATABASE ", databaseName, " TO DISK = '", fileName, "'");

            sqlCommand.ExecuteNonQuery();

            return new FileInfo(fileName);
        }
        #endregion

        // Send backup methods

        #region SendDatabaseBackup
        /// <summary>
        /// Send message with generated zip file (with database backup) in attach.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        public static void SendDatabaseBackup(string email, string subject, string text, string connectionString, string databaseName)
        {
            string zipFile = GenerateZipFile(connectionString, databaseName);

            try
            {
                List<Attachment> attachments = new List<Attachment>();
                using (FileStream stream = File.OpenRead(zipFile))
                {
                    attachments.Add(new Attachment(stream, String.Concat(databaseName, ".zip")));

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
        /// Generate zip file with database backup.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <returns>Path to the generated file.</returns>
        protected static string GenerateZipFile(string connectionString, string databaseName)
        {
            FileInfo backupFile = BackupDatabase(connectionString, databaseName);

            string zipFile = ZipManager.GenerateZipFile(backupFile, databaseName);

            return zipFile;
        }
        #endregion
    }
}