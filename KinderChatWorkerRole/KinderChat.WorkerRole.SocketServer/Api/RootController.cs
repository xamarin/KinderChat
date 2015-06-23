using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using KinderChat.ServerClient.Entities.Ws.Requests;
using KinderChat.ServiceBusShared;
using KinderChat.WorkerRole.SocketServer.Api.Base;
using KinderChat.WorkerRole.SocketServer.Domain;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers;

namespace KinderChat.WorkerRole.SocketServer.Api
{
    public class RootController : BaseController
    {
        private readonly List<BaseController> _childrenControllers = new List<BaseController>();
        private readonly AccessTokenFastValidator _accessTokenFastValidator;
        private readonly ISessionsServer _server;
        private readonly IDevicesRepository _devicesRepository;

        public RootController(AccessTokenFastValidator accessTokenFastValidator, ISessionsServer server, IDevicesRepository devicesRepository)
        {
            _accessTokenFastValidator = accessTokenFastValidator;
            _server = server;
            _devicesRepository = devicesRepository;

            var controllerTypes = Assembly.GetExecutingAssembly().ExportedTypes.Where(t => t.IsSubclassOf(typeof(BaseController)) && t != GetType()).ToList();
            _childrenControllers.AddRange(controllerTypes.Select(ServiceLocator.Resolve).OfType<BaseController>());
        }

        [ApiMethod(requiresAuthentication: false)]
        public AuthenticationResponse Authentication(ISession session, AuthenticationRequest request)
        {
            var respone = request.CreateResponse<AuthenticationResponse>();

            if (string.IsNullOrEmpty(request.AccessToken) ||
                string.IsNullOrEmpty(request.DeviceId))
            {
                respone.Success = false;
                return respone;
            }

            if (!_accessTokenFastValidator.Validate(request.AccessToken, request.DeviceId, request.UserId))
            {
                respone.Success = false;
                return respone;
            }

            if (_devicesRepository.GetDevices(request.UserId).All(d => d != request.DeviceId))
            {
                respone.Success = false;
                respone.Error = Errors.DeviceRegistrationRequired;
                return respone;
            }

            session.AssignUser(request.AccessToken, request.DeviceId, request.UserId);

            respone.ServerInfo = GetServerInfo();
            _childrenControllers.ForEach(c => c.OnAuthenticating(session, respone));

            _server.OnSessionAuthenticated(session);

            Task.Delay(500).ContinueWith(r => _childrenControllers.ForEach(c => c.OnAuthenticated(session)));
            return respone;
        }

        [ApiMethod(requiresAuthentication: false)]
        public RegistrationResponse Registration(ISession session, RegistrationRequest request)
        {
            var response = request.CreateResponse<RegistrationResponse>();
            if (string.IsNullOrWhiteSpace(request.DeviceId) ||
                request.UserId < 1 ||
                request.PublicKey.IsNullOrEmpty())
            {
                response.Success = false;
                return response;
            }

            _devicesRepository.AddDevice(request.UserId, request.DeviceId);
            _devicesRepository.SetPublicKeyForDevice(request.DeviceId, request.PublicKey);

            return response;
        }

        [ApiMethod]
        public ChangePublicKeyResponse ChangePublicKey(ISession session, ChangePublicKeyRequest request)
        {
            throw new NotSupportedException();
            var response = request.CreateResponse<ChangePublicKeyResponse>();
            _devicesRepository.SetPublicKeyForDevice(session.DeviceId, request.NewPublicKey);
            return response;
        }

        public void Stop()
        {
            _childrenControllers.ForEach(c => c.OnStop());
        }

        private string GetServerInfo()
        {
#if DEBUG
            return "{EMPTY_FOR_DEBUG_MODE}";
#else
            try
            {
                return string.Format("Server:{0};ServersCount:{1};ActiveSessionsOnThisServer:{2}",
                    RoleEnvironment.CurrentRoleInstance.Id,
                    RoleEnvironment.CurrentRoleInstance.Role.Instances.Count,
                    _server.ActiveSessionsByUserId.Count);
            }
            catch (Exception exc)
            {
                return exc.ToString();
            }
#endif
        }
    }
}
