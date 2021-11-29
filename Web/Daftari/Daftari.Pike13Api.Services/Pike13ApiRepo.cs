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

    internal enum Pike13APIVersion
    {
        v2,
        v3
    }

    public interface ICoreApiResponse<T>
        where T : class
    {
        List<T> Data { get; set; }
        string next { get; set; }
        string prev { get; set; }
        int total_count { get; set; }
    }

    //public class Pike13StaffMember
    //{
    //    public string bio { get; set; }
    //    public string first_name { get; set; }
    //    public long id { get; set; }
    //    public string last_name { get; set; }
    //    public string middle_name { get; set; }
    //    public string name { get; set; }
    //    public string phone { get; set; }
    //    public Pike13ProfilePhoto profile_photo { get; set; }
    //    public string role { get; set; }
    //}

    //public class Pike13Note
    //{
    //    public string id { get; set; }
    //    public string note { get; set; }

    //    [JsonProperty("public")]
    //    public string publicFlag { get; set; }

    //    public string pinned { get; set; }
    //    public string person_id { get; set; }
    //    public string event_occurrence_id { get; set; }
    //    public string created_at { get; set; }
    //    public string created_by_id { get; set; }
    //    public string updated_at { get; set; }
    //    public string updated_by_id { get; set; }
    //}

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
        
        //////////////////////////////////////////// EVENT OCCURRENCE ///////////////////////////////////////////////////
        public async Task<List<Pike13Event>> GetEventOccurenceAsync(DateTime StartTime, DateTime EndTime, long? Staff_Mmember_Ids = null)
        {
            using (var api = new Pike13CoreAPI<Pike13Event>(_auth))
            {
                api.EndPoint = "event_occurrences";
                //api.IsClientIDNeeded = true;
                api.Params.Add("from", string.Concat(StartTime.ToString("s"), "Z"));
                api.Params.Add("to", string.Concat(EndTime.ToString("s"), "Z"));
                if (Staff_Mmember_Ids.HasValue)
                {
                    api.Params.Add("staff_member_ids", Staff_Mmember_Ids.ToString());
                }

                return await api.GetAllDataAsync();
            }
        }

        public async Task<List<Pike13Event>> GetEventOccurrenceByIdAsync(long? id)
        {
            using (var api = new Pike13CoreAPI<Pike13Event>(_auth))
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
        
        ///////////////////////////////////////////////// VISITS /////////////////////////////////////////////////////////
        public async Task<List<Pike13Visit>> GetVisitsAsync(long id)
        {
            using (var api = new Pike13CoreAPI<Pike13Visit>(_auth))
            {
                api.EndPoint = $"event_occurrences/{id}/visits";
                api.OverrideEndpoint = "visits";
                //api.IsClientIDNeeded = true;

                return await api.GetAllDataAsync();
            }
        }

        public async Task<List<Pike13Visit>> PutVisitAsync(long id, string state)
        {
            using (var api = new Pike13CoreAPI<Pike13Visit>(_auth))
            {
                api.EndPoint = $"visits/{id}";
                api.OverrideEndpoint = "visits";

                api.Request = new
                {
                    visit = new 
                    {
                        state_event = state
                    }
                };

                return await api.PutAsync();
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
        
        //////////////////////////////////////////////// WEB HOOKS /////////////////////////////////////////////////////////
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

    /// <summary>
    /// /////////////////////////////////////////////////////////////
    /// </summary>
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
    //        //httpClient.DefaultRequestHeaders.Authorization =
    //        //new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth.AccessToken);

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

    //public class Pike13WebhooksResponse
    //{
    //    public Pike13WebhooksResponse()
    //    {
    //        data = new Pike13WebhooksData();
    //    }

    //    public Pike13WebhooksData data { get; set; }
    //}

    //public class Pike13WebhooksData
    //{
    //    public Pike13WebhooksData()
    //    {
    //        attributes = new Pike13WebhooksAttrib();
    //    }

    //    public long? id { get; set; }
    //    public string type { get; set; }
    //    public Pike13WebhooksLinks links { get; set; }
    //    /*"links": {
    //      "self": "https://subdomain.pike13.com/api/v3/webhooks/1"
    //    },*/
    //    public Pike13WebhooksAttrib attributes { get; set; }
    //}

    //public class Pike13WebhooksAttrib
    //{
    //    public Pike13WebhooksAttrib()
    //    {

    //    }

    //    public long? account_id { get; set; }
    //    public long? business_id { get; set; }
    //    public string topic { get; set; }
    //    public string target { get; set; }
    //    public string name { get; set; }
    //    public string description { get; set; }
    //}

    //public class Pike13WebhooksLinks
    //{
    //    public string self { get; set; }
    //}


}