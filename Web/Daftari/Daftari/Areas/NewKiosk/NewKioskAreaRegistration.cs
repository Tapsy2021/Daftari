using System.Web.Mvc;

namespace Daftari.Areas.NewKiosk
{
    public class NewKioskAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "NewKiosk";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "new_Kiosk_default",
                "NewKiosk/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}