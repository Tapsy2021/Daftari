using Daftari.Pike13Api.Services;
using System.Web.Mvc;

namespace Daftari.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        // GET: Admin/Dashboard

        public ActionResult Index()
        {
            if (TokenProvider.GetProvider().IsRegistered(User.Identity.Name))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Authorize", "Pike13Access");
            }
        }
    }
}