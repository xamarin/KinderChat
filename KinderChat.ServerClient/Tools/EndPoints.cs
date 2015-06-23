
namespace KinderChat.ServerClient
{
    public static class EndPoints
    {
        public const int DefaultTimeoutInMilliseconds = 60000;

        public const string BaseUrl = "http://patriot-chat.azurewebsites.net";

        #region Authentication

        public const string CreateUserEmail = BaseUrl + "/api/Users/CreateUser?email={0}";

        public const string CreateUserDeviceEmail =
            BaseUrl + "/api/users/CreateUserDevice?email={0}&confirmKey={1}&publicKey={2}&nickname={3}";

        public const string PushMessage = BaseUrl + "/api/Notifications?toDeviceId={0}&fromDeviceId={1}&fromUser={2}";

        #endregion

        #region Admin

        public const string ViewRegistrations = BaseUrl + "/api/Admin/ViewRegistrations";

        public const string ViewFlagUsers = BaseUrl + "/api/Admin/ViewFlagUsers?id={0}";

        public const string TotalAvatars = BaseUrl + "/api/Admin/TotalAvatars";

        public const string GetPopularNames = BaseUrl + "/api/Admin/GetPopularNames";

        public const string GetPopularAvatars = BaseUrl + "/api/Admin/GetPopularAvatars";

        #endregion

        #region User

        public const string ChangeNickname = BaseUrl + "/api/Users/ChangeNickname?nickname={0}";

        public const string ChangeAvatar = BaseUrl + "/api/Users/AddCustomAvatar";

        public const string SendUserInvite = BaseUrl + "/api/Users/SendUserInvite?email={0}";

        public const string GetUserViaId = BaseUrl + "/api/Users/GetUser/{0}";

        public const string GetUserViaEmail = BaseUrl + "/api/Users/GetUser?email={0}&phone=";

        public const string GetUserViaPhone = BaseUrl + "/api/Users/GetUser?phone={0}&email=";

        public const string VerifyUserExistsViaEmail = BaseUrl + "/api/Users/VerifyUserExists?email={0}&phone=";

        public const string VerifyUserExistsViaPhone = BaseUrl + "/api/Users/VerifyUserExists?phone={0}&email=";

        public const string FlagUser = BaseUrl + "/api/Users/FlagUser?userId={0}&flagId={1}";

        public const string GetFlags = BaseUrl + "/api/Flag/GetFlags";
        #endregion

        public const string GetAvatarList = BaseUrl + "/api/Avatar/GetAvatars";

        public const string AddAvatar = BaseUrl + "/api/Users/AddAvatar/{0}";

        public const string GetAccessToken = BaseUrl + "/Token";

        #region WS

        public const int WsPort = 6102;

        //public const string WsHost = "ws://kinderws.cloudapp.net"; //Production
        public const string WsHost = "ws://kinder-server-egorbo.cloudapp.net"; //Integration
        //public const string WsHost = "ws://192.168.1.2"; //local

        public static string WsUrl { get { return string.Format("{0}:{1}", WsHost, WsPort); } } //can't be constant :(

        #endregion

    }
}
