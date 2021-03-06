using Daftari.Models;
using Daftari.Services.Database.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Daftari.Services.Database
{
    public class DbHelper
    {
        private static DbHelper instance;

        public bool Init()
        {
            try
            {

            }
            catch (FormatException) { }

            return true;
        }

        public static DbHelper Instance
        {
            get
            {
                if (instance == null)
                    instance = new DbHelper();
                return instance;
            }
        }

        public Task<User> GetUser()
        {
            return new AuthHandler().GetUserAsync();
        }

        public Task<int> SaveUser(User user)
        {
            return new AuthHandler().SaveUserAsync(user);
        }

        public void SignOut()
        {
            new AuthHandler().SignOut();
        }

        public Task<List<Customer>> GetDependants()
        {
            return new CustomerHandler().GetDependantsAsync();
        }

        public void SaveDependants(List<Customer> dependants)
        {
            new CustomerHandler().SaveDependants(dependants);
        }
    }
}
