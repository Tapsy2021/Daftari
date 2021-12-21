using Daftari.Services.REST.ViewModels;
using Daftari.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Daftari.Services.REST
{
    public class RestService
    {
        private HttpClient _client;

        public RestService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(Constants.URLs.BaseURL),
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.Keys.Media_Type));
        }

        public async Task<DaftariResult> PostAsync(App self, string URL, object model, CancellationToken ct)
        {
            if (!Validations.IsNetworkAvailable())
            {
                throw new HttpException(Constants.Messages.Network_Not_Available);
            }

            // check and throw if cancelled
            ct.ThrowIfCancellationRequested();
            // Authorisation
            if (self.IsUserLoggedIn)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Keys.Bearer, self.Identity.Token);
                _client.DefaultRequestHeaders.Add(Constants.Keys.Device_Id, self.DeviceId);
            }

            try
            {
                var response = await _client.PostAsync(URL, new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, Constants.Keys.Media_Type), ct);

                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<DaftariResult>(result);
                }
                // Other Status (System.Net.HttpStatusCode.Unauthorized-401)
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new AuthorizationException("Unauthorized");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(ex.Message);
            }

            throw new HttpException("Network Error");
        }

        public async Task<DaftariResult> GetAsync(App self, string URL, CancellationToken ct)
        {
            if (!Validations.IsNetworkAvailable())
            {
                throw new HttpException(Constants.Messages.Network_Not_Available);
            }
            // check and throw if cancelled
            ct.ThrowIfCancellationRequested();
            // Authorisation
            if (self.IsUserLoggedIn)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Keys.Bearer, self.Identity.Token);
                _client.DefaultRequestHeaders.Add(Constants.Keys.Device_Id, self.DeviceId);
            }

            try
            {
                var response = await _client.GetAsync(URL);
                //response.EnsureSuccessStatusCode();
                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<DaftariResult>(result);
                }
                // Other Status (System.Net.HttpStatusCode.Unauthorized-401)
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new AuthorizationException("Unauthorized");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(ex.Message);
            }

            throw new HttpException("Network Error");
        }

        public async Task<List<T>> GetListAsync<T>(App self, string URL, CancellationToken ct)
        {
            if (!Validations.IsNetworkAvailable())
            {
                throw new HttpException(Constants.Messages.Network_Not_Available);
            }

            // check and throw if cancelled
            ct.ThrowIfCancellationRequested();
            // Authorisation
            if (self.IsUserLoggedIn)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Keys.Bearer, self.Identity.Token);
                _client.DefaultRequestHeaders.Add(Constants.Keys.Device_Id, self.DeviceId);
            }

            List<T> list = null;
            try
            {
                var response = await _client.GetAsync(URL);
                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<DaftariResult>(content);
                    if (results.IsSuccess)
                    {
                        list = JsonConvert.DeserializeObject<List<T>>(results.Body.ToString());
                    }
                    //list = JsonConvert.DeserializeObject<List<T>>(content);
                }
                // Other Status (System.Net.HttpStatusCode.Unauthorized-401)
                //if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                //{
                //    throw new AuthorizationException("Unauthorized");
                //}
            }
            catch (Exception ex)
            {
                var g = ex.Message;
            }

            return list;
        }

        public async Task<bool> UploadAsync(App self, string URL, Stream image, string FileName, CancellationToken ct)
        {
            // Authorisation
            if (self.IsUserLoggedIn)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Keys.Bearer, self.Identity.Token);
                _client.DefaultRequestHeaders.Add(Constants.Keys.Device_Id, self.DeviceId);
            }

            HttpContent fileStreamContent = new StreamContent(image);
            fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = FileName };
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(fileStreamContent);
                var response = await _client.PostAsync(URL, formData, ct);
                return response.IsSuccessStatusCode;
            }
        }
    }
}
