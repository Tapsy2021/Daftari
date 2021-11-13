using Daftari.AquaCards.DAL;
using Daftari.AquaCards.Models;
using Daftari.Areas.NewKiosk.ViewModel;
using Daftari.Handlers;
using Daftari.Helpers;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Daftari.Areas.NewKiosk.Controllers
{
    public class CardsController : Controller
    {
        private AquaCardsEntities db = new AquaCardsEntities();

        public ActionResult Search()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string inputq)
        {
            if (string.IsNullOrEmpty(inputq))
            {
                return View(new KioskCardVM());
            }

            var customers = db.Customers.Where(c => c.PrimaryPhone == inputq).ToList();
            var dependants = customers.Select(c => c.Dependants.Split(',')).SelectMany(c => c).Where(c => !string.IsNullOrEmpty(c)).Select(c => long.Parse(c)).ToArray();
            var customerIDs = db.Customers.Where(c => dependants.Contains(c.ExternalReference)).Select(c => c.CustomerID).ToList();
            customerIDs.Add(customers.FirstOrDefault()?.CustomerID ?? new Guid());

            var cards = db.StudentCards.Include(s => s.StudentCardDetails).Include(c => c.Customer).Where(s => customerIDs.Contains(s.CustomerID)).OrderByDescending(s => s.Level).ToList();

            return View(new KioskCardVM { Ids = null, Cards = cards });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(long? id)
        {
            if (!id.HasValue)
            {
                return View(new KioskCardVM());
            }
            using (Pike13Api.DAL.Pike13ApiContext dbc = new Pike13Api.DAL.Pike13ApiContext())
            {
                var visits = dbc.Visits.Where(x => x.EventOccurrenceID == id && x.Status != "late_cancel").ToList();

                var references = visits.Select(x => x.PersonID).Distinct().ToList();
                var customerIDs = db.Customers.Where(c => references.Contains(c.ExternalReference)).Select(c => c.CustomerID).ToList();

                var cards = db.StudentCards.Include(s => s.StudentCardDetails).Include(c => c.Customer).Where(s => customerIDs.Contains(s.CustomerID)).OrderByDescending(s => s.Level).ToList();

                ViewBag.DisableSearch = true;

                return View(new KioskCardVM { Ids = null, Cards = cards });
            }            
        }

        public JsonResult GetCertifcate(long id)
        {
            StudentCard card = db.StudentCards.Find(id);
            string cert = "";

            if (card.IsGraduated)
                cert = CardRepo.GetCard(card.Level);

            return Json(new
            {
                card.StudentName,
                card.Instructors,
                GraduationDate = card.GraduationDate?.ToShortDateString() ?? "-",
                cert
            }, JsonRequestBehavior.AllowGet); ;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        //[OutputCache(CacheProfile = "CustomerImages")]
        public FileResult ProfileImage(string FirstName, string LastName)
        {
            var stream = ImageHelper.GenerateCircle(FirstName, LastName);
            stream.Position = 0;

            return new FileStreamResult(stream, "image/png");
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