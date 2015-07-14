using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Memos.Framework.Logging;

namespace Memos.Framework
{
    /// <summary>
    /// Email gateway.
    /// </summary>
    public class EmailGateway
    {
        #region Constructors
        /// <summary>
        /// Email gateway constructor.
        /// </summary>
        /// <param name="to">Recipient address</param>
        /// <param name="subject">Subject</param>
        /// <param name="text">Message text</param>
        public EmailGateway(string to, string subject, string text)
        {
            this.to = to;
            this.subject = subject;
            this.text = text;
        }
        #endregion

        #region Properties

        #region From
        /// <summary>
        /// Sender address.
        /// </summary>
        public string From
        {
            get { return String.Format("{0} <{1}>", ConfigSettings.MailFromName, ConfigSettings.MailFromAddress); }
        }
        #endregion

        #region To
        /// <summary>
        /// to
        /// </summary>
        private string to;
        /// <summary>
        /// Recipient address.
        /// </summary>
        public string To
        {
            get { return to; }
            set { to = value; }
        }
        #endregion

        #region Subject
        /// <summary>
        /// subject
        /// </summary>
        private string subject;
        /// <summary>
        /// Subject of the message.
        /// </summary>
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }
        #endregion

        #region Text
        /// <summary>
        /// text
        /// </summary>
        private string text;
        /// <summary>
        /// Message text.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        #endregion

        #endregion

        #region Send
        /// <summary>
        /// Sends an EMail usign SMTP mail protocol.
        /// </summary>
        public void Send()
        {
            try
            {
                MailMessage message = new MailMessage(From, To, Subject, Text);
                message.IsBodyHtml = true;

                SmtpClient smtpClient =
                    !String.IsNullOrEmpty(ConfigSettings.SmtpPort) ?
                    new SmtpClient(ConfigSettings.SmtpServer, Int32.Parse(ConfigSettings.SmtpPort)) :
                    new SmtpClient(ConfigSettings.SmtpServer);

                if (!String.IsNullOrEmpty(ConfigSettings.SmtpUserName))
                {
                    NetworkCredential networkCredential = new NetworkCredential(ConfigSettings.SmtpUserName, ConfigSettings.SmtpPassword);
                    smtpClient.Credentials = networkCredential;
                }

                smtpClient.Send(message);
                Logger.Log(String.Concat(Subject, " The message was sent to ", To), LogLevel.Info);
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString(), LogLevel.Error);
            }
        }
        #endregion

        #region Send
        /// <summary>
        /// Sends an EMail usign SMTP mail protocol.
        /// </summary>
        /// <param name="from">Sender address</param>
        /// <param name="to">Recipient address</param>
        /// <param name="subject">Subject</param>
        /// <param name="text">Message text</param>
        /// <param name="attachments">Message attachments</param>
        public static void Send(string from, string to, string subject, string text, List<Attachment> attachments)
        {
            Send(from, to, subject, text, attachments, false);
        }
        #endregion

        #region Send
        /// <summary>
        /// Sends an EMail usign SMTP mail protocol.
        /// </summary>
        /// <param name="from">Sender address</param>
        /// <param name="to">Recipient address</param>
        /// <param name="subject">Subject</param>
        /// <param name="text">Message text</param>
        /// <param name="attachments">Message attachments</param>
        /// <param name="throwException">Throw exception</param>
        public static void Send(string from, string to, string subject, string text, List<Attachment> attachments, bool throwException)
        {
            try
            {
                if (String.IsNullOrEmpty(from))
                {
                    from = String.Format("{0} <{1}>", ConfigSettings.MailFromName, ConfigSettings.MailFromAddress);
                }

                MailMessage message = new MailMessage(from, to, subject, text);
                message.IsBodyHtml = true;

                foreach (Attachment attachment in attachments)
                {
                    message.Attachments.Add(attachment);
                }

                SmtpClient smtpClient =
                    !String.IsNullOrEmpty(ConfigSettings.SmtpPort) ?
                    new SmtpClient(ConfigSettings.SmtpServer, Int32.Parse(ConfigSettings.SmtpPort)) :
                    new SmtpClient(ConfigSettings.SmtpServer);

                if (!String.IsNullOrEmpty(ConfigSettings.SmtpUserName))
                {
                    NetworkCredential networkCredential = new NetworkCredential(ConfigSettings.SmtpUserName, ConfigSettings.SmtpPassword);
                    smtpClient.Credentials = networkCredential;
                }

                smtpClient.Send(message);
                Logger.Log(String.Concat(subject, " The message was sent to ", to), LogLevel.Info);
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString(), LogLevel.Error);

                if (throwException)
                {
                    throw;
                }
            }
        }
        #endregion
    }
}