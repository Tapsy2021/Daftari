using System.Security.Principal;

namespace LukeApps.EmployeeData
{
    public static class UserExtensions
    {
        public static string GetDisplayName(this IPrincipal user) =>
             EmployeeProvider.GetEmployeeProvider().GetDisplayName(user.Identity.Name);

        public static Employee GetUserData(this IPrincipal user) =>
             EmployeeProvider.GetEmployeeProvider().GetUserData(user.Identity.Name);

        public static string GetUserDept(this IPrincipal user) =>
            user.GetUserData()?.DepartmentNumber;
    }
}