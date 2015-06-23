using System;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Threading;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using KinderChat.ServerClient;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Logging;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Logging.Loggers;

namespace KinderChat.WorkerRole.SocketServer
{
    public class WorkerRole : RoleEntryPoint
    {
        private static ILogger _logger;
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            _completedEvent.WaitOne();
        }

        static WorkerRole()
        {
            LogFactory.Initialize(n => new AzureLogger(n));
            _logger = LogFactory.GetLogger<WorkerRole>();
        }

        public override bool OnStart()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            bool standalone = !"false".Equals(CloudConfigurationManager.GetSetting("Standalone"), StringComparison.InvariantCultureIgnoreCase);
            var commonSbCs = CloudConfigurationManager.GetSetting("CommonServiceBusConnectionString");
            var internalSbCs = CloudConfigurationManager.GetSetting("InternalServiceBusConnectionString");
            var redisSessionsCs = CloudConfigurationManager.GetSetting("RedisSessionsConnectionString");
            var redisEventsCs = CloudConfigurationManager.GetSetting("RedisMessagesConnectionString");
            var connectionsLimit = int.Parse(CloudConfigurationManager.GetSetting("ConnectionsLimit") ?? "50000");

            ServicePointManager.DefaultConnectionLimit = connectionsLimit;

            if (standalone)
            {
                Bootstrapper.RunInSingleInstanceMode(RoleEnvironment.CurrentRoleInstance.Id, EndPoints.WsPort, 
                    commonSbCs, connectionsLimit);
            }
            else
            {
                Bootstrapper.RunInMultiInstanceMode(RoleEnvironment.CurrentRoleInstance.Id, EndPoints.WsPort,
                    redisSessionsCs, redisEventsCs, internalSbCs, commonSbCs, connectionsLimit);
            }

            return base.OnStart();
        }

        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            _logger.Exception(e.Exception, "CurrentDomain_FirstChanceException");
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Exception(e.ExceptionObject as Exception, "CurrentDomain_UnhandledException");
        }


        public override void OnStop()
        {
            Bootstrapper.Stop();
            _completedEvent.Set();
            base.OnStop();
        }
    }
}
