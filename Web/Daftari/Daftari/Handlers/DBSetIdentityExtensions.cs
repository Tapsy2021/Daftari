using LukeApps.AspIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daftari.Handlers
{
    public static class DBSetIdentityExtensions
    {
        public static IQueryable<ApplicationGroup> GetGroups(this ApplicationDbContext db)
        {
            return db.Groups.Where(q => q.Name != "All Employees");
        }
    }
}