using Daftari.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daftari.Services.Database
{
    public class AuthHandler : BaseHandler
    {
        public Task<User> GetUserAsync()
        {
            return Connection.Table<User>().Where(x => x.Is_Active).FirstOrDefaultAsync();
        }

        public Task<int> SaveUserAsync(User user)
        {
            SignOut();

            return Connection.InsertAsync(user);
        }

        public Task<int> SignOut()
        {
            return Connection.DeleteAllAsync<User>();
            //var exists = Connection.Table<User>().Where(x => x.Is_Active).ToListAsync().Result;
            //foreach (var obj in exists)
            //{
            //    obj.Is_Active = false;
            //}
            //if (exists.Any())
            //{
            //    await Connection.UpdateAllAsync(exists);
            //}
            //if ((Xamarin.Forms.Application.Current as App).Settings != null)
            //    (Xamarin.Forms.Application.Current as App).Settings.Thumbnail_Image = null;
        }
    }
}
