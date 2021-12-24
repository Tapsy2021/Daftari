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
        public async Task<IHttpActionResult> GetSchedule(int Year, int Month, int PersonID)//(DateTime from, DateTime to, long? staff_member_ids)
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
                    Name = customers.Where(cr => cr.ExternalReference == x.PersonID).Select(cr => $"{cr.FirstName} {cr.LastName}").FirstOrDefault(),
                    StaffMembers = string.Join(",", customers
                                    .Where(cr => !string.IsNullOrEmpty(x.EventOccurrance.StaffMembers))
                                    .Where(cr => (x.EventOccurrance.StaffMembers?.Split(',') ?? new string[0]).Contains(cr.ExternalReference.ToString()))
                                    .Select(cr => $"{cr.FirstName} {cr.LastName}").ToList())
                    //person = (from customer in customers
                    //          where customer.ExternalReference == x.PersonID
                    //          join card in cards on customer.ExternalReference equals card.ExternalReferenceID into _card
                    //          from card in _card.DefaultIfEmpty()
                    //          select new
                    //          {
                    //              id = customer.ExternalReference,
                    //              name = customer != null ? $"{customer.FirstName} {customer.LastName}" : "Not Synced",
                    //              level = card?.Level.GetDisplay(),
                    //              lastOpenDateTime = card?.LastOpenDate.ToUniversalTime()
                    //          }).FirstOrDefault() ?? new { id = 0L, name = "Not Synced", level = "", lastOpenDateTime = (DateTime?)null },

                    //Staff_Members = (from id in (x.EventOccurrance.StaffMembers?.Split(',') ?? new string[0])
                    //                 join staff in customers on id equals staff.ExternalReference.ToString() into _staff
                    //                 from staff in _staff.DefaultIfEmpty()
                    //                 where !string.IsNullOrEmpty(x.EventOccurrance.StaffMembers)
                    //                 select new
                    //                 {
                    //                     id = staff?.ExternalReference ?? 0,
                    //                     name = staff != null ? $"{staff.FirstName} {staff.LastName}" : "Not Synced",
                    //                     email = staff?.EmailAddress
                    //                 }).ToList()
                }).Where(x => x.Status != VisitStatus.Late_Cancel.GetDisplay()).ToList();

                return Ok(new DaftariResult<object>() { Body = data, IsSuccess = true });
            }
        }
    }
}
