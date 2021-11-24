using Daftari.Chemicals.DAL;
using Daftari.Chemicals.Enum;
using Daftari.Chemicals.Models;
using Daftari.Handlers;
using Daftari.Pike13Api.DAL;
using Daftari.Pike13Api.Services;
using Daftari.ViewModel;
using LukeApps.AspIdentity;
using LukeApps.EmailHandling;
using LukeApps.Utilities;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Daftari.Controllers
{
    [Authorize(Roles = "Admin,Manager,ChemicalRecords")]
    public class ChemicalRecordsController : Controller
    {
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        //    base.OnActionExecuting(filterContext);
        //}
        private ChemicalsEntities db = new ChemicalsEntities(System.Web.HttpContext.Current.User.Identity.Name);

        [Authorize(Roles = "Admin,ChemicalRecords")]
        public ActionResult Index()
        {
            var cs = db.GetChemicalRecordSettings().Include(c => c.ChemicalCustomFields).FirstOrDefault();
            if (cs != null)
            {
                var columns = new Dictionary<string, string>
                {
                    { "Date", "Submitted At" },
                    { "SubmittedBy", "Submitted By" }
                }; //Name, Description

                if (cs.FreeChlorine == Visibility.Visible)
                {
                    columns.Add("FreeChlorine", cs.GetPropertyShortDisplay(i => i.FreeChlorine));
                }
                if (cs.TotalChlorine == Visibility.Visible)
                {
                    columns.Add("TotalChlorine", cs.GetPropertyShortDisplay(i => i.TotalChlorine));
                }
                if (cs.TotalBromine == Visibility.Visible)
                {
                    columns.Add("TotalBromine", cs.GetPropertyShortDisplay(i => i.TotalBromine));
                }
                if (cs.pH == Visibility.Visible)
                {
                    columns.Add("pH", cs.GetPropertyShortDisplay(i => i.pH));
                }
                if (cs.Alkalinity == Visibility.Visible)
                {
                    columns.Add("Alkalinity", cs.GetPropertyShortDisplay(i => i.Alkalinity));
                }
                if (cs.CalciumHardness == Visibility.Visible)
                {
                    columns.Add("CalciumHardness", cs.GetPropertyShortDisplay(i => i.CalciumHardness));
                }
                if (cs.PoolTemp == Visibility.Visible)
                {
                    columns.Add("PoolTemp", cs.GetPropertyShortDisplay(i => i.PoolTemp) + cs.TempUnits);
                }
                if (cs.AirTemp == Visibility.Visible)
                {
                    columns.Add("AirTemp", cs.GetPropertyShortDisplay(i => i.AirTemp) + cs.TempUnits);
                }
                if (cs.WaterClarity == Visibility.Visible)
                {
                    columns.Add("WaterClarity", cs.GetPropertyShortDisplay(i => i.WaterClarity));
                }
                if (cs.Backwash == Visibility.Visible)
                {
                    columns.Add("Backwash", cs.GetPropertyShortDisplay(i => i.Backwash));
                }
                if (cs.HRR_ORP == Visibility.Visible)
                {
                    columns.Add("HRR_ORP", cs.GetPropertyShortDisplay(i => i.HRR_ORP));
                }
                foreach (var field in cs.ChemicalCustomFields)
                {
                    columns.Add(field.Label.Replace(" ", ""), field.Label);
                }
                ViewBag.ColumnDefinitions = columns;
                return View();
            }

            return RedirectToAction("Settings");
        }

        [Authorize(Roles = "Admin,ChemicalRecords")]
        public async Task<JsonResult> GetJSON(DateTime StartDate, DateTime EndDate)
        {
            var cs = await db.GetChemicalRecordSettings()
                        .Include(c => c.ChemicalCustomFields)
                        .Include(c => c.ChemicalRecords)
                        .Include(c => c.ChemicalRecords.Select(cv => cv.ChemicalCustomValues)).FirstOrDefaultAsync();

            var detailCollection = new List<object>();
            if (cs != null)
            {
                foreach (var cr in cs.ChemicalRecords.Where(cr => cr.Date >= StartDate.ToServerTime() && cr.Date <= EndDate.ToServerTime()))
                {
                    var obj = new Dictionary<string, object>
                    {
                        { "Id", cr.ChemicalRecordID },
                        { "Date", cr.Date.ToClientTime().ToString("yyyy/MM/dd @ HH:mm tt") },
                        { "SubmittedBy", cr.AuditDetail.CreatedEntryUserID }
                    };

                    if (cs.FreeChlorine == Visibility.Visible)
                    {
                        obj.Add("FreeChlorine", cr.FreeChlorine);
                    }
                    if (cs.TotalChlorine == Visibility.Visible)
                    {
                        obj.Add("TotalChlorine", cr.TotalChlorine);
                    }
                    if (cs.TotalBromine == Visibility.Visible)
                    {
                        obj.Add("TotalBromine", cr.TotalBromine);
                    }
                    if (cs.pH == Visibility.Visible)
                    {
                        obj.Add("pH", cr.pH);
                    }
                    if (cs.Alkalinity == Visibility.Visible)
                    {
                        obj.Add("Alkalinity", cr.Alkalinity);
                    }
                    if (cs.CalciumHardness == Visibility.Visible)
                    {
                        obj.Add("CalciumHardness", cr.CalciumHardness);
                    }
                    if (cs.PoolTemp == Visibility.Visible)
                    {
                        obj.Add("PoolTemp", (int?)cr.TempConverter(cr.PoolTemp));
                    }
                    if (cs.AirTemp == Visibility.Visible)
                    {
                        obj.Add("AirTemp", (int?)cr.TempConverter(cr.AirTemp));
                    }
                    if (cs.WaterClarity == Visibility.Visible)
                    {
                        obj.Add("WaterClarity", cr.WaterClarity.ToString());
                    }
                    if (cs.Backwash == Visibility.Visible)
                    {
                        obj.Add("Backwash", cr.Backwash.ToString());
                    }
                    if (cs.HRR_ORP == Visibility.Visible)
                    {
                        obj.Add("HRR_ORP", cr.HRR_ORP);
                    }
                    foreach (var field in cs.ChemicalCustomFields)
                    {
                        obj.Add(field.Label.Replace(" ", ""), cr.ChemicalCustomValues.Where(x => x.ChemicalCustomFieldID == field.ChemicalCustomFieldID).Select(x => x.CustomValue).FirstOrDefault());
                    }
                    var detail = Chemicals.Helpers.AnonymousType.FromDictonaryToAnonymousObject(obj);
                    detailCollection.Add(detail);
                }
            }

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
        [Authorize(Roles = "Admin,ChemicalRecords")]
        public ActionResult Create()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            var cs = db.GetChemicalRecordSettings().Include(c => c.ChemicalCustomFields).FirstOrDefault();

            if (cs == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cr = new ChemicalRecordFormVM
            {
                ChemicalSettings = cs,
                //Date = DateTime.Now,
                Employee = creds.PersonName,
                TempUnits = cs.TempUnits,
                VolumeUnits = cs.VolumeUnits,
                AirTempMax = cs.AirTempMax,
                PoolTempMax = cs.PoolTempMax,
                //ChemicalRecordSettingsID = cs.ChemicalRecordSettingsID,
                ChemicalCustomValues = cs.ChemicalCustomFields.Select(x => new ChemicalCustomValueVM
                {
                    CustomValue = "",
                    ChemicalCustomField = new ChemicalCustomFieldVM
                    {
                        ChemicalCustomFieldID = x.ChemicalCustomFieldID,
                        Label = x.Label,
                        InputType = x.InputType,
                        Required = x.Required,
                        SelectOptions = x.SelectOptions
                    }
                }).ToList()
            };

            ViewBag.Users = new MultiSelectList(GetApplicationUsers(creds.Subdomain), "UserName", "UserName");

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            return View(cr);
        }

        private List<ApplicationUser> GetApplicationUsers(string Subdomain)
        {
            using (var fq = new Pike13ApiContext())
            {
                var Subdomain_Users = fq.AccountPeoples.Where(x => x.Subdomain == Subdomain && x.IsActive).Select(p => p.Token.Username).Distinct().ToList();

                var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var users = UserManager.Users.Include(r => r.Roles).Where(user => Subdomain_Users.Contains(user.UserName)).ToList();

                return users;
            }


            //var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var users = UserManager.Users.Include(r => r.Roles).ToList();
            //using (var fq = new Pike13ApiContext())
            //{
            //    var validUsernames = fq.AccountPeoples.Where(x => x.Subdomain == Subdomain && x.IsActive).Select(p => p.Token.Username).Distinct().ToList();
            //    users = users.Where(user => validUsernames.Any(username => username?.ToLower() == user.UserName?.ToLower())).ToList();
            //}
            //return users;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,ChemicalRecords")]
        public ActionResult Create(ChemicalRecordFormVM request)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            var cs = db.GetChemicalRecordSettings().FirstOrDefault();
            if (ModelState.IsValid)
            {
                var cr = new ChemicalRecord
                {
                    //ChemicalRecordSettingsID = request.ChemicalRecordSettingsID,
                    FreeChlorine = request.FreeChlorine ?? 0,
                    TotalChlorine = request.TotalChlorine ?? 0,
                    TotalBromine = request.TotalBromine ?? 0,
                    pH = request.pH ?? 0,
                    PoolTemp = request.PoolTemp,
                    AirTemp = request.AirTemp,
                    WaterClarity = request.WaterClarity,
                    Alkalinity = request.Alkalinity,
                    CalciumHardness = request.CalciumHardness,
                    Backwash = request.Backwash,
                    Notes = request.Notes,
                    Date = request.Date.Value.ToServerTime(),
                    Employee = request.Employee,
                    HRR_ORP = request.HRR_ORP,
                    SubmittedBy = User.Identity.Name,
                    UnitOfMeasurement = cs.UnitOfMeasurement,
                    ChemicalCustomValues = request.ChemicalCustomValues.Select(x => new ChemicalCustomValue
                    {
                        CustomValue = x.CustomValue,
                        ChemicalCustomFieldID = x.ChemicalCustomField.ChemicalCustomFieldID.Value
                    }).ToList()
                };
                cr.ChemicalSettings = cs;
                db.ChemicalRecords.Add(cr);
                db.SaveChanges();

                try
                {
                    var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                    var chemical_role_id = "";
                    using (var db = new ApplicationDbContext())
                    {
                        chemical_role_id = db.Roles.Where(x => x.Name.ToLower() == "ChemicalRecords".ToLower()).Select(x => x.Id).FirstOrDefault();
                    }

                    var users = GetApplicationUsers(creds.Subdomain)
                                .Where(u => u.Roles.Any(ur => ur.RoleId == chemical_role_id)).ToList();

                    //var users = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().Users
                    //            .Include(r => r.Roles)
                    //            .Where(u => u.Roles.Any(ur => ur.RoleId == chemical_role_id)).ToList();
                    //var users1 = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().Users
                    //            .Include(r => r.Roles)
                    //            .Where(u => u.UserName == "oupa").ToList();

                    //.Where(g => g.Groups.Any(ag => ag.Group.Name == "ChemicalRecords")).ToList();
                    //.Where(x => x.UserName == cr.AuditDetail.CreatedEntryUserID).First();
                    //construct body
                    var send_template = TemplateRepo.GetInstance().GetTemplate("Content/templates/chemicals_template.html");

                    send_template = send_template.Replace("business-name", creds.BusinessName)
                                        .Replace("free-chlorine", cr.FreeChlorine?.ToString("N2"))
                                        .Replace("total-chlorine", cr.TotalChlorine?.ToString("N2"))
                                        .Replace("ph-val", cr.pH?.ToString("N2"))
                                        .Replace("pool-temp", cr.PoolTemp?.ToString("N2"))
                                        .Replace("submitted-by", User.Identity.Name)
                                        .Replace("record-link", $"{baseUrl}ChemicalRecords/View/{cr.ChemicalRecordID}");

                    var mails = new List<EmailMessage>();
                    foreach (var user in users)
                    {
                        mails.Add(new EmailMessage()
                        {
                            SenderName = creds.BusinessName,
                            Body = send_template,
                            Subject = $"Chemical Record Alert",
                            RecipientName = user.UserName,
                            Recipient = user.Email
                        });
                    }

                    using (EmailHandler eh = new EmailHandler(mails, creds.Subdomain))
                        eh.Send();

                }
                catch { }


                return RedirectToAction("Index");
            }

            //var _creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            //var _cs = db.GetChemicalRecordSettings().Include(c => c.ChemicalCustomFields).FirstOrDefault();

            ViewBag.Users = new MultiSelectList(GetApplicationUsers(creds.Subdomain), "UserName", "UserName");

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            request.ChemicalSettings = cs;

            return View(request);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,ChemicalRecords")]
        public async Task<ActionResult> Settings()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            ViewBag.BusinessName = creds.BusinessName;

            ViewBag.InputTypes = Enum.GetNames(typeof(Chemicals.Enum.InputType)).Select(x => new
            {
                key = (int)Enum.Parse(typeof(Chemicals.Enum.InputType), x),
                value = ((Chemicals.Enum.InputType)Enum.Parse(typeof(Chemicals.Enum.InputType), x)).GetDisplay()
            }).ToList();
            ViewBag.YesNo = Enum.GetNames(typeof(YesNo)).Select(x => new
            {
                key = (int)Enum.Parse(typeof(YesNo), x),
                value = ((YesNo)Enum.Parse(typeof(YesNo), x)).GetDisplay()
            }).ToList();

            ViewBag.UnitOfMeasures = new MultiSelectList(Enum.GetNames(typeof(UnitOfMeasurement)).Select(x => new
            {
                value = ((UnitOfMeasurement)Enum.Parse(typeof(UnitOfMeasurement), x)).GetDisplay(),
                text = ((UnitOfMeasurement)Enum.Parse(typeof(UnitOfMeasurement), x)).GetDisplay()
            }).ToList(), "value", "text");

            var cs = await db.GetChemicalRecordSettings().Include(c => c.ChemicalCustomFields).FirstOrDefaultAsync();
            if (cs != null)
            {
                return View(new ChemicalSettingsVM
                {
                    ChemicalRecordSettingsID = cs.ChemicalRecordSettingsID,
                    Volume = cs.Volume,
                    IncreaseChlorine = cs.IncreaseChlorine,
                    NeutralizeChlorine = cs.NeutralizeChlorine,
                    IncreaseAlkalinity = cs.IncreaseAlkalinity,
                    DecreaseAlkalinity = cs.DecreaseAlkalinity,
                    IncreaseCalciumHardness = cs.IncreaseCalciumHardness,
                    IncreaseStabilizer = cs.IncreaseStabilizer,
                    IncreasepH = cs.IncreasepH,
                    DecreasepH = cs.DecreasepH,
                    //Defaults
                    FreeChlorine = cs.FreeChlorine,
                    FreeChlorineLowAlert = cs.FreeChlorineLowAlert,
                    FreeChlorineHighAlert = cs.FreeChlorineHighAlert,
                    TotalChlorine = cs.TotalChlorine,
                    TotalChlorineLowAlert = cs.TotalChlorineLowAlert,
                    TotalChlorineHighAlert = cs.TotalChlorineHighAlert,
                    TotalBromine = cs.TotalBromine,
                    TotalBromineLowAlert = cs.TotalBromineLowAlert,
                    TotalBromineHighAlert = cs.TotalBromineHighAlert,
                    pH = cs.pH,
                    pHLowAlert = cs.pHLowAlert,
                    pHHighAlert = cs.pHHighAlert,
                    PoolTemp = cs.PoolTemp,
                    IdealPoolTemp = cs.IdealPoolTemp,
                    PoolTempLowAlert = cs.PoolTempLowAlert,
                    PoolTempHighAlert = cs.PoolTempHighAlert,
                    AirTemp = cs.AirTemp,
                    WaterClarity = cs.WaterClarity,
                    Alkalinity = cs.Alkalinity,
                    AlkalinityLowAlert = cs.AlkalinityLowAlert,
                    AlkalinityHighAlert = cs.AlkalinityHighAlert,
                    CalciumHardness = cs.CalciumHardness,
                    CalciumHardnessLowAlert = cs.CalciumHardnessLowAlert,
                    CalciumHardnessHighAlert = cs.CalciumHardnessHighAlert,
                    HRR_ORP = cs.HRR_ORP,
                    Backwash = cs.Backwash,
                    Notes = cs.Notes,
                    UnitOfMeasurement = cs.UnitOfMeasurement,
                    PoolDailyCapacity = cs.PoolDailyCapacity,
                    //SubDomain = cs.SubDomain,
                    ChemicalCustomFields = cs.ChemicalCustomFields.Select(x => new ChemicalCustomFieldVM
                    {
                        ChemicalCustomFieldID = x.ChemicalCustomFieldID,
                        Label = x.Label,
                        InputType = x.InputType,
                        Required = x.Required,
                        SelectOptions = x.SelectOptions
                    }).ToList()
                });
            }
            else
            {
                return View(new ChemicalSettingsVM
                {
                    Volume = 10000,
                    IncreaseChlorine = IncreaseChlorine.Calcium_Hypochlorite_67,
                    NeutralizeChlorine = NeutralizeChlorine.Sodium_Sulfite,
                    IncreaseAlkalinity = IncreaseAlkalinity.Sodium_Bicarbonate,
                    DecreaseAlkalinity = DecreaseAlkalinity.Sodium_Bisulfate,
                    IncreaseCalciumHardness = IncreaseCalciumHardness.Calcium_Chloride_77,
                    IncreaseStabilizer = IncreaseStabilizer.Cyanuric_Acid,
                    IncreasepH = IncreasepH.Sodium_Carbonate,
                    DecreasepH = DecreasepH.Muriatic_Acid,
                    //Defaults
                    FreeChlorine = Visibility.Visible,
                    FreeChlorineLowAlert = 2.0,
                    FreeChlorineHighAlert = 4.0,
                    TotalChlorine = Visibility.Visible,
                    TotalChlorineLowAlert = 2.0,
                    TotalChlorineHighAlert = 4.0,
                    TotalBromine = Visibility.Visible,
                    TotalBromineLowAlert = 4.0,
                    TotalBromineHighAlert = 6.0,
                    pH = Visibility.Visible,
                    pHLowAlert = 7.4,
                    pHHighAlert = 7.6,
                    PoolTemp = Visibility.Visible,
                    IdealPoolTemp = 90.0,
                    PoolTempLowAlert = 88.0,
                    PoolTempHighAlert = 92.0,
                    AirTemp = Visibility.Visible,
                    WaterClarity = Visibility.Visible,
                    Alkalinity = Visibility.Visible,
                    AlkalinityLowAlert = 80,
                    AlkalinityHighAlert = 120,
                    CalciumHardness = Visibility.Visible,
                    CalciumHardnessLowAlert = 200,
                    CalciumHardnessHighAlert = 400,
                    HRR_ORP = Visibility.Visible,
                    Backwash = Visibility.Visible,
                    Notes = Visibility.Visible,
                    UnitOfMeasurement = UnitOfMeasurement.Imperial.ToString(),
                    PoolDailyCapacity = 0
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,ChemicalRecords")]
        public ActionResult Settings(ChemicalSettingsVM request)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            if (ModelState.IsValid)
            {
                var cs = db.GetChemicalRecordSettings().Include(c => c.ChemicalCustomFields).FirstOrDefault();
                if (cs != null)
                {
                    cs.Volume = request.Volume;
                    cs.IncreaseChlorine = request.IncreaseChlorine.Value;
                    cs.NeutralizeChlorine = request.NeutralizeChlorine.Value;
                    cs.IncreaseAlkalinity = request.IncreaseAlkalinity.Value;
                    cs.DecreaseAlkalinity = request.DecreaseAlkalinity.Value;
                    cs.IncreaseCalciumHardness = request.IncreaseCalciumHardness.Value;
                    cs.IncreaseStabilizer = request.IncreaseStabilizer.Value;
                    cs.IncreasepH = request.IncreasepH.Value;
                    cs.DecreasepH = request.DecreasepH.Value;
                    //Defaults
                    cs.FreeChlorine = request.FreeChlorine;
                    cs.FreeChlorineLowAlert = request.FreeChlorineLowAlert;
                    cs.FreeChlorineHighAlert = request.FreeChlorineHighAlert;
                    cs.TotalChlorine = request.TotalChlorine;
                    cs.TotalChlorineLowAlert = request.TotalChlorineLowAlert;
                    cs.TotalChlorineHighAlert = request.TotalChlorineHighAlert;
                    cs.TotalBromine = request.TotalBromine;
                    cs.TotalBromineLowAlert = request.TotalBromineLowAlert;
                    cs.TotalBromineHighAlert = request.TotalBromineHighAlert;
                    cs.pH = request.pH;
                    cs.pHLowAlert = request.pHLowAlert;
                    cs.pHHighAlert = request.pHHighAlert;
                    cs.PoolTemp = request.PoolTemp;
                    cs.IdealPoolTemp = request.IdealPoolTemp;
                    cs.PoolTempLowAlert = request.PoolTempLowAlert;
                    cs.PoolTempHighAlert = request.PoolTempHighAlert;
                    cs.AirTemp = request.AirTemp;
                    cs.WaterClarity = request.WaterClarity;
                    cs.Alkalinity = request.Alkalinity;
                    cs.AlkalinityLowAlert = request.AlkalinityLowAlert;
                    cs.AlkalinityHighAlert = request.AlkalinityHighAlert;
                    cs.CalciumHardness = request.CalciumHardness;
                    cs.CalciumHardnessLowAlert = request.CalciumHardnessLowAlert;
                    cs.CalciumHardnessHighAlert = request.CalciumHardnessHighAlert;
                    cs.HRR_ORP = request.HRR_ORP;
                    cs.Backwash = request.Backwash;
                    cs.Notes = request.Notes;
                    cs.UnitOfMeasurement = request.UnitOfMeasurement;
                    cs.PoolDailyCapacity = request.PoolDailyCapacity;

                    var deletedFields = request.ChemicalCustomFields.Where(x => x.IsDeleted).ToList();

                    foreach (var deletedField in deletedFields)
                    {
                        if (deletedField.ChemicalCustomFieldID.HasValue)
                            cs.ChemicalCustomFields.Remove(cs.ChemicalCustomFields.FirstOrDefault(x => x.ChemicalCustomFieldID == deletedField.ChemicalCustomFieldID));
                        request.ChemicalCustomFields.Remove(deletedField);
                    }

                    foreach (var field in request.ChemicalCustomFields)
                    {
                        var ref_field = cs.ChemicalCustomFields.FirstOrDefault(x => x.ChemicalCustomFieldID == field.ChemicalCustomFieldID);
                        if (ref_field != null)
                        {
                            ref_field.Label = field.Label;
                            ref_field.InputType = field.InputType.Value;
                            ref_field.Required = field.Required.Value;
                            ref_field.SelectOptions = field.SelectOptions;
                        }
                        else
                        {
                            cs.ChemicalCustomFields.Add(new ChemicalCustomField
                            {
                                Label = field.Label,
                                InputType = field.InputType.Value,
                                Required = field.Required.Value,
                                SelectOptions = field.SelectOptions
                            });
                        }
                    }

                    db.Entry(cs).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    cs = new ChemicalSettings
                    {
                        ChemicalRecordSettingsID = new Guid(),
                        Volume = request.Volume,
                        IncreaseChlorine = request.IncreaseChlorine.Value,
                        NeutralizeChlorine = request.NeutralizeChlorine.Value,
                        IncreaseAlkalinity = request.IncreaseAlkalinity.Value,
                        DecreaseAlkalinity = request.DecreaseAlkalinity.Value,
                        IncreaseCalciumHardness = request.IncreaseCalciumHardness.Value,
                        IncreaseStabilizer = request.IncreaseStabilizer.Value,
                        IncreasepH = request.IncreasepH.Value,
                        DecreasepH = request.DecreasepH.Value,
                        //Defaults
                        FreeChlorine = request.FreeChlorine,
                        FreeChlorineLowAlert = request.FreeChlorineLowAlert,
                        FreeChlorineHighAlert = request.FreeChlorineHighAlert,
                        TotalChlorine = request.TotalChlorine,
                        TotalChlorineLowAlert = request.TotalChlorineLowAlert,
                        TotalChlorineHighAlert = request.TotalChlorineHighAlert,
                        TotalBromine = request.TotalBromine,
                        TotalBromineLowAlert = request.TotalBromineLowAlert,
                        TotalBromineHighAlert = request.TotalBromineHighAlert,
                        pH = request.pH,
                        pHLowAlert = request.pHLowAlert,
                        pHHighAlert = request.pHHighAlert,
                        PoolTemp = request.PoolTemp,
                        IdealPoolTemp = request.IdealPoolTemp,
                        PoolTempLowAlert = request.PoolTempLowAlert,
                        PoolTempHighAlert = request.PoolTempHighAlert,
                        AirTemp = request.AirTemp,
                        WaterClarity = request.WaterClarity,
                        Alkalinity = request.Alkalinity,
                        AlkalinityLowAlert = request.AlkalinityLowAlert,
                        AlkalinityHighAlert = request.AlkalinityHighAlert,
                        CalciumHardness = request.CalciumHardness,
                        CalciumHardnessLowAlert = request.CalciumHardnessLowAlert,
                        CalciumHardnessHighAlert = request.CalciumHardnessHighAlert,
                        HRR_ORP = request.HRR_ORP,
                        Backwash = request.Backwash,
                        Notes = request.Notes,
                        SubDomain = creds.Subdomain,
                        UnitOfMeasurement = request.UnitOfMeasurement,
                        PoolDailyCapacity = request.PoolDailyCapacity,
                        ChemicalCustomFields = request.ChemicalCustomFields.Where(x => !x.IsDeleted).Select(x => new ChemicalCustomField
                        {
                            ChemicalCustomFieldID = x.ChemicalCustomFieldID ?? 0,
                            Label = x.Label,
                            InputType = x.InputType.Value,
                            Required = x.Required.Value,
                            SelectOptions = x.SelectOptions
                        }).ToList()
                    };
                    db.ChemicalSettings.Add(cs);
                    db.SaveChanges();
                }
                return RedirectToAction("Settings");
            }

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            ViewBag.InputTypes = Enum.GetNames(typeof(Chemicals.Enum.InputType)).Select(x => new
            {
                key = (int)Enum.Parse(typeof(Chemicals.Enum.InputType), x),
                value = ((Chemicals.Enum.InputType)Enum.Parse(typeof(Chemicals.Enum.InputType), x)).GetDisplay()
            }).ToList();
            ViewBag.YesNo = Enum.GetNames(typeof(YesNo)).Select(x => new
            {
                key = (int)Enum.Parse(typeof(YesNo), x),
                value = ((YesNo)Enum.Parse(typeof(YesNo), x)).GetDisplay()
            }).ToList();

            ViewBag.UnitOfMeasures = new MultiSelectList(Enum.GetNames(typeof(UnitOfMeasurement)).Select(x => new
            {
                value = ((UnitOfMeasurement)Enum.Parse(typeof(UnitOfMeasurement), x)).GetDisplay(),
                text = ((UnitOfMeasurement)Enum.Parse(typeof(UnitOfMeasurement), x)).GetDisplay()
            }).ToList(), "value", "text");

            return View(request);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,ChemicalRecords")]
        public async Task<ActionResult> View(long Id)
        {
            if (Id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            var db_cr = await db.ChemicalRecords
                            .Include(cv => cv.ChemicalCustomValues)
                            .Include(cs => cs.ChemicalSettings)
                            .Include(cv => cv.ChemicalCustomValues.Select(cf => cf.ChemicalCustomField))
                            .Include(c => c.ChemicalRecordComments)
                            .Where(x => x.ChemicalRecordID == Id)
                            .FirstOrDefaultAsync();

            var cr = new ChemicalRecordFormVM
            {
                ChemicalRecordID = db_cr.ChemicalRecordID,
                ChemicalSettings = db_cr.ChemicalSettings,
                FreeChlorine = db_cr.FreeChlorine,
                TotalChlorine = db_cr.TotalChlorine,
                TotalBromine = db_cr.TotalBromine,
                pH = db_cr.pH,
                WaterClarity = db_cr.WaterClarity,
                PoolTemp = db_cr.TempConverter(db_cr.PoolTemp),
                AirTemp = db_cr.TempConverter(db_cr.AirTemp),
                Alkalinity = db_cr.Alkalinity,
                Backwash = db_cr.Backwash,
                CalciumHardness = db_cr.CalciumHardness,
                Date = db_cr.Date.ToClientTime(),
                Employee = db_cr.Employee,
                HRR_ORP = db_cr.HRR_ORP,
                Notes = db_cr.Notes,
                SubmittedBy = db_cr.AuditDetail.CreatedEntryUserID,
                TempUnits = db_cr.ChemicalSettings.TempUnits,
                VolumeUnits = db_cr.ChemicalSettings.VolumeUnits,
                ChemicalCustomValues = db_cr.ChemicalCustomValues.Select(x => new ChemicalCustomValueVM
                {
                    ChemicalCustomField = new ChemicalCustomFieldVM
                    {
                        Label = x.ChemicalCustomField.Label
                    },
                    CustomValue = x.CustomValue
                }).ToList(),
                ChemicalRecordComments = db_cr.ChemicalRecordComments.Select(x => new ChemicalRecordCommentVM
                {
                    ChemicalRecordCommentID = x.ChemicalRecordCommentID,
                    ChemicalRecordID = x.ChemicalRecordID,
                    Comment = x.Comment,
                    Date = x.Date.ToClientTime(),
                    SubmittedBy = x.SubmittedBy
                }).ToList()
            };

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            return View(cr);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,ChemicalRecords")]
        public async Task<ActionResult> Edit(long Id)
        {
            if (Id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            var db_cr = await db.ChemicalRecords
                            .Include(cv => cv.ChemicalCustomValues)
                            .Include(cs => cs.ChemicalSettings)
                            .Include(cv => cv.ChemicalCustomValues.Select(cf => cf.ChemicalCustomField))
                            .Where(x => x.ChemicalRecordID == Id)
                            .FirstOrDefaultAsync();

            var cr = new ChemicalRecordFormVM
            {
                ChemicalRecordID = db_cr.ChemicalRecordID,
                ChemicalSettings = db_cr.ChemicalSettings,
                FreeChlorine = db_cr.FreeChlorine,
                TotalChlorine = db_cr.TotalChlorine,
                TotalBromine = db_cr.TotalBromine,
                pH = db_cr.pH,
                WaterClarity = db_cr.WaterClarity,
                PoolTemp = db_cr.TempConverter(db_cr.PoolTemp),
                AirTemp = db_cr.TempConverter(db_cr.AirTemp),
                Alkalinity = db_cr.Alkalinity,
                Backwash = db_cr.Backwash,
                CalciumHardness = db_cr.CalciumHardness,
                Date = db_cr.Date.ToClientTime(),
                Employee = db_cr.Employee,
                HRR_ORP = db_cr.HRR_ORP,
                Notes = db_cr.Notes,
                SubmittedBy = db_cr.SubmittedBy,
                AirTempMax = db_cr.ChemicalSettings.AirTempMax,
                PoolTempMax = db_cr.ChemicalSettings.PoolTempMax,
                ChemicalCustomValues = db_cr.ChemicalCustomValues.Where(x => x.ChemicalCustomField != null).Select(x => new ChemicalCustomValueVM
                {
                    ChemicalCustomField = new ChemicalCustomFieldVM
                    {
                        ChemicalCustomFieldID = x.ChemicalCustomField.ChemicalCustomFieldID,
                        Label = x.ChemicalCustomField.Label,
                        InputType = x.ChemicalCustomField.InputType,
                        Required = x.ChemicalCustomField.Required,
                        SelectOptions = x.ChemicalCustomField.SelectOptions
                    },
                    CustomValue = x.CustomValue,
                    ChemicalCustomValueID = x.ChemicalCustomValueID
                }).ToList()
            };

            ViewBag.Users = new MultiSelectList(GetApplicationUsers(creds.Subdomain), "UserName", "UserName");

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            return View(cr);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,ChemicalRecords")]
        public ActionResult Edit(ChemicalRecordFormVM request)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            if (ModelState.IsValid)
            {
                var cr = db.ChemicalRecords.Include(x => x.ChemicalCustomValues).Include(x => x.ChemicalSettings).FirstOrDefault(x => x.ChemicalRecordID == request.ChemicalRecordID);

                if (cr != null)
                {
                    cr.FreeChlorine = request.FreeChlorine ?? 0;
                    cr.TotalChlorine = request.TotalChlorine ?? 0;
                    cr.TotalBromine = request.TotalBromine ?? 0;
                    cr.pH = request.pH ?? 0;
                    cr.PoolTemp = cr.TempConverter(request.PoolTemp);
                    cr.AirTemp = cr.TempConverter(request.AirTemp);
                    cr.WaterClarity = request.WaterClarity;
                    cr.Alkalinity = request.Alkalinity;
                    cr.CalciumHardness = request.CalciumHardness;
                    cr.Backwash = request.Backwash;
                    cr.HRR_ORP = request.HRR_ORP;
                    cr.Notes = request.Notes;
                    cr.Date = request.Date.Value.ToServerTime();
                    cr.SubmittedBy = request.SubmittedBy;

                    request.ChemicalCustomValues.ForEach(obj =>
                    {
                        var ref_cv = cr.ChemicalCustomValues.FirstOrDefault(x => x.ChemicalCustomValueID == obj.ChemicalCustomValueID);
                        if (ref_cv != null)
                        {
                            ref_cv.CustomValue = obj.CustomValue;
                        }
                    });

                    db.Entry(cr).State = EntityState.Modified;
                    db.SaveChanges();

                    try
                    {
                        var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                        var chemical_role_id = "";
                        using (var db = new ApplicationDbContext())
                        {
                            chemical_role_id = db.Roles.Where(x => x.Name.ToLower() == "ChemicalRecords".ToLower()).Select(x => x.Id).FirstOrDefault();
                        }

                        var users = GetApplicationUsers(creds.Subdomain)
                                    .Where(u => u.Roles.Any(ur => ur.RoleId == chemical_role_id)).ToList();

                        var send_template = TemplateRepo.GetInstance().GetTemplate("Content/templates/chemicals_template.html");

                        send_template = send_template.Replace("business-name", creds.BusinessName)
                                            .Replace("free-chlorine", cr.FreeChlorine?.ToString("N2"))
                                            .Replace("total-chlorine", cr.TotalChlorine?.ToString("N2"))
                                            .Replace("ph-val", cr.pH?.ToString("N2"))
                                            .Replace("pool-temp", cr.PoolTemp?.ToString("N2"))
                                            .Replace("submitted-by", User.Identity.Name)
                                            .Replace("record-link", $"{baseUrl}ChemicalRecords/View/{cr.ChemicalRecordID}");

                        var mails = new List<EmailMessage>();
                        foreach (var user in users)
                        {
                            mails.Add(new EmailMessage()
                            {
                                SenderName = creds.BusinessName,
                                Body = send_template,
                                Subject = $"Chemical Record Alert",
                                RecipientName = user.UserName,
                                Recipient = user.Email
                            });
                        }

                        using (EmailHandler eh = new EmailHandler(mails, creds.Subdomain))
                            eh.Send();

                    }
                    catch { }
                }
                return RedirectToAction("Edit", new { Id = cr.ChemicalRecordID });
            }

            ViewBag.Users = new MultiSelectList(GetApplicationUsers(creds.Subdomain), "UserName", "UserName");

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            var cs = db.GetChemicalRecordSettings().FirstOrDefault();
            request.ChemicalSettings = cs;

            return View(request);
        }

        // GET: Admin/ChemicalRecord/Delete/5
        [Authorize(Roles = "Admin,ChemicalRecords")]
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var chemicalRecord = await db.ChemicalRecords.FindAsync(id);
            if (chemicalRecord == null)
            {
                return HttpNotFound();
            }
            return View(chemicalRecord);
        }

        // POST: Admin/ChemicalRecord/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ChemicalRecords")]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            var chemicalRecord = await db.ChemicalRecords.FindAsync(id);
            db.ChemicalRecords.Remove(chemicalRecord);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,ChemicalRecords")]
        public JsonResult AddComment(ChemicalRecordCommentVM request)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            var comment = new ChemicalRecordComment
            {
                ChemicalRecordID = request.ChemicalRecordID,
                Comment = request.Comment,
                Date = DateTime.Now,
                SubmittedBy = creds.PersonName
            };

            db.ChemicalRecordComments.Add(comment);
            db.SaveChanges();

            return Json(new
            {
                comment.ChemicalRecordCommentID,
                comment.ChemicalRecordID,
                comment.Comment,
                Date = comment.Date.ToClientTime().ToString("yyyy/MM/dd HH:mm:ss"),
                comment.SubmittedBy
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,ChemicalRecords")]
        public async Task<JsonResult> DeleteComment(long? id)
        {
            var comment = await db.ChemicalRecordComments.FindAsync(id);
            if (comment != null)
            {
                db.ChemicalRecordComments.Remove(comment);
                await db.SaveChangesAsync();
            }
            return Json("Deleted", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reports()
        {
            var sd = TokenProvider.GetProvider().GetSubdomain(User.Identity.Name);
            var cs = db.GetChemicalRecordSettings().Include(c => c.ChemicalCustomFields).FirstOrDefault();

            if (cs == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cr = new ChemicalRecordFormVM
            {
                ChemicalSettings = cs,
                //TempUnits = cs.TempUnits,
                //VolumeUnits = cs.VolumeUnits,
                //AirTempMax = cs.AirTempMax,
                //PoolTempMax = cs.PoolTempMax,
                ////ChemicalRecordSettingsID = cs.ChemicalRecordSettingsID,
                //ChemicalCustomValues = cs.ChemicalCustomFields.Select(x => new ChemicalCustomValueVM
                //{
                //    CustomValue = "",
                //    ChemicalCustomField = new ChemicalCustomFieldVM
                //    {
                //        ChemicalCustomFieldID = x.ChemicalCustomFieldID,
                //        Label = x.Label,
                //        InputType = x.InputType,
                //        Required = x.Required,
                //        SelectOptions = x.SelectOptions
                //    }
                //}).ToList()
            };

            return View(cr);
        }

        public async Task<JsonResult> GetReporting(DateTime from, DateTime to, string field)
        {
            var sd = TokenProvider.GetProvider().GetSubdomain(User.Identity.Name);
            if (to.Subtract(from).TotalDays < 6)
            {
                from = to.AddDays(-6).Date;
            }

            var cs = await db.GetChemicalRecordSettings()
                        .Include(c => c.ChemicalRecords).FirstOrDefaultAsync();

            var detailCollection = new List<object>();
            if (cs != null)
            {
                //foreach (var cr in cs.ChemicalRecords.Where(cr => cr.Date >= from.ToServerTime() && cr.Date <= to.ToServerTime()))
                //{
                //    var obj = new Dictionary<string, object>
                //    {
                //        { "Id", cr.ChemicalRecordID },
                //        { "Date", cr.Date.ToClientTime().ToString("yyyy/MM/dd @ HH:mm tt") },
                //        { "SubmittedBy", cr.AuditDetail.CreatedEntryUserID }
                //    };

                //    if (cs.FreeChlorine == Visibility.Visible)
                //    {
                //        obj.Add("FreeChlorine", cr.FreeChlorine);
                //    }
                //    if (cs.TotalChlorine == Visibility.Visible)
                //    {
                //        obj.Add("TotalChlorine", cr.TotalChlorine);
                //    }
                //    if (cs.TotalBromine == Visibility.Visible)
                //    {
                //        obj.Add("TotalBromine", cr.TotalBromine);
                //    }
                //    if (cs.pH == Visibility.Visible)
                //    {
                //        obj.Add("pH", cr.pH);
                //    }
                //    if (cs.Alkalinity == Visibility.Visible)
                //    {
                //        obj.Add("Alkalinity", cr.Alkalinity);
                //    }
                //    if (cs.CalciumHardness == Visibility.Visible)
                //    {
                //        obj.Add("CalciumHardness", cr.CalciumHardness);
                //    }
                //    if (cs.PoolTemp == Visibility.Visible)
                //    {
                //        obj.Add("PoolTemp", (int?)cr.TempConverter(cr.PoolTemp));
                //    }
                //    if (cs.AirTemp == Visibility.Visible)
                //    {
                //        obj.Add("AirTemp", (int?)cr.TempConverter(cr.AirTemp));
                //    }
                //    if (cs.WaterClarity == Visibility.Visible)
                //    {
                //        obj.Add("WaterClarity", cr.WaterClarity.ToString());
                //    }
                //    if (cs.Backwash == Visibility.Visible)
                //    {
                //        obj.Add("Backwash", cr.Backwash.ToString());
                //    }
                //    if (cs.HRR_ORP == Visibility.Visible)
                //    {
                //        obj.Add("HRR_ORP", cr.HRR_ORP);
                //    }
                //    foreach (var field1 in cs.ChemicalCustomFields)
                //    {
                //        obj.Add(field1.Label.Replace(" ", ""), cr.ChemicalCustomValues.Where(x => x.ChemicalCustomFieldID == field1.ChemicalCustomFieldID).Select(x => x.CustomValue).FirstOrDefault());
                //    }
                //    var detail = Chemicals.Helpers.AnonymousType.FromDictonaryToAnonymousObject(obj);
                //    detailCollection.Add(detail);
                //}
            }

            //var visits = new List<Visit>();
            //Chemicals.Models.ChemicalSettings cs;

            //if (to.Subtract(from).TotalDays < 6)
            //{
            //    from = to.AddDays(-6).Date;
            //}
            //var from = DateTime.Today.AddDays(-6);
            //var to = from.AddDays(7).AddSeconds(-1);

            //using (Pike13ApiContext db = new Pike13ApiContext())
            //{
            //    visits = await db.Visits
            //                        .Include(x => x.EventOccurrance)
            //                        .Where(x => x.EventOccurrance.StartAt < to && x.EventOccurrance.EndAt >= from)
            //                        .Where(x => x.EventOccurrance.SubDomain == sd)
            //                        .Where(x => x.EventOccurrance.State != "deleted" && x.EventOccurrance.State != "disabled")
            //                        .ToListAsync();
            //}

            //using (Chemicals.DAL.ChemicalsEntities db = new Chemicals.DAL.ChemicalsEntities())
            //{
            //    cs = await db.ChemicalSettings.Where(q => q.SubDomain == sd).FirstOrDefaultAsync();
            //}

            var week_dates = from.To(to).ToList();
            var grouped_chemicals = (from date in week_dates
                                  join c in cs.ChemicalRecords on date equals c.Date.Date into _c
                                  from c in _c.DefaultIfEmpty()
                                  group new { date, c } by date into i
                                  select new
                                  {
                                      Date = i.Key,
                                      Data = i.Where(x => x.c != null).Select(x => x.c).ToList()
                                  }).ToList();

            var property = typeof(ChemicalRecord).GetProperty(field);

            var model = new
            {
                Labels = week_dates.Select(x => x.ToString("dd MMM")).ToList(),
                Data = grouped_chemicals.Select(visit => visit.Data.Select(x => (double?)property.GetValue(x)).Average())
                        .Select(x => x.HasValue ? Math.Round(x.Value, 2) : x).ToList()
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> Chlorine_Chart_Read(DateTime from, DateTime to)
        {
            if (to.Subtract(from).TotalDays < 6)
            {
                from = to.AddDays(-6).Date;
            }

            var cs = await db.GetChemicalRecordSettings()
                        .Include(c => c.ChemicalRecords).FirstOrDefaultAsync();

            var week_dates = from.To(to).ToList();
            var grouped_chemicals = (from date in week_dates
                                     join c in cs.ChemicalRecords on date equals c.Date.Date into _c
                                     from c in _c.DefaultIfEmpty()
                                     group new { date, c } by date into i
                                     select new
                                     {
                                         Date = i.Key,
                                         Data = i.Where(x => x.c != null).Select(x => x.c).ToList()
                                     }).ToList();

            var model = new
            {
                Labels = week_dates.Select(x => x.ToString("dd MMM")).ToList(),
                FreeChlorine = grouped_chemicals.Select(visit => visit.Data.Select(x => x.FreeChlorine).Average())
                                .Select(x => x.HasValue ? Math.Round(x.Value, 2) : x).ToList(),
                CombinedChlorine = grouped_chemicals.Select(visit => visit.Data.Select(x => x.TotalChlorine - x.FreeChlorine).Average())
                                .Select(x => x.HasValue ? Math.Round(x.Value, 2) : x).ToList()
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}