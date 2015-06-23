using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace KinderChat.ServerClient
{
    public class WebManager : IWebManager
    {
        public WebManager(string authToken = null)
        {
            AuthenticationToken = authToken;
        }
        public class Result
        {
            public Result(bool isSuccess, string json)
            {
                IsSuccess = isSuccess;
                ResultJson = json;
            }

            public bool IsSuccess { get; private set; }
            public string ResultJson { get; private set; } 
        }

        public bool IsNetworkAvailable
        {
            get { return NetworkInterface.GetIsNetworkAvailable(); }
        }

        public static string AuthenticationToken { get; set; }
        public async Task<Result> PostData(Uri uri, MultipartContent header, StringContent content)
        {
            var httpClient = new HttpClient();
            try
            {
                if (!string.IsNullOrEmpty(AuthenticationToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticationToken);
                }
                HttpResponseMessage response;
                if (header == null)
                {
                    if(content == null) content = new StringContent(string.Empty);
                    response = await httpClient.PostAsync(uri, content);
                }
                else
                {
                    response = await httpClient.PostAsync(uri, header);
                }
                var responseContent = await response.Content.ReadAsStringAsync();
                return new Result(response.IsSuccessStatusCode, responseContent);
            }
            catch (Exception ex)
            {
                throw new WebException("Kinder Chat API Error: Service error", ex);
            }
        }

        public async Task<Result> PutData(Uri uri, StringContent json)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteData(Uri uri, StringContent json)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> GetData(Uri uri)
        {
            var httpClient = new HttpClient();

            try
            {
                if (!string.IsNullOrEmpty(AuthenticationToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticationToken);
                }
                var response = await httpClient.GetAsync(uri);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new WebException("Kinder Chat API Error: Service not found.");
                }
                var responseContent = await response.Content.ReadAsStringAsync();
                return string.IsNullOrEmpty(responseContent) ? new Result(response.IsSuccessStatusCode, string.Empty) : new Result(response.IsSuccessStatusCode, responseContent);
            }
            catch (Exception ex)
            {

                throw new WebException("Kinder Chat API Error: Service error", ex);
            }
        }
    }
}
