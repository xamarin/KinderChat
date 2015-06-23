using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using KinderChat.ServiceBusShared;
using KinderChat.WorkerRole.SocketServer.Api;
using KinderChat.WorkerRole.SocketServer.Domain;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Persistence;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Persistence.InMemory;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Persistence.Redis;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Transport.InternalBus.ServiceBus;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Transport.InternalBus.Stub;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Transport.Sockets;
using SuperWebSocket.SubProtocol;

namespace KinderChat.WorkerRole.SocketServer
{
    public static class Bootstrapper
    {
        public static void RunInSingleInstanceMode(string serverName, int port,
            string commonServiceBusConnectionString,
            int connections)
        {
            Run(serverName, port, commonServiceBusConnectionString, connections, 
                builder =>
                {
                    builder.RegisterType<InMemoryDeviceRepository>().As<IDevicesRepository>().SingleInstance();
                    builder.RegisterType<InMemorySessionsRegistry>().As<IGlobalSessionsRegistry>().SingleInstance();
                    builder.RegisterType<InMemoryGroupChatsRepository>().As<IGroupChatsRepository>().SingleInstance();
                    builder.RegisterGeneric(typeof (InMemoryUndeliveredEventsRepository<>))
                        .As(typeof (IUndeliveredEventsRepository<>)).SingleInstance();
                    builder.RegisterType<StubInternalMessageBus>().As<IInternalMessageBus>().SingleInstance();
                });
        }

        public static void RunInMultiInstanceMode(string serverName, int port,
            string redisSessionsConnectionString,
            string redisEventsConnectionString,
            string internalServiceBusConnectionString,
            string commonServiceBusConnectionString,
            int connections)
        {
            Run(serverName, port, commonServiceBusConnectionString, connections,
                builder =>
                {
                    builder.Register(c => new RedisDeviceRepository(redisEventsConnectionString)).As<IDevicesRepository>().SingleInstance();
                    builder.Register(c => new RedisSessionsRegistry(redisSessionsConnectionString)).As<IGlobalSessionsRegistry>().SingleInstance();
                    builder.Register(c => new RedisGroupChatsRepository(redisEventsConnectionString)).As<IGroupChatsRepository>().SingleInstance();
                    builder.RegisterGeneric(typeof(RedisUndeliveredEventsRepository<>))
                        .WithParameter("redisConnectionString", redisEventsConnectionString)
                        .As(typeof(IUndeliveredEventsRepository<>)).SingleInstance();
                    builder.Register(c => new InternalMessagesQueue(internalServiceBusConnectionString, serverName)).As<IInternalMessageBus>().SingleInstance();
                });
        }

        public static void Stop()
        {
            ServiceLocator.Resolve<RootController>().Stop();
            ServiceLocator.Resolve<ISessionsServer>().Stop();
        }

        private static void Run(string serverName, int port,
            string commonServiceBusConnectionString,
            int connections,
            Action<ContainerBuilder> registerAdditionalTypes)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CoreModule>();
            registerAdditionalTypes(builder);
            builder.Register(c => new ProcessedMessagesQueue(commonServiceBusConnectionString)).SingleInstance();
            builder.Register(c => new LiveServer(new BasicSubProtocol<LiveSession>("Basic", new List<Assembly> { Assembly.GetExecutingAssembly() }),
                                        c.Resolve<IGlobalSessionsRegistry>())).As<ISessionsServer>().SingleInstance();

            var container = builder.Build();
            ServiceLocator.Init(container);
            var server = container.Resolve<ISessionsServer>();

            server.InitializeAndRun(serverName, port, connections);
        }
    }
}
