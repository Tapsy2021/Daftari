using Daftari.Pike13Api.DAL;
using Daftari.Pike13Api.Models;
using Daftari.Pike13Api.ViewModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Daftari.Pike13Api.Services
{
    public class TokenProvider
    {
        private static readonly TokenProvider instance =
                  new TokenProvider();

        public static TokenProvider GetProvider()
        {
            return instance;
        }

        private Dictionary<string, AccountPeople> userDict;
        private Dictionary<string, string> subDomainDict;
        //private Dictionary<long, string> businessIdDict;
        //private Dictionary<long, string> businessSubdomainDict;
        private IEnumerable<AccountPeople> AccountPeoples;

        // Note: constructor is 'private'
        private TokenProvider()
        {
            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                var users = db.Tokens.Include(t => t.People)
                    .OrderByDescending(t => t.RefreshTime)
                    .AsEnumerable()
                    .GroupBy(t => t.Username)
                    .Select(t => t.First().People.Where(p => p.IsActive).First())
                    .ToList();

                // Load list of available users
                userDict = users?.ToDictionary(u => u.Username);

                AccountPeoples = db.AccountPeoples.Include(a => a.Token).ToList();

                subDomainDict = AccountPeoples.GroupBy(a => a.Subdomain).Select(a => new { Subdomain = a.Key, a.FirstOrDefault(p => p.Username.Contains("Daftari"))?.Token.AccessToken })
                                .ToDictionary(a => a.Subdomain, b => b.AccessToken);

                //businessIdDict = subDomains.GroupBy(a => a.BusinessID).Select(a => new { BusinessID = a.Key, a.FirstOrDefault(p => p.Username.Contains("Daftari"))?.Token.AccessToken })
                //                .Where(a => !string.IsNullOrEmpty(a.AccessToken))
                //                .ToDictionary(a => a.BusinessID, b => b.AccessToken);

                //businessSubdomainDict = subDomains.Select(a => new { a.BusinessID, a.Subdomain })
                //                .ToDictionary(a => a.BusinessID, b => b.Subdomain);


                //subDomainDict = db.AccountPeoples.Include(a => a.Token).GroupBy(a => a.Subdomain).AsEnumerable()
                //    .Select(a => new { Subdomain = a.Key, a.FirstOrDefault(p => p.Username.Contains("Daftari"))?.Token.AccessToken })
                //    .ToDictionary(a => a.Subdomain, b => b.AccessToken);
            }
        }

        public void ClearDictionaries()
        {
            userDict.Clear();
        }

        public void AddToDictionary(AccountPeople user)
        {
            userDict.Add(user.Username, user);
        }

        public PikeTokenVm GetAccessDetails(string username)
        {
            var ap = getUserInfo(username);
            if (ap == null)
            {
                return new PikeTokenVm();
            }

            return new PikeTokenVm
            {
                PersonName = ap.PersonName,
                Photo = ap.Photo,
                AccessToken = ap.AccessToken,
                Subdomain = ap.Subdomain,
                Role = ap.Role,
                ID = ap.PersonID,
                BusinessName = ap.BusinessName,
            };
        }

        public string GetStaffAccessCode(string subdomain)
        {
            return subDomainDict[subdomain];
        }

        public string GetSubdomain(string username)
        {
            var ap = getUserInfo(username);
            return ap?.Subdomain;
        }

        public List<string> GetSubdomains()
        {
            return subDomainDict.Select(x => x.Key).ToList();
        }

        public string GetAccessCode(string username)
        {
            var ap = getUserInfo(username);
            return ap?.AccessToken;
        }

        public string GetAccessCode(long business_id)
        {
            return AccountPeoples.GroupBy(a => a.BusinessID).Select(a => new { BusinessID = a.Key, a.FirstOrDefault(p => p.Username.Contains("Daftari"))?.Token.AccessToken })
                                .Where(a => !string.IsNullOrEmpty(a.AccessToken))
                                .ToDictionary(a => a.BusinessID, b => b.AccessToken)[business_id];
            //return businessIdDict[business_id];
        }

        public long GetBusinessId(string subdomain)
        {
            return AccountPeoples.GroupBy(x => x.Subdomain)
                    .Select(a => new { Subdomain = a.Key, BusinessID = a.Max(x => x.BusinessID) })
                   .ToDictionary(a => a.Subdomain, b => b.BusinessID)[subdomain];
        }

        public string GetSubdomain(long business_id)
        {
            return AccountPeoples.GroupBy(x => x.BusinessID)
                    .Select(a => new { BusinessID = a.Key, Subdomain = a.Max(x => x.Subdomain) })
                   .ToDictionary(a => a.BusinessID, b => b.Subdomain)[business_id];
        }

        public bool IsRegistered(string username)
        {
            return getUserInfo(username) != null;
        }

        public AccountPeople GetUserData(string username)
        {
            AccountPeople user = getUserInfo(username);

            if (user == null)
                user = new AccountPeople();

            return user;
        }

        public int AddToken(Token token)
        {
            userDict.Remove(token.Username);
            using (Pike13ApiContext db = new Pike13ApiContext())
            {
                var tokensToDelete = db.Tokens.Where(t => t.Username == token.Username).ToList();
                db.Tokens.RemoveRange(tokensToDelete);
                db.Tokens.Add(token);
                var pp = token.People.FirstOrDefault(p => p.IsActive);
                pp.Token = token;
                AddToDictionary(pp);
                return db.SaveChanges();
            }
        }

        public int ActivateAccountPerson(string username, long id)
        {
            userDict.Remove(username);
            using (Pike13ApiContext at = new Pike13ApiContext())
            {
                var userToken = at.Tokens.Include(t => t.People).Where(t => t.Username == username).OrderByDescending(t => t.RefreshTime).First();
                var pps = userToken.People.Where(p => p.AccountPeopleID != id).ToList();
                pps.Select(p => { p.IsActive = false; return p; }).ToList().ForEach(p => at.Entry(p));

                var pp = at.AccountPeoples.Find(id);
                pp.IsActive = true;
                at.Entry(pp);

                AddToDictionary(pp);
                return at.SaveChanges();
            }
        }

        private AccountPeople getUserInfo(string username)
        {
            AccountPeople user = null;
            if (username != null)
                userDict?.TryGetValue(username, out user);

            return user;
        }
    }
}