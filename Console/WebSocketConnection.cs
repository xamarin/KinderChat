using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KinderChat.ServerClient.Entities.Ws;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace KinderChat
{
    public class WebSocketConnection : ILiveConnection
    {
        private JsonNetWebSocket socket;
		private List<Action> subscribeActions = new List<Action>();
        private readonly List<Action<Exception>> actionsToHandleError = new List<Action<Exception>>();
        private TimeSpan connectionTimeout = TimeSpan.FromSeconds(60);
        private TimeSpan sendPingTimeout = TimeSpan.FromSeconds(150);
        private TimeSpan pingInterval = TimeSpan.FromSeconds(150);

        private LiveConnectionState state;

        public event Action Closed = delegate { };

        public event Action<Exception> Error = delegate { };

        public WebSocketConnection()
        {
            StartSendPing();
        }

        private Task ConnectAsyncInternal(string url)
        {
            var connectTask = new TaskCompletionSource<bool>();

            lock (actionsToHandleError)
                actionsToHandleError.Add(e => connectTask.TrySetException(e));

            socket = new JsonNetWebSocket(url);
            socket.EnableAutoSendPing = false;
            socket.Closed += OnClosed;
            socket.Error += OnError;
            socket.Opened += (s, e) =>
            {
                State = LiveConnectionState.Opened;
                connectTask.TrySetResult(true);
            };
            State = LiveConnectionState.Opening;
            socket.Open();
            return connectTask.Task;
        }

        private async void StartSendPing()
        {
            while (true)
            {
                if (State == LiveConnectionState.Opened)
                {
                    var sendPingTask = SendRequestAsync<bool>("Ping", true);
                    if ((await Task.WhenAny(sendPingTask, Task.Delay(sendPingTimeout))) != sendPingTask)
                    {
                        OnError(this, new ErrorEventArgs(new Exception("Ping timeout")));
                    }
                    else
                    {
                        await sendPingTask;
                    }
                }
                await Task.Delay(pingInterval);
            }
        }

        public async Task ConnectAsync(string url)
        {
            var connectTask = ConnectAsyncInternal(url);
            if ((await Task.WhenAny(connectTask, Task.Delay(connectionTimeout))) != connectTask)
            {
                //default connection timeout is too big
                var exception = new Exception("Connection timeout");
                Close();
                OnError(this, new ErrorEventArgs(exception));
                throw exception;
            }
        }

        public void Send(string method, object content)
        {
            try
            {
                //usually it should not fail fast
                socket.Send(method, content);
            }
            catch (Exception exc)
            {
                OnError(this, new ErrorEventArgs(exc));
            }
        }

        public Task<TResponse> SendRequestAsync<TResponse>(string method, object request)
        {
            var task = new TaskCompletionSource<TResponse>();
            
            lock (actionsToHandleError)
                actionsToHandleError.Add(e => task.TrySetException(e));

            try
            {
                socket.Query(method, request, (TResponse response) => task.TrySetResult(response));
            }
            catch (Exception exc)
            {
                OnError(this, new ErrorEventArgs(exc));
            }
            return task.Task;
        }

        public void Subscribe<TData>(string eventName, Action<TData> dataCallback)
        {
            socket.On(eventName, dataCallback);
        }

        public void Close()
        {
            CancelRequestWithException(new OperationCanceledException());

            try
            {
                if (socket != null && socket.State != WebSocketState.Closed)
                    socket.Close();
            }
            catch (Exception) {}
        }

        public LiveConnectionState State
        {
            get
            {
                if (state == LiveConnectionState.Opened && socket.State != WebSocketState.Open)
                {
                    state = LiveConnectionState.Closed;
                }
                return state;
            }
            set { state = value; }
        }

        private void CancelRequestWithException(Exception exc)
        {
            State = LiveConnectionState.Closed;
            lock (actionsToHandleError)
            {
                foreach (var action in actionsToHandleError.ToList())
                {
                    action(exc);
                }
                actionsToHandleError.Clear();
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            //at first, let's notify all who _await_ SendRequestAsync or ConnectAsync about exception
            CancelRequestWithException(e.Exception);
            Error(e.Exception);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            //at first, let's notify all who _await_ SendRequestAsync or ConnectAsync about connection closing
            CancelRequestWithException(new Exception("Connection is closed"));
            Closed();
        }
    }

    public class JsonNetWebSocket : JsonWebSocket
    {
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public JsonNetWebSocket(string uri) : base(uri)
        {
        }

        //by default it uses DataContractJsonSerializer but SuperWebSocket server uses Json.net...

        protected override object DeserializeObject(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, jsonSerializerSettings);
        }

        protected override string SerializeObject(object target)
        {
            return JsonConvert.SerializeObject(target, jsonSerializerSettings);
        }
    }
}
