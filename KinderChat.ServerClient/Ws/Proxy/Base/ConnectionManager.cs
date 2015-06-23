using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KinderChat.ServerClient.Entities.Ws.Requests;
using KinderChat.ServerClient.Ws.Events;
using KinderChat.ServerClient.Ws.Exceptions;

namespace KinderChat.ServerClient.Ws.Proxy
{
    public class ConnectionManager
    {
        private readonly ILiveConnection connection;
        private readonly ICredentialsProvider credentialsProvider;
        private readonly TimeSpan reconnectInterval = TimeSpan.FromSeconds(50);
        private readonly TimeSpan closeConnectionOnPauseInterval = TimeSpan.FromSeconds(100);
        private readonly Dictionary<long, TaskCompletionSource<BaseResponse>> requestsTasks = new Dictionary<long, TaskCompletionSource<BaseResponse>>();
        private CancellationTokenSource pauseCancellationTokenSource = new CancellationTokenSource();
        private Task<bool> connectionTask;
        private bool onPause = false;

        public ConnectionManager(ILiveConnection connection, ICredentialsProvider credentialsProvider)
        {
            this.connection = connection;
            this.credentialsProvider = credentialsProvider;

            connection.Closed += OnConnectionClosed;
            connection.Error += OnConnectionError;
            OnTick();
        }

        public event Action<PushedEvent> EventPushed = delegate { };

        public event Action<AuthenticationResponse> Authenticated = delegate { }; 

        /// <summary>
        /// Should be called when:
        /// 1) AccessToken and MyId are changed
        /// 2) OnResume from background
        /// 3) Connection type changed (3g -> WiFi).
        /// However, none of this is required since it has a timer (reconnectInterval and auto-ping). These steps just help to reconnect faster
        /// </summary>
        public async Task<bool> TryKeepConnectionAsync()
        {
            if (connectionTask != null)
            {
                return await connectionTask;
            }
            else
            {
                connectionTask = TryKeepConnectionInternalAsync();
                var result = await connectionTask;
                connectionTask = null;
                return result;
            }
        }

        public async void HandlePause()
        {
            if (!onPause)
            {
                try
                {
                    await Task.Delay(closeConnectionOnPauseInterval, pauseCancellationTokenSource.Token);
                    onPause = true;
                    ForceClose();
                }
                catch (OperationCanceledException) { }
            }
        }

        public async void HandleResume()
        {
            pauseCancellationTokenSource.Cancel();
            pauseCancellationTokenSource = new CancellationTokenSource();
            if (onPause)
            {
                onPause = false;
                await TryKeepConnectionAsync();
            }
        }

        public void ForceClose()
        {
            connection.Close();
        }

        internal async Task<TResponse> SendRequestAndWaitResponse<TResponse>(BaseRequest request, bool openConnection = true)
            where TResponse : BaseResponse
        {
            //TODO: custom timeout

            if (openConnection && !await TryKeepConnectionAsync())
            {
                throw new ConnectionException();
            }

            request.AssignNewToken();
            var taskSource = new TaskCompletionSource<BaseResponse>();
            lock (requestsTasks)
            {
                requestsTasks[request.RequestToken] = taskSource;
            }
            connection.Send("Request", request);
            return (TResponse)await taskSource.Task.ConfigureAwait(false);
        }

        internal async Task SendRequest(BaseRequest request)
        {
            if (!await TryKeepConnectionAsync())
            {
                throw new ConnectionException();
            }

            request.AssignNewToken();
            connection.Send("Request", request);
        }

        private async Task<bool> TryKeepConnectionInternalAsync()
        {
            if (onPause)
                return false;

            if (string.IsNullOrEmpty(credentialsProvider.AccessToken) || credentialsProvider.UserId < 0 )
                return false;

            if (connection.State != LiveConnectionState.Closed)
                return true;

            try
            {
                await connection.ConnectAsync(EndPoints.WsUrl);
                if (onPause)
                {
                    ForceClose();
                    return false;
                }

                connection.Subscribe("Event", (PushedEvent e) => EventPushed(e));
                connection.Subscribe<BaseResponse>("Response", OnResponse);
                var authenticationRequest = new AuthenticationRequest
                    {
                        AccessToken = credentialsProvider.AccessToken,
                        DeviceId = credentialsProvider.DeviceId,
                        UserId = credentialsProvider.UserId,
                    };

                var authResponse = await SendRequestAndWaitResponse<AuthenticationResponse>(authenticationRequest, false);

                if (!authResponse.Success)
                {
                    switch (authResponse.Error)
                    {
                        case Errors.DeviceRegistrationRequired:
                            var registrationResponse = await SendRequestAndWaitResponse<RegistrationResponse>(
                                new RegistrationRequest
                                {
                                    DeviceId = credentialsProvider.DeviceId,
                                    PublicKey = credentialsProvider.PublicKey,
                                    UserId = credentialsProvider.UserId
                                }, false);
                            if (registrationResponse.Success)
                            {
                                authResponse = await SendRequestAndWaitResponse<AuthenticationResponse>(authenticationRequest, false);
                            }
                            else
                            {
                                return false;
                            }
                        break;

                        default:
                            return false;
                    }
                }

                Authenticated(authResponse);

                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        private void OnConnectionError(Exception exc)
        {
            FailPendingRequests(exc);
        }

        private void OnConnectionClosed()
        {
            FailPendingRequests(new ConnectionException());
        }

        private void FailPendingRequests(Exception exception)
        {
            lock (requestsTasks)
            {
                foreach (var requestsTask in requestsTasks)
                {
                    requestsTask.Value.TrySetException(exception);
                }
                requestsTasks.Clear();
            }
        }

        private void OnResponse(BaseResponse response)
        {
            if (response == null)
                return;

            lock (requestsTasks)
            {
                TaskCompletionSource<BaseResponse> task;
                if (requestsTasks.TryGetValue(response.RequestToken, out task))
                {
                    requestsTasks.Remove(response.RequestToken);
                    task.TrySetResult(response);
                }
            }
        }

        private async void OnTick()
        {
            while (true)
            {
                await TryKeepConnectionAsync();
                await Task.Delay(reconnectInterval); //Timer is unavailable in PCL
            }
        }
    }
}
