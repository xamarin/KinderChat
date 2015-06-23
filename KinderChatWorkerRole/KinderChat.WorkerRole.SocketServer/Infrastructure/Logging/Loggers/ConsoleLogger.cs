using System;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Logging.Loggers
{
    /// <summary>
    /// </summary>
    public class AzureLogger : ILogger
    {
        private readonly string _typeName;

        public AzureLogger(string typeName)
        {
            _typeName = typeName;
        }

        public void Exception(Exception exc)
        {
            System.Diagnostics.Trace.TraceError(_typeName + ": " +  GetExceptionDescription(exc));
        }

        public void Exception(Exception exc, string captionFormat, params object[] args)
        {
            System.Diagnostics.Trace.TraceError(_typeName + ": " + string.Format(captionFormat + " ", args) +  GetExceptionDescription(exc));
        }

        public void Error(string format, params object[] args)
        {
            System.Diagnostics.Trace.TraceError(_typeName + ": " + string.Format(format, args));
        }

        public void Warning(string format, params object[] args)
        {
            System.Diagnostics.Trace.TraceWarning(_typeName + ": " + string.Format(format, args));
        }

        public void Info(string format, params object[] args)
        {
            System.Diagnostics.Trace.TraceInformation(_typeName + ": " + string.Format(format, args));
        }

        public void Debug(string format, params object[] args)
        {
        }

        public void Trace(string format, params object[] args)
        {
        }

        private string GetExceptionDescription(Exception exc)
        {
            int level = 0;
            string details = "";
            while (exc != null)
            {
                details += string.Format("{3}) Type: {0}. Message: {1}. StackTrace: {2}\n", exc.GetType().Name, exc.Message, exc.StackTrace, level);
                exc = exc.InnerException;
                level++;
            }
            return details;
        }
    }
}
