using System.Web.Mvc;

namespace LukeApps.BugsTracker
{
    public class LukeExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext exceptionContext)
        {
            BugsHandler app = new BugsHandler(exceptionContext.Exception, exceptionContext.HttpContext);
            app.Log_Error();

            exceptionContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml"
            };
        }
    }
}