using Daftari.AquaCards.DAL;
using Daftari.AquaCards.Models;
using Daftari.Handlers;
using Daftari.Pike13Api.DAL;
using Daftari.Pike13Api.Models;
using Daftari.Pike13Api.Services;
using Daftari.ViewModel;
using LukeApps.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Daftari.Controllers
{
   
    public class CustomersController : BaseController
    {
        private AquaCardsEntities db = new AquaCardsEntities(System.Web.HttpContext.Current.User.Identity.Name);

        // GET: Customers
        [Authorize(Roles = "Admin,Manager,FrontDesk,Customers")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin,Manager,FrontDesk,Customers")]
        public async Task<JsonResult> GetJSON()
        {
            List<Customer> customers = await db.GetCustomers().ToListAsync();

            var detailCollection = customers.Select(s => new
            {
                CustomerID = s.CustomerID,
                FullName = s.FullName,
                MotherName = s.MotherName,
                FatherName = s.FatherName,
                Address = s.Address,
                City = s.City,
                Region = s.Region,
                PrimaryPhone = s.PrimaryPhone,
                CellPhone = s.CellPhone,
                AlternatePhone = s.AlternatePhone,
                EmailAddress = s.EmailAddress,
                Birthday = s.Birthday,
                IsMember = s.IsMember,
                IsSubToCommunications = s.IsSubToCommunications,
                CustomerStatus = s.CustomerStatus.GetDisplay(),
                Referral = s.Referral,
                AuditDetail = s.AuditDetail.ToString()
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

        // GET: Customers/Details/5
        [Authorize(Roles = "Admin,Manager,FrontDesk,Customers")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.GetCustomers().FirstAsync(c => c.CustomerID == id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(new RegistrationFormSummaryVM(customer));
        }

        // GET: Customers/Edit/5
        [Authorize(Roles = "Admin,Manager,FrontDesk,Customers")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.GetCustomers().FirstAsync(c => c.CustomerID == id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(new RegistrationFormVM()
            {
                CustomerID = customer.CustomerID,
                FullName = customer.FullName,
                MotherName = customer.MotherName,
                FatherName = customer.FatherName,
                Address = customer.Address,
                City = customer.City,
                Birthday = customer.Birthday,
                IsMember = customer.IsMember,
                IsSubToCommunications = customer.IsSubToCommunications,
                Region = customer.Region,
                PrimaryPhone = customer.PrimaryPhone,
                CellPhone = customer.CellPhone,
                AlternatePhone = customer.AlternatePhone,
                EmailAddress = customer.EmailAddress,
                Referral = customer.Referral
            });
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,FrontDesk,Customers")]
        public async Task<ActionResult> Edit(RegistrationFormVM customerForm)
        {
            if (ModelState.IsValid)
            {
                Customer customer = new Customer(customerForm);
                Customer PreCustomer = db.GetCustomers().First(c => c.CustomerID == customerForm.CustomerID);

                PreCustomer.FullName = customer.FullName;
                PreCustomer.MotherName = customer.MotherName;
                PreCustomer.FatherName = customer.FatherName;
                PreCustomer.Address = customer.Address;
                PreCustomer.City = customer.City;
                PreCustomer.Region = customer.Region;
                PreCustomer.PrimaryPhone = customer.PrimaryPhone;
                PreCustomer.CellPhone = customer.CellPhone;
                PreCustomer.AlternatePhone = customer.AlternatePhone;
                PreCustomer.Birthday = customer.Birthday;
                PreCustomer.EmailAddress = customer.EmailAddress;
                PreCustomer.Referral = customer.Referral;
                PreCustomer.IsMember = customer.IsMember;
                PreCustomer.IsSubToCommunications = customer.IsSubToCommunications;

                db.Entry(PreCustomer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(customerForm);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,FrontDesk,Customers")]
        public async Task<ActionResult> GetRegistratonForm(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.GetCustomers().FirstAsync(c => c.CustomerID == id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return Json(new RegistrationFormSummaryVM(customer),
            JsonRequestBehavior.AllowGet);
        }

        // GET: Customers/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.GetCustomers().FirstAsync(c => c.CustomerID == id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Customer customer = await db.GetCustomers().FirstAsync(c => c.CustomerID == id);
            db.Customers.Remove(customer);
            await db.SaveChangesAsync();
            return RedirectToAction("Customers");
        }
        [Authorize(Roles = "Admin,Manager,FrontDesk,Customers,Commmunication")]
        public async Task<JsonResult> SyncCustomers(int type)
        {
            string syncDate = null;
            string now = DateTime.Now.ToUniversalTime().ToString("s");
            string dteType = null;
            string syncsettingKey = null;
            string flagsettingKey;
            string sd = TokenProvider.GetProvider().GetSubdomain(User.Identity.Name);

            switch (type)
            {
                case 0:
                    dteType = "updated_since";
                    syncsettingKey = sd + "PplSyncDte";
                    flagsettingKey = sd + "PplSyncFlag";
                    break;

                case 9:
                    syncsettingKey = sd + "PplSyncDte";
                    flagsettingKey = sd + "PplSyncFlag";
                    break;

                default:
                    return Json(new { msg = "Invalid Type", type = 2 }, JsonRequestBehavior.AllowGet);
            }

            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                var setting = db.Settings.Find(syncsettingKey);
                if (setting != null)
                {
                    syncDate = setting.Value;
                    setting.Value = now;
                    db.Entry(setting).State = EntityState.Modified;
                }
                else
                {
                    db.Settings.Add(new Setting { Key = syncsettingKey, Value = now });
                }

                var flag = db.Settings.Find(flagsettingKey);
                if (flag != null)
                {
                    if (bool.Parse(flag.Value))
                    {
                        return Json(new { msg = "Sync In Progress", type = 1 }, JsonRequestBehavior.AllowGet);
                    }
                    flag.Value = true.ToString();
                    db.Entry(flag).State = EntityState.Modified;
                }
                else
                {
                    db.Settings.Add(new Setting { Key = flagsettingKey, Value = true.ToString() });
                }

                await db.SaveChangesAsync();
            }

            try
            {
                List<Pike13Person> data;
                if (dteType != null)
                {
                    data = await new Pike13ApiRepo(User.Identity.Name).GetPeopleAsync(dteType, syncDate);
                }
                else
                {
                    data = await new Pike13ApiRepo(User.Identity.Name).GetPeopleAsync();
                }

                processAndSync(data);
            }
            catch (Exception ex)
            {
                using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(ex, this.HttpContext))
                {
                    bh.Log_Error();
                }

                revertSettings(flagsettingKey, syncsettingKey, syncDate);

                return Json(new { msg = ex.Message, type = 2 }, JsonRequestBehavior.AllowGet);
            }

            revertSettings(flagsettingKey, syncsettingKey);

            return Json(new { msg = "Success", type = 2 }, JsonRequestBehavior.AllowGet);
        }

        private static void revertSettings(string flagsettingKey, string syncsettingKey, string syncdate = null)
        {
            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                var flag = db.Settings.Find(flagsettingKey);

                flag.Value = false.ToString();
                db.Entry(flag).State = EntityState.Modified;

                if (syncdate != null)
                {
                    var setting = db.Settings.Find(syncsettingKey);
                    setting.Value = syncdate;
                    db.Entry(setting).State = EntityState.Modified;
                }

                db.SaveChanges();
            }
        }

        private int processAndSync(List<Pike13Person> data)
        {
            var sd = TokenProvider.GetProvider().GetSubdomain(User.Identity.Name);
            var customers = data.Select(c => new Customer()
            {
                ExternalReference = c.id,
                FirstName = c.first_name,
                MiddleName = c.middle_name,
                LastName = c.last_name,
                GuardianName = c.guardian_name,
                FullName = string.Format("{0}{1}{2}", c.first_name == null ? "" : c.first_name + " ", c.middle_name == null ? "" : c.middle_name + " ", c.last_name ?? ""),
                PrimaryPhone = c.phone,
                EmailAddress = c.email,
                Address = c.address,
                City = c.timezone,
                IsMember = c.membership == "No Membership" ? false : true,
                Birthday = c.birthdate == null ? (DateTime?)null : DateTime.Parse(c.birthdate),
                IsSubToCommunications = true,
                SubDomain = sd,
                PhotoMD = c.profile_photo?.x200,
                PhotoLG = c.profile_photo?.x400,
                Dependants = string.Join(",", c.dependents.Select(d => d.id)),
                Providers = string.Join(",", c.providers.Select(p => p.id)),
            });

            var extIDs = customers.Select(c => c.ExternalReference).ToArray();

            using (AquaCardsEntities db = new AquaCardsEntities(User.Identity.Name))
            {
                var existingCustomers = db.Customers.Where(q => q.SubDomain == sd && extIDs.Contains(q.ExternalReference)).ToList();
                var newCustomers = customers.Where(c => !existingCustomers.Any(e => e.ExternalReference == c.ExternalReference));
                db.Customers.AddRange(newCustomers);

                foreach (var item in existingCustomers)
                {
                    var ext = customers.Where(c => c.ExternalReference == item.ExternalReference).First();
                    item.FirstName = ext.FirstName;
                    item.LastName = ext.LastName;
                    item.FullName = ext.FullName;
                    item.GuardianName = ext.GuardianName;

                    if (ext.EmailAddress != null)
                        item.EmailAddress = ext.EmailAddress;

                    if (ext.PrimaryPhone != null)
                        item.PrimaryPhone = ext.PrimaryPhone;
                    item.EmailAddress = ext.EmailAddress;

                    if (ext.Birthday != null)
                        item.Birthday = ext.Birthday;

                    item.PhotoMD = ext.PhotoMD;
                    item.PhotoLG = ext.PhotoLG;
                    item.Dependants = ext.Dependants;
                    item.Providers = ext.Providers;
                    db.Entry(item).State = EntityState.Modified;
                }

                return db.SaveChanges();
            }
        }
    }
}