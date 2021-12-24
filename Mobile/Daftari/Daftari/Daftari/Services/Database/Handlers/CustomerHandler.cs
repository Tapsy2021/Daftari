using Daftari.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Daftari.Services.Database.Handlers
{
    public class CustomerHandler : BaseHandler
    {
        public Task<List<Customer>> GetDependantsAsync()
        {
            return Connection.Table<Customer>().ToListAsync();
        }

        public void SaveDependants(List<Customer> dependants)
        {
            Connection.Table<Customer>().DeleteAsync();
            dependants.ForEach(obj =>
            {
                obj.UpdatedDate = DateTime.Now;
                Connection.InsertAsync(obj);
            });
        }
    }
}
