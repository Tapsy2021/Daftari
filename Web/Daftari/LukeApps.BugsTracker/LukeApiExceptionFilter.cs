using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Http.ExceptionHandling;

namespace LukeApps.BugsTracker
{
    public class LukeApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext exceptionContext)
        {
            BugsHandler app = new BugsHandler(exceptionContext);
            app.Log_Error();

            exceptionContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}
