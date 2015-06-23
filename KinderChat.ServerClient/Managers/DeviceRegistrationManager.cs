using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KinderChat.ServerClient.Entities;
using KinderChat.ServerClient.Interfaces;

namespace KinderChat.ServerClient.Managers
{
    public class DeviceRegistrationManager : IDeviceRegistrationManager
    {
        public async Task RegisterAsync(string handle, IEnumerable<string> tags, PlatformType platformType, string regId = null)
        {
            if (regId == null)
            {
                regId = await RequestNewRegistrationIdAsync();
            }

            string platform = string.Empty;
            switch (platformType)
            {
                case PlatformType.Android:
                    platform = "gcm";
                    break;
                case PlatformType.iOS:
                    platform = "apns";
                    break;
                case PlatformType.Windows:
                    platform = "wns";
                    break;
            }

            var deviceRegistration = new DeviceRegistration
            {
                Platform = platform,
                Handle = handle,
                Tags = tags.ToArray<string>()
            };

            var statusCode = await UpdateRegistrationAsync(regId, deviceRegistration);

            if (statusCode == HttpStatusCode.Gone)
            {
                regId = await RequestNewRegistrationIdAsync();
                statusCode = await UpdateRegistrationAsync(regId, deviceRegistration);
            }
        }

        public async Task<HttpStatusCode> UpdateRegistrationAsync(string regId, DeviceRegistration deviceRegistration)
        {
            using (var httpClient = new HttpClient())
            {
                var putUri = "https://kinder-chat.azurewebsites.net/api/register" + "/?id=" + regId;

                string json = JsonConvert.SerializeObject(deviceRegistration);
                var response = await httpClient.PutAsync(putUri, new StringContent(json, Encoding.UTF8, "application/json"));
                return response.StatusCode;
            }
        }

        public async Task<string> RequestNewRegistrationIdAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync("https://kinder-chat.azurewebsites.net/api/register", new StringContent(""));
                if (response.IsSuccessStatusCode)
                {
                    string regId = await response.Content.ReadAsStringAsync();
                    regId = regId.Substring(1, regId.Length - 2);
                    return regId;
                }
                else
                {
                    throw new Exception();
                }
            }
        }
    }
}
