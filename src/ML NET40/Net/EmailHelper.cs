using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace ML.Net
{
    public class EmailHelper
    {
        /// <summary>
        /// send email
        /// </summary>
        /// <param name="address"></param>
        /// <param name="fromName"></param>
        /// <param name="toAddress"></param>
        /// <param name="serverHost"></param>
        /// <param name="port"></param>
        /// <param name="subject">此电子邮件的主题行</param>
        /// <param name="content"></param>
        /// <param name="isHtml"></param>
        public static void Send(string fromAddress, string fromName, List<string> toAddress, string subject, string content, bool isBodyHtml, string host, int port)
        {
            MailAddress fromMailAddress = new MailAddress(fromAddress, fromName);
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = fromMailAddress;
            foreach (string item in toAddress)
            {
                mailMessage.To.Add(item);
            }
            mailMessage.Attachments.Add(new Attachment(""));
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = isBodyHtml;
            mailMessage.Body = content;
            SmtpClient smtpClient = new SmtpClient(host, port);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Send(mailMessage);
        }

        public static void Send(string address, string formName, string toAddress, string serverHost, int port, string subject, string content, bool isHtml, string userName, string password)
        {
            SmtpClient smtpClient = new SmtpClient();
            MailMessage mailMessage = new MailMessage();
            MailAddress from = new MailAddress(address, formName);
            smtpClient.Credentials = new NetworkCredential(userName, password);
            smtpClient.Host = serverHost;
            smtpClient.Port = port;
            mailMessage.From = from;
            mailMessage.To.Add(toAddress);
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = isHtml;
            mailMessage.Body = content;
            smtpClient.Send(mailMessage);
        }

        public static bool IsEmailValid(string inputEmail)
        {
            string pattern = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(inputEmail);
        }
    }
}