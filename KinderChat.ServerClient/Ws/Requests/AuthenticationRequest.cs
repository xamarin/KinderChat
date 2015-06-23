namespace KinderChat.ServerClient.Entities.Ws.Requests
{
    public class RegistrationRequest : BaseRequest
    {
        public string DeviceId { get; set; }
        
        public long UserId { get; set; }

        public byte[] PublicKey { get; set; }
    }


    public class RegistrationResponse : BaseResponse
    {
    }
}
