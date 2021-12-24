using Daftari.Models;
using Daftari.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Daftari.Services.REST.Helpers
{
    public class Pike13AccessHelper : BaseHelper
    {
        public static async Task<List<Visit>> GetVisitsAsync(object param, CancellationTokenSource ct_Token)
        {
            var queryString = GetQueryString(param);
            try
            {
                return await new RestService().GetListAsync<Visit>((Application.Current as App), Constants.URLs.Get_Visits + queryString, ct_Token.Token);
            }
            catch { }
            return null;
        }
    }
}
