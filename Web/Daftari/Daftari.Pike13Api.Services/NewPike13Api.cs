using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Daftari.Pike13Api.Services
{
    internal class NewPike13Api<T> : IDisposable
        where T : class
    {
        private Pike13ApiAuth _auth;

        private HttpContent _content;

        public NewPike13Api(Pike13ApiAuth auth)

        {
            _auth = auth;
            //request = new Pike13APIRequest();
        }

        public string EndPoint { get; set; }
        public object request { get; set; }
        public string Type { get; set; } = "queries";
        public UriBuilder URL { get; set; }

        public Dictionary<string, string> Params { get; set; }

        public Pike13APIMode Pike13APIMode { get; set; } = Pike13APIMode.Desk;

        public string OverrideEndpoint { get; set; }

        public void Dispose()
        {
            _auth = null;
        }

        public async Task<List<T>> PutAsync()
        {
            var response = await putReponseAsync();
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new ResponseContractResolver(OverrideEndpoint ?? EndPoint);
            //var me = await response.Content.ReadAsStringAsync();
            var jResponse = JsonConvert.DeserializeObject<FhqCoreApIResponse<T>>(await response.Content.ReadAsStringAsync(), settings);
            return jResponse.Data;
        }

        protected async Task<HttpResponseMessage> putReponseAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Put, bodyBuilder());
            request.Content = _content;
            request.Content.Headers.ContentType.MediaType = "application/json"; // not sure if neccesary
            request.Content.Headers.ContentType.CharSet = "UTF-8";

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var client = new HttpClient();

            var response = await client.SendAsync(request);
            //response.EnsureSuccessStatusCode();
            return response;
        }

        private string geturltemplate()
        {
            switch (Pike13APIMode)
            {
                case Pike13APIMode.Desk:
                    return "{0}/api/v2/desk/{1}?access_token={2}";

                case Pike13APIMode.Front:
                    return "{0}/api/v2/front/{1}";
                    
                default:
                    throw new NotImplementedException();
            }
        }

        private string bodyBuilder()
        {
            if (URL == null)
            {
                //"{0}/api/v2/desk/{1}"
                URL = new UriBuilder(string.Format(geturltemplate(), _auth.Host, EndPoint, _auth.AccessToken));
            }

            //var req = new
            //{
            //    data = request
            //};
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(request, settings);
            _content = new StringContent(json, Encoding.UTF8, "application/vnd.api+json");

            return URL.ToString();
        }

        private Uri queryBuilder(bool isParamsAttached)
        {
            if (EndPoint == null)
            {
                throw new Exception("No Query Added");
            }

            if (URL == null)
            {
                URL = new UriBuilder(string.Format(geturltemplate(), _auth.Host, EndPoint));
            }

            var parameters = new Dictionary<string, string>(Params);
            //if (IsClientIDNeeded)
            //{
            //    parameters.Add("client_id", _auth.ClientID);
            //}
            //if (IsPagingQuery)
            //{
            //    parameters.Add("per_page", "100");
            //    parameters.Add("page", CurrentPage.ToString());
            //}
            parameters.Add("access_token", _auth.AccessToken);

            if (isParamsAttached)
            {
                var query = HttpUtility.ParseQueryString(URL.Query);
                foreach (var param in parameters)
                {
                    query.Add(param.Key, param.Value);
                }
                URL.Query = query.ToString();
            }
            else
            {
                _content = new FormUrlEncodedContent(parameters);
            }

            return URL.Uri;
        }

        //public async Task<Pike13Report> GetReportAsync()
        //{
        //    bool hasMore = true;
        //    Pike13Report report = null;
        //    while (hasMore)
        //    {
        //        var response = await postReponseAsync();

        //        var apiResponse = JsonConvert.DeserializeObject<Pike13APIResponse>(await response.Content.ReadAsStringAsync());
        //        if (report == null)
        //        {
        //            report = new Pike13Report
        //            {
        //                rows = apiResponse.data.attributes.rows,
        //                fields = apiResponse.data.attributes.fields
        //            };
        //            hasMore = apiResponse.data.attributes.has_more;
        //            request.attributes.page.starting_after = apiResponse.data.attributes.last_key;
        //        }
        //        else
        //        {
        //            report.rows.AddRange(apiResponse.data.attributes.rows);
        //            hasMore = apiResponse.data.attributes.has_more;
        //            request.attributes.page.starting_after = apiResponse.data.attributes.last_key;
        //        }
        //    }
        //    return report;
        //}

        //protected async Task<HttpResponseMessage> postReponseAsync()
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Post, queryBuilder());
        //    request.Content = _content;
        //    request.Content.Headers.ContentType.MediaType = "application/vnd.api+json";
        //    request.Content.Headers.ContentType.CharSet = "UTF-8";

        //    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Authorization =
        //    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth.AccessToken);

        //    var response = await httpClient.SendAsync(request);
        //    //response.EnsureSuccessStatusCode();
        //    return response;
        //}

        //private string queryBuilder()
        //{
        //    if (EndPoint == null)
        //    {
        //        throw new Exception("No Query Added");
        //    }

        //    if (URL == null)
        //    {
        //        URL = new UriBuilder(string.Format("{0}/desk/api/v3/reports/{1}/{2}?access_token={3}", _auth.Host, EndPoint, Type, _auth.AccessToken));
        //    }
        //    request.type = Type;
        //    //request.clientid = _auth.ClientID;
        //    var req = new Pike13RequestWrapper();
        //    req.data = request;
        //    var settings = new JsonSerializerSettings();
        //    settings.NullValueHandling = NullValueHandling.Ignore;
        //    var json = JsonConvert.SerializeObject(req, settings);
        //    _content = new StringContent(json, Encoding.UTF8, "application/vnd.api+json");

        //    return URL.ToString();
        //}

        //private class Pike13RequestWrapper
        //{
        //    public Pike13APIRequest data { get; set; }
        //}
    }

}
