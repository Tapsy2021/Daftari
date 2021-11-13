using Daftari.Forms.DAL;
using Daftari.Forms.Enum;
using Daftari.Forms.Models;
using Daftari.Handlers;
using Daftari.Helpers;
using Daftari.Pike13Api.DAL;
using Daftari.Pike13Api.Services;
using Daftari.ViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using LukeApps.AspIdentity;
using LukeApps.EmailHandling;
using LukeApps.Utilities;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Daftari.Controllers
{
    [Authorize]
    //[Authorize(Roles = "Admin,Manager,Forms")]
    public class FormsController : Controller
    {
        public const string ImageTempDirectory = "~/App_Data/uploads/temp";
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        //    base.OnActionExecuting(filterContext);
        //}
        private FormsEntities db = new FormsEntities(System.Web.HttpContext.Current.User.Identity.Name);

        // GET: Forms
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Manager,Forms")]
        public ActionResult CreateCompletedForm(Guid CustomFormId)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            var fs = db.GetFormSettings()
                .Include(c => c.FormCustomFields)
                .Include(s => s.FormSignatureFields)
                .Where(f => f.FormSettingsID == CustomFormId).FirstOrDefault();

            if (fs == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cf = new CompletedFormVM
            {
                FormGuidID = Guid.NewGuid(),
                FormSettings = fs,
                FormCustomValues = fs.FormCustomFields.Select(x => new FormCustomValueVM
                {
                    CustomValue = "",
                    FormCustomField = new FormCustomFieldVM
                    {
                        FormCustomFieldID = x.FormCustomFieldID,
                        Label = x.Label,
                        InputType = x.InputType,
                        Required = x.Required,
                        SelectOptions = x.SelectOptions
                    }
                }).ToList(),
                FormSignatureValues = fs.FormSignatureFields.Select(x => new FormSignatureValueVM
                {
                    SignatureContent = "",
                    FormSignatureField = new FormSignatureFieldVM
                    {
                        FormSignatureFieldID = x.FormSignatureFieldID,
                        Name = x.Name,
                        Required = x.Required
                    }
                }).ToList()
            };

            ViewBag.Users = new SelectList(GetApplicationUsers(creds.Subdomain), "UserName", "UserName");
            using (var db = new ApplicationDbContext())
            {
                ViewBag.Groups = new SelectList(db.Groups.ToList(), "Name", "Name");
            }            

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            return View(cf);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Manager,Forms")]
        public ActionResult CreateCompletedForm(CompletedFormVM request)
        {
            if (ModelState.IsValid)
            {
                var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
                var fs = db.GetFormSettings().Where(x=> x.FormSettingsID == request.FormSettings.FormSettingsID).FirstOrDefault();
                //if dir not exists create
                if (!Directory.Exists(Server.MapPath(ImageTempDirectory)))
                {
                    Directory.CreateDirectory(Server.MapPath(ImageTempDirectory));
                }
                var fileEntries = Directory.GetFiles(Server.MapPath(ImageTempDirectory)).Where(x => x.Contains(request.FormGuidID.ToString())).ToList();
                foreach (string fileName in fileEntries)
                {
                    var oFileInfo = new FileInfo(fileName);
                    string mimeType = MimeMapping.GetMimeMapping(fileName);
                    request.FormAttachments.Add(new FormAttachmentVM
                    {
                        FileBytes = FileHelper.GetImageFileBytes(fileName),
                        FileName = oFileInfo.Name,
                        ContentType = MimeMapping.GetMimeMapping(fileName)
                    });
                }

                var fr = new Form
                {
                    FormID = request.FormID,
                    FormSettings = fs,
                    FormAttachments = request.FormAttachments.Select(x => new FormAttachment
                    {
                        FormAttachmentID = Guid.NewGuid(),
                        FileName = Path.GetFileName(x.FileName).Replace(request.FormGuidID.ToString() + "_" , ""),
                        ContentType = x.ContentType,
                        FileBytes = ImageHelper.IsValidImage(x.ContentType) ? ImageHelper.CreateThumbnail(x.FileBytes, 512) : x.FileBytes
                    }).ToList(),
                    FormCustomValues = request.FormCustomValues.Select(x => new FormCustomValue
                    {
                        CustomValue = x.CustomValue,
                        FormCustomFieldID = x.FormCustomField.FormCustomFieldID.Value
                    }).ToList(),
                    FormSignatureValues = request.FormSignatureValues.Select(x => new FormSignatureValue
                    {
                        SignatureContent = x.SignatureContent,
                        FormSignatureFieldID = x.FormSignatureField.FormSignatureFieldID.Value
                    }).ToList(),
                    ApprovalProcess = fs.ApprovalProcess?.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(x => new ApprovalProcess
                    {
                        UserName = x,
                        Status = ApprovalStatus.Pending
                    }).ToList()
                };
                fr.FormSettings = fs;
                db.Forms.Add(fr);
                db.SaveChanges();

                foreach(var fileName in fileEntries)
                {
                    System.IO.File.Delete(fileName);
                }

                try
                {
                    var users = GetApplicationUsers(creds.Subdomain);
                    var send_to_users = users.Where(x => fs.SendNotificationsTo?.Split(',').Contains(x.UserName) ?? false).ToList();
                    var approval_users = users.Where(x => fs.ApprovalProcess?.Split(',').Contains(x.UserName) ?? false).ToList();
                    send_to_users = send_to_users.Where(su => !approval_users.Any(au => au.UserName == su.UserName)).ToList();

                    var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

                    var mails = new List<EmailMessage>();
                    //construct body
                    var send_template = TemplateRepo.GetInstance().GetTemplate("Content/templates/form_template.html");
                    var approval_template = TemplateRepo.GetInstance().GetTemplate("Content/templates/form_approval_template.html");

                    send_template = send_template.Replace("form-title", fs.Title)
                                        .Replace("business-name", creds.BusinessName)
                                        .Replace("submitted-by", fr.AuditDetail.CreatedEntryUserID)
                                        .Replace("form-link", $"{baseUrl}Forms/ViewCompletedForm/{fr.FormID}");
                    foreach (var user in send_to_users)
                    {
                        var mail = new EmailMessage()
                        {
                            SenderName = creds.BusinessName,
                            Body = send_template,
                            Subject = $"{fs.Title} form submitted",
                            RecipientName = user.UserName,
                            Recipient = user.Email
                        };
                        mails.Add(mail);
                    }

                    approval_template = approval_template.Replace("form-title", fs.Title)
                                        .Replace("business-name", creds.BusinessName)
                                        .Replace("submitted-by", fr.AuditDetail.CreatedEntryUserID)
                                        .Replace("form-link", $"{baseUrl}Forms/EditCompletedForm/{fr.FormID}");

                    foreach (var user in approval_users)
                    {
                        var mail = new EmailMessage()
                        {
                            SenderName = creds.BusinessName,
                            Body = approval_template,
                            Subject = $"{fs.Title} form submitted for approval",
                            RecipientName = user.UserName,
                            Recipient = user.Email
                        };
                        mails.Add(mail);
                    }

                    using (EmailHandler eh = new EmailHandler(mails, creds.Subdomain))
                        eh.Send();

                }
                catch { }

                return RedirectToAction("Index");
            }

            return View(request);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Manager,Forms")]
        public async Task<ActionResult> EditCompletedForm(long Id)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            var db_cr = await db.Forms
                            .Include(fv => fv.FormCustomValues)
                            .Include(fs => fs.FormSignatureValues)
                            .Include(cs => cs.FormSettings)
                            .Include(cv => cv.FormCustomValues.Select(cf => cf.FormCustomField))
                            .Include(cv => cv.FormSignatureValues.Select(cf => cf.FormSignatureField))
                            .Include(fa => fa.FormAttachments)
                            .Include(s => s.ApprovalProcess)
                            .Where(x => x.FormID == Id)
                            .FirstOrDefaultAsync();

            var cr = new CompletedFormVM
            {
                FormID = db_cr.FormID,
                FormGuidID = Guid.NewGuid(),
                FormSettings = db_cr.FormSettings,
                FormAttachments = db_cr.FormAttachments.Where(x => db_cr.FormSettings.IsAttachmentEnabled).Select(x => new FormAttachmentVM
                {
                    FormAttachmentID = x.FormAttachmentID,
                    FileName = x.FileName,
                    ContentType = x.ContentType,
                    FileSize = x.FileBytes.LongLength.GetSizeInMemory(),
                    Base64 = ImageHelper.IsValidImage(x.ContentType) ? ImageHelper.CreateThumbnail(x.FileBytes, 48)?.ToBase64(x.ContentType) : null
                }).ToList(),
                FormSignatureValues = db_cr.FormSignatureValues.Where(x =>x.FormSignatureField != null).Select(x => new FormSignatureValueVM
                {
                    FormSignatureField = new FormSignatureFieldVM
                    {
                        FormSignatureFieldID = x.FormSignatureField.FormSignatureFieldID,
                        Name = x.FormSignatureField.Name,
                        Required = x.FormSignatureField.Required
                    },
                    SignatureContent = x.SignatureContent,
                    FormSignatureValueID = x.FormSignatureValueID
                }).ToList(),
                FormCustomValues = db_cr.FormCustomValues.Where(x => x.FormCustomField != null).Select(x => new FormCustomValueVM
                {
                    FormCustomField = new FormCustomFieldVM
                    {
                        FormCustomFieldID = x.FormCustomField.FormCustomFieldID,
                        Label = x.FormCustomField.Label,
                        InputType = x.FormCustomField.InputType,
                        Required = x.FormCustomField.Required,
                        SelectOptions = x.FormCustomField.SelectOptions
                    },
                    CustomValue = x.CustomValue,
                    FormCustomValueID = x.FormCustomValueID
                }).ToList(),
                ApprovalProcess = db_cr.ApprovalProcess.Select(x => new ApprovalProcessVM
                {
                    UserName = x.UserName,
                    Status = x.Status
                }).ToList()
            };

            //ViewBag.Users = new SelectList(GetApplicationUsers(creds.Subdomain), "UserName", "UserName");
            ViewBag.Users = GetApplicationUsers(creds.Subdomain);
            using (var db = new ApplicationDbContext())
            {
                ViewBag.Groups = db.Groups.ToList();
            }

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
                var users = UserManager.Users.Where(user => Subdomain_Users.Contains(user.UserName)).ToList();

                //users = users.Where(user => validUsernames.Any(username => username.ToLower() == user.UserName.ToLower()));
                return users;
            }

            //var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var users = UserManager.Users;

            //return users.ToList();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Manager,Forms")]
        public ActionResult EditCompletedForm(CompletedFormVM request)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            if (ModelState.IsValid)
            {
                var cf = db.Forms.Include(x => x.FormCustomValues)
                                 .Include(x => x.FormSignatureValues)
                                 .FirstOrDefault(x => x.FormID == request.FormID);

                if (cf != null)
                {
                    request.FormCustomValues.ForEach(obj =>
                    {
                        var ref_fv = cf.FormCustomValues.FirstOrDefault(x => x.FormCustomValueID == obj.FormCustomValueID);
                        if (ref_fv != null)
                        {
                            ref_fv.CustomValue = obj.CustomValue;
                        }
                    });

                    request.FormSignatureValues.ForEach(obj =>
                    {
                        var ref_fs = cf.FormSignatureValues.FirstOrDefault(x => x.FormSignatureValueID == obj.FormSignatureValueID);
                        if (ref_fs != null)
                        {
                            ref_fs.SignatureContent = obj.SignatureContent;
                        }
                    });

                    db.Entry(cf).State = EntityState.Modified;
                    db.SaveChanges();

                    if (!Directory.Exists(Server.MapPath(ImageTempDirectory)))
                    {
                        Directory.CreateDirectory(Server.MapPath(ImageTempDirectory));
                    }
                    var fileEntries = Directory.GetFiles(Server.MapPath(ImageTempDirectory)).Where(x => x.Contains(request.FormGuidID.ToString())).ToList();
                    foreach (string fileName in fileEntries)
                    {
                        var oFileInfo = new FileInfo(fileName);
                        var contentType = MimeMapping.GetMimeMapping(fileName);
                        var fileBytes = FileHelper.GetImageFileBytes(fileName); //ImageHelper.IsValidImage(x.ContentType) ? ImageHelper.CreateThumbnail(x.FileBytes, 48)
                        db.FormAttachments.Add(new FormAttachment
                        {
                            FormAttachmentID = Guid.NewGuid(),
                            FileBytes = ImageHelper.IsValidImage(contentType) ? ImageHelper.CreateThumbnail(fileBytes, 512) : fileBytes,
                            FileName = Path.GetFileName(oFileInfo.Name).Replace(request.FormGuidID.ToString() + "_", ""),
                            ContentType = contentType,
                            FormID = request.FormID
                        });
                    }
                    db.SaveChanges();
                    foreach (var fileName in fileEntries)
                    {
                        System.IO.File.Delete(fileName);
                    }
                    return RedirectToAction("EditCompletedForm", new { Id = cf.FormID });
                }                
            }

            ViewBag.Users = GetApplicationUsers(creds.Subdomain);
            using (var db = new ApplicationDbContext())
            {
                ViewBag.Groups = db.Groups.ToList();
            }

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;

            return View(request);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Manager,Forms")]
        public async Task<ActionResult> ViewCompletedForm(long Id)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            var db_cr = await db.Forms
                            .Include(cv => cv.FormCustomValues)
                            .Include(fs => fs.FormSignatureValues)
                            .Include(cs => cs.FormSettings)
                            .Include(cv => cv.FormCustomValues.Select(cf => cf.FormCustomField))
                            .Include(cv => cv.FormSignatureValues.Select(cf => cf.FormSignatureField))
                            .Include(c => c.FormAttachments)
                            .Include(s => s.ApprovalProcess)
                            .Where(x => x.FormID == Id)
                            .FirstOrDefaultAsync();

            var cr = new CompletedFormVM
            {
                FormSettings = db_cr.FormSettings,
                FormID = db_cr.FormID,
                FormCustomValues = db_cr.FormCustomValues.Select(x => new FormCustomValueVM
                {
                    FormCustomField = new FormCustomFieldVM
                    {
                        Label = x.FormCustomField.Label,
                        InputType = x.FormCustomField.InputType
                    },
                    CustomValue = x.CustomValue
                }).ToList(),
                FormSignatureValues = db_cr.FormSignatureValues.Select(x => new FormSignatureValueVM
                {
                    FormSignatureField = new FormSignatureFieldVM
                    {
                        Name = x.FormSignatureField.Name
                    },
                    SignatureContent = x.SignatureContent
                }).ToList(),
                FormAttachments = db_cr.FormAttachments.Select(x => new FormAttachmentVM
                {
                    FormAttachmentID = x.FormAttachmentID,
                    FormID = x.FormID,
                    FileName = x.FileName,
                    ContentType = x.ContentType
                }).ToList(),
                Submitted_At = db_cr.AuditDetail.CreatedDate,//.ToString("yyyy-MM-dd HH:mm:ss"),
                Submitted_By = db_cr.AuditDetail.CreatedEntryUserID,
                ApprovalStatus = db_cr.CurrentStatus,
                ApprovalProcess = db_cr.ApprovalProcess.Select(x => new ApprovalProcessVM
                {
                    UserName = x.UserName,
                    Status = x.Status,
                    LastModifiedDate = x.LastModifiedDate
                }).ToList()
            };

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            return View(cr);
        }

        public ActionResult CustomForms()
        {
            return View();
        }

        public async Task<JsonResult> GetJSON()
        {
            var sd = TokenProvider.GetProvider().GetSubdomain(User.Identity.Name);
            var f = await db.Forms
                .Include(fs => fs.FormSettings)
                .Include(ap => ap.ApprovalProcess)
                .Where(x=> x.FormSettings.SubDomain == sd)
                .ToListAsync();

            var detailCollection = f.Where(x => x.FormSettings != null)
            .Where(x => x.FormSettings.IsPublic ||
                        x.AuditDetail.CreatedEntryUserID == User.Identity.Name ||
                        User.IsInRole("Admin") || User.IsInRole("Manager"))
            .OrderByDescending(x => x.AuditDetail.CreatedDate).Select(x => new
            {
                Id = x.FormID,
                Title = x.FormSettings.Title,
                Status = x.CurrentStatus?.ToString() ?? "N/A",
                Submitted_By = x.AuditDetail.CreatedEntryUserID,
                Submitted_At = x.AuditDetail.CreatedDate.ToClientTime().ToString("yyyy/MM/dd @ HH:mm tt")
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

        private ApprovalStatus? GetApprovalStatus(List<ApprovalProcess> list)
        {
            if (!list.Any())
            {
                return null;
            }
            else if (list.All(x => x.Status == ApprovalStatus.Approved))
            {
                return ApprovalStatus.Approved;
            }
            else if (list.Any(x => x.Status == ApprovalStatus.Rejected))
            {
                return ApprovalStatus.Rejected;
            }
            else 
            {
                return ApprovalStatus.Pending;
            }
        }

        //[Authorize(Roles = "Admin,Manager,Forms")]
        public async Task<ActionResult> DeleteCompletedForm(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var form = await db.Forms.Include(x => x.FormSettings).Where(x => x.FormID == id).FirstOrDefaultAsync();
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }

        [HttpPost, ActionName("DeleteCompletedForm")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,Manager,Forms")]
        public async Task<ActionResult> DeleteCompletedFormConfirmed(long id)
        {
            var form = await db.Forms.FindAsync(id);
            db.Forms.Remove(form);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult CreateCustomForm()
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            ViewBag.BusinessName = creds.BusinessName;

            ViewBag.InputTypes = Enum.GetNames(typeof(Forms.Enum.InputType)).Select(x => new
            {
                key = (int)Enum.Parse(typeof(Forms.Enum.InputType), x),
                value = ((Forms.Enum.InputType)Enum.Parse(typeof(Forms.Enum.InputType), x)).GetDisplay()
            }).ToList();
            ViewBag.YesNo = Enum.GetNames(typeof(YesNo)).Select(x => new
            {
                key = (int)Enum.Parse(typeof(YesNo), x),
                value = ((YesNo)Enum.Parse(typeof(YesNo), x)).GetDisplay()
            }).ToList();

            ViewBag.Users = new MultiSelectList(GetApplicationUsers(creds.Subdomain), "UserName", "UserName");
            using (var db = new ApplicationDbContext())
            {
                ViewBag.Groups = new SelectList(db.Groups.ToList(), "Name", "Name");
            }

            return View(new FormSettingsVM
            {
                //Defaults   
                FormCustomFields = new List<FormCustomFieldVM>
                {
                    new FormCustomFieldVM()
                }
            });            
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> EditCustomForm(Guid Id)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            ViewBag.BusinessName = creds.BusinessName;

            ViewBag.InputTypes = Enum.GetNames(typeof(Forms.Enum.InputType)).Select(x => new
            {
                key = (int)Enum.Parse(typeof(Forms.Enum.InputType), x),
                value = ((Forms.Enum.InputType)Enum.Parse(typeof(Forms.Enum.InputType), x)).GetDisplay()
            }).ToList();
            ViewBag.YesNo = Enum.GetNames(typeof(YesNo)).Select(x => new
            {
                key = (int)Enum.Parse(typeof(YesNo), x),
                value = ((YesNo)Enum.Parse(typeof(YesNo), x)).GetDisplay()
            }).ToList();

            ViewBag.Users = new MultiSelectList(GetApplicationUsers(creds.Subdomain), "UserName", "UserName");
            using (var db = new ApplicationDbContext())
            {
                ViewBag.Groups = new SelectList(db.Groups.ToList(), "Name", "Name");
            }

            var fs = await db.GetFormSettings()
                            .Include(cf => cf.FormCustomFields)
                            .Include(c => c.FormSignatureFields)
                            .Where(x => x.FormSettingsID == Id)
                            .FirstOrDefaultAsync();

            if (fs == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(new FormSettingsVM
            {
                FormSettingsID = fs.FormSettingsID,
                Title = fs.Title,
                AccessLevel = fs.AccessLevel,
                Description = fs.Description,
                IsAttachmentEnabled = fs.IsAttachmentEnabled,
                IsPublic = fs.IsPublic,
                SendNotificationsTo = fs.SendNotificationsTo?.Split(',').ToList() ?? new List<string>(),
                AprrovalProcess = fs.ApprovalProcess?.Split(',').ToList() ?? new List<string>(),
                SubDomain = fs.SubDomain,
                FormCustomFields = fs.FormCustomFields.Select(ff => new FormCustomFieldVM
                {
                    FormCustomFieldID = ff.FormCustomFieldID,
                    Label = ff.Label,
                    InputType = ff.InputType,
                    Required = ff.Required,
                    SelectOptions = ff.SelectOptions
                }).ToList(),
                FormSignatureFields = fs.FormSignatureFields.Select(fsf => new FormSignatureFieldVM
                {
                    FormSignatureFieldID = fsf.FormSignatureFieldID,
                    Name = fsf.Name,
                    Required = fsf.Required
                }).ToList()
            });
        }
        //[Authorize(Roles = "Admin,Manager,Forms")]
        public async Task<JsonResult> GetFormsJSON()
        {
            var fs = await db.GetFormSettings()
                            .Include(cf => cf.FormCustomFields)
                            .Include(c => c.FormSignatureFields)
                            .ToListAsync();

            var user = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().Users
                        .Include(x => x.Groups).Include(x => x.Groups.Select(g => g.Group)).Where(x => x.UserName == User.Identity.Name).First();

            var inGroups = user.Groups.Select(x => x.Group.Name).ToList();

            var detailCollection = fs
            .Where(x => x.AccessLevel == "All Employees" ||
                        inGroups.Contains(x.AccessLevel) || 
                        User.IsInRole("Admin") || 
                        User.IsInRole("Manager"))
            .OrderByDescending(x => x.AuditDetail.CreatedDate)
            .Select(x => new
            {
                Id = x.FormSettingsID,
                Title = x.Title,
                AccessLevel = x.AccessLevel,
                Description = x.Description
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

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult CreateCustomForm(FormSettingsVM request)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            if (ModelState.IsValid)
            {
                var fs = new FormSettings
                {
                    Title = request.Title,
                    Description = request.Description,
                    AccessLevel = request.AccessLevel,
                    SendNotificationsTo = string.Join(",", request.SendNotificationsTo?.Where(x => !string.IsNullOrEmpty(x)) ?? new List<string>()),
                    IsAttachmentEnabled = request.IsAttachmentEnabled,  
                    IsPublic = request.IsPublic,
                    SubDomain = creds.Subdomain,
                    ApprovalProcess = string.Join(",", request.AprrovalProcess?.Where(x => !string.IsNullOrEmpty(x)) ?? new List<string>()),
                    FormCustomFields = request.FormCustomFields.Where(x => !x.IsDeleted).Select(x => new FormCustomField
                    {
                        Label = x.Label,
                        InputType = x.InputType.Value,
                        Required = x.Required ?? YesNo.No,
                        SelectOptions = x.SelectOptions
                    }).ToList(),
                    FormSignatureFields = request.FormSignatureFields.Where(x => !x.IsDeleted).Select(x => new FormSignatureField
                    {
                        Name = x.Name,
                        Required = x.Required.Value
                    }).ToList()
                };
                db.FormSettings.Add(fs);
                db.SaveChanges();
                
                return RedirectToAction("CustomForms");
            }

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            ViewBag.InputTypes = Enum.GetNames(typeof(Forms.Enum.InputType)).Select(x => new
            {
                key = (int)Enum.Parse(typeof(Forms.Enum.InputType), x),
                value = ((Forms.Enum.InputType)Enum.Parse(typeof(Forms.Enum.InputType), x)).GetDisplay()
            }).ToList();
            ViewBag.YesNo = Enum.GetNames(typeof(YesNo)).Select(x => new
            {
                key = (int)Enum.Parse(typeof(YesNo), x),
                value = ((YesNo)Enum.Parse(typeof(YesNo), x)).GetDisplay()
            }).ToList();

            ViewBag.Users = new MultiSelectList(GetApplicationUsers(creds.Subdomain), "UserName", "UserName");
            using (var db = new ApplicationDbContext())
            {
                ViewBag.Groups = new SelectList(db.Groups.ToList(), "Name", "Name");
            }

            return View(request);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult EditCustomForm(FormSettingsVM request)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            if (ModelState.IsValid)
            {
                var fs = db.GetFormSettings()
                            .Include(cf => cf.FormCustomFields)
                            .Include(csf => csf.FormSignatureFields)
                            .Where(f => f.FormSettingsID == request.FormSettingsID)
                            .FirstOrDefault();

                if (fs == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                fs.Title = request.Title;
                fs.AccessLevel = request.AccessLevel;
                fs.Description = request.Description;
                fs.SendNotificationsTo = string.Join(",", request.SendNotificationsTo?.Where(x => !string.IsNullOrEmpty(x)) ?? new List<string>());
                fs.ApprovalProcess = string.Join(",", request.AprrovalProcess?.Where(x => !string.IsNullOrEmpty(x)) ?? new List<string>());
                fs.IsAttachmentEnabled = request.IsAttachmentEnabled;
                fs.IsPublic = request.IsPublic;

                var deletedCustomFields = request.FormCustomFields.Where(x => x.IsDeleted).ToList();
                var deletedSignatureFields = request.FormSignatureFields.Where(x => x.IsDeleted).ToList();

                foreach (var deletedField in deletedCustomFields)
                {
                    if (deletedField.FormCustomFieldID.HasValue)
                        fs.FormCustomFields.Remove(fs.FormCustomFields.FirstOrDefault(x => x.FormCustomFieldID == deletedField.FormCustomFieldID));
                    request.FormCustomFields.Remove(deletedField);
                }

                foreach (var deletedField in deletedSignatureFields)
                {
                    if (deletedField.FormSignatureFieldID.HasValue)
                        fs.FormSignatureFields.Remove(fs.FormSignatureFields.FirstOrDefault(x => x.FormSignatureFieldID == deletedField.FormSignatureFieldID));
                    request.FormSignatureFields.Remove(deletedField);
                }

                foreach (var field in request.FormCustomFields)
                {
                    var ref_field = fs.FormCustomFields.FirstOrDefault(x => x.FormCustomFieldID == field.FormCustomFieldID);
                    if (ref_field != null)
                    {
                        ref_field.Label = field.Label;
                        ref_field.InputType = field.InputType.Value;
                        ref_field.Required = field.Required ?? YesNo.No;
                        ref_field.SelectOptions = field.SelectOptions;
                    }
                    else
                    {
                        fs.FormCustomFields.Add(new FormCustomField
                        {
                            Label = field.Label,
                            InputType = field.InputType.Value,
                            Required = field.Required ?? YesNo.No,
                            SelectOptions = field.SelectOptions
                        });
                    }
                }

                foreach (var field in request.FormSignatureFields)
                {
                    var ref_field = fs.FormSignatureFields.FirstOrDefault(x => x.FormSignatureFieldID == field.FormSignatureFieldID);
                    if (ref_field != null)
                    {
                        ref_field.Name = field.Name;
                        ref_field.Required = field.Required.Value;
                    }
                    else
                    {
                        fs.FormSignatureFields.Add(new FormSignatureField
                        {
                            Name = field.Name,
                            Required = field.Required.Value
                        });
                    }
                }

                db.Entry(fs).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("EditCustomForm", new { id = fs.FormSettingsID });
            }

            ViewBag.Subdomain = creds.Subdomain;
            ViewBag.AccessCode = creds.AccessToken;
            ViewBag.InputTypes = Enum.GetNames(typeof(Forms.Enum.InputType)).Select(x => new
            {
                key = (int)Enum.Parse(typeof(Forms.Enum.InputType), x),
                value = ((Forms.Enum.InputType)Enum.Parse(typeof(Forms.Enum.InputType), x)).GetDisplay()
            }).ToList();
            ViewBag.YesNo = Enum.GetNames(typeof(YesNo)).Select(x => new
            {
                key = (int)Enum.Parse(typeof(YesNo), x),
                value = ((YesNo)Enum.Parse(typeof(YesNo), x)).GetDisplay()
            }).ToList();

            ViewBag.Users = new MultiSelectList(GetApplicationUsers(creds.Subdomain), "UserName", "UserName");
            using (var db = new ApplicationDbContext())
            {
                ViewBag.Groups = new SelectList(db.Groups.ToList(), "Name", "Name");
            }

            return View(request);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Manager,Forms")]
        public ActionResult StageImage(CompletedFormVM request)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
            if (ModelState.IsValid)
            {
                if (request.File != null)
                {
                    // if path not exists create
                    if (!Directory.Exists(Server.MapPath(ImageTempDirectory)))
                    {
                        Directory.CreateDirectory(Server.MapPath(ImageTempDirectory));
                    }
                    string yourPath = $"{ImageTempDirectory}/{request.FormGuidID.ToString()}_{request.File.FileName}";
                    request.File.SaveAs(HttpContext.Server.MapPath(yourPath));
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        //[Authorize(Roles = "Admin,Manager,Forms")]
        public async Task<JsonResult> DeleteAttachment(Guid? id)
        {
            var att = await db.FormAttachments.FindAsync(id);
            if (att != null)
            {
                att.FileBytes = null;
                db.Entry(att).State = EntityState.Modified;
                await db.SaveChangesAsync();
                db.FormAttachments.Remove(att);
                await db.SaveChangesAsync();
            }
            return Json("Deleted", JsonRequestBehavior.AllowGet);
        }
        
        //[Authorize(Roles = "Admin,Manager,Forms")]
        public JsonResult RemoveAttachment(string fname)
        {
            if (!Directory.Exists(Server.MapPath(ImageTempDirectory)))
            {
                Directory.CreateDirectory(Server.MapPath(ImageTempDirectory));
            }
            var fileEntries = Directory.GetFiles(Server.MapPath(ImageTempDirectory)).Where(x => x.ToLower().EndsWith(fname.ToLower())).ToList();
            foreach (var fileName in fileEntries)
            {
                System.IO.File.Delete(fileName);
            }

            return Json("Deleted", JsonRequestBehavior.AllowGet);
        }

        //[Authorize(Roles = "Admin,Manager,Forms")]
        public ActionResult GetFormAttachment(Guid id)
        {
            var att = db.FormAttachments.Find(id);
            if (att != null)
            {
                return File(att.FileBytes, att.ContentType, att.FileName);
            }
            return null;
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> DeleteCustomForm(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var customForm = await db.FormSettings.FindAsync(id);
            if (customForm == null)
            {
                return HttpNotFound();
            }
            return View(customForm);
        }

        [HttpPost, ActionName("DeleteCustomForm")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> DeleteCustomFormConfirmed(Guid id)
        {
            var customForm = await db.FormSettings.FindAsync(id);
            db.FormSettings.Remove(customForm);
            await db.SaveChangesAsync();
            return RedirectToAction("CustomForms");
        }

        [Authorize(Roles = "Admin,Manager,Forms")]
        public async Task<JsonResult> StatusChange(long? FormID, ApprovalStatus status)
        {
            var approval_process = await db.ApprovalProcess.Where(x => x.FormID == FormID).ToListAsync();
            var approval = approval_process.First(x => x.UserName == User.Identity.Name);
            var prev_status = approval.Status;
            approval.Status = status;
            if (prev_status != status)
            {
                approval.LastModifiedDate = DateTime.Now;
            }
            db.Entry(approval).State = EntityState.Modified;
            db.SaveChanges();
            var final_status = GetApprovalStatus(approval_process);
            if (prev_status != status && final_status != ApprovalStatus.Pending)
            {
                //if (final_status != ApprovalStatus.Pending)
                //{

                //}
                //set email to createby
                try
                {
                    var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);
                    var fr = await db.Forms.Include(fs => fs.FormSettings).FirstOrDefaultAsync(x => x.FormID == FormID);
                    var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

                    var user = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().Users
                                .Where(x => x.UserName == fr.AuditDetail.CreatedEntryUserID).First();
                    //construct body
                    var send_template = TemplateRepo.GetInstance().GetTemplate("Content/templates/form_status_template.html");

                    send_template = send_template.Replace("form-title", fr.FormSettings.Title)
                                        .Replace("business-name", creds.BusinessName)
                                        .Replace("replied-by", User.Identity.Name)
                                        .Replace("form-status", "Status: " + final_status.ToString())
                                        .Replace("form-link", $"{baseUrl}Forms/ViewCompletedForm/{fr.FormID}");

                    var mails = new List<EmailMessage>() {
                        new EmailMessage()
                        {
                            SenderName = creds.BusinessName,
                            Body = send_template,
                            Subject = $"{fr.FormSettings.Title} response status",
                            RecipientName = fr.AuditDetail.CreatedEntryUserID,
                            Recipient = user.Email
                        }
                    };
                    using (EmailHandler eh = new EmailHandler(mails, creds.Subdomain))
                        eh.Send();

                }
                catch { }
            }

            return Json("Changed", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> PrintPDF(long Id)
        {
            var creds = TokenProvider.GetProvider().GetAccessDetails(User.Identity.Name);

            var db_cr = await db.Forms
                            .Include(cv => cv.FormCustomValues)
                            .Include(fs => fs.FormSignatureValues)
                            .Include(cs => cs.FormSettings)
                            .Include(cv => cv.FormCustomValues.Select(cf => cf.FormCustomField))
                            .Include(cv => cv.FormSignatureValues.Select(cf => cf.FormSignatureField))
                            .Include(c => c.FormAttachments)
                            .Include(s => s.ApprovalProcess)
                            .Where(x => x.FormID == Id)
                            .FirstOrDefaultAsync();

            var cr = new CompletedFormVM
            {
                FormSettings = db_cr.FormSettings,
                FormID = db_cr.FormID,
                FormCustomValues = db_cr.FormCustomValues.Select(x => new FormCustomValueVM
                {
                    FormCustomField = new FormCustomFieldVM
                    {
                        Label = x.FormCustomField.Label,
                        InputType = x.FormCustomField.InputType
                    },
                    CustomValue = x.CustomValue
                }).ToList(),
                FormSignatureValues = db_cr.FormSignatureValues.Select(x => new FormSignatureValueVM
                {
                    FormSignatureField = new FormSignatureFieldVM
                    {
                        Name = x.FormSignatureField.Name
                    },
                    SignatureContent = x.SignatureContent
                }).ToList(),
                FormAttachments = db_cr.FormAttachments.Where(x => db_cr.FormSettings.IsAttachmentEnabled).Select(x => new FormAttachmentVM
                {
                    FormAttachmentID = x.FormAttachmentID,
                    FileName = x.FileName,
                    ContentType = x.ContentType,
                    FileSize = x.FileBytes.LongLength.GetSizeInMemory(),
                    Base64 = ImageHelper.IsValidImage(x.ContentType) ? ImageHelper.CreateThumbnail(x.FileBytes, 128)?.ToBase64(x.ContentType) : null
                }).ToList(),
                Submitted_At = db_cr.AuditDetail.CreatedDate,//.ToString("yyyy-MM-dd HH:mm:ss"),
                Submitted_By = db_cr.AuditDetail.CreatedEntryUserID,
                ApprovalStatus = db_cr.CurrentStatus,
                ApprovalProcess = db_cr.ApprovalProcess.Select(x => new ApprovalProcessVM
                {
                    UserName = x.UserName,
                    Status = x.Status,
                    LastModifiedDate = x.LastModifiedDate
                }).ToList()
            };

            var HTMLViewStr = RenderViewToString(ControllerContext, "~/Views/Forms/CompletedFormPrint.cshtml", cr, creds.BusinessName);

            using(var stream = new MemoryStream())
            {
                using (var pdfDoc = new Document(PageSize.A4, 10f, 10f, 50f, 0f))
                {
                    var writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                    tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
                    tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor()); // use our new processor

                    CssFilesImpl cssFiles = new CssFilesImpl();
                    cssFiles.Add(XMLWorkerHelper.GetInstance().GetDefaultCSS());
                    var cssResolver = new StyleAttrCSSResolver(cssFiles);
                    cssResolver.AddCss(@"code { padding: 2px 4px; }", "utf-8", true);
                    var charset = Encoding.UTF8;
                    var hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
                    hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // inject the tagProcessors
                    var htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(pdfDoc, writer));
                    var pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                    var worker = new XMLWorker(pipeline, true);
                    var xmlParser = new XMLParser(true, worker, charset);
                    xmlParser.Parse(new StringReader(HTMLViewStr));

                    pdfDoc.Close();
                    return File(stream.ToArray(), "application/pdf", $"{cr.FormSettings.Title} Form - {DateTime.Today.ToClientTime().ToString("yyyyMMdd")} - {Id}.pdf");
                }
            }
        }

        public class CustomImageTagProcessor : iTextSharp.tool.xml.html.Image
        {
            public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
            {
                IDictionary<string, string> attributes = tag.Attributes;
                string src;
                if (!attributes.TryGetValue(HTML.Attribute.SRC, out src))
                    return new List<IElement>(1);

                if (string.IsNullOrEmpty(src))
                    return new List<IElement>(1);

                if (src.StartsWith("data:image/", StringComparison.InvariantCultureIgnoreCase))
                {
                    // data:[<MIME-type>][;charset=<encoding>][;base64],<data>
                    var base64Data = src.Substring(src.IndexOf(",") + 1);
                    var imagedata = Convert.FromBase64String(base64Data);
                    var image = iTextSharp.text.Image.GetInstance(imagedata);

                    var list = new List<IElement>();
                    var htmlPipelineContext = GetHtmlPipelineContext(ctx);
                    list.Add(GetCssAppliers().Apply(new Chunk((iTextSharp.text.Image)GetCssAppliers().Apply(image, tag, htmlPipelineContext), 0, 0, true), tag, htmlPipelineContext));
                    return list;
                }
                else
                {
                    return base.End(ctx, tag, currentContent);
                }
            }
        }

        protected string RenderViewToString(ControllerContext context, string viewName, object model, string BusinessName = null)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);
            var tempData = new TempDataDictionary
            {
                { "AbsolutePath", Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/" }
            };
            if (!string.IsNullOrEmpty(BusinessName))
            {
                tempData.Add("BusinessName", BusinessName);
            }

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}