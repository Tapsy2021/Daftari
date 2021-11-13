using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace LukeApps.EmailHandling
{
    public class EmailMessage
    {
        private Dictionary<string, string> _parameters = null;

        private List<Attachment> _att = null;

        private string[] _cCRecipients;

        private string[] _bCCRecipients;

        public EmailMessage()
        {
            _parameters = new Dictionary<string, string>();
            _att = new List<Attachment>();
        }

        public EmailMessage(EmailMessage mail)
        {
            Sender = mail.Sender;
            SenderName = mail.SenderName;
            Recipient = mail.Recipient;
            RecipientName = mail.RecipientName;

            CCRecipients = mail.CCRecipients;
            BCCRecipients = mail.BCCRecipients;

            Subject = mail.Subject;
            Body = mail.Body;

            _att = mail.Attachments;
            _parameters = mail.Parameters;
        }

        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string Recipient { get; set; }
        public string RecipientName { get; set; }

        public string[] CCRecipients
        {
            get => _cCRecipients?.ToList().Distinct().ToArray();
            set => _cCRecipients = value;
        }

        public string[] BCCRecipients
        {
            get => _bCCRecipients?.ToList().Distinct().ToArray();
            set => _bCCRecipients = value;
        }

        public string Subject { get; set; }
        public string Body { get; set; }

        public List<Attachment> Attachments => _att;
        public Dictionary<string, string> Parameters => _parameters;

        public void AddAttachement(string attachmentFilePath) =>
            _att.Add(new Attachment(Path.Combine(HttpRuntime.AppDomainAppPath, attachmentFilePath)));

        public void AddAttachement(byte[] fileBytes, string name, string ContentType) =>
            _att.Add(new Attachment(new MemoryStream(fileBytes), name, ContentType));

        public void AddAttachements(List<HttpPostedFileBase> attachments)
        {
            foreach (HttpPostedFileBase attachment in attachments)
            {
                if (attachment != null)
                {
                    string fileName = Path.GetFileName(attachment.FileName);
                    _att.Add(new Attachment(attachment.InputStream, fileName, attachment.ContentType));
                }
            }
        }

        public void AddParameter(string Key, string Value) =>
              _parameters.Add(string.Format("{{{0}}}", Key), Value);
    }
}