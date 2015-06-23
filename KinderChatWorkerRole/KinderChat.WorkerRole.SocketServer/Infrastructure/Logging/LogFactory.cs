using System;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Logging.Loggers;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Logging
{
    public static class LogFactory
    {
        private static Func<string, ILogger> _logCreator = null;

        public static void Initialize(Func<string, ILogger> logCreator)
        {
            _logCreator = logCreator;
        }

        public static ILogger GetLogger<T>()
        {
            if (_logCreator == null)
                return new NullLogger();
            
            return _logCreator(typeof (T).Name);
        }

        public static ILogger GetLogger(string logger)
        {
            if (_logCreator == null)
                return new NullLogger();

            return _logCreator(logger);
        }
    }
}