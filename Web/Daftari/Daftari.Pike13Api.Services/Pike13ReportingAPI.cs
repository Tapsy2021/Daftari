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
    internal class Pike13ReportingAPI<T> : IDisposable
        where T : class
    {
        private Pike13ApiAuth _auth;

        private StringContent _content;
        private HttpClient _client;

        public Pike13ReportingAPI(Pike13ApiAuth auth)

        {
            _auth = auth;
            _client = new HttpClient();
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
                var response = await ReturnReponseAsync();

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

        public async Task<T> PostReportAsync()
        {
            var response = await ReturnReponseAsync();
            var apiResponse = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());

            return apiResponse;
        }

        protected async Task<HttpResponseMessage> ReturnReponseAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, queryBuilder());
            request.Content = _content;
            request.Content.Headers.ContentType.MediaType = "application/vnd.api+json";
            request.Content.Headers.ContentType.CharSet = "UTF-8";

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            //var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Authorization =
            //new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth.AccessToken);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
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
              //URL = new UriBuilder(string.Format("{0}/api/v3/{1}?access_token={2}", _auth.Host, Type, _auth.AccessToken));
                URL = new UriBuilder(string.Format("{0}/desk/api/v3/reports/{1}/{2}?access_token={3}", _auth.Host, EndPoint, Type, _auth.AccessToken));
            }
            request.type = Type;
          
            var req = new
            {
                data = request
            };
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(req, settings);
            _content = new StringContent(json, Encoding.UTF8, "application/vnd.api+json");

            return URL.ToString();
        }
    }

}
