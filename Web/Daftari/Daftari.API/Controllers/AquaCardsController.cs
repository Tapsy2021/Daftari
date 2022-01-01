using Daftari.API.ViewModels;
using Daftari.AquaCards.DAL;
using Daftari.Pike13Api.Services;
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
    public class AquaCardsController : ApiController
    {
        [Authorize]
        [HttpGet]
        [Route("api/aquacards/getcards")]
        public async Task<IHttpActionResult> GetCards()
        {
            var user = TokenProvider.GetProvider().GetUserData(User.Identity.Name);
            using (AquaCardsEntities db = new AquaCardsEntities())
            {
                var customer = await db.Customers.Where(x => x.SubDomain == user.Subdomain && x.ExternalReference == user.PersonID).ToListAsync();
                var dependant_ids = customer.Select(c => c.Dependants?.Split(',')).SelectMany(c => c).Where(c => !string.IsNullOrEmpty(c)).Select(c => long.Parse(c)).ToList();

                var customer_ids = await db.Customers.Where(x => x.SubDomain == user.Subdomain && dependant_ids.Contains(x.ExternalReference)).Select(x =>x.CustomerID).ToListAsync();
                customer_ids.Add(customer.Select(x => x.CustomerID).FirstOrDefault());

                var cards = db.StudentCards.Include(s => s.StudentCardDetails)
                                            .Include(c => c.Customer)
                                            .Where(s => customer_ids.Contains(s.CustomerID))
                                            .OrderByDescending(s => s.Level).ToList();

                var data = cards.Select(x => new
                {
                    x.StudentCardID,
                    x.Initial,
                    x.Customer?.FirstName,
                    x.Customer?.LastName,
                    x.StudentName,
                    x.BirthDate,
                    x.Level,
                    x.Customer?.PhotoLG,
                    StudentCardDetails = x.StudentCardDetails?.Select(cd => new
                    {
                        cd.StudentCardDetailID,
                        Skill = new
                        {
                            cd.Skill?.SkillID,
                            cd.Skill?.SetName,
                            cd.Skill?.Name,
                            cd.Skill?.SkillLevel,
                            cd.Skill?.SkillDifficulty
                        },
                        cd.IsComplete,
                        cd.CompleteDate,
                        cd.CompletedBy
                    }).ToList()
                }).ToList();

                return Ok(new DaftariResult<object>() { Body = data, IsSuccess = true });
            }
        }

    }
}
