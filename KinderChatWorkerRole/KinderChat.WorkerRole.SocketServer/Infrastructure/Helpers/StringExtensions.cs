namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers
{
    public static class StringExtensions
    {
        public static string Cut(this string source, int maxLength = 4)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            if (source.Length > maxLength)
                return source.Substring(0, maxLength);
            return source;
        }
    }
}
