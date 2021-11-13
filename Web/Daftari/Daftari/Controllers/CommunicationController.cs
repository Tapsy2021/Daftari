using Daftari.AquaCards.DAL;
using Daftari.AquaCards.Models;
using Daftari.Handlers;
using Daftari.Pike13Api.Services;
using Daftari.SMSHandling;
using Daftari.ViewModel;
using LukeApps.EmailHandling;
using LukeApps.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Daftari.Controllers
{
    [Authorize(Roles = "Admin,Manager,FrontDesk,Commmunication")]
    public class CommunicationController : BaseController
    {
        private AquaCardsEntities db = new AquaCardsEntities(System.Web.HttpContext.Current.User.Identity.Name);
        private SMSEngine sms = new SMSEngine(System.Web.HttpContext.Current.User.Identity.Name, true);

        private List<HttpPostedFileBase> fileList;

        private int totalContent;

        public ActionResult AddNewMobile()
        {
            var mobile = new Mobile();
            return PartialView("_Mobile", mobile);
        }

        //fast json export to client
        public async Task<JsonResult> getJSON(DateTime? from = null, DateTime? to = null)
        {
            var sMSLogs = await sms.GetLogsAsync(from ?? DateTime.Now.Date, to ?? DateTime.Now.Date.AddHours(24));
            var detailCollection = sMSLogs.Select(s => new
            {
                Recipients = s.Recipients,
                API = s.SMSAPI.GetDisplay(),
                Message = s.Message,
                Language = s.Language.ToString(),
                ScheddateTime = s.ScheddateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                TimeSent = s.TimeSent.ToString("yyyy-MM-dd HH:mm:ss"),
                SMSPushResult = s.SMSStatus,
                CreatedEntryUser = s.AuditDetail.CreatedEntryUserID,
                RecipientType = s.RecipientType.ToString(),
                optValue = s.optValue
            });
            var TotalRecords = detailCollection.Count();
            return Json(new
            {
                iTotalRecords = TotalRecords,
                iTotalDisplayRecords = TotalRecords,
                aaData = detailCollection
            },
            JsonRequestBehavior.AllowGet);
        }

        // GET: Communication

        public ActionResult Index()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            if (creds.Subdomain != "atss-muscat")
                return RedirectToAction("Index", "Dashboard", new { errorMessage = "No Permission" });
            return View();
        }

        // GET: Communication/Send

        public ActionResult Send()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            if (creds.Subdomain != "atss-muscat")
                return RedirectToAction("Index", "Dashboard", new { errorMessage = "No Permission" });
            return View(new SMSMessage());
        }

        // POST: Communication/Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(SMSMessage msg)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            if (creds.Subdomain != "atss-muscat")
                return RedirectToAction("Index", "Dashboard", new { errorMessage = "No Permission" });

            if (ModelState.IsValid)
            {
                msg.setMobileNumbersFromCSV();
                sms.Send(msg);
                ViewBag.ErrorMessage = sms.Result;
                return View("Index");
            }
            return View(sms);
        }

        [HttpPost]
        public ActionResult SendMailsToCustomer(string extIDs)
        {
            var customers = db.GetCustomers().Where(c => c.IsSubToCommunications == true).ToList();

            List<string> ids = null;

            if (extIDs != null)
            {
                ids = getCustomerIds(customers, extIDs, true);
            }

            ViewBag.Customers = new MultiSelectList(customers.Where(c => c.EmailAddress != null), "CustomerID", "FullName", ids);
            return View("SendCustomerMails", new CustomerMailsVM());
        }

        // GET: Communication/SendCustomerMails
        public ActionResult SendCustomerMails(string errorMessage = null, string extIDs = null)
        {
            ViewBag.ErrorMessage = errorMessage;
            var customers = db.GetCustomers().Where(c => c.IsSubToCommunications == true).ToList();

            List<string> ids = null;

            if (extIDs != null)
            {
                ids = getCustomerIds(customers, extIDs, true);
            }

            ViewBag.Customers = new MultiSelectList(customers.Where(c => c.EmailAddress != null), "CustomerID", "FullName", ids);
            return View(new CustomerMailsVM());
        }

        // POST: Communication/SendCustomerMails
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendCustomerMails(CustomerMailsVM cMails)
        {
            if (ModelState.IsValid)
            {
                if (!cMails.Customers.Any())
                {
                    ModelState.AddModelError("Customers", "Select atleast one customer");
                    ViewBag.Customers = new MultiSelectList(db.GetCustomers().Where(c => c.IsSubToCommunications == true && c.EmailAddress != null), "CustomerID", "FullName", cMails.Customers);
                    return View(cMails);
                }

                var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

                var customerList = db.GetCustomers().Where(c => c.IsSubToCommunications == true && c.EmailAddress != null && cMails.Customers.Contains(c.CustomerID)).ToList();
                proccessFiles(Request.Files);

                var mails = new List<EmailMessage>();

                foreach (var customer in customerList)
                {
                    if (totalContent > 14680064)
                    {
                        ModelState.AddModelError("Attachements", "Attached files are greater that 14MB.");
                        ViewBag.Customers = new SelectList(db.GetCustomers().Where(c => c.IsSubToCommunications == true && c.EmailAddress != null), "CustomerID", "FullName");
                        return View(cMails);
                    }

                    var mail = new EmailMessage()
                    {
                        SenderName = creds.BusinessName,
                        Body = cMails.EmailMessage,
                        Subject = cMails.Subject,
                        RecipientName = customer.FullName,
                        Recipient = customer.EmailAddress
                    };

                    mail.AddAttachements(fileList);
                    mails.Add(mail);
                }

                int status;
                using (EmailHandler eh = new EmailHandler(mails, creds.Subdomain))
                    status = eh.Send();

                return RedirectToAction("SendCustomerMails", new { errorMessage = (status == 0 ? "Failed. Contact Admin for Support." : "Success") });
            }

            ViewBag.Customers = new MultiSelectList(db.GetCustomers().Where(c => c.IsSubToCommunications == true && c.EmailAddress != null), "CustomerID", "FullName", cMails.Customers);
            return View(cMails);
        }

        private List<string> getCustomerIds(List<Customer> customers, string extReferences, bool IsEmail)
        {
            var externalRefArray = extReferences.Split(',').Where(l => !string.IsNullOrEmpty(l)).Select(l => long.Parse(l)).ToArray();
            var selectedcustomers = customers.Where(c => externalRefArray.Contains(c.ExternalReference)).ToList();
            List<string> gplist = new List<string>();

            if (selectedcustomers.Any())
            {
                foreach (var customer in selectedcustomers)
                {
                    if ((IsEmail && customer.EmailAddress != null) || (!IsEmail && customer.PrimaryPhone != null))
                    {
                        gplist.Add(customer.CustomerID.ToString());
                    }
                    else
                    {
                        var providerIDs = customer.Providers.Split(',').Where(c => !string.IsNullOrEmpty(c)).Select(c => long.Parse(c)).ToArray();

                        var provider = customers.Where(c => providerIDs.Contains(c.ExternalReference)).FirstOrDefault();

                        if (provider != null)
                            gplist.Add(provider.CustomerID.ToString());
                    }
                }
            }

            return gplist;
        }

        [HttpPost]
        public ActionResult SendSMSToCustomer(string extIDs)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            if (creds.Subdomain != "atss-muscat")
                return RedirectToAction("Index", "Dashboard", new { errorMessage = "No Permission" });

            var customers = db.GetCustomers().Where(c => c.IsSubToCommunications == true).ToList();
            List<string> ids = null;

            if (extIDs != null)
            {
                ids = getCustomerIds(customers, extIDs, true);
            }

            ViewBag.Customers = new MultiSelectList(customers.Where(c => c.PrimaryPhone != null), "CustomerID", "FullName", ids);
            return View("SendCustomerSMS", new CustomerSMSVM());
        }

        // GET: Communication/SendCustomerSMS

        public ActionResult SendCustomerSMS(string extIDs = null)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            if (creds.Subdomain != "atss-muscat")
                return RedirectToAction("Index", "Dashboard", new { errorMessage = "No Permission" });

            var customers = db.GetCustomers().Where(c => c.IsSubToCommunications == true).ToList();
            List<string> ids = null;

            if (extIDs != null)
            {
                ids = getCustomerIds(customers, extIDs, true);
            }

            ViewBag.Customers = new MultiSelectList(customers.Where(c => c.PrimaryPhone != null), "CustomerID", "FullName", ids);
            return View(new CustomerSMSVM());
        }

        // POST: Communication/SendCustomerSMS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendCustomerSMS(CustomerSMSVM cSms)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            if (creds.Subdomain != "atss-muscat")
                return RedirectToAction("Index", "Dashboard", new { errorMessage = "No Permission" });

            if (ModelState.IsValid)
            {
                var customerList = db.GetCustomers().Where(c => c.IsSubToCommunications == true && c.PrimaryPhone != null && cSms.Customers.Contains(c.CustomerID)).ToList();

                foreach (var customer in customerList)
                {
                    var msg = new SMSMessage
                    {
                        Message = string.Format("Dear {0},\n{1}", customer.FullName, cSms.Message),
                        Language = cSms.Language,
                        SMSAPI = cSms.SMSAPI
                    };
                    
                    msg.AddPhone(customer.PrimaryPhone);
                    sms.Send(msg);
                    ViewBag.ErrorMessage = sms.Result;
                }

                return RedirectToAction("Index");
            }

            ViewBag.Customers = new MultiSelectList(db.GetCustomers().Where(c => c.IsSubToCommunications == true && c.PrimaryPhone != null), "CustomerID", "FullName", cSms.Customers);
            return View(cSms);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                sms.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void proccessFiles(HttpFileCollectionBase files)
        {
            var arrFile = files.AllKeys;
            totalContent = 0;
            fileList = new List<HttpPostedFileBase>();
            for (int loop = 0; loop < arrFile.Length; loop++)
            {
                if (files[loop].ContentLength > 0)
                {
                    totalContent += files[loop].ContentLength;
                    fileList.Add(files[loop]);
                }
            }
        }
    }
}