namespace KinderChat.ServerClient.Ws.Entities
{
    public class PublicKeyInfo
    {
        public string DeviceId { get; set; }
        
        public byte[] PublicKey { get; set; }

        public long UserId { get; set; }
    }
}
