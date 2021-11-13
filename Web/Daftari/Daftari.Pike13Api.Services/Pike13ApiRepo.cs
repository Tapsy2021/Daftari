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
    internal enum Pike13APIMode
    {
        Desk,
        Front
    }

    public interface ICoreApiResponse<T>
        where T : class
    {
        List<T> Data { get; set; }
        string next { get; set; }
        string prev { get; set; }
        int total_count { get; set; }
    }

    public class FhqCoreApIResponse<T> : ICoreApiResponse<T>
           where T : class
    {
        public List<T> Data { get; set; }

        public string next { get; set; }

        public string prev { get; set; }

        public int total_count { get; set; }
    }

    public class Pike13ApiRepo
    {
        private Pike13ApiAuth _auth;

        public Pike13ApiRepo(string username)
        {
            _auth = new Pike13ApiAuth(username);
        }

        public Pike13ApiRepo(long BusinessID)
        {
            _auth = new Pike13ApiAuth(BusinessID);
        }

        public async Task<Pike13Report> GetClientsAsync()
        {
            using (var api = new Pike13JsonApi(_auth))
            {
                api.EndPoint = "clients";
                api.request.attributes.fields.AddRange(new List<string>
                {
                    "account_claim_date",
                    "account_credit_amount",
                    "account_manager_emails",
                    "account_manager_names",
                    "account_manager_phones",
                    "address",
                    "age",
                    "also_staff",
                    "birthdate",
                    "business_id",
                    "business_name",
                    "business_subdomain",
                    "client_since_date",
                    "completed_visits",
                    "currency_code",
                    "current_plan_revenue_category",
                    "current_plan_types",
                    "current_plans",
                    "custom_fields",
                    "days_since_last_visit",
                    "days_until_birthday",
                    "dependent_names",
                    "email",
                    "first_name",
                    "first_visit_date",
                    "franchise_id",
                    "full_name",
                    "future_visits",
                    "guardian_email",
                    "guardian_name",
                    "has_membership",
                    "has_payment_on_file",
                    "has_plan_on_hold",
                    "has_signed_waiver",
                    "home_location_name",
                    "is_schedulable",
                    "key",
                    "last_email_bounced",
                    "last_invoice_amount",
                    "last_invoice_date",
                    "last_invoice_id",
                    "last_invoice_unpaid",
                    "last_membership_end_date",
                    "last_name",
                    "last_signed_waiver_name",
                    "last_site_access_date",
                    "last_visit_date",
                    "last_visit_id",
                    "last_visit_service",
                    "middle_name",
                    "net_paid_amount",
                    "next_pass_plan_end_date",
                    "person_id",
                    "person_state",
                    "phone",
                    "primary_staff_name",
                    "revenue_amount",
                    "source_name",
                    "staff_member_who_added",
                    "tenure",
                    "tenure_group",
                    "unpaid_visits",
                });

                //var startFmtTime = StartTime.Date.ToString("yyyy-MM-dd HH\\:mm\\:ss");
                //var endFmtTime = EndTime.Date.AddHours(24).AddSeconds(-1).ToString("yyyy-MM-dd HH\\:mm\\:ss");
                //api.request.attributes.filter = new List<object>()
                //{
                //    new string[] { "btw", "transaction_at", startFmtTime, endFmtTime }
                //};

                return await api.GetReportAsync();
            }
        }

        public async Task<Pike13Report> GetEnrollmentsAsync(DateTime Date)
        {
            using (var api = new Pike13JsonApi(_auth))
            {
                api.EndPoint = "enrollments";
                api.request.attributes.fields.AddRange(new List<string>
                {
                    "person_id",
                    "full_name",
                    "service_id",
                    "service_category",
                    "service_name",
                    "start_at",
                    "end_at",
                    "instructor_names",
                });

                var StartTime = Date.Date.ToString("yyyy-MM-dd HH\\:mm\\:ss");
                var EndTime = Date.Date.AddHours(24).AddSeconds(-1).ToString("yyyy-MM-dd HH\\:mm\\:ss");
                api.request.attributes.filter = new List<object>()
                {
                    "and",
                    new string[][] {
                        new string[] { "ne", "service_id", "141059" },
                        new string[] { "ne", "service_id", "141061" },
                        new string[] { "gt", "start_at", StartTime },
                        new string[] { "lt", "end_at", EndTime}
                    }
                };

                return await api.GetReportAsync();
            }
        }

        public async Task<List<Pike13Event>> GetEventOccurenceAsync(DateTime StartTime, DateTime EndTime, long? Staff_Mmember_Ids = null, string dateType = null, string dateSince = null)
        {
            using (var api = new Pike13API<Pike13Event>(_auth))
            {
                api.EndPoint = "event_occurrences";
                api.IsClientIDNeeded = true;
                api.Params.Add("from", string.Concat(StartTime.ToString("s"), "Z"));
                api.Params.Add("to", string.Concat(EndTime.ToString("s"), "Z"));
                //api.Params.Add("from", string.Concat(StartTime.ToUniversalTime().ToString("s"), "Z"));
                //api.Params.Add("to", string.Concat(EndTime.ToUniversalTime().ToString("s"), "Z"));
                if (Staff_Mmember_Ids.HasValue)
                {
                    api.Params.Add("staff_member_ids", Staff_Mmember_Ids.ToString());
                }

                return await api.GetAllDataAsync();
            }
        }

        public async Task<List<Pike13Visit>> GetVisitsAsync(long id)
        {
            using (var api = new Pike13API<Pike13Visit>(_auth))
            {
                api.EndPoint = $"event_occurrences/{id}/visits";
                api.OverrideEndpoint = "visits";
                //api.IsClientIDNeeded = true;

                return await api.GetAllDataAsync();
            }
        }

        public async Task<List<Pike13Event>> GetEventOccurrenceByIdAsync(long? id)
        {
            using (var api = new Pike13API<Pike13Event>(_auth))
            {
                api.EndPoint = $"event_occurrences/{id}";
                api.OverrideEndpoint = "event_occurrences";
                return await api.GetAllDataAsync();
            }
        }

        public async Task<Pike13Report> GetEventOccurenceByStaffAsync(DateTime StartTime, DateTime EndTime)
        {
            using (var api = new Pike13JsonApi(_auth))
            {
                api.EndPoint = "event_occurrence_staff_members";
                api.request.attributes.fields.AddRange(new List<string>
                {
                    "full_name",
                    "service_id",
                    "service_category",
                    "service_name",
                    "start_at",
                    "end_at",
                    "registered_enrollment_count",
                    "paid_count",
                    "duration_in_minutes",
                });

                api.request.attributes.filter = new List<object>()
                {
                    "and",
                    new string[][] {
                        new string[] { "ne", "service_id", "141059" },
                        new string[] { "ne", "service_id", "141061" },
                        new string[] { "gt", "start_at", StartTime.ToString("yyyy-MM-dd HH\\:mm\\:ss") },
                        new string[] { "lt", "end_at", EndTime.ToString("yyyy-MM-dd HH\\:mm\\:ss") }
                    }
                };
                return await api.GetReportAsync();
            }
        }

        public async Task<Pike13Report> GetInvoiceItemTransactionsAsync(DateTime StartTime, DateTime EndTime)
        {
            using (var api = new Pike13JsonApi(_auth))
            {
                api.EndPoint = "invoice_item_transactions";
                api.request.attributes.fields.AddRange(new List<string>
                {
                    "business_id",
                    "business_name",
                    "business_subdomain",
                    "commission_recipient_name",
                    "created_by_name",
                    "credit_card_name",
                    "currency_code",
                    "error_message",
                    "external_payment_name",
                    "failed_at",
                    "failed_date",
                    "franchise_id",
                    "grants_membership",
                    "invoice_autobill",
                    "invoice_due_date",
                    "invoice_id",
                    "invoice_item_id",
                    "invoice_number",
                    "invoice_payer_home_location",
                    "invoice_payer_id",
                    "invoice_payer_name",
                    "invoice_payer_primary_staff_name_at_sale",
                    "invoice_state",
                    "key",
                    "net_paid_amount",
                    "net_paid_revenue_amount",
                    "net_paid_tax_amount",
                    "payment_method",
                    "payment_transaction_id",
                    "payments_amount",
                    "plan_id",
                    "processing_method",
                    "processor_transaction_id",
                    "product_id",
                    "product_name",
                    "product_name_at_sale",
                    "product_type",
                    "refunds_amount",
                    "revenue_category",
                    "sale_location_name",
                    "transaction_amount",
                    "transaction_at",
                    "transaction_autopay",
                    "transaction_date",
                    "transaction_id",
                    "transaction_state",
                    "transaction_type",
                    "voided_at",
                });

                var startFmtTime = StartTime.Date.ToString("yyyy-MM-dd HH\\:mm\\:ss");
                var endFmtTime = EndTime.Date.AddHours(24).AddSeconds(-1).ToString("yyyy-MM-dd HH\\:mm\\:ss");
                api.request.attributes.filter = new List<object>()
                {
                    new string[] { "btw", "transaction_at", startFmtTime, endFmtTime }
                };

                return await api.GetReportAsync();
            }
        }

        public async Task<List<Pike13Person>> GetPeopleAsync(string dateType = null, string dateSince = null)
        {
            using (var api = new Pike13API<Pike13Person>(_auth))
            {
                api.EndPoint = "people";

                if (dateSince != null)
                    api.Params.Add(dateType, dateSince);

                //api.Params.Add("is_member", "true");
                api.Params.Add("include_relationships", "true");

                return await api.GetAllDataAsync(true);
            }
        }

        public async Task<List<Pike13StaffMember>> GetStaffMembersAsync()
        {
            using (var api = new Pike13API<Pike13StaffMember>(_auth))
            {
                api.EndPoint = "staff_members";

                return await api.GetAllDataAsync();
            }
        }

        public async Task<List<Pike13Note>> GetPersonNotesAsync(long id)
        {
            using (var api = new Pike13API<Pike13Note>(_auth))
            {
                api.EndPoint = $"people/{id}/notes";
                api.OverrideEndpoint = "notes";
                return await api.GetAllDataAsync();
            }
        }
    
        public async Task<Pike13WebhooksResponse> SubscribeToVisits(string UrlSelf)
        {
            using (var api = new Pike13HooksApi(_auth))
            {
                api.request.attributes.target = UrlSelf;// "https://daftari.app/Pike13Access/OnVisitCreated";
                api.request.attributes.topic = "visit.updated";

                return await api.GetReportAsync();
            }
        }

        public async Task<Pike13WebhooksResponse> SubscribeToWebhooks(string UrlSelf, string Topic)
        {
            using (var api = new Pike13HooksApi(_auth))
            {
                api.request.attributes.target = UrlSelf;// "https://daftari.app/Pike13Access/OnVisitCreated";
                api.request.attributes.topic = Topic;

                return await api.GetReportAsync();
            }
        }
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

    public class Pike13StaffMember
    {
        public string bio { get; set; }
        public string first_name { get; set; }
        public long id { get; set; }
        public string last_name { get; set; }
        public string middle_name { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public Pike13ProfilePhoto profile_photo { get; set; }
        public string role { get; set; }
    }

    public class Pike13Note
    {
        public string id { get; set; }
        public string note { get; set; }

        [JsonProperty("public")]
        public string publicFlag { get; set; }

        public string pinned { get; set; }
        public string person_id { get; set; }
        public string event_occurrence_id { get; set; }
        public string created_at { get; set; }
        public string created_by_id { get; set; }
        public string updated_at { get; set; }
        public string updated_by_id { get; set; }
    }

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

        private FormUrlEncodedContent _content;

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
    }

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
                var test = await response.Content.ReadAsStringAsync();
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
    /// <summary>
    /// /////////////////////////////////////////////////////////////
    /// </summary>
    internal class Pike13HooksApi : IDisposable
    {
        private Pike13ApiAuth _auth;

        private StringContent _content;
        public Pike13HooksApi(Pike13ApiAuth auth)
        {
            _auth = auth;
            request = new Pike13APIRequest();
            // not required for this purpose
            request.attributes.fields = null;
            request.attributes.page = null;
        }
        //public string EndPoint { get; set; }
        public Pike13APIRequest request { get; set; }
        public string Type { get; set; } = "webhooks";
        public UriBuilder URL { get; set; }

        public void Dispose()
        {
            _auth = null;
        }

        public async Task<Pike13WebhooksResponse> GetReportAsync()
        {
            var response = await postReponseAsync();
            var apiResponse = JsonConvert.DeserializeObject<Pike13WebhooksResponse>(await response.Content.ReadAsStringAsync());

            return apiResponse;
        }

        protected async Task<HttpResponseMessage> postReponseAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, queryBuilder());
            request.Content = _content;
            request.Content.Headers.ContentType.MediaType = "application/vnd.api+json";
            request.Content.Headers.ContentType.CharSet = "UTF-8";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Authorization =
            //new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth.AccessToken);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return response;
        }

        private string queryBuilder()
        {
            if (URL == null)
            {
                URL = new UriBuilder(string.Format("{0}/api/v3/{1}?access_token={2}", _auth.Host, Type, _auth.AccessToken));
            }
            request.type = Type;

            var req = new Pike13RequestWrapper
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

        private class Pike13RequestWrapper
        {
            public Pike13APIRequest data { get; set; }
        }
    }

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
        public Pike13WebhooksAttrib()
        {
            
        }

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