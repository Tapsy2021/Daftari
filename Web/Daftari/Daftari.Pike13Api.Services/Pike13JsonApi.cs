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
    internal class Pike13JsonApi : IDisposable
    {
        private Pike13ApiAuth _auth;

        private StringContent _content;

        public Pike13JsonApi(Pike13ApiAuth auth)

        {
            _auth = auth;
            request = new Pike13APIRequest();
        }

        public string EndPoint { get; set; }
        public Pike13APIRequest request { get; set; }
        public string Type { get; set; } = "queries";
        public UriBuilder URL { get; set; }

        public void Dispose()
        {
            _auth = null;
        }

        public async Task<Pike13Report> GetReportAsync()
        {
            bool hasMore = true;
            Pike13Report report = null;
            while (hasMore)
            {
                var response = await postReponseAsync();

                var apiResponse = JsonConvert.DeserializeObject<Pike13APIResponse>(await response.Content.ReadAsStringAsync());
                if (report == null)
                {
                    report = new Pike13Report
                    {
                        rows = apiResponse.data.attributes.rows,
                        fields = apiResponse.data.attributes.fields
                    };
                    hasMore = apiResponse.data.attributes.has_more;
                    request.attributes.page.starting_after = apiResponse.data.attributes.last_key;
                }
                else
                {
                    report.rows.AddRange(apiResponse.data.attributes.rows);
                    hasMore = apiResponse.data.attributes.has_more;
                    request.attributes.page.starting_after = apiResponse.data.attributes.last_key;
                }
            }
            return report;
        }

        protected async Task<HttpResponseMessage> postReponseAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, queryBuilder());
            request.Content = _content;
            request.Content.Headers.ContentType.MediaType = "application/vnd.api+json";
            request.Content.Headers.ContentType.CharSet = "UTF-8";

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth.AccessToken);

            var response = await httpClient.SendAsync(request);
            //response.EnsureSuccessStatusCode();
            return response;
        }

        private string queryBuilder()
        {
            if (EndPoint == null)
            {
                throw new Exception("No Query Added");
            }

            if (URL == null)
            {
                URL = new UriBuilder(string.Format("{0}/desk/api/v3/reports/{1}/{2}?access_token={3}", _auth.Host, EndPoint, Type, _auth.AccessToken));
            }
            request.type = Type;
            //request.clientid = _auth.ClientID;
            var req = new Pike13RequestWrapper();
            req.data = request;
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            var json = JsonConvert.SerializeObject(req, settings);
            _content = new StringContent(json, Encoding.UTF8, "application/vnd.api+json");

            return URL.ToString();
        }

        private class Pike13RequestWrapper
        {
            public Pike13APIRequest data { get; set; }
        }
    }

}
