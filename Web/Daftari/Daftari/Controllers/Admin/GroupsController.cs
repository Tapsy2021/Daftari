using Daftari.Handlers;
using Daftari.ViewModel;
using LukeApps.AspIdentity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Daftari.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GroupsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }
        //All Employeesl
        public async Task<JsonResult> GetJSON()
        {
            var groups = await db.GetGroups().Include(u => u.UserGroups).ToListAsync();

            var detailCollection = groups.Select(r => new
            {
                r.Id,
                r.Name,
                Description = "",
                UserCount = r.UserGroups.Count()
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
            return View(new GroupVM());
        }

        [HttpPost]
        public async Task<ActionResult> Create(GroupVM request)
        {
            if (ModelState.IsValid)
            {
                if (db.Groups.Any(g => g.Name == request.GroupName.Trim()))
                {
                    ViewBag.Message = "Group already exists";
                    return View(request);
                }

                var group = new ApplicationGroup { Name = request.GroupName.Trim() };
                db.Groups.Add(group);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(request);
        }

        [HttpGet]
        public ActionResult Edit(long Id)
        {
            var group = db.Groups.FirstOrDefault(x => x.Id == Id);

            return View(new GroupVM
            {
                Id = group.Id,
                GroupName = group.Name
            });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(GroupVM request)
        {
            if (ModelState.IsValid)
            {
                var groups = await db.Groups.ToListAsync();
                if (groups.Any(r => r.Name.ToLower() == request.GroupName.Trim().ToLower() && r.Id != request.Id))
                {
                    ViewBag.Message = "Group already exists";
                    return View(request);
                }
                var group = groups.FirstOrDefault(x => x.Id == request.Id);
                if (group != null)
                {
                    group.Name = request.GroupName.Trim();
                    db.Entry(group).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            return View(request);
        }

        public async Task<ActionResult> Delete(long Id)
        {
            var group = await db.Groups.FirstOrDefaultAsync(g => g.Id == Id);
            db.Groups.Remove(group);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}