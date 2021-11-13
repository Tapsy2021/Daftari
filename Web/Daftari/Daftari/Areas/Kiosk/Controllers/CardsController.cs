using Daftari.AquaCards.DAL;
using Daftari.Areas.Kiosk.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Daftari.Areas.Kiosk.Controllers
{
    public class CardsController : Controller
    {
        private AquaCardsEntities db = new AquaCardsEntities();

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string q)
        {
            var customers = db.Customers.Where(c => c.PrimaryPhone == q).ToList();
            var dependants = customers.Select(c => c.Dependants.Split(',')).SelectMany(c => c).Where(c => !string.IsNullOrEmpty(c)).Select(c => long.Parse(c)).ToArray();
            var customerIDs = db.Customers.Where(c => dependants.Contains(c.ExternalReference)).Select(c => c.CustomerID).ToList();
            customerIDs.Add(customers.FirstOrDefault()?.CustomerID ?? new Guid());
            return RedirectToAction("Index", new { ids = string.Join(",", customerIDs), displayId = (Guid?)customerIDs.FirstOrDefault() });
        }

        public ActionResult Index(string ids, Guid? displayId)
        {
            List<KeyValuePair<Guid, string>> customerPairs;

            List<KeyValuePair<Guid, string>> pairs = new List<KeyValuePair<Guid, string>>();

            if (ids != null)
            {
                var idsArr = ids.Split(',').Select(i => new Guid(i)).ToArray();
                customerPairs = db.Customers.Where(c => idsArr.Contains(c.CustomerID)).AsEnumerable().Select(c => new KeyValuePair<Guid, string>(c.CustomerID, c.FullName)).ToList();
                pairs = customerPairs.ToList();

                foreach (var item in customerPairs)
                {
                    if (item.Key == displayId)
                    {
                        pairs.Remove(item);
                    }
                    else if (!db.StudentCards.Any(c => c.CustomerID == item.Key))
                    {
                        pairs.Remove(item);
                    }
                }
            }

            var card = db.StudentCards.Include(s => s.StudentCardDetails).Include(c => c.Customer).Where(s => s.CustomerID == displayId).OrderByDescending(s => s.Level).FirstOrDefault();

            return View(new KioskCardVM { Ids = pairs, Card = card });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}