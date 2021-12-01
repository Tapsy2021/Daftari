using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Daftari.Pike13Api.Services
{
    //internal class Pike13HooksApi : IDisposable
    //{
    //    private Pike13ApiAuth _auth;

    //    private StringContent _content;
    //    public Pike13HooksApi(Pike13ApiAuth auth)
    //    {
    //        _auth = auth;
    //        request = new Pike13APIRequest();
    //        // not required for this purpose
    //        request.attributes.fields = null;
    //        request.attributes.page = null;
    //    }
    //    //public string EndPoint { get; set; }
    //    public Pike13APIRequest request { get; set; }
    //    public string Type { get; set; } = "webhooks";
    //    public UriBuilder URL { get; set; }

    //    public void Dispose()
    //    {
    //        _auth = null;
    //    }

    //    public async Task<Pike13WebhooksResponse> GetReportAsync()
    //    {
    //        var response = await postReponseAsync();
    //        var apiResponse = JsonConvert.DeserializeObject<Pike13WebhooksResponse>(await response.Content.ReadAsStringAsync());

    //        return apiResponse;
    //    }

    //    protected async Task<HttpResponseMessage> postReponseAsync()
    //    {
    //        var request = new HttpRequestMessage(HttpMethod.Post, queryBuilder());
    //        request.Content = _content;
    //        request.Content.Headers.ContentType.MediaType = "application/vnd.api+json";
    //        request.Content.Headers.ContentType.CharSet = "UTF-8";

    //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
    //        var httpClient = new HttpClient();

    //        var response = await httpClient.SendAsync(request);
    //        response.EnsureSuccessStatusCode();
    //        return response;
    //    }

    //    private string queryBuilder()
    //    {
    //        if (URL == null)
    //        {
    //            URL = new UriBuilder(string.Format("{0}/api/v3/{1}?access_token={2}", _auth.Host, Type, _auth.AccessToken));
    //        }
    //        request.type = Type;

    //        var req = new Pike13RequestWrapper
    //        {
    //            data = request
    //        };
    //        var settings = new JsonSerializerSettings
    //        {
    //            NullValueHandling = NullValueHandling.Ignore
    //        };
    //        var json = JsonConvert.SerializeObject(req, settings);
    //        _content = new StringContent(json, Encoding.UTF8, "application/vnd.api+json");

    //        return URL.ToString();
    //    }

    //    private class Pike13RequestWrapper
    //    {
    //        public Pike13APIRequest data { get; set; }
    //    }
    //}

    public class Pike13WebhooksResponse
    {
        public Pike13WebhooksResponse()
        {
            data = new Pike13WebhooksData();
        }

        public Pike13WebhooksData data { get; set; }
    }

    public class Pike13WebhooksData
    {
        public Pike13WebhooksData()
        {
            attributes = new Pike13WebhooksAttrib();
        }

        public long? id { get; set; }
        public string type { get; set; }
        public Pike13WebhooksLinks links { get; set; }
        /*"links": {
          "self": "https://subdomain.pike13.com/api/v3/webhooks/1"
        },*/
        public Pike13WebhooksAttrib attributes { get; set; }
    }

    public class Pike13WebhooksAttrib
    {
        //public Pike13WebhooksAttrib()
        //{

        //}

        public long? account_id { get; set; }
        public long? business_id { get; set; }
        public string topic { get; set; }
        public string target { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class Pike13WebhooksLinks
    {
        public string self { get; set; }
    }
}
