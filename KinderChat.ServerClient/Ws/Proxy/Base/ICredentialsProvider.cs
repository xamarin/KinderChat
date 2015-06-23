namespace KinderChat.ServerClient.Ws.Proxy
{
    public interface ICredentialsProvider
    {
        string DeviceId { get; }

        string AccessToken { get; }

        long UserId { get; }
        
        byte[] PublicKey { get; }
    }
}
