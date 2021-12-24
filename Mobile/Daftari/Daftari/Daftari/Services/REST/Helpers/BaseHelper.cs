using System.Linq;
using System.Web;

namespace Daftari.Services.REST.Helpers
{
    public class BaseHelper
    {
        protected static string GetQueryString(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            if (!properties.Any())
            {
                return "";
            }
            return "?" + string.Join("&", properties.ToArray());
        }
    }
}
