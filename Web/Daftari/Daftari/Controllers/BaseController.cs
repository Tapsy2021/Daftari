
using LukeApps.Utilities;
using System.Text;
using System.Web.Mvc;

namespace Daftari.Controllers
{
    public abstract class BaseController : Controller
    {
        protected new JsonResult Json(object data, JsonRequestBehavior behavior)
        {
            return new JsonNetResult()
            {
                Data = data,
                JsonRequestBehavior = behavior
            };
        }

        protected new JsonResult Json(object data)
        {
            return new JsonNetResult()
            {
                Data = data,
            };
        }
    }
}