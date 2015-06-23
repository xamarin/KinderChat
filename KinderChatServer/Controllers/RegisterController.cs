using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus.Notifications;
using KinderChatServer.Core;

namespace KinderChatServer.Controllers
{
    public class RegisterController : ApiController
    {
        private readonly NotificationHubClient _hub;

        public RegisterController()
        {
            _hub = Notifications.Instance.Hub;
        }

        public class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
        }

        // POST api/register
        // This creates a registration id
        [HttpPost]
        public async Task<string> Post(string handle = null)
        {
            string newRegistrationId = null;

            // make sure there are no existing registrations for this push handle (used for iOS and Android)
            if (handle != null)
            {
                var registrations = await _hub.GetRegistrationsByChannelAsync(handle, 100);

                foreach (var registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                    }
                    else
                    {
                        await _hub.DeleteRegistrationAsync(registration);
                    }
                }
            }

            if (newRegistrationId == null)
                newRegistrationId = await _hub.CreateRegistrationIdAsync();

            return newRegistrationId;
        }

        [HttpPut]
        // PUT api/register/5
        // This creates or updates a registration (with provided channelURI) at the specified id
        public async Task<HttpResponseMessage> Put(string id, DeviceRegistration deviceUpdate)
        {
            RegistrationDescription registration = null;
            switch (deviceUpdate.Platform)
            {
                case "mpns":
                    var toastTemplate = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<wp:Notification xmlns:wp=\"WPNotification\">" +
                           "<wp:Toast>" +
                                "<wp:Text1>$(message)</wp:Text1>" +
                           "</wp:Toast> " +
                        "</wp:Notification>";
                    registration = new MpnsTemplateRegistrationDescription(deviceUpdate.Handle, toastTemplate);
                    break;
                case "wns":
                    toastTemplate = @"<toast><visual><binding template=""ToastText01""><text id=""1"">$(message)</text></binding></visual></toast>";
                    registration = new WindowsTemplateRegistrationDescription(deviceUpdate.Handle, toastTemplate);
                    break;
                case "apns":
                    var alertTemplate = "{\"aps\":{\"alert\":\"$(message)\"}}";
                    registration = new AppleTemplateRegistrationDescription(deviceUpdate.Handle, alertTemplate);
                    break;
                case "gcm":
                    var messageTemplate = "{\"data\":{\"msg\":\"$(message)\"}}";
                    registration = new GcmTemplateRegistrationDescription(deviceUpdate.Handle, messageTemplate);
                    break;
                default:
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            registration.RegistrationId = id;
            registration.Tags = new HashSet<string>(deviceUpdate.Tags);
            try
            {
                await _hub.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (MessagingException e)
            {
                ReturnGoneIfHubResponseIsGone(e);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/register/5
        public async Task<HttpResponseMessage> Delete(string id)
        {
            await _hub.DeleteRegistrationAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        {
            var webex = e.InnerException as WebException;
            if (webex == null || webex.Status != WebExceptionStatus.ProtocolError) return;
            var response = (HttpWebResponse)webex.Response;
            if (response.StatusCode == HttpStatusCode.Gone)
                throw new HttpRequestException(HttpStatusCode.Gone.ToString());
        }
    }
}
