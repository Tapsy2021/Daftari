using Daftari.AquaCards.DAL;
using Daftari.AquaCards.Enum;
using Daftari.AquaCards.Models;
using Daftari.Handlers;
using Daftari.Pike13Api.Services;
using Daftari.ViewModel;
using LukeApps.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Daftari.Controllers
{
    public class GraduatedCardsController : Controller
    {
        private AquaCardsEntities db = new AquaCardsEntities(System.Web.HttpContext.Current.User.Identity.Name);

        // GET: Admin/StudentCards
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<JsonResult> GetJSON(DateTime? from, DateTime? to)
        {
            var cards = await db.GetCards().Where(c => c.IsGraduated).ToListAsync();

            if (from != null && to != null)
            {
                cards = cards.Where(x => x.GraduationDate >= from && x.GraduationDate <= to).ToList();
            }

            var detailCollection = cards.Select(s => new
            {
                s.StudentCardID,
                Level = s.Level.GetDisplay(),
                s.CardNumber,
                s.StudentName,
                s.Age,
                StartDate = s.StartDate.ToString("dd/MM/yyyy"),
                s.Plan,
                s.Instructors,
                s.Status,
                s.IsCompleted,
                s.IsGraduated,
                s.IsExpired,
                GraduationDate = s.GraduationDate?.ToString("dd/MM/yyyy"),
                s.GraduatedBy,
                CreatedDate = s.AuditDetail.CreatedDate.ToString("yyyy-MM-dd HH:mm"),
                CreatedEntryUser = s.AuditDetail.CreatedEntryUserID,
                LastModifiedDate = s.AuditDetail.LastModifiedDate?.ToString("yyyy-MM-dd HH:mm") ?? s.AuditDetail.CreatedDate.ToString("yyyy-MM-dd HH:mm"),
                LastModifiedEntryUser = s.AuditDetail.LastModifiedEntryUserID ?? s.AuditDetail.CreatedEntryUserID,
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

        // GET: Admin/StudentCards/Details/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentCard studentCard = await db.StudentCards.FindAsync(id);
            if (studentCard == null)
            {
                return HttpNotFound();
            }
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            if (studentCard.Customer.SubDomain != creds.Subdomain)
            {
                return HttpNotFound();
            }
            return View(studentCard);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            ViewBag.CustomerID = new SelectList(db.GetCustomers().Where(c => c.SubDomain == creds.Subdomain), "CustomerID", "FullName");
            return View(new ProgressCardRequestVM());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create(ProgressCardRequestVM request)
        {
            if (ModelState.IsValid)
            {
                var cid = new Guid(request.CustomerID);
                var customer = db.Customers.Find(cid);
                var skills = db.Skills.Where(s => s.SkillLevel == request.Level).ToList();

                var card = new StudentCard()
                {
                    CustomerID = cid,
                    ExternalReferenceID = customer.ExternalReference,
                    StudentName = $"{request.FirstName} {request.LastName}",
                    LastName = request.LastName,
                    BirthDate = request.Birthday,
                    Level = request.Level,
                    StartDate = DateTime.Now,
                    Initial = request.LastName?.Substring(0, 1),
                    Instructors = request.Instructors,
                    Plan = request.Plan,
                    IsManual = true,
                    StudentCardDetails = skills.Select(s => new StudentCardDetail()
                    {
                        SkillID = s.SkillID,
                        Skill = s
                    }).ToList()
                };

                db.StudentCards.Add(card);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = card.StudentCardID });
            }
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            ViewBag.CustomerID = new SelectList(db.GetCustomers().Where(c => c.SubDomain == creds.Subdomain), "CustomerID", "FullName");

            return View(request);
        }

        [Authorize(Roles = "Admin,Manager")]
        public JsonResult GetCustomerInfo(string id)
        {
            var customer = db.GetCustomers().First(c => c.CustomerID == new Guid(id));
            return Json(new
            {
                id = customer.CustomerID,
                fname = customer.FirstName,
                lname = customer.LastName,
                birthday = customer.Birthday?.ToString("yyyy-MM-dd"),
                age = customer.Birthday != null ? ((int)((DateTime.Now - customer.Birthday.Value).TotalDays / 365)).ToString("00") : null
            }, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/StudentCards/Create
        [Authorize(Roles = "Admin,Manager,Instructor")]
        public async Task<ActionResult> Open(long? sid = null, string instructors = null, SkillLevel? lvl = null, long? eid = null, string plan = null, string startdate = null)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            var vmd = await processCardValidity(sid, lvl);

            if (vmd.Status != CardProcessStatus.Success && vmd.Status != CardProcessStatus.NoCardFound)
            {
                throw new Exception(vmd.StatusMsg);
            }

            var apiRepo = new Pike13ApiRepo(User.Identity.Name);
            var notes = await apiRepo.GetPersonNotesAsync(vmd.Student.ExternalReference);

            if (vmd.Card == null)
            {
                var skills = db.Skills.Where(s => s.SkillLevel == vmd.Level).ToList();

                vmd.Card = new StudentCard()
                {
                    CustomerID = vmd.Student.CustomerID,
                    ExternalReferenceID = vmd.Student.ExternalReference,
                    StudentName = $"{vmd.Student.FirstName} {vmd.Student.LastName}",
                    LastName = vmd.Student.LastName,
                    BirthDate = vmd.Student.Birthday,
                    Level = (SkillLevel)vmd.Level,
                    StartDate = DateTime.Now,
                    Initial = vmd.Student.LastName.Substring(0, 1),
                    Instructors = instructors,
                    StudentCardDetails = skills.Select(s => new StudentCardDetail()
                    {
                        SkillID = s.SkillID,
                        Skill = s
                    }).ToList()
                };

                if (plan != null)
                {
                    vmd.Card.Plan = plan;
                }

                db.StudentCards.Add(vmd.Card);
                db.SaveChanges();
            }
            else
            {
                var instructorsInputArray = instructors?.Split(',');
                var instructorsList = vmd.Card.Instructors?.Split(',').ToList();
                var isChanged = false;

                if (instructorsInputArray != null)
                {
                    foreach (var item in instructorsInputArray)
                    {
                        if (instructorsList != null)
                        {
                            if (!instructorsList.Any(i => i == item))
                            {
                                isChanged = true;
                                instructorsList.Add(item);
                            }
                        }
                    }
                }

                if (plan != null)
                {
                    vmd.Card.Plan = plan;
                }

                if (isChanged)
                {
                    vmd.Card.Instructors = string.Join(",", instructorsList);
                    db.Entry(vmd.Card).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            var staffMembers = (await apiRepo.GetStaffMembersAsync()).Select(s => new { id = s.id.ToString(), name = s.first_name + " " + s.last_name }).ToDictionary(s => s.id, s => s.name);

            vmd.Card.Comments = notes?.Select(n => new Tuple<string, string>(getName(staffMembers, n.created_by_id), n.note)).ToList();
            ViewBag.EventID = eid;
            ViewBag.StartDate = startdate;
            ViewBag.PhotoUrl = vmd.Student.PhotoLG;
            ViewBag.AccessCode = creds.AccessToken;
            ViewBag.Subdomain = creds.Subdomain;

            if (creds.Role == "limited_staff_member")
            {
                ViewBag.BorrowedAccessCode = TokenProvider.GetProvider().GetStaffAccessCode(creds.Subdomain);
            }
            else
            {
                ViewBag.BorrowedAccessCode = creds.AccessToken;
            }

            return View(vmd.Card);
        }

        private class CardValidityMetaData
        {
            public CardProcessStatus Status { get; set; }
            public string StatusMsg => Status.GetDisplay();
            public Customer Student { get; set; }
            public SkillLevel? Level { get; set; }

            public StudentCard Card { get; set; }
        }

        public enum CardProcessStatus
        {
            Success,
            NoCardFound,

            [Display(Name = "Student is not found in the system. Please synchronize.")]
            NoStudentFound,

            [Display(Name = "Student Level Info Or Birthday is missing. Please contact your manager.")]
            NoStudentInfoFound,

            [Display(Name = "Student did not complete previous card. Please contact your manager.")]
            IncompleteCardFound,

            [Display(Name = "Student did not attend classes for more than three months. Please contact your manager.")]
            CardArchived,
        }

        private async Task<CardValidityMetaData> processCardValidity(long? sid, SkillLevel? lvl)
        {
            Customer student = db.GetCustomers().FirstOrDefault(c => c.ExternalReference == sid);

            if (student == null)
                return new CardValidityMetaData { Status = CardProcessStatus.NoStudentFound };

            if ((lvl == null && (student?.Birthday ?? DateTime.MinValue) == DateTime.MinValue))
                return new CardValidityMetaData { Status = CardProcessStatus.NoStudentInfoFound, Student = student };

            StudentCard card = await db.GetCards().OrderByDescending(o => o.Level).FirstOrDefaultAsync(s => s.ExternalReferenceID == sid);

            SkillLevel level;

            if (lvl != null)
            {
                level = (SkillLevel)lvl;
            }
            else
            {
                if (card == null)
                {
                    var age = (DateTime.Now - ((student?.Birthday ?? DateTime.Now))).TotalDays;
                    level = lvl ?? (age > 122 && age <= 365 ? SkillLevel.One : age <= 910 ? SkillLevel.Two : SkillLevel.Three);
                }
                else
                {
                    if (card.IsGraduated && card.Level != SkillLevel.Eight)
                    {
                        level = card.Level + 1;
                    }
                    else
                    {
                        level = card.Level;
                    }
                }
            }

            if (card == null)
            {
                return new CardValidityMetaData { Status = CardProcessStatus.NoCardFound, Level = level, Student = student, Card = card };
            }
            else
            {
                if (card.IsExpired && !card.IsGraduated)
                {
                    return new CardValidityMetaData { Status = CardProcessStatus.CardArchived, Level = level, Student = student, Card = card };
                }

                if (card.Level != level && !card.IsCompleted)
                {
                    return new CardValidityMetaData { Status = CardProcessStatus.IncompleteCardFound, Level = level, Student = student, Card = card };
                }

                if (level > card.Level && card.IsGraduated && card.Level != SkillLevel.Eight)
                {
                    return new CardValidityMetaData
                    {
                        Status = CardProcessStatus.Success,
                        Level = (SkillLevel)(((int)card.Level) + 1),
                        Student = student,
                        Card = null
                    };
                }
            }

            return new CardValidityMetaData { Status = CardProcessStatus.Success, Level = level, Student = student, Card = card };
        }

        [Authorize(Roles = "Admin,Manager,Instructor")]
        public async Task<JsonResult> GetCardValidity(long? sid, SkillLevel? lvl)
        {
            var vmd = await processCardValidity(sid, lvl);
            return Json(new { Status = (int)vmd.Status, vmd.StatusMsg }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Manager")]
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

        // GET: Admin/StudentCards/DeleteAllArchive/5
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult DeleteAllArchive()
        {
            return View();
        }

        // POST: Admin/StudentCards/DeleteAllArchive/5
        [HttpPost, ActionName("DeleteAllArchive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> DeleteAllArchiveConfirmed()
        {
            List<StudentCard> cards = db.GetCards().Where(c => !c.IsGraduated).AsEnumerable().Where(c => c.IsExpired).ToList();
            db.StudentCards.RemoveRange(cards);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> Edit(long? id)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            var card = await db.GetCards().Include(c => c.Customer).FirstOrDefaultAsync(s => s.StudentCardID == id);
            var student = card.Customer;
            var apiRepo = new Pike13ApiRepo(User.Identity.Name);
            var staffMembers = (await apiRepo.GetStaffMembersAsync()).Select(s => new { id = s.id.ToString(), name = s.first_name + " " + s.last_name }).ToDictionary(s => s.id,s=>s.name);
            var notes = await apiRepo.GetPersonNotesAsync(student.ExternalReference);

            card.Comments = notes?.Select(n => new Tuple<string, string>(getName(staffMembers, n.created_by_id), n.note)).ToList();
            ViewBag.StartDate = card.StartDate;
            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;

            if (creds.Role == "limited_staff_member")
            {
                ViewBag.BorrowedAccessCode = TokenProvider.GetProvider().GetStaffAccessCode(creds.Subdomain);
            }
            else
            {
                ViewBag.BorrowedAccessCode = creds.AccessToken;
            }

            return View("Open", card);
        }

        private string getName(Dictionary<string, string> staff, string id)
        {
            if (staff.TryGetValue(id, out string name))
                return name;
            else
                return "-";
        }


        public class ProgressVm
        {
            public long id { get; set; }

            public string instructors { get; set; }

            public bool isToGraduate { get; set; }

            public long[] data { get; set; }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Instructor")]
        public async Task<JsonResult> SaveProcess([ModelBinder(typeof(JsonNetModelBinder))] ProgressVm data)
        {
            StudentCard card = await db.StudentCards.Include(s => s.StudentCardDetails).FirstOrDefaultAsync(s => s.StudentCardID == data.id);

            card.StudentCardDetails.Where(d => data.data.Contains(d.StudentCardDetailID)).Select(d => { d.IsComplete = true; d.CompleteDate = DateTime.Now; d.CompletedBy = User.Identity.Name; return d; }).ToList();

            if (data.isToGraduate)
            {
                if (card.StudentCardDetails.All(c => c.IsComplete))
                {
                    card.IsGraduated = true;
                    card.GraduatedBy = User.Identity.Name;
                    card.GraduationDate = DateTime.Now;
                }
                else
                {
                    return Json(new { text = "Progress is not fully filled out. Cannot publish.", isPublished = false });
                }
            }

            db.Entry(card).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { text = "Success", isPublished = true });
        }

        // GET: Admin/StudentCards/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentCard studentCard = await db.StudentCards.FindAsync(id);
            if (studentCard == null)
            {
                return HttpNotFound();
            }
            return View(studentCard);
        }

        // POST: Admin/StudentCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            StudentCard studentCard = await db.StudentCards.FindAsync(id);
            db.StudentCards.Remove(studentCard);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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