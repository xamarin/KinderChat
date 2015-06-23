using KinderChat.ServerClient.Ws.Proxy;

namespace Console
{
    public class AdhocCredentialsProvider : ICredentialsProvider
    {
        public string DeviceId { get; set; }

        public string AccessToken { get; set; }

        public long UserId { get; set; }

        public byte[] PublicKey { get; set; }
    }
}
