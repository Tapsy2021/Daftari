using LukeApps.AspIdentity;
using LukeApps.AspIdentity.ViewModel;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Daftari.API.Services;
using Daftari.API.ViewModels;
using System.Linq;
using Daftari.Pike13Api.Services;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;

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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return Request.GetOwinContext().Authentication;
            }
        }


        [HttpPost]
        [Route("api/account/login")]
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
                switch (result)
                {
                    case SignInStatus.Success:
                        var user = await UserManager.FindByNameAsync(model.Username);
                        var token = new AuthService().GetSecurityToken(user.UserName, user.Id, "User");
                        var person = TokenProvider.GetProvider().GetUserData(user.UserName);
                        var FirstName = person.PersonName?.Split(' ').FirstOrDefault() ?? user.FirstName;
                        var LastName = person.PersonName?.Split(' ').LastOrDefault() ?? user.LastName;

                        return Ok(new DaftariResult<LoginResult>() { Body = new LoginResult() { AccessToken = token, FirstName = FirstName, LastName = LastName, Message = "Logged in" }, IsSuccess = true });
                        //    CookieHelper.StoreInCookie("Session", null, "TimezoneOffset", model.TimezoneOffset?.ToString(), System.DateTime.Today.AddYears(10));
                        //    return RedirectToLocal(returnUrl);

                        //case SignInStatus.LockedOut:
                        //    return View("Lockout");

                        //case SignInStatus.RequiresVerification:
                        //    CookieHelper.StoreInCookie("Session", null, "TimezoneOffset", model.TimezoneOffset?.ToString(), System.DateTime.Today.AddYears(10));
                        //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                        case SignInStatus.Failure:
                        default:
                        return Ok(new DaftariResult<string> { Body = "Invalid login attempt.", IsSuccess = false });
                }
            }
            else
            {
                return BadRequest("Missing login details.");
            }
        }

        [Authorize]
        [Route("api/account/logoff")]
        public IHttpActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return Ok(new DaftariResult<string> { Body = "Logged out.", IsSuccess = true });
        }

        //[HttpPost]
        //[Route("api/account/getname1")]
        //public string GetName1()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        if (identity != null)
        //        {
        //            IEnumerable<Claim> claims = identity.Claims;
        //        }
        //        return "Valid";
        //    }
        //    else
        //    {
        //        return "Invalid";
        //    }
        //}

        //[Authorize]
        //[HttpPost]
        //[Route("api/account/getname2")]
        //public object GetName2()
        //{
        //    var identity = User.Identity as ClaimsIdentity;
        //    if (identity != null)
        //    {
        //        IEnumerable<Claim> claims = identity.Claims;
        //        var name = claims.Where(p => p.Type == "name").FirstOrDefault()?.Value;
        //        return new
        //        {
        //            data = name
        //        };

        //    }
        //    return null;
        //}
    }
}
