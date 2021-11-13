using Daftari.AquaCards.DAL;
using Daftari.Handlers;
using Daftari.Pike13Api.DAL;
using Daftari.Pike13Api.Enum;
using Daftari.Pike13Api.Models;
using Daftari.Pike13Api.Services;
using Daftari.ViewModel;
using LukeApps.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Daftari.Controllers
{
    //[Authorize]
    public class Pike13AccessController : BaseController
    {
        public ActionResult Authorize()
        {
            return Redirect(new Pike13ApiAuth(User.Identity.Name, false).GetAuthorizationURL(Url.Action("Callback", "Pike13Access", null, Request.Url.Scheme)));
        }

        public async Task<ActionResult> Callback(string code = null)
        {
            var fhq = new Pike13ApiAuth(User.Identity.Name, Url.Action("Callback", "Pike13Access", null, Request.Url.Scheme), code);
            await fhq.AcquireTokenAsync(User.Identity.Name);
            return RedirectToAction("BusinessSelect", "Pike13Access");
        }

        public async Task<ActionResult> BusinessSelect()
        {
            using (Pike13ApiContext at = new Pike13ApiContext())
            {
                return View(await at.Tokens.Include(t => t.People).Where(t => t.Username == User.Identity.Name).OrderByDescending(t => t.TokenID).FirstOrDefaultAsync());
            }
        }

        [HttpPost]
        public JsonResult SaveActiveBusiness(long id)
        {
            TokenProvider.GetProvider().ActivateAccountPerson(User.Identity.Name, id);
            return Json(new
            {
                msg = "Success",
                redirecturl = Url.Action("/", "Dashboard")
            });
        }

        [Authorize(Roles = "Admin,Manager,Instructor")]
        public async Task<JsonResult> GetParentInfo(string externalRefs)
        {
            using (AquaCardsEntities db = new AquaCardsEntities())
            {
                var esternalRefArray = externalRefs.Split(',').Where(l => !string.IsNullOrEmpty(l)).Select(l => long.Parse(l)).ToArray();
                var customers = await db.Customers.Where(c => esternalRefArray.Contains(c.ExternalReference)).ToListAsync();
                if (customers.Any())
                {
                    List<GuardianPhoneJson> gplist = new List<GuardianPhoneJson>();

                    foreach (var customer in customers)
                    {
                        if (customer.PrimaryPhone != null)
                        {
                            gplist.Add(new GuardianPhoneJson { order = Array.IndexOf(esternalRefArray, customer.ExternalReference), extid = customer.ExternalReference.ToString(), name = customer.GuardianName, phone = customer.PrimaryPhone });
                        }
                        else
                        {
                            var providerIDs = customer.Providers.Split(',').Where(c => !string.IsNullOrEmpty(c)).Select(c => long.Parse(c)).ToArray();

                            var provider = await db.Customers.Where(c => providerIDs.Contains(c.ExternalReference) && c.PrimaryPhone != null).FirstOrDefaultAsync();

                            if (provider != null)
                                gplist.Add(new GuardianPhoneJson { order = Array.IndexOf(esternalRefArray, customer.ExternalReference), extid = customer.ExternalReference.ToString(), name = provider.FullName, phone = provider.PrimaryPhone });
                        }
                    }

                    return Json(gplist.OrderBy(g => g.order), JsonRequestBehavior.AllowGet);
                }

                return Json(new List<GuardianPhoneJson> {
                    new GuardianPhoneJson { extid = "-", name = "-", phone = "-" } }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<JsonResult> GetCardInfo(string externalRefs)
        {
            using (AquaCardsEntities db = new AquaCardsEntities())
            {
                var esternalRefArray = externalRefs.Split(',').Where(l => !string.IsNullOrEmpty(l)).Select(l => long.Parse(l)).ToArray();

                var cards = await db.StudentCards.Where(s => esternalRefArray.Contains(s.ExternalReferenceID)).Include(s => s.StudentCardDetails).GroupBy(c => c.ExternalReferenceID).ToListAsync();
                if (cards.Any())
                {
                    List<ReportJson> gplist = new List<ReportJson>();

                    foreach (var customer in cards)
                    {
                        var card = customer.OrderByDescending(c => c.Level).FirstOrDefault();
                        if (card != null)
                        {
                            gplist.Add(new ReportJson
                            {
                                order = Array.IndexOf(esternalRefArray, customer.Key),
                                extid = customer.Key.ToString(),
                                name = card.Customer.FullName,
                                level = card.Level.GetDisplay(),
                                lastOpenDateTime = card.LastOpenDate.ToUniversalTime().ToString("yyyy-MM-dd hh:mm:ss tt"),
                            });
                        }
                    }

                    return Json(gplist.OrderBy(g => g.order), JsonRequestBehavior.AllowGet);
                }

                return Json(new List<ReportJson> {
                    new ReportJson { extid = "-", name = "-", level = "-" } }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Admin,Manager,Instructor,RosterSchedule")]
        public ActionResult RosterSchedule()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            if (creds.Role == "limited_staff_member")
            {
                ViewBag.AccessCode = TokenProvider.GetProvider().GetStaffAccessCode(creds.Subdomain);
            }
            else
            {
                ViewBag.AccessCode = creds.AccessToken;
            }
            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.Role = creds.Role;
            ViewBag.PersonID = creds.ID;
            ViewBag.CanView = creds.Role != "limited_staff_member";
            ViewBag.UserFullName = creds.PersonName;
            return View(new RosterScheduleVM());
        }

        [Authorize(Roles = "Admin,Manager")]
        public ActionResult CardRosterReport()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            if (creds.Role == "limited_staff_member")
            {
                ViewBag.AccessCode = TokenProvider.GetProvider().GetStaffAccessCode(creds.Subdomain);
            }
            else
            {
                ViewBag.AccessCode = creds.AccessToken;
            }
            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.Role = creds.Role;
            ViewBag.PersonID = creds.ID;
            ViewBag.CanView = creds.Role != "limited_staff_member";
            ViewBag.UserFullName = creds.PersonName;
            return View(new RosterScheduleVM());
        }

        [Authorize(Roles = "Admin,Manager")]
        public ActionResult AquaCardReport()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);


            //temp
            //var new_events = new List<EventOccurrance>();
            //var event_occurances = await new Pike13ApiRepo(User.Identity.Name).GetEventOccurenceAsync(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1).AddMinutes(-1));
            //if (event_occurances.Any())
            //{
            //    foreach (var ev in event_occurances)
            //    {
            //        var event_o = new EventOccurrance
            //        {
            //            AttendanceComplete = ev.attendance_complete,
            //            CapacityRemaining = ev.capacity_remaining,
            //            EndAt = ev.end_at,
            //            EventID = ev.event_id,
            //            Full = ev.full,
            //            EventOccurrenceID = ev.id,
            //            LocationID = ev.location_id,
            //            Name = ev.name,
            //            StartAt = ev.start_at,
            //            State = ev.state,
            //            ServiceID = ev.service_id,
            //            Timezone = ev.timezone,
            //            people = string.Join(",", ev.people.Select(x => x.id).OrderBy(x => x)),
            //            StaffMembers = string.Join(",", ev.staff_members.Select(x => x.StaffID).OrderBy(x => x)),
            //            SubDomain = creds.Subdomain,
            //            Description = ev.description,
            //            VisitsCount = ev.visits_count
            //        };

            //        var visits = await new Pike13ApiRepo(User.Identity.Name).GetVisitsAsync(ev.id);
            //        if (visits.Any())
            //        {
            //            event_o.Visits = visits.Select(x => new Visit
            //            {
            //                CancelledAt = x.cancelled_at,
            //                CompletedAt = x.completed_at,
            //                EventOccurrenceID = x.event_occurrence_id,
            //                VisitID = x.id,
            //                NoshowAt = x.noshow_at,
            //                OnlyStaffCanCancel = x.only_staff_can_cancel,
            //                Paid = x.paid,
            //                PaidForBy = x.paid_for_by,
            //                PersonID = x.person_id,
            //                PunchID = x.punch_id,
            //                RegisteredAt = x.registered_at,
            //                State = x.state,
            //                Status = x.status,
            //                UpdatedAt = x.updated_at,
            //                CreatedAt = x.created_at
            //            }).ToList();
            //        }
            //        new_events.Add(event_o);
            //    }
            //}

            //using (Pike13ApiContext db = new Pike13ApiContext())
            //{
            //    db.EventOccurrances.AddRange(new_events);
            //    await db.SaveChangesAsync();
            //}

            if (creds.Role == "limited_staff_member")
            {
                ViewBag.AccessCode = TokenProvider.GetProvider().GetStaffAccessCode(creds.Subdomain);
            }
            else
            {
                ViewBag.AccessCode = creds.AccessToken;
            }
            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.Role = creds.Role;
            ViewBag.PersonID = creds.ID;
            ViewBag.CanView = creds.Role != "limited_staff_member";
            ViewBag.UserFullName = creds.PersonName;
            return View(new AquaCardReportVM());
        }

        [Authorize(Roles = "Admin,Manager,Instructor,AquaCards,RosterSchedule")]
        public ActionResult AquaCardRoster()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            if (creds.Role == "limited_staff_member")
            {
                ViewBag.AccessCode = TokenProvider.GetProvider().GetStaffAccessCode(creds.Subdomain);
            }
            else
            {
                ViewBag.AccessCode = creds.AccessToken;
            }
            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.Role = creds.Role;
            ViewBag.PersonID = creds.ID;
            ViewBag.CanView = creds.Role != "limited_staff_member";
            ViewBag.UserFullName = creds.PersonName;
            return View(new AquaCardReportVM());
        }

        [Authorize(Roles = "Admin,Manager,Instructor,AquaCards")]
        public ActionResult AquaCards()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            if (creds.Role == "limited_staff_member")
            {
                ViewBag.AccessCode = TokenProvider.GetProvider().GetStaffAccessCode(creds.Subdomain);
            }
            else
            {
                ViewBag.AccessCode = creds.AccessToken;
            }

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.Role = creds.Role;
            ViewBag.PersonID = creds.ID;
            ViewBag.CanView = creds.Role != "limited_staff_member";
            ViewBag.UserFullName = creds.PersonName;
            return View(new RosterScheduleVM());
        }

        [Authorize(Roles = "Admin,Manager,FrontDesk,Attendance")]
        public ActionResult Attendance()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            if (creds.Role == "limited_staff_member")
            {
                ViewBag.AccessCode = TokenProvider.GetProvider().GetStaffAccessCode(creds.Subdomain);
            }
            else
            {
                ViewBag.AccessCode = creds.AccessToken;
            }

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.Role = creds.Role;
            ViewBag.PersonID = creds.ID;
            ViewBag.CanView = creds.Role != "limited_staff_member";
            ViewBag.UserFullName = creds.PersonName;
            return View(new RosterScheduleVM());
        }

        [Authorize(Roles = "Admin,Manager,FrontDesk,Attendance")]
        public ActionResult NewAttendance()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            if (creds.Role == "limited_staff_member")
            {
                ViewBag.AccessCode = TokenProvider.GetProvider().GetStaffAccessCode(creds.Subdomain);
            }
            else
            {
                ViewBag.AccessCode = creds.AccessToken;
            }

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.Role = creds.Role;
            ViewBag.PersonID = creds.ID;
            ViewBag.CanView = creds.Role != "limited_staff_member";
            ViewBag.UserFullName = creds.PersonName;
            return View(new RosterScheduleVM());
        }

        //[Authorize(Roles = "Admin,Manager,Instructor,AquaCards,RosterSchedule")]
        public ActionResult StatusReport()
        {
            return View(new StatusReportVM());
        }
        public async Task<JsonResult> GetJSON(DateTime from, DateTime to, long? staff_member_ids)
        {
            var eventOccurrances = new List<EventOccurrance>();
            //var subdomain = TokenProvider.GetProvider().GetSubdomain(User.Identity.Name);

            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                //from = from.ToServerTime();
                //to = to.ToServerTime();
                eventOccurrances = await db.GetEventOccurrances().Include(v => v.Visits)
                                        .Where(x => x.StartAt < to && x.EndAt >= from)
                                        .Where(x => x.State != "deleted" && x.State != "disabled")
                                        //.Where(x => x.SubDomain == subdomain)
                                        .ToListAsync();
            }

            var all_customers = eventOccurrances.Where(x => !string.IsNullOrEmpty(x.people))
                            .SelectMany(x => x.people?.Split(',').Select(c => long.Parse(c)) ?? new long[0])
                            .Concat(eventOccurrances.Where(x => !string.IsNullOrEmpty(x.StaffMembers))
                            .SelectMany(x => x.StaffMembers?.Split(',').Select(c => long.Parse(c)) ?? new long[0]))
                            .Union(eventOccurrances.SelectMany(x => x.Visits).Where(x => x.PersonID.HasValue).Select(x => x.PersonID.Value)).Distinct().ToList();

            var customers = new List<AquaCards.Models.Customer>();
            var cards = new List<AquaCards.Models.StudentCard>();
            using (AquaCardsEntities db = new AquaCardsEntities(System.Web.HttpContext.Current.User.Identity.Name))
            {
                customers = await db.GetCustomers().Where(x => all_customers.Contains(x.ExternalReference)).ToListAsync();
                cards = (await db.StudentCards.Where(s => all_customers.Contains(s.ExternalReferenceID))
                                    //.Include(s => s.StudentCardDetails)
                                    .ToListAsync())
                                    .GroupBy(c => c.ExternalReferenceID, (Key, i) => i.OrderBy(s => s.Level).Last()).ToList();
            }

            var TotalRecords = eventOccurrances.Select(event_o => new
            {
                attendance_complete = event_o.AttendanceComplete,
                capacity_remaining = event_o.CapacityRemaining,
                start_at = event_o.StartAt,
                end_at = event_o.EndAt,
                event_id = event_o.EventID,
                full = event_o.Full,
                id = event_o.EventOccurrenceID,
                location_id = event_o.LocationID,
                people = (from id in (event_o.people?.Split(',') ?? new string[0])
                          join customer in customers on id equals customer.ExternalReference.ToString() into _customer
                          from customer in _customer.DefaultIfEmpty()
                          join card in cards on customer?.ExternalReference equals card.ExternalReferenceID into _card
                          from card in _card.DefaultIfEmpty()
                          where !string.IsNullOrEmpty(event_o.people)
                          select new
                          {
                              id = customer?.ExternalReference ?? 0,
                              name = customer != null ? $"{customer.FirstName} {customer.LastName}" : "Not Synced",
                              email = customer?.EmailAddress,
                              level = card?.Level.GetDisplay(),
                              lastOpenDateTime = card?.LastOpenDate.ToUniversalTime(),//.ToString("yyyy-MM-dd hh:mm:ss tt")
                          }).ToList(),
                staff_members = (from id in (event_o.StaffMembers?.Split(',') ?? new string[0])                                 
                                 join staff in customers on id equals staff.ExternalReference.ToString() into _staff
                                 from staff in _staff.DefaultIfEmpty()
                                 where !string.IsNullOrEmpty(event_o.StaffMembers)
                                 select new
                                 {
                                     id = staff?.ExternalReference ?? 0,
                                     name = staff != null ? $"{staff.FirstName} {staff.LastName}" : "Not Synced",
                                     email = staff?.EmailAddress
                                 }).ToList(),
                state = event_o.State,
                service_id = event_o.ServiceID,
                name = event_o.Name,
                timezone = event_o.Timezone,
                description = event_o.Description,
                visits_count = event_o.VisitsCount,
                visits = event_o.Visits.Select(x => new
                {
                    cancelled_at = x.CancelledAt,
                    completed_at = x.CompletedAt,
                    created_at = x.CreatedAt,
                    event_occurrence_id = x.EventOccurrenceID,
                    id = x.VisitID,
                    noshow_at = x.NoshowAt,
                    only_staff_can_cancel = x.OnlyStaffCanCancel,
                    paid = x.Paid,
                    paid_for_by = x.PaidForBy,
                    person_id = x.PersonID,
                    punch_id = x.PunchID,
                    registered_at = x.RegisteredAt,
                    state = x.State,
                    status = x.Status,
                    updated_at = x.UpdatedAt,
                    person = (from customer in customers
                            where customer.ExternalReference == x.PersonID
                            join card in cards on customer.ExternalReference equals card.ExternalReferenceID into _card
                            from card in _card.DefaultIfEmpty()
                            select new
                            {
                                id = customer.ExternalReference,
                                name = customer.FullName,
                                email = customer.EmailAddress,
                                level = card?.Level.GetDisplay(),
                                lastOpenDateTime = card?.LastOpenDate.ToUniversalTime(),//.ToString("yyyy-MM-dd hh:mm:ss tt"),
                                photoMD = customer.PhotoMD
                            }).FirstOrDefault() ?? new { id = 0L, name = "Not Synced", email = "N/A", level = "", lastOpenDateTime = (DateTime?)null, photoMD = "" }
                }).Where(x => x.status != VisitStatus.Late_Cancel.GetDisplay()).ToList()
            })
            .Where(ev => ev.visits.Any())    
            .Where(ev => !staff_member_ids.HasValue || ev.staff_members.Any(x => x.id == staff_member_ids)).ToList();

            //TotalRecords = TotalRecords.Where(x => x.visits.Any(v => v.person.lastOpenDateTime != null)).ToList();

            return Json(new
            {
                event_occurrences = TotalRecords
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetStatusJSON(DateTime from, DateTime to, long? staff_member_ids)
        {
            var eventOccurrances = new List<EventOccurrance>();

            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                eventOccurrances = await db.GetEventOccurrances().Include(v => v.Visits)
                                        .Where(x => x.StartAt < to && x.EndAt >= from)
                                        .Where(x => x.State != "deleted" && x.State != "disabled")
                                        //.Where(x => x.SubDomain == subdomain)
                                        .ToListAsync();
            }

            var all_customers = eventOccurrances.Where(x => !string.IsNullOrEmpty(x.people))
                            .SelectMany(x => x.people?.Split(',').Select(c => long.Parse(c)) ?? new long[0])
                            .Concat(eventOccurrances.Where(x => !string.IsNullOrEmpty(x.StaffMembers))
                            .SelectMany(x => x.StaffMembers?.Split(',').Select(c => long.Parse(c)) ?? new long[0]))
                            .Union(eventOccurrances.SelectMany(x => x.Visits).Where(x => x.PersonID.HasValue).Select(x => x.PersonID.Value)).Distinct().ToList();

            var customers = new List<AquaCards.Models.Customer>();
            var cards = new List<AquaCards.Models.StudentCard>();
            using (AquaCardsEntities db = new AquaCardsEntities(System.Web.HttpContext.Current.User.Identity.Name))
            {
                customers = await db.GetCustomers().Where(x => all_customers.Contains(x.ExternalReference)).ToListAsync();
                cards = (await db.StudentCards.Where(s => all_customers.Contains(s.ExternalReferenceID))
                                    //.Include(s => s.StudentCardDetails)
                                    .ToListAsync())
                                    .GroupBy(c => c.ExternalReferenceID, (Key, i) => i.OrderBy(s => s.Level).Last()).ToList();
            }

            var TotalRecords = eventOccurrances.Select(event_o => new
            {
                attendance_complete = event_o.AttendanceComplete,
                capacity_remaining = event_o.CapacityRemaining,
                start_at = event_o.StartAt,
                end_at = event_o.EndAt,
                event_id = event_o.EventID,
                full = event_o.Full,
                id = event_o.EventOccurrenceID,
                location_id = event_o.LocationID,
                people = (from id in (event_o.people?.Split(',') ?? new string[0])
                          join customer in customers on id equals customer.ExternalReference.ToString() into _customer
                          from customer in _customer.DefaultIfEmpty()
                          join card in cards on customer?.ExternalReference equals card.ExternalReferenceID into _card
                          from card in _card.DefaultIfEmpty()
                          where !string.IsNullOrEmpty(event_o.people)
                          select new
                          {
                              id = customer?.ExternalReference ?? 0,
                              name = customer != null ? $"{customer.FirstName} {customer.LastName}" : "Not Synced",
                              email = customer?.EmailAddress,
                              level = card?.Level.GetDisplay(),
                              lastOpenDateTime = card?.LastOpenDate.ToUniversalTime(),//.ToString("yyyy-MM-dd hh:mm:ss tt")
                          }).ToList(),
                staff_members = (from id in (event_o.StaffMembers?.Split(',') ?? new string[0])
                                 join staff in customers on id equals staff.ExternalReference.ToString() into _staff
                                 from staff in _staff.DefaultIfEmpty()
                                 where !string.IsNullOrEmpty(event_o.StaffMembers)
                                 select new
                                 {
                                     id = staff?.ExternalReference ?? 0,
                                     name = staff != null ? $"{staff.FirstName} {staff.LastName}" : "Not Synced",
                                     email = staff?.EmailAddress
                                 }).ToList(),
                state = event_o.State,
                service_id = event_o.ServiceID,
                name = event_o.Name,
                timezone = event_o.Timezone,
                description = event_o.Description,
                visits_count = event_o.VisitsCount,
                visits = event_o.Visits.Select(x => new
                {
                    cancelled_at = x.CancelledAt,
                    completed_at = x.CompletedAt,
                    created_at = x.CreatedAt,
                    event_occurrence_id = x.EventOccurrenceID,
                    id = x.VisitID,
                    noshow_at = x.NoshowAt,
                    only_staff_can_cancel = x.OnlyStaffCanCancel,
                    paid = x.Paid,
                    paid_for_by = x.PaidForBy,
                    person_id = x.PersonID,
                    punch_id = x.PunchID,
                    registered_at = x.RegisteredAt,
                    state = x.State,
                    status = x.Status,
                    updated_at = x.UpdatedAt,
                    person = (from customer in customers
                              where customer.ExternalReference == x.PersonID
                              join card in cards on customer.ExternalReference equals card.ExternalReferenceID into _card
                              from card in _card.DefaultIfEmpty()
                              select new
                              {
                                  id = customer.ExternalReference,
                                  name = customer.FullName,
                                  email = customer.EmailAddress,
                                  level = card?.Level.GetDisplay(),
                                  lastOpenDateTime = card?.LastOpenDate.ToUniversalTime(),//.ToString("yyyy-MM-dd hh:mm:ss tt"),
                                  photoMD = customer.PhotoMD
                              }).FirstOrDefault() ?? new { id = 0L, name = "Not Synced", email = "N/A", level = "", lastOpenDateTime = (DateTime?)null, photoMD = "" }
                }).Where(x => x.status != VisitStatus.Late_Cancel.GetDisplay()).ToList()
            })
            .Where(ev => ev.visits.Any())
            .Where(ev => !staff_member_ids.HasValue || ev.staff_members.Any(x => x.id == staff_member_ids)).ToList();

            //TotalRecords = TotalRecords.Where(x => x.visits.Any(v => v.person.lastOpenDateTime != null)).ToList();

            return Json(new
            {
                event_occurrences = TotalRecords
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetReporting(DateTime from, DateTime to)
        {
            var visits = new List<Visit>();
            Chemicals.Models.ChemicalSettings cs;

            if (to.Subtract(from).TotalDays < 6)
            {
                from = to.AddDays(-6).Date;
            }
            //var from = DateTime.Today.AddDays(-6);
            //var to = from.AddDays(7).AddSeconds(-1);
            var sd = TokenProvider.GetProvider().GetSubdomain(User.Identity.Name);
            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                visits = await db.Visits
                                    .Include(x => x.EventOccurrance)
                                    .Where(x => x.EventOccurrance.StartAt < to && x.EventOccurrance.EndAt >= from)
                                    .Where(x => x.EventOccurrance.SubDomain == sd)
                                    .Where(x => x.EventOccurrance.State != "deleted" && x.EventOccurrance.State != "disabled")
                                    .ToListAsync();
            }

            using (Chemicals.DAL.ChemicalsEntities db = new Chemicals.DAL.ChemicalsEntities())
            {
                cs = await db.ChemicalSettings.Where(q => q.SubDomain == sd).FirstOrDefaultAsync();
            }            

            var week_dates = from.To(to).ToList();
            var grouped_visits = (from date in week_dates
                                  join v in visits on date equals v.EventOccurrance.StartAt.Value.Date into _v
                                  from v in _v.DefaultIfEmpty()
                                  group new { date, v } by date into i
                                  select new
                                  {
                                      Date = i.Key,
                                      Data = i.Where(x => x.v != null).Select(x => x.v).ToList()
                                  }).ToList();

            var model = new StatusReportVM
            {
                Title = to.Date == DateTime.Today ? $"Today's View ({to.Date.ToString("dd MMM")})" : $"View ({to.Date.ToString("dd MMM")})",
                Labels = week_dates.Select(x => x.ToString("dd MMM")).ToList(),
                Total_Stundents = grouped_visits.Select(visit => visit.Data.Select(x => x.PersonID).Distinct().Count()).ToList(),
                Total_Capacity = grouped_visits.Select(x => cs?.PoolDailyCapacity ?? 0).ToList(),
                //Total_Capacity = grouped_visits.Select(visit => visit.Data.Select(x => x.EventOccurrenceID).Distinct().Count() * 4).ToList(),
                Total_Cancelled_Stundents = grouped_visits.Select(visit => visit.Data.Where(x => x.Status == "late_cancel").Select(x => x.PersonID).Distinct().Count()).ToList(),
                Total_No_Show_Stundents = grouped_visits.Select(visit => visit.Data.Where(x => x.Status == "noshow").Select(x => x.PersonID).Distinct().Count()).ToList(),
                Total_Classes = grouped_visits.Select(visit => visit.Data.Select(x => x.EventOccurrenceID).Distinct().Count()).ToList(),
                Unpaid_Students = grouped_visits.Select(visit => visit.Data.Where(x => x.Paid == false).Select(x => x.PersonID).Distinct().Count()).ToList(),
                Paid_By_Makeup = grouped_visits.Select(visit => visit.Data.Where(x => x.Paid == true && (x.PaidForBy?.Contains("") ?? false)).Select(x => x.PersonID).Distinct().Count()).ToList()
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> AutoSyncEventOccurrences(bool all = false)
        {
            using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(new Exception("Auto Sync Started"), this.HttpContext))
            {
                bh.Log_Error();
            }
            var from = DateTime.Today;
            var to = from.AddDays(1).AddSeconds(-1);
            var tasks = new List<Task<JsonResult>>();
            var businessIDs = new List<long?>();

            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                businessIDs = await db.TopicSubsriptions.Select(x => x.BusinessID).Distinct().ToListAsync();
            }

            businessIDs.ForEach(id => tasks.Add(SyncEventOccurrences(from, to, 9, id, all)));

            await Task.WhenAll(tasks);
            //return await SyncEventOccurrences(from, to, 9);
            using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(new Exception("Auto Sync Ended"), this.HttpContext))
            {
                bh.Log_Error();
            }
            return Json(new { msg = "Success"}, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SyncEventOccurrences(DateTime from, DateTime to, int type, long? business_id = null, bool all = false)
        {
            string syncDate = null;
            string now = DateTime.Now.ToString("s");
            string dteType = null;
            string syncsettingKey = null;
            string flagsettingKey;
            string sd = null;

            if (!business_id.HasValue)
            {
                sd = TokenProvider.GetProvider().GetSubdomain(User.Identity.Name);
                business_id = TokenProvider.GetProvider().GetBusinessId(sd);
            }
            else
            {
                sd = TokenProvider.GetProvider().GetSubdomain(business_id.Value);
            }
            using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(new Exception($"Initiated b-id => {business_id}, sd => {sd}"), this.HttpContext))
            {
                bh.Log_Error();
            }

            switch (type)
            {
                case 0:
                    dteType = "updated_since";
                    syncsettingKey = sd + "EvtSyncDte";
                    flagsettingKey = sd + "EvtSyncFlag";
                    break;

                case 9:
                    syncsettingKey = sd + "EvtSyncDte";
                    flagsettingKey = sd + "EvtSyncFlag";
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
                        using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(new Exception($"Already running b-id => {business_id}, sd => {sd}"), this.HttpContext))
                        {
                            bh.Log_Error();
                        }
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
                List<Pike13Event> data;
                if (dteType != null)
                {
                    data = await new Pike13ApiRepo(business_id.Value).GetEventOccurenceAsync(from, to, null, dteType, syncDate);
                }
                else
                {
                    data = await new Pike13ApiRepo(business_id.Value).GetEventOccurenceAsync(from, to);
                }

                await ProcessAndSync(data, from, to, sd, all);
            }
            catch (Exception ex)
            {
                using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(ex, this.HttpContext))
                {
                    bh.Log_Error();
                }

                RevertSettings(flagsettingKey, syncsettingKey, syncDate);

                return Json(new { msg = ex.Message, type = 2 }, JsonRequestBehavior.AllowGet);
            }

            RevertSettings(flagsettingKey, syncsettingKey);

            return Json(new { msg = "Success", type = 2 }, JsonRequestBehavior.AllowGet);
        }

        private static void RevertSettings(string flagsettingKey, string syncsettingKey, string syncdate = null)
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

        private async Task<int> ProcessAndSync(List<Pike13Event> data, DateTime from, DateTime to, string subdomain, bool All)
        {
            var business_id = TokenProvider.GetProvider().GetBusinessId(subdomain);

            var count = 0;
            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                var new_event_occurrences = new List<EventOccurrance>();
                var modified_event_occurrences = new List<EventOccurrance>();
                foreach (var ev in data)
                {
                    try
                    {
                        var event_occurrence = await db.EventOccurrances.Include(v => v.Visits)
                                            .Where(e => e.SubDomain == subdomain && e.EventOccurrenceID == ev.id)
                                            .FirstOrDefaultAsync();
                        // If it does not in our db yet
                        if (event_occurrence == null)
                        {
                            var visits = await new Pike13ApiRepo(business_id).GetVisitsAsync(ev.id);
                            // ensures that the record was not added while we waited
                            event_occurrence = await db.EventOccurrances.Include(v => v.Visits)
                                            .Where(x => x.EventOccurrenceID == ev.id)
                                            .FirstOrDefaultAsync();

                            if (event_occurrence == null)
                            {
                                event_occurrence = new EventOccurrance
                                {
                                    AttendanceComplete = ev.attendance_complete,
                                    CapacityRemaining = ev.capacity_remaining,
                                    EndAt = ev.end_at,
                                    EventID = ev.event_id,
                                    Full = ev.full,
                                    EventOccurrenceID = ev.id,
                                    LocationID = ev.location_id,
                                    Name = ev.name,
                                    StartAt = ev.start_at,
                                    State = ev.state,
                                    ServiceID = ev.service_id,
                                    Timezone = ev.timezone,
                                    people = string.Join(",", ev.people.Select(x => x.id).OrderBy(x => x)),
                                    StaffMembers = string.Join(",", ev.staff_members.Select(x => x.StaffID).OrderBy(x => x)),
                                    SubDomain = subdomain,
                                    Description = ev.description,
                                    VisitsCount = ev.visits_count,
                                    Visits = visits.Select(x => new Visit
                                    {
                                        CancelledAt = x.cancelled_at,
                                        CompletedAt = x.completed_at,
                                        EventOccurrenceID = x.event_occurrence_id,
                                        VisitID = x.id,
                                        NoshowAt = x.noshow_at,
                                        OnlyStaffCanCancel = x.only_staff_can_cancel,
                                        Paid = x.paid,
                                        PaidForBy = x.paid_for_by,
                                        PersonID = x.person_id,
                                        PunchID = x.punch_id,
                                        RegisteredAt = x.registered_at,
                                        State = x.state,
                                        Status = x.status,
                                        UpdatedAt = x.updated_at,
                                        CreatedAt = x.created_at
                                    }).ToList()
                                };

                                db.EventOccurrances.Add(event_occurrence);
                                await db.SaveChangesAsync();
                            }

                            new_event_occurrences.Add(event_occurrence);
                        }
                        else if (!event_occurrence.EqualsRaw(ev))
                        {
                            event_occurrence.AttendanceComplete = ev.attendance_complete;
                            event_occurrence.CapacityRemaining = ev.capacity_remaining;
                            event_occurrence.EndAt = ev.end_at;
                            event_occurrence.EventID = ev.event_id;
                            event_occurrence.Full = ev.full;
                            event_occurrence.LocationID = ev.location_id;
                            event_occurrence.Name = ev.name;
                            event_occurrence.StartAt = ev.start_at;
                            event_occurrence.State = ev.state;
                            event_occurrence.ServiceID = ev.service_id;
                            event_occurrence.Timezone = ev.timezone;
                            event_occurrence.people = string.Join(",", ev.people.Select(x => x.id).OrderBy(x => x));
                            event_occurrence.StaffMembers = string.Join(",", ev.staff_members.Select(x => x.StaffID).OrderBy(x => x));
                            event_occurrence.Description = ev.description;
                            event_occurrence.VisitsCount = ev.visits_count;
                            event_occurrence.AuditDetail.LastModifiedDate = DateTime.Now;
                            event_occurrence.AuditDetail.LastModifiedEntryUserID = User.Identity.Name;

                            db.Entry(event_occurrence).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }

                        modified_event_occurrences.Add(event_occurrence);
                    } 
                    catch (Exception ex)
                    {
                        using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(ex, this.HttpContext))
                        {
                            bh.Log_Error();
                        }
                    }
                    
                    count++;
                }

                await db.SaveChangesAsync();

                if (All) 
                {
                    // For all existing events
                    foreach (var event_occurrence in modified_event_occurrences)
                    {
                        try
                        {
                            // Get all the ID's of people and compare to visit IDs
                            var people = string.Join(",", event_occurrence.Visits.Select(x => x.PersonID).OrderBy(x => x));
                            if (event_occurrence.people != people)
                            {
                                // If they don't match, get all the visits and compare with existing
                                var visits = await new Pike13ApiRepo(business_id).GetVisitsAsync(event_occurrence.EventOccurrenceID);
                                var can_save = false;
                                foreach (var pike_visit in visits)
                                {
                                    var visit = event_occurrence.Visits.FirstOrDefault(x => x.VisitID == pike_visit.id);
                                    if (visit == null)
                                    {
                                        event_occurrence.Visits.Add(new Visit
                                        {
                                            CancelledAt = pike_visit.cancelled_at,
                                            CompletedAt = pike_visit.completed_at,
                                            EventOccurrenceID = pike_visit.event_occurrence_id,
                                            VisitID = pike_visit.id,
                                            NoshowAt = pike_visit.noshow_at,
                                            OnlyStaffCanCancel = pike_visit.only_staff_can_cancel,
                                            Paid = pike_visit.paid,
                                            PaidForBy = pike_visit.paid_for_by,
                                            PersonID = pike_visit.person_id,
                                            PunchID = pike_visit.punch_id,
                                            RegisteredAt = pike_visit.registered_at,
                                            State = pike_visit.state,
                                            Status = pike_visit.status,
                                            UpdatedAt = pike_visit.updated_at,
                                            CreatedAt = pike_visit.created_at,
                                            IsDeleted = false
                                        });
                                        can_save = true;
                                    }
                                    else
                                    {
                                        if (visit.LastModified < pike_visit.LastModified)
                                        {
                                            visit.CancelledAt = pike_visit.cancelled_at;
                                            visit.CompletedAt = pike_visit.completed_at;
                                            visit.EventOccurrenceID = pike_visit.event_occurrence_id;
                                            visit.VisitID = pike_visit.id;
                                            visit.NoshowAt = pike_visit.noshow_at;
                                            visit.OnlyStaffCanCancel = pike_visit.only_staff_can_cancel;
                                            visit.Paid = pike_visit.paid;
                                            visit.PaidForBy = pike_visit.paid_for_by;
                                            visit.PersonID = pike_visit.person_id;
                                            visit.PunchID = pike_visit.punch_id;
                                            visit.RegisteredAt = pike_visit.registered_at;
                                            visit.State = pike_visit.state;
                                            visit.Status = pike_visit.status;
                                            visit.UpdatedAt = pike_visit.updated_at;
                                            visit.CreatedAt = pike_visit.created_at;
                                            visit.AuditDetail.LastModifiedDate = DateTime.Now;
                                            db.Entry(visit).State = EntityState.Modified;
                                            //can_save = true;          
                                            //db.Visits.Remove(visit);                                        
                                        }
                                    }
                                }
                                // To delete
                                var to_delete = event_occurrence.Visits.Where(vx => !visits.Any(vn => vn.id == vx.VisitID)).ToList();
                                to_delete.ForEach(obj =>
                                {
                                    event_occurrence.Visits.Remove(obj);
                                    can_save = true;
                                });

                                if (can_save)
                                {
                                    db.Entry(event_occurrence).State = EntityState.Modified;
                                }
                            }  
                            if (db.ChangeTracker.HasChanges())
                            {
                                await db.SaveChangesAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(ex, this.HttpContext))
                            {
                                bh.Log_Error();
                            }
                        }

                        count++;
                    }

                    await db.SaveChangesAsync();
                    //remove additionals
                    var eventOccurrances = await db.EventOccurrances
                                        .Where(x => x.StartAt < to && x.EndAt >= from)
                                        .Where(x => x.State != "deleted" && x.State != "disabled")
                                        .Where(x => x.SubDomain == subdomain)
                                        .ToListAsync();

                    eventOccurrances = eventOccurrances.Where(exists_ev => !modified_event_occurrences.Any(new_ev => new_ev.EventOccurrenceID == exists_ev.EventOccurrenceID)).ToList();
                    foreach (var event_occurrence in eventOccurrances)
                    {
                        event_occurrence.State = "disabled";
                        db.Entry(event_occurrence).State = EntityState.Modified;
                    }                    
                    await db.SaveChangesAsync();
                }

                return await db.SaveChangesAsync();
            }
        }

    }
}