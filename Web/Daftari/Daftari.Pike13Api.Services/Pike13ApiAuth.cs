using Daftari.Pike13Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Daftari.Pike13Api.Services
{
    public class Pike13ApiAuth
    {
        private string _code;
        private FrontDeskConfig _config;
        private string _redirecturl;

        public Pike13ApiAuth(string username, string redirecturl, string code)
        {
            _config = new FrontDeskConfig(username);
            _redirecturl = redirecturl;
            _code = code;
        }

        public Pike13ApiAuth(string username, bool IsCachedTokenNeeded = true)
        {
            _config = new FrontDeskConfig(username);
            if (IsCachedTokenNeeded)
            {
                _config.AccessToken = TokenProvider.GetProvider().GetAccessCode(username);
            }
        }

        public Pike13ApiAuth(long BusinessID)
        {
            var token = TokenProvider.GetProvider();
            var sd = token.GetSubdomain(BusinessID);
            
            _config = new FrontDeskConfig(BusinessID, sd);
            _config.AccessToken = token.GetAccessCode(BusinessID);
        }

        public string AccessToken { get { return _config.AccessToken; } }

        public string ClientID { get { return _config.ClientID; } }

        public string Host { get { return _config.Host; } }

        public async Task AcquireTokenAsync(string user)
        {
            using (var client = new HttpClient())
            {
                string tokenEndPoint = _config.OHost + "/oauth/token";
                var request = new HttpRequestMessage(HttpMethod.Post, tokenEndPoint);

                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type","authorization_code"},
                    { "code", _code },
                    { "redirect_uri", _redirecturl },
                    { "client_id", _config.ClientID },
                    { "client_secret", _config.Secret },
                });

                request.Content.Headers.ContentType.CharSet = "UTF-8";
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                _config.AccessToken = payload.Value<string>("access_token");

                string peopleEndPoint = _config.OHost + "/api/v2/account/people?access_token=" + _config.AccessToken;
                var requestppl = new HttpRequestMessage(HttpMethod.Get, peopleEndPoint);

                var responseppl = await client.SendAsync(requestppl);
                responseppl.EnsureSuccessStatusCode();
                dynamic pp = JObject.Parse(await responseppl.Content.ReadAsStringAsync());
                var people = new List<Pike13Person>();

                foreach (var person in pp.people)
                {
                    people.Add(JsonConvert.DeserializeObject<Pike13Person>(Convert.ToString(person)));
                }
                Token tk = new Token()
                {
                    Username = user,
                    AccessToken = _config.AccessToken,
                    RefreshTime = DateTime.Now,
                    People = people.Select((a, i) => new AccountPeople()
                    {
                        BusinessID = long.Parse(a.business_id),
                        BusinessName = a.business_name,
                        IsClient = a.is_client,
                        PersonID = a.id,
                        PersonName = a.name,
                        Role = a.role,
                        Subdomain = a.subdomain,
                        TimeZone = a.timezone,
                        Photo = a.profile_photo?.x200,
                        IsActive = i == 0
                    }).ToList()
                };
                TokenProvider.GetProvider().AddToken(tk);
            }
        }

        public string GetAuthorizationURL(string callback)
        {
            return _config.OHost + "/oauth/authorize?client_id=" + _config.ClientID + "&response_type=code&redirect_uri=" + callback;
        }
    }

    internal class FrontDeskConfig
    {
        public FrontDeskConfig(string username)
        {
            var sd = TokenProvider.GetProvider().GetSubdomain(username);
            init(sd);
        }

        public FrontDeskConfig(long businessID, string Subdomain)
        {
            init(Subdomain);
        }

        private void init(string subdomain)
        {
            Host = "https://" + subdomain + "." + ConfigurationManager.AppSettings["fdhqApiHost"];
            OHost = "https://" + ConfigurationManager.AppSettings["fdhqApiHost"];
            ClientID = ConfigurationManager.AppSettings["fdhqApiClientID"];
            Secret = ConfigurationManager.AppSettings["fdhqApiSecret"];
        }

        public string AccessToken { get; set; }
        public string ClientID { get; set; }
        public string Host { get; set; }
        public string OHost { get; set; }
        public string Secret { get; set; }
    }
}