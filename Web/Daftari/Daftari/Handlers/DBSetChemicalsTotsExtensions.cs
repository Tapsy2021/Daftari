using Daftari.Chemicals.DAL;
using Daftari.Chemicals.Models;
using Daftari.Pike13Api.Services;
using System.Data.Entity;
using System.Linq;

namespace Daftari.Handlers
{
    public static class DBSetChemicalsExtensions
    {
        public static IQueryable<ChemicalSettings> GetChemicalRecordSettings(this ChemicalsEntities db)
        {
            var sd = TokenProvider.GetProvider().GetSubdomain(System.Web.HttpContext.Current.User.Identity.Name);
            return db.ChemicalSettings.Where(q => q.SubDomain == sd);
        }

        public static IQueryable<ChemicalSettings> CompleteChemicalSettings(this ChemicalsEntities db)
        {
            var sd = TokenProvider.GetProvider().GetSubdomain(System.Web.HttpContext.Current.User.Identity.Name);
            return db.ChemicalSettings
                    .Include(c => c.ChemicalCustomFields)
                    //.Include(c => c.ChemicalRecords)
                    .Include("ChemicalRecord.ChemicalCustomValue")
                    .Where(q => q.SubDomain == sd);
        }
    }
}