using System;

namespace KinderChat.WorkerRole.SocketServer.Api.Base
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiMethodAttribute : Attribute
    {
        public bool RequiresAuthentication { get; set; }

        public ApiMethodAttribute() : this(true)
        {
        }

        public ApiMethodAttribute(bool requiresAuthentication)
        {
            RequiresAuthentication = requiresAuthentication;
        }
    }
}