using System;
using System.Threading;
using KinderChat.WorkerRole.SocketServer;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Logging;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Logging.Loggers;

namespace WorkerRoleConsoleHost
{
    class Program
    {
        private static int port;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter port to host websockets (0 for default & single instance):");
            port = int.Parse(Console.ReadLine());

            LogFactory.Initialize(name => new ConsoleLogger(name));
            ThreadPool.QueueUserWorkItem(_ => Initialize());
            Console.ReadKey();
            Bootstrapper.Stop();
        }

        private static async void Initialize()
        {
            if (port == 0)
            {
                Bootstrapper.RunInSingleInstanceMode(serverName: "Server", port: 6102,
                    commonServiceBusConnectionString: "Endpoint=sb://kinderchattest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=c1VtNbMPHDwx2nsxz+zwzyGCc2Izv32Y1c0oR7gy9oc=",
                    connections: 1000);
                return;
            }
            Bootstrapper.RunInMultiInstanceMode(serverName: "Server" + port, port: port,
                redisSessionsConnectionString: "kinderchattest.redis.cache.windows.net,password=LnY8qaNLc9tUBY8jQ8fJbNs5EndCgGwl3uPzz2oAGzY=,syncTimeout=10000",
                redisEventsConnectionString: "kinderchattest.redis.cache.windows.net,password=LnY8qaNLc9tUBY8jQ8fJbNs5EndCgGwl3uPzz2oAGzY=,syncTimeout=10000",
                internalServiceBusConnectionString: "Endpoint=sb://kinderchattest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=c1VtNbMPHDwx2nsxz+zwzyGCc2Izv32Y1c0oR7gy9oc=",
                commonServiceBusConnectionString: "Endpoint=sb://kinderchattest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=c1VtNbMPHDwx2nsxz+zwzyGCc2Izv32Y1c0oR7gy9oc=",
                connections: 1000);
        }
    }
}
