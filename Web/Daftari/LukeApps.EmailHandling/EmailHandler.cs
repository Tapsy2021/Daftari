using LukeApps.BugsTracker;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LukeApps.EmailHandling
{
    public class EmailHandler : IDisposable
    {        
        private readonly string host;
        private readonly int port;
        private readonly string user;
        private readonly string pass;
        private readonly bool isSSL;
        private readonly bool isAllowed;
        private readonly string domain;
        private readonly string sender;

        private List<ComposedEmail> messages = null;
        private SmtpClient smtp = null;

        private string emailTemplate;

        public EmailHandler(IEnumerable<EmailMessage> mails, string branchSubdomain, string templateOverride = null)
        {
            host = ConfigurationManager.AppSettings["MailServer"];
            port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            user = ConfigurationManager.AppSettings["EmailAuthUser-" + branchSubdomain];
            pass = ConfigurationManager.AppSettings["EmailAuthPass-" + branchSubdomain];
            isSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL"]);
            sender = ConfigurationManager.AppSettings["EmailFromAddress-" + branchSubdomain];
            domain = ConfigurationManager.AppSettings["EmailDomain"];
            isAllowed = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableEmail"]);

            emailTemplate = TemplateRepo.GetInstance().GetTemplate(templateOverride);

            messages = new List<ComposedEmail>();

            foreach (var mail in mails)
                addMail(mail);
        }

        public EmailHandler(EmailMessage mail, string templateOverride = null)
        {
            host = ConfigurationManager.AppSettings["MailServer"];
            port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            user = ConfigurationManager.AppSettings["EmailAuthUser"];
            pass = ConfigurationManager.AppSettings["EmailAuthPass"];
            isSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL"]);
            sender = ConfigurationManager.AppSettings["EmailFromAddress"];
            domain = ConfigurationManager.AppSettings["EmailDomain"];
            isAllowed = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableEmail"]);

            emailTemplate = TemplateRepo.GetInstance().GetTemplate(templateOverride);

            messages = new List<ComposedEmail>();

            addMail(mail);
        }

        public int Send()
        {
            if (isAllowed)
            {
                smtpInitalize();

                foreach (var message in messages)
                {
                    try
                    {
                        smtp.Send(message.Content);
                    }
                    catch (Exception ex)
                    {
                        using (BugsHandler bh = new BugsHandler(ex, "-"))
                            bh.Log_Error($"{message.Content.To.ToString()} - {message.Content.Subject}");

                        return 0;
                    }
                }

                return 1;
            }
            else
            {
                return 0;
            }
        }

        public Task SendAsync()
        {
            if (isAllowed)
            {
                smtpInitalize();

                foreach (var message in messages)
                {
                    if (ConfigurationManager.AppSettings["Environment"] == "Test")
                    {
                        message.Content.To.Clear();
                        message.Content.CC.Clear();
                        message.Content.Bcc.Clear();
                        message.Content.To.Add("contact@vishj.com");
                    }

                    try
                    {
                        smtp.Send(message.Content);
                    }
                    catch (Exception ex)
                    {
                        using (BugsHandler bh = new BugsHandler(ex, "-"))
                            bh.Log_Error($"{message.Content.To.ToString()} - {message.Content.Subject}");
                    }
                }

                return Task.FromResult(1);
            }
            return Task.FromResult(0);
        }

        public string SendReturnError()
        {
            smtpInitalize();

            foreach (var message in messages)
            {
                try
                {
                    smtp.Send(message.Content);
                }
                catch (Exception ex)
                {
                    return ex.Message;    
                }
            }

            return "Success";
        }

        public void Dispose()
        {
            if (messages != null)
            {
                foreach (var mail in messages)
                {
                    mail.Dispose();
                }
            }

            if (smtp != null)
                smtp.Dispose();

            emailTemplate = null;
        }

        private void addMail(EmailMessage mail)
        {
            if (mail.Parameters.ContainsKey("{Name}"))
                mail.Parameters["{Name}"] = mail.RecipientName;
            else
                mail.Parameters.Add("{Name}", mail.RecipientName);

            if (mail.Parameters.ContainsKey("{Message}"))
                mail.Parameters["{Message}"] = mail.Body;
            else
                mail.Parameters.Add("{Message}", mail.Body);

            var sender = mail.Sender ?? this.sender;
            var recipients = mail.Recipient.Split(',');
            var message = new MailMessage(new MailAddress(sender, mail.SenderName ?? sender), new MailAddress(recipients[0], mail.RecipientName))
            {
                Subject = mail.Subject.Replace('\n', ' ').Replace('\r', ' '),
                IsBodyHtml = true
            };

            int recipientsLength = recipients.Length;
            if (recipientsLength > 1)
                for (int i = 0; i < recipientsLength; i++)
                    message.To.Add(recipients[i]);

            var mailCCRecipients = mail.CCRecipients;
            if (mailCCRecipients != null)
                foreach (var CCRecipient in mailCCRecipients)
                    if (CCRecipient != null)
                        message.CC.Add(CCRecipient);

            var mailBCCRecipients = mail.BCCRecipients;
            if (mailBCCRecipients != null)
                foreach (var BCCRecipient in mailBCCRecipients)
                    if (BCCRecipient != null)
                        message.Bcc.Add(BCCRecipient);

            var attachements = mail.Attachments;
            if (attachements.Count != 0)
                foreach (var attachement in attachements)
                    message.Attachments.Add(attachement);

            messages.Add(new ComposedEmail(message, mail.Parameters, emailTemplate));
        }

        /// <summary>
        /// Sends Email Asynchronously
        /// </summary>
        /// <returns>int</returns>
        //public Task SendAsync()
        //{
        //    messageIntialize();
        //    smtpInitalize();
        //    return _smtp.SendMailAsync(_message);
        //}
        private void smtpInitalize()
        {
            smtp = new SmtpClient(host, port);

            if (user.Length > 0 && pass.Length > 0)
            {
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                if (string.IsNullOrEmpty(domain))
                    smtp.Credentials = new NetworkCredential(user, pass);
                else
                    smtp.Credentials = new NetworkCredential(user, pass, domain);

                smtp.EnableSsl = isSSL;
            }
        }
    }
}