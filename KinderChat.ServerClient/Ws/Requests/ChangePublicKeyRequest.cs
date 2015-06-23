namespace KinderChat.ServerClient.Entities.Ws.Requests
{
    public class ChangePublicKeyRequest : BaseRequest
    {
        public byte[] NewPublicKey { get; set; }
    }

    public class ChangePublicKeyResponse : BaseResponse
    {
    }
}