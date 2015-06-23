using System.Reflection;
using Autofac;
using KinderChat.ServiceBusShared;
using KinderChat.WorkerRole.SocketServer.Api;
using KinderChat.WorkerRole.SocketServer.Api.Base;
using Module = Autofac.Module;

namespace KinderChat.WorkerRole.SocketServer
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AccessTokenFastValidator>().SingleInstance();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(i => i.IsSubclassOf(typeof(BaseController))).AsSelf().SingleInstance();
            base.Load(builder);
        }
    }
}
