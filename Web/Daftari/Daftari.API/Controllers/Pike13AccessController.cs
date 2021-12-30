using Daftari.API.ViewModels;
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Daftari.API.Controllers
{
    public class Pike13AccessController : ApiController
    {

        [Authorize]
        [HttpGet]
        [Route("api/pike13access/getschedule")]
        public async Task<IHttpActionResult> GetSchedule(int Year, int Month, int PersonID)
        {
            var visits = new List<Visit>();

            var from = new DateTime(Year, Month, 1);
            var to = from.AddMonths(1).AddMinutes(-1);
            //var customers = new List<Customer>();
            var cards = new List<StudentCard>();
            var user = TokenProvider.GetProvider().GetUserData(User.Identity.Name);
            var sd = user.Subdomain;
            
            var dependants = new List<long?>();
            ////get by username
            using (AquaCardsEntities db = new AquaCardsEntities())
            {
                var _customer = await db.Customers.Where(x => x.SubDomain == sd && x.ExternalReference == user.PersonID).Select(x => x.Dependants).ToListAsync();
                dependants = _customer.Select(c => c?.Split(',')).SelectMany(c => c).Where(c => !string.IsNullOrEmpty(c)).Select(c => (long?)long.Parse(c)).ToList();
                //validate parent
                dependants = dependants.Where(x=> x == PersonID).ToList();
                using (Pike13ApiContext pikedb = new Pike13ApiContext())
                {
                    visits = await pikedb.Visits.Include(e => e.EventOccurrance)
                            .Where(x => x.EventOccurrance.StartAt < to && x.EventOccurrance.EndAt >= from)
                            .Where(x => x.EventOccurrance.SubDomain == sd)
                            .Where(x => x.EventOccurrance.State != "deleted" && x.EventOccurrance.State != "disabled")
                            .Where(x => dependants.Contains(x.PersonID))
                            .ToListAsync();
                }

                var all_persons = visits.Select(x => x.PersonID).Union(visits.Where(x => !string.IsNullOrEmpty(x.EventOccurrance.StaffMembers))
                    .SelectMany(x => x.EventOccurrance.StaffMembers?.Split(',').Select(c => (long?)long.Parse(c))).Distinct() ?? new long?[0]);

                var customers = await db.Customers.Where(x => x.SubDomain == sd && all_persons.Contains(x.ExternalReference)).ToListAsync();
                cards = (await db.StudentCards.Where(s => all_persons.Contains(s.ExternalReferenceID))
                        .ToListAsync())
                        .GroupBy(c => c.ExternalReferenceID, (Key, i) => i.OrderBy(s => s.Level).Last()).ToList();

                var data = visits.Select(x => new
                {
                    x.EventOccurrenceID,
                    x.VisitID,
                    x.Paid,
                    x.PersonID,
                    x.State,
                    x.Status,
                    ServiceName = x.EventOccurrance.Name,
                    x.EventOccurrance.StartAt,
                    x.EventOccurrance.EndAt,
                    level = cards.Where(card => card.ExternalReferenceID == x.PersonID).Select(lvl => (int?)lvl.Level).FirstOrDefault(),
                    Name = customers.Where(cr => cr.ExternalReference == x.PersonID).Select(cr => $"{cr.FirstName} {cr.LastName}").FirstOrDefault(),
                    StaffMembers = string.Join(",", customers
                                    .Where(cr => !string.IsNullOrEmpty(x.EventOccurrance.StaffMembers))
                                    .Where(cr => (x.EventOccurrance.StaffMembers?.Split(',') ?? new string[0]).Contains(cr.ExternalReference.ToString()))
                                    .Select(cr => $"{cr.FirstName} {cr.LastName}").ToList())
                }).Where(x => x.Status != VisitStatus.Late_Cancel.GetDisplay()).ToList();

                return Ok(new DaftariResult<object>() { Body = data, IsSuccess = true });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/pike13access/cancelvisit")]
        public async Task<IHttpActionResult> CancelVisit([FromBody]VisitVM model)
        {
            try
            {
                using (Pike13ApiContext db = new Pike13ApiContext())
                {
                    var visit = await db.Visits.FindAsync(model.VisitID);
                    if (visit == null || visit.State == VisitState.Late_Canceled.GetDisplay() || visit.State != VisitState.Registered.GetDisplay())
                        return Ok(new DaftariResult<string>() { Body = "Visit successfully cancelled", IsSuccess = true });

                    var user = TokenProvider.GetProvider().GetUserData(User.Identity.Name);
                    var dependants = new List<long?>();
                    ////get by username
                    using (AquaCardsEntities _db = new AquaCardsEntities())
                    {
                        var _customer = await _db.Customers.Where(x => x.SubDomain == user.Subdomain && x.ExternalReference == user.PersonID).Select(x => x.Dependants).ToListAsync();
                        dependants = _customer.Select(c => c?.Split(',')).SelectMany(c => c).Where(c => !string.IsNullOrEmpty(c)).Select(c => (long?)long.Parse(c)).ToList();
                    }

                    if (!dependants.Contains(visit.PersonID))
                    {
                        return Content(HttpStatusCode.BadRequest, new DaftariResult<string>() { Body = "Invalid Dependant", IsSuccess = false });
                        //return BadRequest("Invalid Dependant");
                    }

                    var new_visit = await new Pike13ApiRepo(User.Identity.Name).PutVisitAsync(model.VisitID, "late_cancel");

                    if ((new_visit?.Any() ?? false) && new_visit[0].state != visit.State)
                    {
                        var pike_visit = new_visit.First();

                        visit.CancelledAt = pike_visit.cancelled_at;
                        visit.CompletedAt = pike_visit.completed_at;
                        visit.Paid = pike_visit.paid;
                        visit.PaidForBy = pike_visit.paid_for_by;
                        visit.PersonID = pike_visit.person_id;
                        visit.PunchID = pike_visit.punch_id;
                        visit.State = pike_visit.state;
                        visit.Status = pike_visit.status;
                        visit.UpdatedAt = pike_visit.updated_at;
                        visit.AuditDetail.LastModifiedDate = DateTime.Now;
                        db.Entry(visit).State = EntityState.Modified;
                        await db.SaveChangesAsync();                        
                    }
                    return Ok(new DaftariResult<string>() { Body = "Visit successfully cancelled", IsSuccess = true });
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message.Contains("404")) // not found
                    return Ok(new DaftariResult<string>() { Body = "Visit successfully cancelled", IsSuccess = true });
            }
            //catch (Exception)
            //{
            //    return BadRequest();
            //}

            return Content(HttpStatusCode.BadRequest, new DaftariResult<string>() { Body = "Failed to cancel class", IsSuccess = false });
            //return BadRequest("Internal Server Error");
        }
    }
}
