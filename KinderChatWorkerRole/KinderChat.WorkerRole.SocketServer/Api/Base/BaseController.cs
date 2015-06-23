using System;
using System.Collections.Generic;
using System.Threading;
using KinderChat.ServerClient.Entities.Ws.Requests;
using KinderChat.WorkerRole.SocketServer.Domain;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Logging;

namespace KinderChat.WorkerRole.SocketServer.Api.Base
{
    public class BaseController
    {
        private ILogger _logger;
        private readonly List<IPulsable> _pulsedObjects = new List<IPulsable>();
        private readonly Timer _timer;

        protected BaseController()
        {
            _timer = new Timer(_ => OnTick(), null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
        }

        protected void RegisterPulsable(IPulsable obj)
        {
            _pulsedObjects.Add(obj);
        }

        protected ILogger Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger(GetType().Name)); }
        }

        internal virtual void OnStop()
        {
            if (_timer != null) _timer.Dispose();
            foreach (var pulsedObject in _pulsedObjects)
            {
                pulsedObject.Dispose();
            }
            _pulsedObjects.Clear();
        }

        internal virtual void OnAuthenticated(ISession session)
        {
        }

        internal virtual void OnAuthenticating(ISession session, AuthenticationResponse respone)
        {
        }

        private void OnTick()
        {
            foreach (var pulsedObject in _pulsedObjects)
            {
                //Probably, we can use pauses between these calls
                pulsedObject.HandleTimerTick();
            }
        }
    }
}
