using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using KinderChat.ServerClient.Entities.Ws.Requests;
using KinderChat.WorkerRole.SocketServer.Api.Base;
using KinderChat.WorkerRole.SocketServer.Domain;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Logging;
using SuperWebSocket.SubProtocol;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Transport.Sockets.Commands
{
    public class Request : JsonSubCommand<LiveSession, BaseRequest>
    {
        private readonly ILogger Logger = LogFactory.GetLogger<Request>();

        private static readonly Dictionary<Type, Tuple<MethodInfo, BaseController, ApiMethodAttribute>> ControllersMethods =
            new Dictionary<Type, Tuple<MethodInfo, BaseController, ApiMethodAttribute>>();

        static Request()
        {
            try
            {
                var controllerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(BaseController)));
                var methods = new List<Tuple<MethodInfo, BaseController>>();

                foreach (var controllerType in controllerTypes)
                {
                    var controller = ServiceLocator.Resolve(controllerType) as BaseController;
                    methods.AddRange(controllerType.GetMethods().Select(i => new Tuple<MethodInfo, BaseController>(i, controller)));
                }

                foreach (var method in methods)
                {
                    var parameters = method.Item1.GetParameters();
                    if (parameters.Length != 2)
                        continue;

                    var apiMethodAttribute = method.Item1.GetCustomAttribute<ApiMethodAttribute>();
                    if (apiMethodAttribute != null &&
                        parameters[0].ParameterType == typeof(ISession) && 
                        parameters[1].ParameterType.IsSubclassOf(typeof(BaseRequest)))
                    {
                        ControllersMethods[parameters[1].ParameterType] = new Tuple<MethodInfo, BaseController, ApiMethodAttribute>(method.Item1, method.Item2, apiMethodAttribute);
                    }
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        protected override void ExecuteJsonCommand(LiveSession session, BaseRequest request)
        {            
            var methodAndInstancePair = ControllersMethods[request.GetType()];
            if (methodAndInstancePair.Item3.RequiresAuthentication && !session.IsAuthenticated)
            {
                var response = request.CreateResponse<BaseResponse>();
                response.Success = false;
                response.Error = Errors.AutheniticationRequired;
                session.Send("Response", response);
            }

            Logger.Trace("{0}  {1}  {2}", methodAndInstancePair.Item1.Name, request.GetType().Name, session.ToString());

            try
            {
                var responseObj = methodAndInstancePair.Item1.Invoke(methodAndInstancePair.Item2,
                    new object[] { session, request });

                if (methodAndInstancePair.Item1.ReturnType.IsSubclassOf(typeof(Task)))
                {
                    AwaitAndSend(session, responseObj);
                }
                else if (methodAndInstancePair.Item1.ReturnType != typeof(void))
                {
                    session.Send("Response", responseObj);
                }
            }
            catch (Exception exc)
            {
                Logger.Exception(exc, "{0} failed ({1},  {2})", methodAndInstancePair.Item1.Name, request.GetType().Name, session.ToString());
            }
        }

        private async void AwaitAndSend(LiveSession session, object responseObj)
        {
            var value = await (dynamic)responseObj;
            session.Send(value);
        }
    }
}
