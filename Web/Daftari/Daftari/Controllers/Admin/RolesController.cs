using Daftari.ViewModel;
using LukeApps.AspIdentity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Daftari.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private RoleManager<IdentityRole> RoleManager;
        ApplicationDbContext db = new ApplicationDbContext();

        public RolesController()
        {
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            RoleManager = new RoleManager<IdentityRole>(roleStore);
        }
        // GET: Roles
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetJSON()
        {
            var roles = await RoleManager.Roles.Include(u => u.Users).ToListAsync();

            var detailCollection = roles
                .OrderByDescending(x => x.Id)
                .Select(r => new
            {
                r.Id,
                r.Name,
                Description = "",
                UserCount = r.Users.Count()
            }).ToList();

            var TotalRecords = detailCollection.Count();
            return Json(new
            {
                iTotalRecords = TotalRecords,
                iTotalDisplayRecords = TotalRecords,
                aaData = detailCollection
            },
            JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new RoleVM());
        }

        [HttpPost]
        public async Task<ActionResult> Create(RoleVM request)
        {
            if (ModelState.IsValid)
            {
                if (RoleManager.RoleExists(request.RoleName.Trim()))
                {
                    ViewBag.Message = "Role already exists";
                    return View(request);
                }

                var role = new IdentityRole { Id = Guid.NewGuid().ToString().ToLower(), Name = request.RoleName.Trim() };
                var result = await RoleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.Message = "Internal Error occured";
            }

            return View(request);
        }

        [HttpGet]
        public ActionResult Edit(string Id)
        {
            var role = RoleManager.FindById(Id);

            return View(new RoleVM 
            {
                Id = role.Id,
                RoleName = role.Name
            });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(RoleVM request)
        {
            if (ModelState.IsValid)
            {
                var roles = await RoleManager.Roles.ToListAsync();
                if (roles.Any(r => r.Name == request.RoleName.Trim() && r.Id != request.Id))
                {
                    ViewBag.Message = "Role already exists";
                    return View(request);
                }
                var role = roles.FirstOrDefault(x => x.Id == request.Id);
                if (role != null)
                {
                    role.Name = request.RoleName.Trim();
                    await RoleManager.UpdateAsync(role);
                }

                return RedirectToAction("Index");
            }

            return View(request);
        }

        public async Task<ActionResult> Delete(string Id)
        {
            var role = await RoleManager.FindByIdAsync(Id);
            await RoleManager.DeleteAsync(role);

            return RedirectToAction("Index");
        }
    }
}