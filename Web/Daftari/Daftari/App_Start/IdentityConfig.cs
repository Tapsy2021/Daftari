using Daftari.SMSHandling;
using Daftari.SMSHandling.Enums;
using LukeApps.AspIdentity;
using LukeApps.EmailHandling;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Daftari
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var mail = new EmailMessage();
            mail.Recipient = message.Destination;
            mail.RecipientName = message.Destination.Split('@')[0];
            mail.Body = message.Body;
            mail.Subject = message.Subject;
            using (EmailHandler eh = new EmailHandler(mail))
                return eh.SendAsync();
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            SMSEngine sms = new SMSEngine(message.Destination);

            return sms.SendAsync(new SMSMessage()
            {
                Language = Language.English,
                Message = message.Body,
                Mobiles = new System.Collections.Generic.List<Mobile> { new Mobile { Number = message.Destination } }
            });
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 5,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public void AddUserToGroup(string userId, string groupName)
        {
            using(var _db = new ApplicationDbContext())
            {
                var group = _db.Groups.Where(x => x.Name == groupName).FirstOrDefault();
                var user = _db.Users.Find(userId);

                var userGroup = new ApplicationUserGroup
                {
                    Group = group,
                    GroupId = group.Id,
                    User = user,
                    UserId = user.Id
                };

                user.Groups.Add(userGroup);
                _db.SaveChanges();
            }
        }
    
        public async Task<int> AddToGroupAsync(string userId, string groupName)
        {
            using (var _db = new ApplicationDbContext())
            {
                var group = _db.Groups.Where(x => x.Name == groupName).FirstOrDefault();
                var user = _db.Users.Find(userId);

                var userGroup = new ApplicationUserGroup
                {
                    Group = group,
                    GroupId = group.Id,
                    User = user,
                    UserId = user.Id
                };

                user.Groups.Add(userGroup);
                return await _db.SaveChangesAsync();
            }
        }

        public async Task<IList<string>> GetGroupAsync(string userId)
        {
            using (var _db = new ApplicationDbContext())
            {
                var groups = await _db.UserGroups.Include(x => x.Group).Where(x => x.UserId == userId).ToListAsync();
                return groups.Select(x => x.Group.Name).Distinct().ToList();
            }
        }

        public async Task<int> RemoveFromGroupsAsync(string userId, params string[] groups)
        {
            using (var _db = new ApplicationDbContext())
            {
                var groupsToDelete = _db.UserGroups.Include(g => g.Group).Where(x => x.UserId == userId && groups.Contains(x.Group.Name)).ToList();
                foreach(var group in groupsToDelete)
                {
                    _db.UserGroups.Remove(group);
                }

                return await _db.SaveChangesAsync();
            }
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}