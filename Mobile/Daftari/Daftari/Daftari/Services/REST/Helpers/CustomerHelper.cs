using Daftari.Models;
using Daftari.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Daftari.Services.REST.Helpers
{
    public class CustomerHelper : BaseHelper
    {
        public static async Task<List<Customer>> GetDependantsAsync(CancellationTokenSource ct_Token)
        {
            try
            {
                return await new RestService().GetListAsync<Customer>((Application.Current as App), Constants.URLs.Get_Dependants, ct_Token.Token);
            }
            catch { }
            return null;
        }
    }
}
