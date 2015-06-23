using System;
using KinderChat.WorkerRole.SocketServer.Domain;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Logging;
using SuperSocket.SocketBase;
using SuperWebSocket;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Transport.Sockets
{
    public class LiveSession : WebSocketSession<LiveSession>, ISession
    {
        private static readonly ILogger Logger = LogFactory.GetLogger<LiveSession>();

        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(DeviceId); } }

        public string AccessToken { get; private set; }

        public string DeviceId { get; private set; }
        
        public long UserId { get; private set; }

        public void AssignUser(string accessToken, string deviceId, long userId)
        {
            AccessToken = accessToken;
            DeviceId = deviceId;
            UserId = userId;
        }

        public void Send(string eventName, object dto)
        {
            SendJsonMessage(eventName, dto);
        }

        protected override void HandleException(Exception e)
        {
            Logger.Exception(e, "HandleException");
            base.HandleException(e);
        }

        private void SendJsonMessage(string name, object content)
        {
            string str = !IsSimpleType(content.GetType()) ? AppServer.JsonSerialize(content) : content.ToString();
            string json = string.Format("{0} {1}", name, str);
            Send(json);
        }

        private bool IsSimpleType(Type type)
        {
            return type.IsValueType || type.IsPrimitive || type == typeof(string) || Convert.GetTypeCode(type) != TypeCode.Object;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
