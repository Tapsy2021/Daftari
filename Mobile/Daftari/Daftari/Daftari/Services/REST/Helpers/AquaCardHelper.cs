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
    public class AquaCardHelper
    {
        public static async Task<List<StudentCard>> GetCardsAsync(CancellationTokenSource ct_Token)
        {
            try
            {
                return await new RestService().GetListAsync<StudentCard>((Application.Current as App), Constants.URLs.Get_Cards, ct_Token.Token);
            }
            catch { }
            return null;
        }
    }
}
