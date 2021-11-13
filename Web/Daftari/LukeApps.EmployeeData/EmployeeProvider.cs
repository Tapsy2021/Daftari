using LukeApps.AspIdentity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace LukeApps.EmployeeData
{
    public class EmployeeProvider
    {
        private static readonly EmployeeProvider instance =
                  new EmployeeProvider();

        public static EmployeeProvider GetEmployeeProvider()
        {
            return instance;
        }

        // private List<TebADUser> _users;
        private Dictionary<string, Employee> userDict;

        private Dictionary<string, Employee> alluserDict;

        // Note: constructor is 'private'
        //private EmployeeProvider()
        //{
        //    domainName = ConfigurationManager.AppSettings["AuthDomain"];
        //    using (EmployeeDataEntities db = new EmployeeDataEntities())
        //    {
        //        var users = db.Employees.ToList();
        //        // Load list of available users
        //        userDict = users
        //            .Where(u => u.UserStatus == UserStatus.Active)
        //            .ToDictionary(u => u.Username);

        //        alluserDict = users
        //            .ToDictionary(u => u.Username);
        //    }
        //}

        private EmployeeProvider()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var users = db.Users.ToList();
                // Load list of available users
                userDict = users
                    .Select(u => new Employee()
                    {
                        Username = u.UserName,
                        DisplayName = $"{u.FirstName} {u.LastName}",
                        ElectronicSignature = u.ESignature,
                        JobTitle = u.JobTitle,
                        TelephoneNumber = u.TelephoneNumber,
                        Initials = u.Initials,
                        CompanyMail = u.Email,
                        UserStatus = UserStatus.Active
                    })
                    .ToDictionary(u => u.Username);

                alluserDict = users
                    .Select(u => new Employee()
                    {
                        Username = u.UserName,
                        DisplayName = $"{u.FirstName} {u.LastName}",
                        ElectronicSignature = u.ESignature,
                        JobTitle = u.JobTitle,
                        TelephoneNumber = u.TelephoneNumber,
                        Initials = u.Initials,
                        CompanyMail = u.Email,
                        UserStatus = UserStatus.Active
                    })
                    .ToDictionary(u => u.Username);
            }
        }

        public List<Employee> Users => userDict.Values.OrderBy(v => v.DisplayName).ToList();

        public List<Employee> OldUsers => alluserDict.Values.Where(u => u.UserStatus != UserStatus.Active).ToList();

        public List<Employee> AllUsers => alluserDict.Values.OrderBy(v => v.DisplayName).ToList();

        public void ClearDictionaries()
        {
            userDict.Clear();
            alluserDict.Clear();
        }

        public void AddToDictionary(Employee user)
        {
            alluserDict.Add(user.Username, user);
            if (user.UserStatus == UserStatus.Active)
                userDict.Add(user.Username, user);
        }

        public IEnumerable<Employee> GetUsers(params string[] EmployeeNumbers)
        {
            foreach (var no in EmployeeNumbers)
            {
                userDict.TryGetValue(no, out Employee user);
                if (user != null)
                    yield return (user);
            }
        }

        public string GetDisplayName(string sAMAccountName)
        {
            Employee user = getUserInfo(sAMAccountName);

            if (user == null)
                return "Employee " + (sAMAccountName ?? "") + " not in database";
            else
                return (user.UserStatus != UserStatus.Active ? $"{user.DisplayName} ({user.UserStatus})" : user.DisplayName);
        }

        public string GetEmployeeNumberAndDisplayName(string sAMAccountName)
        {
            Employee user = getUserInfo(sAMAccountName);

            return user == null ? "Employee " + (sAMAccountName ?? "-") + " not in database" : user.Summary;
        }

        public Employee GetUserData(string sAMAccountName)
        {
            Employee user = getUserInfo(sAMAccountName);

            if (user == null)
                user = new Employee();

            return user;
        }

        public bool SaveElectronicSignature(string username, string dataURL)
        {
            username = cleanUsername(username);
            var user = userDict[username];
            user.ElectronicSignature = dataURL;
            return EditUser(user) > 0;
        }

        public string GetElectronicSignature(string sAMAccountName)
        {
            Employee user = getUserInfo(sAMAccountName);

            return user?.ElectronicSignature;
        }

        public int AddUser(Employee tebADUser)
        {
            return AddUsers(new List<Employee>() { tebADUser });
        }

        public int AddUsers(List<Employee> ADUsers)
        {
            using (EmployeeDataEntities db = new EmployeeDataEntities())
            {
                foreach (var aDUser in ADUsers)
                {
                    db.Employees.Add(aDUser);
                    AddToDictionary(aDUser);
                }
                return db.SaveChanges();
            }
        }

        public int EditUser(Employee tebADUser)
        {
            return EditUsers(new List<Employee>() { tebADUser });
        }

        public int EditUsers(List<Employee> ADUsers)
        {
            using (EmployeeDataEntities db = new EmployeeDataEntities())
            {
                var usernames = ADUsers.Select(t => t.Username).ToArray();
                var dbUsers = db.Employees.Where(t => usernames.Contains(t.Username)).ToDictionary(t => t.Username);
                foreach (var aDUser in ADUsers)
                {
                    var preTebADUser = dbUsers[aDUser.Username];
                    userDict.Remove(aDUser.Username);
                    alluserDict.Remove(aDUser.Username);

                    preTebADUser.Initials = string.IsNullOrWhiteSpace(aDUser.Initials) ? preTebADUser.Initials : aDUser.Initials;
                    preTebADUser.DisplayName = aDUser.DisplayName;
                    preTebADUser.CompanyMail = aDUser.CompanyMail;
                    preTebADUser.CompanyMail2 = aDUser.CompanyMail2;
                    preTebADUser.TelephoneNumber = aDUser.TelephoneNumber;
                    preTebADUser.DisplayPicture = aDUser.DisplayPicture;
                    preTebADUser.ElectronicSignature = aDUser.ElectronicSignature;
                    preTebADUser.OfficialDepartment = aDUser.OfficialDepartment;
                    preTebADUser.DepartmentOverride = aDUser.DepartmentOverride;
                    preTebADUser.JobTitle = aDUser.JobTitle;
                    preTebADUser.Location = aDUser.Location;
                    preTebADUser.UserStatus = aDUser.UserStatus;

                    db.Entry(preTebADUser).State = EntityState.Modified;
                    AddToDictionary(preTebADUser);
                }
                return db.SaveChanges();
            }
        }

        public int ChangeUserStatus(Employee tebADUser, UserStatus status)
        {
            return ChangeUserStatus(new List<Employee>() { tebADUser }, status);
        }

        public int ChangeUserStatus(List<Employee> ADUsers, UserStatus status)
        {
            using (EmployeeDataEntities db = new EmployeeDataEntities())
            {
                foreach (var aDUser in ADUsers)
                {
                    var PreTebADUser = db.Employees.Where(t => t.Username == aDUser.Username).FirstOrDefault();
                    userDict.Remove(aDUser.Username);
                    alluserDict.Remove(aDUser.Username);
                    PreTebADUser.UserStatus = status;

                    db.Entry(PreTebADUser).State = EntityState.Modified;
                    AddToDictionary(PreTebADUser);
                }
                return db.SaveChanges();
            }
        }

        private Employee getUserInfo(string sAMAccountName)
        {
            Employee user = null;
            if (sAMAccountName != null)
                alluserDict.TryGetValue(cleanUsername(sAMAccountName), out user);

            return user;
        }

        private string cleanUsername(string user)
        {
            //if (user.Length < domainName.Length || user.Substring(0, domainName.Length) != domainName)
            return user;
            //else
            //return user.Substring(domainName.Length);
        }
    }
}