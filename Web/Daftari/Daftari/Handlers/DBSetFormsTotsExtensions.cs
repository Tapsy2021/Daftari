using Daftari.Chemicals.DAL;
using Daftari.Chemicals.Models;
using Daftari.Forms.DAL;
using Daftari.Forms.Models;
using Daftari.Pike13Api.Services;
using System.Data.Entity;
using System.Linq;

namespace Daftari.Handlers
{
    public static class DBSetFormsExtensions
    {
        public static IQueryable<FormSettings> GetFormSettings(this FormsEntities db)
        {
            var sd = TokenProvider.GetProvider().GetSubdomain(System.Web.HttpContext.Current.User.Identity.Name);
            return db.FormSettings.Where(q => q.SubDomain == sd);
        }
    }
}