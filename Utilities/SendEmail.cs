﻿//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017-2019 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using EASendMail;
using InfinityGroup.VesselMonitoring.Globals;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Utilities
{
    public static class SendEmail
    {
        static private object _lock;
        static private System.Threading.Timer _timer;

        static SendEmail()
        {
            EmailQueue = new ObservableCollection<EmailItem>();
            _lock = new object();
            _timer = new System.Threading.Timer(
                ProcessEmailQueue,
                null,
                System.Threading.Timeout.Infinite,
                System.Threading.Timeout.Infinite);

        }

        static public ObservableCollection<EmailItem> EmailQueue { get; set; }
        static public string FromEmailAddress { get; set; }
        static public string FromEmailPassword { get; set; }
        static public int SMTPEncryptionMethod { get; set; }
        static public int SMTPPort { get; set; }
        static public string SMTPServerName { get; set; }
        static public void Send(string to, string vesselName, string subject, string body, string attachmentFileName)
        {
            EmailItem item = new EmailItem(FromEmailAddress, to, vesselName, subject, body, attachmentFileName);

            lock (_lock)
            {
                EmailQueue.Add(item);
            }

            // Try sending email 3 seconds from now.
            _timer.Change(3000, 3000);
        }

        async static private void ProcessEmailQueue(object myObject)
        {
            if (EmailQueue.Count > 0)
            {
                EmailItem item = null;

                // Get the next item of the list.
                lock (_lock)
                {
                    if (EmailQueue.Count > 0)
                    {
                        item = EmailQueue[0];
                        EmailQueue.Remove(item);
                    }
                    else
                    {
                        _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    }
                }

                // Try to send the email.
                await SendEmailMessage(item, () =>
                {
                },
                (ex) =>
                {
                    // We failed to send the email. Put it back on the end of the list and try again.
                    lock (_lock)
                    {
                        EmailQueue.Add(item);
                    }

                    Telemetry.TrackException(ex);
                });
            }
        }

        async static private Task SendEmailMessage(EmailItem item, Action successCallback, Action<Exception> failureCallback)
        {
            if (null != item)
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                oMail.From = new MailAddress(item.FromEmailId);
                oMail.To = new AddressCollection(item.ToEmailId);
                oMail.Sender = new MailAddress(item.DisplayName);
                oMail.Subject = item.Subject;
                oMail.TextBody = item.Body;
                oMail.ReplyTo = new MailAddress("Do not reply");
                oMail.Priority = MailPriority.High;

                if (item.AttachmentName.Length > 0)
                {
                    byte[] content = null;
                    oMail.AddAttachment(item.AttachmentName, content);
                }

                SmtpServer oServer = new SmtpServer(SMTPServerName);
                oServer.User = FromEmailAddress;
                oServer.Password = FromEmailPassword;
                oServer.ConnectType = (SmtpConnectType) SMTPEncryptionMethod;
                oServer.Port = SMTPPort;

                try
                {
                    SmtpClient oSmtp = new SmtpClient();
                    await oSmtp.SendMailAsync(oServer, oMail);
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    failureCallback(ex);
                }
            }

            successCallback();
        }
    }

    public class EmailItem
    {
        public EmailItem(string myFromEmailId,
                         string myToEmailId,
                         string myDisplayName,
                         string mySubject,
                         string myBody,
                         string myFileName)
        {
            this.FromEmailId = myFromEmailId;
            this.ToEmailId = myToEmailId;
            this.DisplayName = myDisplayName;
            this.Subject = mySubject;
            this.Body = myBody;
            this.AttachmentName = myFileName;
        }

        public string FromEmailId { get; set; }
        public string ToEmailId { get; set; }
        public string DisplayName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string AttachmentName { get; set; }
    }
}
