using Daftari.API.ViewModels;
using Daftari.AquaCards.DAL;
using Daftari.Pike13Api.Services;
using LukeApps.Utilities.Helpers;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Daftari.API.Controllers
{
    public class CustomersController : ApiController
    {
        [Authorize]
        [HttpGet]
        [Route("api/customers/dependants")]
        public async Task<IHttpActionResult> GetDependants()
        {
            var user = TokenProvider.GetProvider().GetUserData(User.Identity.Name);
            using (AquaCardsEntities db = new AquaCardsEntities())
            {
                var customer = await db.Customers.Where(x => x.SubDomain == user.Subdomain && x.ExternalReference == user.PersonID).ToListAsync();
                var dependant_ids = customer.Select(c => c.Dependants?.Split(',')).SelectMany(c => c).Where(c => !string.IsNullOrEmpty(c)).Select(c => long.Parse(c)).ToList();

                var dependants = await db.Customers.Where(x => x.SubDomain == user.Subdomain && dependant_ids.Contains(x.ExternalReference)).ToListAsync();

                var data = dependants.Select(x => new
                {
                    x.CustomerID,
                    PersonID = x.ExternalReference,
                    x.FirstName,
                    x.LastName,
                    x.PhotoMD
                }).ToList();

                return Ok(new DaftariResult<object>() { Body = data, IsSuccess = true });
            }
        }

        [HttpGet]
        [Route("api/account/initialImage")]
        public IHttpActionResult InitialImage(string FirstName, string LastName)
        {
            var stream = ImageHelper.GenerateCircle(FirstName, LastName);
            stream.Position = 0;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.GetBuffer())
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            response.Content.Headers.ContentLength = stream.Length;

            return ResponseMessage(response);
        }
    }
}
