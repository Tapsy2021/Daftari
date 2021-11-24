using Daftari.Pike13Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Daftari.Pike13Api.Services
{
    public class ResponseContractResolver : DefaultContractResolver
    {
        public ResponseContractResolver(string endpoint)
        {
            this.PropertyMappings = new Dictionary<string, string>
        {
            {"Data", endpoint},
        };
        }

        private Dictionary<string, string> PropertyMappings { get; set; }

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName = null;
            var resolved = this.PropertyMappings.TryGetValue(propertyName, out resolvedName);
            return (resolved) ? resolvedName : base.ResolvePropertyName(propertyName);
        }
    }

    internal class Pike13API<T> : Pike13ApiBase
    where T : class
    {
        public object request { get; set; }
        public Pike13API(Pike13ApiAuth auth)
            : base(auth)
        {

        }

        public async Task<List<T>> GetAllDataAsync(bool isPagingQuery = false)
        {
            IsPagingQuery = isPagingQuery;

            if (isPagingQuery)
            {
                return await getAllPagedDataAsync();
            }
            else
            {
                return await getAllDataAsync();
            }
        }

        public async Task<ICoreApiResponse<T>> GetDataByPageAsync(int page = 1)
        {
            IsPagingQuery = true;
            return await getDataByPageAsync(page);
        }

        //public async Task<List<T>> PutAsync()
        //{
        //    HttpResponseMessage response = await putReponseAsync();
        //    var settings = new JsonSerializerSettings();
        //    settings.ContractResolver = new ResponseContractResolver(OverrideEndpoint ?? EndPoint);
        //    var jResponse = JsonConvert.DeserializeObject<FhqCoreApIResponse<T>>(await response.Content.ReadAsStringAsync(), settings);
        //    return jResponse.Data;
        //}

        private async Task<List<T>> getAllDataAsync()
        {
            HttpResponseMessage response = await getReponseAsync();
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new ResponseContractResolver(OverrideEndpoint ?? EndPoint);
            var jResponse = JsonConvert.DeserializeObject<FhqCoreApIResponse<T>>(await response.Content.ReadAsStringAsync(), settings);
            return jResponse.Data;
        }

        private async Task<List<T>> getAllPagedDataAsync()
        {
            int totalPages = 1;
            List<T> _data = new List<T>();
            for (int i = 1; i <= totalPages; i++)
            {
                if (i == 1)
                {
                    var a = await getDataByPageAsync(1);

                    if (a.total_count == 0)
                        throw new Exception("No new updates");

                    _data.AddRange(a.Data);
                    totalPages = DivideRoundingUp(a.total_count, a.Data.Count);
                }
                else
                {
                    _data.AddRange((await getDataByPageAsync(i)).Data);
                }
                Thread.Sleep(1000);
            }
            return _data;
        }

        private async Task<ICoreApiResponse<T>> getDataByPageAsync(int page)
        {
            CurrentPage = page;
            HttpResponseMessage response = await getReponseAsync();
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new ResponseContractResolver(OverrideEndpoint ?? EndPoint);
            return JsonConvert.DeserializeObject<FhqCoreApIResponse<T>>(await response.Content.ReadAsStringAsync(), settings);
        }

    }

    internal class Pike13ApiBase : IDisposable
    {
        private Pike13ApiAuth _auth;

        //protected HttpClient client = new HttpClient();

        private HttpContent _content;

        public Pike13ApiBase(Pike13ApiAuth auth)
        {
            _auth = auth;
            Params = new Dictionary<string, string>();
        }

        public string EndPoint { get; set; }
        public Pike13APIMode Pike13APIMode { get; set; } = Pike13APIMode.Desk;
        public bool IsClientIDNeeded { get; set; }

        /// <summary>
        /// Override key word in response. By Default, Endpoint is used.
        /// </summary>
        public string OverrideEndpoint { get; set; }

        public Dictionary<string, string> Params { get; set; }
        protected int CurrentPage { get; set; }
        protected bool IsPagingQuery { get; set; }

        public void Dispose()
        {
            //client.Dispose();
            _auth = null;
            client.Dispose();
        }

        protected static int DivideRoundingUp(int x, int y)
        {
            // TODO: Define behaviour for negative numbers
            int remainder;
            int quotient = Math.DivRem(x, y, out remainder);
            return remainder == 0 ? quotient : quotient + 1;
        }

        private HttpClient client = new HttpClient();

        protected async Task<HttpResponseMessage> getReponseAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = queryBuilder(true)
            };
            //request.Content.Headers.ContentType.CharSet = "UTF-8";
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return response;
        }

        protected async Task<HttpResponseMessage> postReponseAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = queryBuilder(true),
                Content = _content
            };
            request.Content.Headers.ContentType.CharSet = "UTF-8";

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return response;
        }

        //protected async Task<HttpResponseMessage> putReponseAsync()
        //{
        //    HttpRequestMessage request = new HttpRequestMessage
        //    {
        //        Method = HttpMethod.Put,
        //        RequestUri = queryBuilder()                
        //    };
        //    request.Content = _content;
        //    request.Content.Headers.ContentType.MediaType = "application/vnd.api+json"; // not sure if neccesary
        //    request.Content.Headers.ContentType.CharSet = "UTF-8";

        //    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        //    var response = await client.SendAsync(request);
        //    response.EnsureSuccessStatusCode();
        //    return response;
        //}

        private string geturltemplate()
        {
            switch (Pike13APIMode)
            {
                case Pike13APIMode.Desk:
                    return "{0}/api/v2/desk/{1}";

                case Pike13APIMode.Front:
                    return "{0}/api/v2/desk/{1}";
                    ;
                default:
                    throw new NotImplementedException();
            }
        }

        private Uri queryBuilder(bool isParamsAttached)
        {
            if (EndPoint == null)
            {
                throw new Exception("No Query Added");
            }
            var parameters = new Dictionary<string, string>(Params);
            if (IsClientIDNeeded)
            {
                parameters.Add("client_id", _auth.ClientID);
            }
            if (IsPagingQuery)
            {
                parameters.Add("per_page", "100");
                parameters.Add("page", CurrentPage.ToString());
            }
            parameters.Add("access_token", _auth.AccessToken);

            UriBuilder url = new UriBuilder(string.Format(geturltemplate(), _auth.Host, EndPoint));

            if (isParamsAttached)
            {
                var query = HttpUtility.ParseQueryString(url.Query);
                foreach (var param in parameters)
                {
                    query.Add(param.Key, param.Value);
                }
                url.Query = query.ToString();
            }
            else
            {
                _content = new FormUrlEncodedContent(parameters);
            }

            return url.Uri;
        }

        //private string queryBuilder()
        //{
        //    var req = new Pike13RequestWrapper
        //    {
        //        data = request
        //    };
        //    var settings = new JsonSerializerSettings
        //    {
        //        NullValueHandling = NullValueHandling.Ignore
        //    };
        //    var json = JsonConvert.SerializeObject(req, settings);
        //    _content = new StringContent(json, Encoding.UTF8, "application/vnd.api+json");

        //    return URL.ToString();
        //}

        private class Pike13RequestWrapper
        {
            public Pike13APIRequest data { get; set; }
        }
    }

    public class FhqCoreApIResponse<T> : ICoreApiResponse<T>
           where T : class
    {
        public List<T> Data { get; set; }

        public string next { get; set; }

        public string prev { get; set; }

        public int total_count { get; set; }
    }

    public class Pike13APIPagination
    {
        public int limit { get; set; } = 100;
        public string starting_after { get; set; }
    }

    public class Pike13APIQuery
    {
        public Pike13APIQuery()
        {
            fields = new List<string>();
            page = new Pike13APIPagination();
        }

        public List<string> fields { get; set; }
        public List<object> filter { get; set; }
        public Pike13APIPagination page { get; set; }
        public List<string> sort { get; set; }
        public string target { get; set; }
        public string topic { get; set; }
    }

    public class Pike13APIRequest
    {
        public Pike13APIRequest()
        {
            attributes = new Pike13APIQuery();
        }

        public Pike13APIQuery attributes { get; set; }

        [JsonProperty("id")]
        public string clientid { get; set; }

        public string type { get; set; }

    }

    public class Pike13APIResponse
    {
        public Pike13APIResponse()
        {
            data = new Pike13ReportPageData();
        }

        public Pike13ReportPageData data { get; set; }
        public object meta { get; set; }
    }

    public class Pike13Field
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Pike13Report
    {
        public Pike13Report()
        {
            fields = new List<Pike13Field>();
        }

        public List<Pike13Field> fields { get; set; }
        public List<string[]> rows { get; set; }
    }

    public class Pike13ReportPage
    {
        public Pike13ReportPage()
        {
            fields = new List<Pike13Field>();
        }

        public double duration { get; set; }
        public List<Pike13Field> fields { get; set; }
        public bool has_more { get; set; }
        public string last_key { get; set; }
        public List<string[]> rows { get; set; }
    }

    public class Pike13ReportPageData
    {
        public Pike13ReportPageData()
        {
            attributes = new Pike13ReportPage();
        }

        public Pike13ReportPage attributes { get; set; }
    }

}
