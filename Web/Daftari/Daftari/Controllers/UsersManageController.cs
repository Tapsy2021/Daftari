using Daftari.Handlers;
using Daftari.Pike13Api.DAL;
using LukeApps.AspIdentity;
using LukeApps.AspIdentity.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Daftari.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;
        ApplicationDbContext db = new ApplicationDbContext();
        
        public UsersManageController()
        {
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            _roleManager = new RoleManager<IdentityRole>(roleStore);
        }

        public UsersManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
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
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public string ResetRoles(string sd)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                var peoples = db.AccountPeoples.Include(p => p.Token).Where(s => s.Subdomain == sd).ToList();

                foreach (var item in peoples)
                {
                    var user = UserManager.FindByName(item.Token.Username);

                    if (user != null)
                    {
                        UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());
                        switch (item.Role)
                        {
                            case "limited_staff_member":
                                UserManager.AddToRole(user.Id, "Instructor");
                                break;

                            case "manager":
                                UserManager.AddToRole(user.Id, "Manager");
                                break;

                            case "owner":
                                UserManager.AddToRole(user.Id, "Admin");
                                UserManager.AddToRole(user.Id, "Manager");
                                break;

                            case "primary_owner":
                                UserManager.AddToRole(user.Id, "Admin");
                                UserManager.AddToRole(user.Id, "Manager");
                                break;

                            case "staff_member":
                                UserManager.AddToRole(user.Id, "FrontDesk");
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            return "Success";
        }

        //public void ClearUserGroups(string userId)
        //{
        //    ClearUserRoles(userId);
        //    ApplicationUser user = _db.Users.Find(userId);
        //    user.Groups.Clear();
        //    _db.SaveChanges();
        //}

        //public void ClearUserRoles(string userId)
        //{
        //    ApplicationUser user = _userManager.FindById(userId);
        //    var currentRoles = new List<IdentityUserRole>();

        //    currentRoles.AddRange(user.Roles);
        //    foreach (IdentityUserRole role in currentRoles)
        //    {
        //        _userManager.RemoveFromRole(userId, role.Role.Name);
        //    }
        //}

        // GET: Admin/ApplicationUsers
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Manager,FrontDesk")]
        public async Task<JsonResult> GetJSON()
        {
            List<ApplicationUser> users = await UserManager.Users.Include(g => g.Groups).ToListAsync();
            var roleDict = await _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToDictionaryAsync(r => r.Id);
            var groupDict = await db.GetGroups().Select(r => new { r.Id, r.Name }).ToDictionaryAsync(r => r.Id);
            Dictionary<string, string> dict;
            using (var fq = new Pike13ApiContext())
            {
                dict = fq.AccountPeoples.GroupBy(p => p.Token.Username)
                                        .ToList()
                                        .Select(c => new { c.Key, SubDomains = string.Join(", ", c.Select(p => p.Subdomain + ": " + p.Role).ToArray()) })
                                        .ToDictionary(p => p.Key, p => p.SubDomains);
            }

            var detailCollection = users.Select(s => new
            {
                UserName = s.UserName,
                Email = s.Email,
                EmailConfirmed = s.EmailConfirmed,
                PhoneNumber = s.PhoneNumber,
                PhoneNumberConfirmed = s.PhoneNumberConfirmed,
                TwoFactorEnabled = s.TwoFactorEnabled,
                Branches = getBranches(dict, s.UserName),
                Roles = string.Join(", ", s.Roles.Select(r => roleDict[r.RoleId].Name)),
                Groups = string.Join(", ", s.Groups.Select(r => groupDict[r.GroupId].Name)),
                CreatedOn = s.CreatedOn.ToString("dd/MM/yyyy"),
                CreatedBy = s.CreatedBy,
            });

            var TotalRecords = detailCollection.Count();
            return Json(new
            {
                iTotalRecords = TotalRecords,
                iTotalDisplayRecords = TotalRecords,
                aaData = detailCollection
            },
            JsonRequestBehavior.AllowGet);
        }

        private string getBranches(Dictionary<string, string> dict, string key)
        {
            dict.TryGetValue(key, out string val);
            return val;
        }

        // GET: Admin/ApplicationUsers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = await UserManager.Users.FirstOrDefaultAsync(u => u.UserName == id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: /Account/Register
        [Authorize(Roles = "Admin")]
        public ActionResult Register()
        {
            ViewBag.Roles = new MultiSelectList(_roleManager.Roles.ToList(), "Name", "Name");
            ViewBag.Groups = new MultiSelectList(db.GetGroups().ToList(), "Name", "Name");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email, CreatedBy = User.Identity.Name, CreatedOn = DateTime.Now };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.CreateUserIdentityAsync(user);

                    var roles = model.UserRoles?.Split(',') ?? new string[0];

                    foreach (var role in roles)
                    {
                        await UserManager.AddToRoleAsync(user.Id, role);
                    }

                    var groups = model.UserGroups?.Split(',') ?? new string[0];

                    foreach (var group in groups)
                    {
                        UserManager.AddUserToGroup(user.Id, group);
                    }

                    //Confirmation Email
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Dashboard");
                }
                AddErrors(result);
            }
            ViewBag.Roles = new MultiSelectList(_roleManager.Roles.ToList(), "Name", "Name", model.UserRoles?.Split(','));
            ViewBag.Groups = new MultiSelectList(db.GetGroups().ToList(), "Name", "Name", model.UserGroups?.Split(','));

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: Admin/ApplicationUsers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = await UserManager.Users.Include(g => g.Groups).FirstOrDefaultAsync(u => u.UserName == id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            var roleDict = await _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToDictionaryAsync(r => r.Id);
            var groupDict = await db.GetGroups().Select(r => new { r.Id, r.Name }).ToDictionaryAsync(r => r.Id);

            ViewBag.Roles = new MultiSelectList(roleDict.Select(r => new { Name = r.Value.Name }), "Name", "Name");
            ViewBag.Groups = new MultiSelectList(groupDict.Select(r => new { Name = r.Value.Name }), "Name", "Name");
            return View(new EditUserViewModel
            {
                UserName = applicationUser.UserName,
                Email = applicationUser.Email,
                EmailConfirmed = applicationUser.EmailConfirmed,
                PhoneNumber = applicationUser.PhoneNumber,
                PhoneNumberConfirmed = applicationUser.PhoneNumberConfirmed,
                TwoFactorEnabled = applicationUser.TwoFactorEnabled,
                UserRoles = applicationUser.Roles.Select(r => roleDict[r.RoleId].Name).ToArray(),
                UserGroups = applicationUser.Groups.Select(r => groupDict[r.GroupId].Name).ToArray()
            });
        }

        // POST: Admin/ApplicationUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = await UserManager.Users.Include(g => g.Groups).FirstOrDefaultAsync(u => u.UserName == user.UserName);

                applicationUser.TwoFactorEnabled = user.TwoFactorEnabled;
                applicationUser.Email = user.Email;
                applicationUser.EmailConfirmed = user.EmailConfirmed;
                applicationUser.PhoneNumber = user.PhoneNumber;
                applicationUser.PhoneNumberConfirmed = user.PhoneNumberConfirmed;

                await UserManager.UpdateAsync(applicationUser);

                var rolesToDelete = await UserManager.GetRolesAsync(applicationUser.Id);
                await UserManager.RemoveFromRolesAsync(applicationUser.Id, rolesToDelete.ToArray());

                var rolesToAdd = user.UserRoles ?? new string[0];

                foreach (var role in rolesToAdd)
                {
                    await UserManager.AddToRoleAsync(applicationUser.Id, role);
                }

                var groupsToDelete = await UserManager.GetGroupAsync(applicationUser.Id);
                await UserManager.RemoveFromGroupsAsync(applicationUser.Id, groupsToDelete.ToArray());

                var groupsToAdd = user.UserGroups ?? new string[0];

                foreach (var group in groupsToAdd)
                {
                    await UserManager.AddToGroupAsync(applicationUser.Id, group);
                }

                return RedirectToAction("Index", new { id = applicationUser.UserName });
            }

            var roleDict = await _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToDictionaryAsync(r => r.Id);
            var groupDict = await db.GetGroups().Select(r => new { r.Id, r.Name }).ToDictionaryAsync(r => r.Id);

            ViewBag.Roles = new MultiSelectList(roleDict.Select(r => new { Name = r.Value.Name }), "Name", "Name", null, user.UserRoles);
            ViewBag.Groups = new MultiSelectList(groupDict.Select(r => new { Name = r.Value.Name }), "Name", "Name", null, user.UserGroups);
            return View(user);
        }

        // GET: Admin/ApplicationUsers/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = await UserManager.Users.FirstOrDefaultAsync(u => u.UserName == id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ApplicationUser user = await UserManager.Users.FirstOrDefaultAsync(u => u.UserName == id);
                var logins = user.Logins;
                var rolesForUser = await UserManager.GetRolesAsync(user.Id);

                using (var transaction = db.Database.BeginTransaction())
                {
                    foreach (var login in logins.ToList())
                    {
                        await UserManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                    }

                    if (rolesForUser.Count() > 0)
                    {
                        foreach (var item in rolesForUser.ToList())
                        {
                            // item should be the name of the role
                            var result = await UserManager.RemoveFromRoleAsync(user.Id, item);
                        }
                    }

                    await UserManager.DeleteAsync(user);
                    transaction.Commit();
                }

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        //
        // GET: /Users/ResetPassword
        public ActionResult ResetPassword(string id)
        {
            return id == null ? View("Error") : View(new ResetPasswordViewModel() { Code = id, Email = "placeholder@daftari.app" });
        }

        //
        // POST: /Users/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await UserManager.FindByNameAsync(model.Code);
            if (user == null)
            {
                return HttpNotFound();
            }

            user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("Not Succeeded");
            }

            return RedirectToAction("Index", "UsersManage");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                db.Dispose();
                _roleManager.Dispose();
                _roleManager = null;
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }
    }
}