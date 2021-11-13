using Daftari.AquaCards.DAL;
using Daftari.AquaCards.Models;
using Daftari.Pike13Api.Services;
using System.Data.Entity;
using System.Linq;

namespace Daftari.Handlers
{
    public static class DBSetAquaTotsExtensions
    {
        public static IQueryable<Customer> GetCustomers(this AquaCardsEntities db)
        {
            var sd = TokenProvider.GetProvider().GetSubdomain(System.Web.HttpContext.Current.User.Identity.Name);
            return db.Customers.Where(q => q.SubDomain == sd);
        }

        public static IQueryable<StudentCard> GetCards(this AquaCardsEntities db)
        {
            var sd = TokenProvider.GetProvider().GetSubdomain(System.Web.HttpContext.Current.User.Identity.Name);
            return db.StudentCards.Include(s => s.StudentCardDetails).Where(q => q.Customer.SubDomain == sd);
        }

        //public static IQueryable<ChemicalSettings> GetChemicalRecordSettings(this AquaCardsEntities db)
        //{
        //    var sd = TokenProvider.GetProvider().GetSubdomain(System.Web.HttpContext.Current.User.Identity.Name);
        //    return db.ChemicalSettings.Where(q => q.SubDomain == sd);
        //}

        //public static IQueryable<ChemicalSettings> CompleteChemicalSettings(this AquaCardsEntities db)
        //{
        //    var sd = TokenProvider.GetProvider().GetSubdomain(System.Web.HttpContext.Current.User.Identity.Name);
        //    return db.ChemicalSettings
        //            .Include(c => c.ChemicalCustomFields)
        //            //.Include(c => c.ChemicalRecords)
        //            .Include("ChemicalRecord.ChemicalCustomValue")
        //            .Where(q => q.SubDomain == sd);
        //}
    }
}