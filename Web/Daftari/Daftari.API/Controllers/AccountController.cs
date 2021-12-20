using LukeApps.AspIdentity;
using LukeApps.AspIdentity.ViewModel;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Daftari.API.Controllers
{
    public class AccountController : ApiController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [HttpPost]
        //[Route("login")]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IHttpActionResult> Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                //var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);
                //switch (result)
                //{
                //    case SignInStatus.Success:
                //        CookieHelper.StoreInCookie("Session", null, "TimezoneOffset", model.TimezoneOffset?.ToString(), System.DateTime.Today.AddYears(10));
                //        return RedirectToLocal(returnUrl);

                //    case SignInStatus.LockedOut:
                //        return View("Lockout");

                //    case SignInStatus.RequiresVerification:
                //        CookieHelper.StoreInCookie("Session", null, "TimezoneOffset", model.TimezoneOffset?.ToString(), System.DateTime.Today.AddYears(10));
                //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                //    case SignInStatus.Failure:
                //    default:
                //        ModelState.AddModelError("", "Invalid login attempt.");
                //        return View(model);
                //}
            }
            else
            {
                return BadRequest("Missing login details.");
            }
            return null;
        }
    }
}
