using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace LukeApps.EmailHandling
{
    internal class ComposedEmail : IDisposable
    {
        internal ComposedEmail(MailMessage mail, Dictionary<string, string> parameters, string emailTemplate)
        {
            string emailBody = emailTemplate;

            foreach (var item in parameters)
                emailBody = emailBody.Replace(item.Key, item.Value);

            mail.Body = emailBody;
            Content = mail;
        }

        internal MailMessage Content { get; }

        public void Dispose()
        {
            Content.Dispose();
        }
    }
}