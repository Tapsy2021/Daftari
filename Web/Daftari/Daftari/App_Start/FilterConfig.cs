using LukeApps.BugsTracker;
using System.Web.Mvc;

namespace Daftari
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LukeExceptionFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}