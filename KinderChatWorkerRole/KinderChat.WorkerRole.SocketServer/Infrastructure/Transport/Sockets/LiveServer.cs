using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using KinderChat.WorkerRole.SocketServer.Domain;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Logging;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperWebSocket;
using SuperWebSocket.SubProtocol;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Transport.Sockets
{

    public class LiveServer : WebSocketServer<LiveSession>, ISessionsServer
    {
        private static readonly ILogger Logger = Infrastructure.Logging.LogFactory.GetLogger<LiveServer>();
        private readonly IGlobalSessionsRegistry _sessionsRegistry;
        private readonly ConcurrentDictionary<string, ISession> _activeSessionsByDeviceId = new ConcurrentDictionary<string, ISession>();
        private readonly ConcurrentDictionary<long, List<ISession>> _activeSessionsByUserId = new ConcurrentDictionary<long, List<ISession>>();
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        private string _instanceName;

        public LiveServer(ISubProtocol<LiveSession> subProtocol, IGlobalSessionsRegistry sessionsRegistry)
            : base(subProtocol)
        {
            _sessionsRegistry = sessionsRegistry;
        }

        public ConcurrentDictionary<string, ISession> ActiveSessionsByDeviceId
        {
            get { return _activeSessionsByDeviceId; }
        }

        public ConcurrentDictionary<long, List<ISession>> ActiveSessionsByUserId
        {
            get { return _activeSessionsByUserId; }
        }

        public void OnSessionAuthenticated(ISession session)
        {
            Logger.Debug("Auth. Id={0}, DeviceId={1}", session.UserId, session.DeviceId.Cut());
            _sessionsRegistry.Set(session.DeviceId, _instanceName);
            ActiveSessionsByDeviceId[session.DeviceId] = session;
            ActiveSessionsByUserId.ChangeListFromDictionary(session.UserId, list => list.Add(session));
        }

        protected override void OnSessionClosed(LiveSession session, CloseReason reason)
        {
            if (session.IsAuthenticated)
            {
                Logger.Debug("Closed. Id={0}, DeviceId={1}", session.UserId, session.DeviceId.Cut());
                _sessionsRegistry.Remove(session.DeviceId, _instanceName);
                ISession sessionToRemove;
                ActiveSessionsByDeviceId.TryRemove(session.DeviceId, out sessionToRemove);
                ActiveSessionsByUserId.ChangeListFromDictionary(session.UserId, list => list.Remove(session));
            }
            else
            {
                Logger.Debug("Closed. Unauthenticated.");
            }

            base.OnSessionClosed(session, reason);
        }

        protected override void OnNewSessionConnected(LiveSession session)
        {
            Logger.Debug("New session");
            base.OnNewSessionConnected(session);
        }

        public void InitializeAndRun(string instanceName, int port, int maxConnections)
        {
            this._instanceName = instanceName;
            bool setup = Setup(new ServerConfig
            {
                Port = port,
                Ip = "Any",
                MaxRequestLength = int.MaxValue,
                MaxConnectionNumber = maxConnections,
                Mode = SocketMode.Tcp, // http://notcp.io :)
#if DEBUG
                LogBasicSessionActivity = false,
                LogCommand = false,
                LogAllSocketException = true,
#else
                LogBasicSessionActivity = false,
                LogCommand = false,
                LogAllSocketException = false,
#endif
                SendingQueueSize = 5000,
                ListenBacklog = 1000,
                KeepAliveInterval = 30,
                Name = instanceName
            }, logFactory: new ConsoleLogFactory());

            if (!setup)
            {
                Trace.TraceError("SocketServer: Failed to setup.");
                return;
            }
            if (!Start())
            {
                Trace.TraceError("SocketServer: Failed to start.");
                return;
            }
        }

        public override object JsonDeserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, _jsonSerializerSettings);
        }

        public override string JsonSerialize(object target)
        {
            return JsonConvert.SerializeObject(target, _jsonSerializerSettings);
        }
    }
}
