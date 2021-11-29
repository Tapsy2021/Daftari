using Daftari.AquaCards.DAL;
using Daftari.AquaCards.Models;
using Daftari.Pike13Api.DAL;
using Daftari.Pike13Api.Enum;
using Daftari.Pike13Api.Models;
using Daftari.Pike13Api.Services;
using LukeApps.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Daftari.Controllers
{
    public class WebhooksController : BaseController
    {
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Visits()
        {
            return View();
        }

        public ActionResult Persons()
        {
            return View();
        }

        public async Task<JsonResult> GetVisitsJSON()
        {
            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                var Subdomain = TokenProvider.GetProvider().GetSubdomain(User.Identity.Name);

                var subscribed_topics = await db.TopicSubsriptions.Where(x => x.Subdomain == Subdomain).ToListAsync();

                var available_topics = Enum.GetNames(typeof(VisitTopic)).Select(x => new
                {
                    key = (int)Enum.Parse(typeof(VisitTopic), x),
                    value = ((VisitTopic)Enum.Parse(typeof(VisitTopic), x)).GetDisplay()
                }).ToList();

                var detailCollection = (from avail in available_topics
                              join sub in subscribed_topics on avail.value equals sub.Topic into _sub
                              from sub in _sub.DefaultIfEmpty()
                              orderby avail.key
                              select new
                              {
                                  TopicSubsriptionID = sub?.TopicSubsriptionID,
                                  TopicID = sub?.TopicID,
                                  Topic = avail.value,
                                  Status = sub != null ? "Active" : "Not Subscribed"
                              }).ToList();

                var TotalRecords = detailCollection.Count();
                return Json(new
                {
                    iTotalRecords = TotalRecords,
                    iTotalDisplayRecords = TotalRecords,
                    aaData = detailCollection
                },
                JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> GetPersonsJSON()
        {
            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                var Subdomain = TokenProvider.GetProvider().GetSubdomain(User.Identity.Name);

                var subscribed_topics = await db.TopicSubsriptions.Where(x => x.Subdomain == Subdomain).ToListAsync();

                var available_topics = Enum.GetNames(typeof(PersonState)).Select(x => new
                {
                    key = (int)Enum.Parse(typeof(PersonState), x),
                    value = ((PersonState)Enum.Parse(typeof(PersonState), x)).GetDisplay()
                }).ToList();

                var detailCollection = (from avail in available_topics
                                        join sub in subscribed_topics on avail.value equals sub.Topic into _sub
                                        from sub in _sub.DefaultIfEmpty()
                                        orderby avail.key
                                        select new
                                        {
                                            TopicSubsriptionID = sub?.TopicSubsriptionID,
                                            TopicID = sub?.TopicID,
                                            Topic = avail.value,
                                            Status = sub != null ? "Active" : "Not Subscribed"
                                        }).ToList();

                var TotalRecords = detailCollection.Count();
                return Json(new
                {
                    iTotalRecords = TotalRecords,
                    iTotalDisplayRecords = TotalRecords,
                    aaData = detailCollection
                },
                JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> PersonsSubscribe(string Topic)
        {
            try
            {
                var url = Url.Action("OnPersonChanged", "/Webhooks", null, Request.Url.Scheme);
                var response = await new Pike13ApiRepo(User.Identity.Name).SubscribeToWebhooks(url, Topic);
                var subdomain = TokenProvider.GetProvider().GetSubdomain(User.Identity.Name);
                //save this data
                using (Pike13ApiContext db = new Pike13ApiContext())
                {
                    var topic = new TopicSubsription
                    {
                        TopicID = response.data.id,
                        Subdomain = subdomain,
                        Link = response.data.links.self,
                        BusinessID = response.data.attributes.business_id,
                        Topic = response.data.attributes.topic
                    };
                    db.TopicSubsriptions.Add(topic);
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
            return RedirectToAction("Persons");
        }
        public async Task<ActionResult> PersonsUnsubscribe(long? id)
        {
            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                var topic = await db.TopicSubsriptions.FindAsync(id);
                db.TopicSubsriptions.Remove(topic);
                await db.SaveChangesAsync();
                return RedirectToAction("Persons");
            }
        }
        public async Task<ActionResult> VisitsSubscribe(string Topic)
        {
            try
            {
                var url = Url.Action("OnVisitChanged", "/Webhooks", null, Request.Url.Scheme);
                //using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(new Exception(url), this.HttpContext))
                //{
                //    bh.Log_Error();
                //}
                var response = await new Pike13ApiRepo(User.Identity.Name).SubscribeToWebhooks(url, Topic);
                var subdomain = TokenProvider.GetProvider().GetSubdomain(User.Identity.Name);
                //save this data
                using (Pike13ApiContext db = new Pike13ApiContext())
                {
                    var topic = new TopicSubsription
                    {
                        TopicID = response.data.id,
                        Subdomain = subdomain,
                        Link = response.data.links.self,
                        BusinessID = response.data.attributes.business_id,
                        Topic = response.data.attributes.topic
                    };
                    db.TopicSubsriptions.Add(topic);
                    await db.SaveChangesAsync();
                }

                //return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(ex, this.HttpContext))
                {
                    bh.Log_Error();
                }
                //return Json(ex, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Visits");
        }

        public async Task<ActionResult> VisitsUnsubscribe(long? id)
        {
            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                var topic = await db.TopicSubsriptions.FindAsync(id);
                db.TopicSubsriptions.Remove(topic);
                await db.SaveChangesAsync();
                return RedirectToAction("Visits");
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<JsonResult> TestVisits()
        {
            try
            {

                //https://atss-muscat.pike13.com/api/v3/webhooks/14392
                //var pikeEv = (await new Pike13ApiRepo(User.Identity.Name).GetTest(14392)).First();
                var pikeEv = (await new Pike13ApiRepo(20168).GetEventOccurrenceByIdAsync(154487784)).First();
                //var ev = await new Pike13ApiRepo(User.Identity.Name).GetEventOccurrenceByIdAsync(129253590);

                //var temp = Url.Action("OnVisitCreated", "Pike13Access", null, Request.Url.Scheme);
                //var response = await new Pike13ApiRepo(User.Identity.Name).SubscribeToVisits("https://aqt.nsc.om//Pike13Access/OnVisitCreated");



                return Json(pikeEv, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(ex, this.HttpContext))
                {
                    bh.Log_Error();
                }
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> OnPersonChanged(PersonResponse payload)
        {
            try
            {
                //using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(new Exception(Newtonsoft.Json.JsonConvert.SerializeObject(payload)), this.HttpContext))
                //{
                //    bh.Log_Error();
                //}
                if (payload != null && payload.Data != null)
                {
                    var pike_person = payload.Data.People?.FirstOrDefault();
                    if (pike_person == null)
                        throw new Exception("Webhooks Person - no person object found");

                    var subdomain = TokenProvider.GetProvider().GetSubdomain(payload.Business_Id.Value);

                    using (Pike13ApiContext dba = new Pike13ApiContext())
                    {
                        if (await dba.TopicSubsriptions.Where(x => x.Subdomain == subdomain && x.Topic == payload.Topic && x.TopicID == payload.Webhook_Id).AnyAsync())
                        {
                            using (AquaCardsEntities db = new AquaCardsEntities())
                            {
                                var customer = await db.Customers.Where(q => q.SubDomain == subdomain && q.ExternalReference == pike_person.id).FirstOrDefaultAsync();
                                if (customer == null && payload.Topic != PersonState.Deleted.GetDisplay())
                                {
                                    customer = new Customer
                                    {
                                        ExternalReference = pike_person.id,
                                        FirstName = pike_person.first_name,
                                        MiddleName = pike_person.middle_name,
                                        LastName = pike_person.last_name,
                                        GuardianName = pike_person.guardian_name,
                                        FullName = string.Format("{0}{1}{2}", pike_person.first_name == null ? "" : pike_person.first_name + " ", pike_person.middle_name == null ? "" : pike_person.middle_name + " ", pike_person.last_name ?? ""),
                                        PrimaryPhone = pike_person.phone,
                                        EmailAddress = pike_person.email,
                                        Address = pike_person.address,
                                        City = pike_person.timezone,
                                        IsMember = pike_person.membership == "No Membership" ? false : true,
                                        Birthday = pike_person.birthdate == null ? (DateTime?)null : DateTime.Parse(pike_person.birthdate),
                                        IsSubToCommunications = true,
                                        SubDomain = subdomain,
                                        PhotoMD = pike_person.profile_photo?.x200,
                                        PhotoLG = pike_person.profile_photo?.x400,
                                        Dependants = string.Join(",", pike_person.dependents?.Select(d => d.id).ToList() ?? new List<long>()),
                                        Providers = string.Join(",", pike_person.providers?.Select(p => p.id).ToList() ?? new List<long>()),
                                    };
                                    db.Customers.Add(customer);
                                }
                                else
                                {
                                    customer.FirstName = pike_person.first_name;
                                    customer.LastName = pike_person.last_name;
                                    customer.MiddleName = pike_person.middle_name;
                                    customer.GuardianName = pike_person.guardian_name;
                                    customer.FullName = string.Format("{0}{1}{2}", pike_person.first_name == null ? "" : pike_person.first_name + " ", pike_person.middle_name == null ? "" : pike_person.middle_name + " ", pike_person.last_name ?? "");

                                    if (pike_person.email != null)
                                        customer.EmailAddress = pike_person.email;

                                    if (pike_person.phone != null)
                                        customer.PrimaryPhone = pike_person.phone;

                                    if (pike_person.birthdate != null)
                                        customer.Birthday = DateTime.Parse(pike_person.birthdate);

                                    customer.PhotoMD = pike_person.profile_photo?.x200;
                                    customer.PhotoLG = pike_person.profile_photo?.x400;
                                    customer.Dependants = string.Join(",", pike_person.dependents?.Select(d => d.id).ToList() ?? new List<long>());
                                    customer.Providers = string.Join(",", pike_person.providers?.Select(p => p.id).ToList() ?? new List<long>());
                                    //db.Entry(customer).State = EntityState.Modified;
                                    if (payload.Topic != PersonState.Deleted.GetDisplay())
                                    {
                                        db.Entry(customer).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        db.Customers.Remove(customer);
                                    }
                                }
                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            return new HttpStatusCodeResult(System.Net.HttpStatusCode.Gone);
                        }
                    }
                }
                else
                {
                    throw new Exception($"Webhooks Visit - Payload has no data, id => {payload?.Business_Id}, topic {payload?.Topic}");
                }
            } 
            catch (Exception ex)
            {
                using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(ex, this.HttpContext))
                {
                    bh.Log_Error();
                }
            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        public async Task<ActionResult> OnVisitChanged(VisitResponse payload)
        {
            try
            {
                if (payload != null && payload.Data != null)
                {
                    // only one record can be processed at a time
                    var pike_visit = payload.Data.Visits?.FirstOrDefault();
                    if (pike_visit == null)
                        throw new Exception("Webhooks Visit - no visit object found");

                    var subdomain = TokenProvider.GetProvider().GetSubdomain(payload.Business_Id.Value);
                    
                    using (Pike13ApiContext db = new Pike13ApiContext())
                    {
                        if (await db.TopicSubsriptions.Where(x => x.Subdomain == subdomain && x.Topic == payload.Topic && x.TopicID == payload.Webhook_Id).AnyAsync())
                        {
                            var event_occurrence = await db.EventOccurrances.Include(v => v.Visits)
                                                    .Where(x => x.EventOccurrenceID == pike_visit.event_occurrence_id)
                                                    .FirstOrDefaultAsync();

                            if (event_occurrence == null)
                            {
                                var pikeEv = (await new Pike13ApiRepo(payload.Business_Id.Value).GetEventOccurrenceByIdAsync(pike_visit.event_occurrence_id)).First();

                                event_occurrence = await db.EventOccurrances.Include(v => v.Visits)
                                                        .Where(x => x.EventOccurrenceID == pike_visit.event_occurrence_id)
                                                        .FirstOrDefaultAsync();

                                if (event_occurrence == null)
                                {
                                    event_occurrence = new EventOccurrance
                                    {
                                        AttendanceComplete = pikeEv.attendance_complete,
                                        CapacityRemaining = pikeEv.capacity_remaining,
                                        EndAt = pikeEv.end_at,
                                        EventID = pikeEv.event_id,
                                        Full = pikeEv.full,
                                        EventOccurrenceID = pikeEv.id,
                                        LocationID = pikeEv.location_id,
                                        Name = pikeEv.name,
                                        StartAt = pikeEv.start_at,
                                        State = pikeEv.state,
                                        ServiceID = pikeEv.service_id,
                                        Timezone = pikeEv.timezone,
                                        people = pikeEv.people.Any() ? string.Join(",", pikeEv.people.Select(x => x.id).OrderBy(x => x)) : pike_visit.person_id.ToString(),
                                        StaffMembers = string.Join(",", pikeEv.staff_members.Select(x => x.StaffID).OrderBy(x => x)),
                                        SubDomain = subdomain,
                                        Description = pikeEv.description,
                                        VisitsCount = pikeEv.visits_count,
                                        Visits = pikeEv.visits.Any() ? pikeEv.visits.Select(x => new Visit
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
                                            CreatedAt = x.created_at,
                                            Unpaid = x.status == VisitStatus.Unpaid.GetDisplay()
                                        }).ToList() : new List<Visit>
                                        {
                                            new Visit
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
                                                IsDeleted = payload.Topic == VisitTopic.Deleted.GetDisplay(),
                                                Unpaid = pike_visit.status == VisitStatus.Unpaid.GetDisplay()
                                            }
                                        }
                                    };

                                    db.EventOccurrances.Add(event_occurrence);
                                    await db.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                var visit = event_occurrence.Visits.FirstOrDefault(x => x.VisitID == pike_visit.id);
                                if (visit == null && payload.Topic != VisitTopic.Deleted.GetDisplay())
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
                                        IsDeleted = payload.Topic == VisitTopic.Deleted.GetDisplay(),
                                        Unpaid = pike_visit.status == VisitStatus.Unpaid.GetDisplay()
                                    });

                                    if (!string.IsNullOrEmpty(event_occurrence.people))
                                    {
                                        var people = event_occurrence.people.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList();
                                        people.Add(pike_visit.person_id?.ToString());
                                        event_occurrence.people = string.Join(",", people.Distinct());
                                    }
                                    else
                                    {
                                        event_occurrence.people = pike_visit.person_id?.ToString();
                                    }

                                    db.Entry(event_occurrence).State = EntityState.Modified;
                                }
                                else
                                {
                                    if ((visit.LastModified < pike_visit.LastModified || payload.Topic != VisitTopic.New.GetDisplay()) && visit.State != VisitTopic.Deleted.GetDisplay())
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
                                        if (pike_visit.status == VisitStatus.Unpaid.GetDisplay())
                                        {
                                            visit.Unpaid = true;
                                        }
                                        else if (pike_visit.state != VisitState.Completed.GetDisplay())
                                        {
                                            visit.Unpaid = false;
                                        }

                                        if (payload.Topic != VisitTopic.Deleted.GetDisplay())
                                        {
                                            db.Entry(visit).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            db.Visits.Remove(visit);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception($"Visit - Id => {visit.VisitID} is either late or deleted");
                                    }
                                }
                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            return new HttpStatusCodeResult(System.Net.HttpStatusCode.Gone);
                        }
                    }
                }
                else
                {
                    throw new Exception($"Webhooks Visit - Payload has no data, id => {payload?.Business_Id}, topic {payload?.Topic}");
                }
            }
            catch (Exception ex)
            {
                using (LukeApps.BugsTracker.BugsHandler bh = new LukeApps.BugsTracker.BugsHandler(ex, this.HttpContext))
                {
                    bh.Log_Error();
                }
            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }
    }
}