using System;
using System.Configuration;
using System.IO;
using System.Linq;
using BookingAds.Services.SendMail.Abstractions;
using MailKit.Net.Smtp;
using MimeKit;

namespace BookingAds.Services.SendMail
{
    public class SendMailService : ISendMailService
    {
        public void SendMail(string mailBody, string[] mailTo, string subject, bool isHtml = false)
        {
            if (mailTo == null)
            {
                return;
            }

            mailTo = mailTo.Where(o => !string.IsNullOrEmpty(o)).ToArray();
            if (mailTo.Length == 0)
            {
                return;
            }

            using (var message = new MimeMessage())
            {
                var from = MailboxAddress.Parse(ConfigurationManager.AppSettings["MailFrom"]);
                message.From.Add(from);

                foreach (var itemMailTo in mailTo)
                {
                    var to = MailboxAddress.Parse(itemMailTo);
                    message.To.Add(to);
                }

                message.Subject = subject;
                message.Body = new TextPart(isHtml ? "html" : "plain")
                {
                    Text = mailBody,
                };

                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect(ConfigurationManager.AppSettings["MailServerDomain"], int.Parse(ConfigurationManager.AppSettings["MailPort"]), false);
                    client.Authenticate(ConfigurationManager.AppSettings["MailUsername"], ConfigurationManager.AppSettings["MailPassword"]);
                    client.Send(message);
                    client.Disconnect(true);

                    // CA2202
                    // client.Dispose();
                }
            }
        }

        public void SendMail(string mailBody, string mailTo, string subject, bool isHtml = false)
        {
            if (string.IsNullOrEmpty(mailTo) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(mailBody))
            {
                // validate
                return;
            }

            try
            {
                using (var message = new MimeMessage())
                {
                    var from = MailboxAddress.Parse(ConfigurationManager.AppSettings["MailFrom"]);
                    message.From.Add(from);
                    message.To.Add(MailboxAddress.Parse(mailTo)); // Chuyển đổi mailTo thành MailboxAddress

                    message.Subject = subject;
                    message.Body = new TextPart(isHtml ? "html" : "plain")
                    {
                        Text = mailBody,
                    };

                    using (SmtpClient client = new SmtpClient())
                    {
                        try
                        {
                            // write log
                            string logMessage = $"Connecting to SMTP server: {ConfigurationManager.AppSettings["MailServerDomain"]} on port {ConfigurationManager.AppSettings["MailPort"]}";
                            WriteToLog(logMessage);

                            // connect to server mail
                            client.Connect(ConfigurationManager.AppSettings["MailServerDomain"], int.Parse(ConfigurationManager.AppSettings["MailPort"]), false);

                            // write log when authenticationg
                            logMessage = "Authenticating with SMTP server...";
                            WriteToLog(logMessage);

                            client.Authenticate(ConfigurationManager.AppSettings["MailUsername"], ConfigurationManager.AppSettings["MailPassword"]);

                            // write log when sending mail
                            logMessage = "Sending email...";
                            WriteToLog(logMessage);

                            // send email
                            client.Send(message);

                            // write log when disconnecting
                            logMessage = "Disconnecting from SMTP server...";
                            WriteToLog(logMessage);
                            client.Disconnect(true);
                        }
                        catch (Exception ex)
                        {
                            WriteToLog("Error: " + ex.Message);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        private static void WriteToLog(string logMessage)
        {
            string logFilePath = @"C:\Logs\smtp_log.txt";
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + logMessage);
            }
        }
    }
}