using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace KinderChatServer.Tools
{
    public class TokenFilterAttribute : AuthorizationFilterAttribute
    {
        // TODO: Add Security Token
        private const string SecurityToken = "";

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            // Get the authentication header.
            var authenticationHeaderValue = actionContext.Request.Headers.Authorization;

            // If it's null, toss the user out right off the bat.
            if (authenticationHeaderValue != null)
            {
                if (authenticationHeaderValue.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(authenticationHeaderValue.Parameter))
                {
                    // Get the credential value and convert it back to plain text.
                    var credential = ConvertCredentialValue(authenticationHeaderValue);

                    if (credential.Equals(SecurityToken))
                    {
                        // If the credential matches our hard coded security token,
                        // Let the request go through.
                        return;
                    }
                }
            }

            // If it did not match, return a 404 error. This way anyone who might snoop around won't
            // hit it and get an "unauthorized" request, showing that something actually DOES exist at this endpoint.
            HandleUnauthorizedRequest(actionContext);
        }

        public string ConvertCredentialValue(System.Net.Http.Headers.AuthenticationHeaderValue authHeader)
        {
            var rawCredential = authHeader.Parameter;
            var encodingValue = Encoding.GetEncoding("iso-8859-1");
            return encodingValue.GetString(Convert.FromBase64String(rawCredential));
        }

        private void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.NotFound);
        }
    }
}
