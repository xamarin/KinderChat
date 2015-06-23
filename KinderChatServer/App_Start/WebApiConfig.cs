using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using WebApiThrottle;

namespace KinderChatServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            // Attempt to throttle heavy users with rate limits.
            config.MessageHandlers.Add(new ThrottlingHandler()
            {
                Policy = new ThrottlePolicy()
                {
                    ClientThrottling = true,
                    EndpointThrottling = true,
                    EndpointRules = new Dictionary<string, RateLimits>
                    {
                        { "api/Messages/PostMessage", new RateLimits { PerMinute = 150} },
                        { "api/Messages/GetConversation", new RateLimits { PerMinute = 50} }
                    }
                },
                Repository = new CacheRepository()
            });
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                "Register",                                          
                "api/{controller}/{id}",                            
                new { id = RouteParameter.Optional }
            );
        }
    }
}
