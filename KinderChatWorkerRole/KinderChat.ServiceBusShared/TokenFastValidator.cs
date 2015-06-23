namespace KinderChat.ServiceBusShared
{
    public class AccessTokenFastValidator
    {
        public AccessTokenFastValidator()
        {
            
        }

        public void SaveToken(string token, string deviceId, long userId)
        {
        }

        public bool Validate(string accessToken, string deviceId, long userId)
        {
            //TODO: just a quick call - does deviceId and userId belong to this token or not
            //TODO: implement using Redis cache

            return true;
        }
    }
}
