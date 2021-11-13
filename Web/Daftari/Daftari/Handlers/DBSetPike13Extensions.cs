using Daftari.Pike13Api.DAL;
using Daftari.Pike13Api.Models;
using Daftari.Pike13Api.Services;
using System.Linq;

namespace Daftari.Handlers
{
    public static class DBSetPike13Extensions
    {
        public static IQueryable<EventOccurrance> GetEventOccurrances(this Pike13ApiContext db)
        {
            var sd = TokenProvider.GetProvider().GetSubdomain(System.Web.HttpContext.Current.User.Identity.Name);
            return db.EventOccurrances.Where(q => q.SubDomain == sd);
        }
    }
}