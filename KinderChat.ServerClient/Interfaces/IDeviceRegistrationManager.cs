using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KinderChat.ServerClient.Entities;

namespace KinderChat.ServerClient.Interfaces
{
    public interface IDeviceRegistrationManager
    {
        Task RegisterAsync(string handle, IEnumerable<string> tags, PlatformType platformType, string regId = null);

        Task<HttpStatusCode> UpdateRegistrationAsync(string regId, DeviceRegistration deviceRegistration);

        Task<string> RequestNewRegistrationIdAsync();
    }

    public enum PlatformType
    {
        Android,
        iOS,
        Windows
    }
}
