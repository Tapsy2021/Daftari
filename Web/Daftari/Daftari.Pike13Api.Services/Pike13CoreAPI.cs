using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Daftari.Pike13Api.Services
{
    /// <summary>
    /// Based on Pike13 Core API (v2)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Pike13CoreAPI<T> : IDisposable
        where T : class
    {
        private Pike13ApiAuth _auth;

        private HttpContent _content;
        private HttpClient _client;
        /// <summary>
        /// EndPoint of the Request
        /// </summary>
        public string EndPoint { get; set; }
        /// <summary>
        /// Request Body
        /// </summary>
        public object Request { get; set; }
        public string Type { get; set; } = "queries";
        public UriBuilder URL { get; set; }

        public Dictionary<string, string> Params { get; set; }

        public Pike13APIMode Pike13APIMode { get; set; } = Pike13APIMode.Desk;

        public string OverrideEndpoint { get; set; }
        protected bool IsPagingQuery { get; set; }
        protected int CurrentPage { get; set; }
        public bool IsBearerRequired { get; set; }

        public Pike13CoreAPI(Pike13ApiAuth auth)

        {
            _auth = auth;
            _client = new HttpClient();
            Params = new Dictionary<string, string>();
        }

        public void Dispose()
        {
            _auth = null;
            _client.Dispose();
        }

        public async Task<List<T>> DeleteAsync()
        {
            return await ReturnResponse(HttpMethod.Delete);
        }

        public async Task<List<T>> PutAsync()
        {
            return await ReturnResponse(HttpMethod.Put);
        }

        public async Task<List<T>> PostAsync()
        {
            return await ReturnResponse(HttpMethod.Post);
        }
        
        public async Task<List<T>> GetAllDataAsync(bool isPagingQuery = false)
        {
            IsPagingQuery = isPagingQuery;

            if (isPagingQuery)
            {
                return await ReturnPagedResponse();
            }
            else
            {
                return await ReturnResponse(HttpMethod.Get);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        private async Task<List<T>> ReturnPagedResponse()
        {
            int totalPages = 1;
            List<T> _data = new List<T>();
            for (int i = 1; i <= totalPages; i++)
            {
                if (i == 1)
                {
                    var a = await ReturnResponse(1);

                    if (a.total_count == 0)
                        throw new Exception("No new updates");

                    _data.AddRange(a.Data);
                    totalPages = DivideRoundingUp(a.total_count, a.Data.Count);
                }
                else
                {
                    _data.AddRange((await ReturnResponse(i)).Data);
                }
                Thread.Sleep(1000);
            }
            return _data;
        }

        protected static int DivideRoundingUp(int x, int y)
        {
            // TODO: Define behaviour for negative numbers
            int quotient = Math.DivRem(x, y, out int remainder);
            return remainder == 0 ? quotient : quotient + 1;
        }
        //////////////////////////////////////////////////////////////////////////////

        protected async Task<List<T>> ReturnResponse(HttpMethod httpMethod)
        {
            var response = await GetResponseMessage(httpMethod);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new ResponseContractResolver(OverrideEndpoint ?? EndPoint)
            };

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new List<T>();
            }

            var jResponse = JsonConvert.DeserializeObject<FhqCoreApIResponse<T>>(await response.Content.ReadAsStringAsync(), settings);
            return jResponse.Data;
        }

        protected async Task<ICoreApiResponse<T>> ReturnResponse(int page = 0)
        {
            CurrentPage = page;
            var response = await GetResponseMessage(HttpMethod.Get);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new ResponseContractResolver(OverrideEndpoint ?? EndPoint)
            };
            return JsonConvert.DeserializeObject<FhqCoreApIResponse<T>>(await response.Content.ReadAsStringAsync(), settings);
        }

        protected async Task<HttpResponseMessage> GetResponseMessage(HttpMethod httpMethod)
        {
            var uri = httpMethod != HttpMethod.Get ? BodyBuilder() : QueryBuilder(true);

            var request = new HttpRequestMessage(httpMethod, uri);

            if (httpMethod != HttpMethod.Get)
            {
                request.Content = _content;
                request.Content.Headers.ContentType.MediaType = "application/json";
                request.Content.Headers.ContentType.CharSet = "UTF-8";
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return response;
        }

        private Uri BodyBuilder()
        {
            if (EndPoint == null)
            {
                throw new Exception("No Query Added");
            }

            if (URL == null)
            {
                URL = new UriBuilder(string.Format("{0}/api/v2/{1}/{2}?access_token={3}", _auth.Host, Pike13APIMode.ToString().ToLower(), EndPoint, _auth.AccessToken));
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(Request, settings);
            _content = new StringContent(json, Encoding.UTF8, "application/vnd.api+json");

            if (IsBearerRequired)
            {
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth.AccessToken);
            }

            return URL.Uri;
        }

        private Uri QueryBuilder(bool isParamsAttached)
        {
            if (EndPoint == null)
            {
                throw new Exception("No Query Added");
            }

            if (URL == null)
            {
                URL = new UriBuilder(string.Format("{0}/api/v2/{1}/{2}", _auth.Host, Pike13APIMode.ToString().ToLower(), EndPoint));
            }

            var parameters = new Dictionary<string, string>(Params);
            //if (IsClientIDNeeded)
            //{
            //    parameters.Add("client_id", _auth.ClientID);
            //}
            if (IsPagingQuery)
            {
                parameters.Add("per_page", "100");
                parameters.Add("page", CurrentPage.ToString());
            }
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
