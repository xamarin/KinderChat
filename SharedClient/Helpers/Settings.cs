// Helpers/Settings.cs
using Refractored.Xam.Settings;
using Refractored.Xam.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KinderChat
{
	public enum AppTheme
	{
		Blue,
		Pink,
		Black
	}
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters. 
	/// </summary>
	public static class Settings
	{
		static ISettings AppSettings {
			get {
				return CrossSettings.Current;
			}
		}

        static Dictionary<string, object> cachedValues = new Dictionary<string, object>(); 

		#region Setting Constants


		const string KinderPointsPendingKey = "points_pending_key";
		static readonly int KinderPointsPendingDefault = 0;

		const string KinderPointsKey = "kinder_points_key";
		static readonly int KinderPointsDefault = 500;

		const string LinkedToParentKey = "linked_to_parent";
		static readonly bool LinkedToParentDefault = false;

		const string InForegroundKey = "app_in_foreground";
		static readonly bool InForegroundDefault = false;

		const string NotificationRegIdKey = "notification_reg";
		static readonly string NotificationRegIdDefault = string.Empty;

		const string ParentEmailKey = "parent_email";
		static readonly string ParentEmailDefault = string.Empty;

		const string FirstRunKey = "first_run_key";
		static readonly bool FirstRunDefault = true;

		const string PublicKeyKey = "public_key";
		static readonly string PublicKeyDefault = string.Empty;


		const string PrivateKeyKey = "private_key";
		static readonly string PrivateKeyDefault = string.Empty;

		const string AccessTokenKey = "access_token_key";
		static readonly string AccessTokenDefault = string.Empty;

		const string UserDeviceIdKey = "username_key";
		static readonly string UserDeviceIdDefault = string.Empty;

		const string UserDeviceLoginIdKey = "username_login_key";
		static readonly string UserDeviceLoginIdDefault = string.Empty;

		const string DevicePasswordKey = "device_password";
		static readonly string DevicePasswordDefault = string.Empty;

		const string EmailKey = "email_key";
		static readonly string EmailDefault = string.Empty;


		const string MyIdKey = "my_id";
		static readonly int MyIdDefault = -1;


		const string PhoneNumberKey = "phone_number_key";
		static readonly string PhoneNumberDefault = string.Empty;


		const string NickNameKey = "nick_name_key";
		static readonly string NickNameDefault = string.Empty;

		const string KeyValidUntilKey = "key_valid";
		const long KeyValidUntilDefault = 0;

		const string AppThemeKey = "theme_key";
		const int AppThemeDefault = (int)AppTheme.Blue;

		const string AvatarKey = "avatar_key";
		static string AvatarDefault = string.Empty;

		const string CustomAvatarKey = "custom_avatar_key";
		static string CustomAvatarDefault = string.Empty;

		const string HubRegistrationIdKey = "hub_registration_id";
		static string HubRegistrationIdDefault = string.Empty;

		const string CustomAvatarIdKey = "custom_avatar_id_key";
		const int CustomAvatarIdDefault = 0;

		const string NewFriendIdKey = "new_friend_id_key";
		public const int NewFriendIdDefault = -1;
		#endregion
		public static int GetNotificationId()
		{
			var id = GetValueOrDefault ("notification_id", 0);
			var newId = id + 1;
			AddOrUpdateValue ("notification_id", newId);
			return id;
		}

		public static int GenerateTempFriendId() 
		{
			var current = GetValueOrDefault(NewFriendIdKey, NewFriendIdDefault);
			current--;
			AddOrUpdateValue (NewFriendIdKey, current);
			return current;
		}

		public static int KinderPointsPending
		{
			get
			{
				return GetValueOrDefault(KinderPointsPendingKey, KinderPointsPendingDefault);
			}
			set
			{
				AddOrUpdateValue (KinderPointsPendingKey, value);
			}
		}

		public static int KinderPoints
		{
			get
			{
				return GetValueOrDefault(KinderPointsKey, KinderPointsDefault);
			}
			set
			{
				AddOrUpdateValue (KinderPointsKey, value);
			}
		}

		public static bool LinkedToParent
		{
			get
			{
				return GetValueOrDefault(LinkedToParentKey, LinkedToParentDefault);
			}
			set
			{
				AddOrUpdateValue (LinkedToParentKey, value);
			}
		}

		public static bool InForeground
		{
			get
			{
				return AppSettings.GetValueOrDefault(InForegroundKey, InForegroundDefault);
			}
			set
			{
                AppSettings.AddOrUpdateValue(InForegroundKey, value);
			}
		}


		public static string NotificationRegId
		{
			get
			{
				return GetValueOrDefault(NotificationRegIdKey, NotificationRegIdDefault);
			}
			set
			{
				AddOrUpdateValue (NotificationRegIdKey, value);
			}
		}


		public static string ParentEmail
		{
			get
			{
				return GetValueOrDefault(ParentEmailKey, ParentEmailDefault);
			}
			set
			{
				AddOrUpdateValue (ParentEmailKey, value);
			}
		}

		public static bool FirstRun
		{
			get
			{
				return GetValueOrDefault(FirstRunKey, FirstRunDefault);
			}
			set
			{
				AddOrUpdateValue (FirstRunKey, value);
			}
		}

		public static int CustomAvatarId
		{
			get
			{
				return GetValueOrDefault(CustomAvatarIdKey, CustomAvatarIdDefault);
			}
			set
			{
				AddOrUpdateValue (CustomAvatarIdKey, value);
			}
		}

		public static string HubRegistrationId
		{
			get
			{
				return GetValueOrDefault(HubRegistrationIdKey, HubRegistrationIdDefault);
			}
			set
			{
				AddOrUpdateValue (HubRegistrationIdKey, value);
			}
		}

		public static string CustomAvatar
		{
			get
			{
				return GetValueOrDefault(CustomAvatarKey, CustomAvatarDefault);
			}
			set
			{
				AddOrUpdateValue (CustomAvatarKey, value);
			}
		}

		public static string Avatar
		{
			get
			{
				return GetValueOrDefault(AvatarKey, AvatarDefault);
			}
			set
			{
				AddOrUpdateValue (AvatarKey, value);
			}
		}

		public static AppTheme AppTheme
		{
			get
			{
				return (AppTheme)GetValueOrDefault(AppThemeKey, AppThemeDefault);
			}
			set
			{
				AddOrUpdateValue (AppThemeKey, (int)value);
			}
		}

		public static long KeyValidUntil
		{
			get
			{
				return GetValueOrDefault(KeyValidUntilKey, KeyValidUntilDefault);
			}
			set
			{
				AddOrUpdateValue (KeyValidUntilKey, value);
			}
		}


		public static string AccessToken {
			get {
				return GetValueOrDefault (AccessTokenKey, AccessTokenDefault);
			}
			set {
				AddOrUpdateValue (AccessTokenKey, value);
			}
		}
		
		public static string DevicePassword {
			get {
				return GetValueOrDefault (DevicePasswordKey, DevicePasswordDefault);
			}
			set {
				AddOrUpdateValue (DevicePasswordKey, value);
			}
		}

		public static string UserDeviceLoginId {
			get {
				return GetValueOrDefault (UserDeviceLoginIdKey, UserDeviceLoginIdDefault);
			}
			set {
				AddOrUpdateValue (UserDeviceLoginIdKey, value);
			}
		}

		public static string UserDeviceId {
			get {
				return GetValueOrDefault (UserDeviceIdKey, UserDeviceIdDefault);
			}
			set {
				AddOrUpdateValue (UserDeviceIdKey, value);
			}
		}

		public static string Email {
			get {
				return GetValueOrDefault (EmailKey, EmailDefault);
			}
			set {
				AddOrUpdateValue (EmailKey, value);
			}
		}

		public static string PhoneNumber {
			get {
				return GetValueOrDefault (PhoneNumberKey, PhoneNumberDefault);
			}
			set {
				AddOrUpdateValue (PhoneNumberKey, value);
			}
		}

		public static string NickName {
			get {
				return GetValueOrDefault (NickNameKey, NickNameDefault);
			}
			set {
				AddOrUpdateValue (NickNameKey, value);
			}
		}

		public static int MyId {
			get {
				return GetValueOrDefault (MyIdKey, MyIdDefault);
			}
			set {
				AddOrUpdateValue (MyIdKey, value);
			}
		}

	    public static byte[] PublicKey
        {
            get {
				var keyBase64 = GetValueOrDefault(PublicKeyKey, PublicKeyDefault);
				return Convert.FromBase64String(keyBase64);
            }
            set { 
				var keyBase64 = Convert.ToBase64String(value);
				AddOrUpdateValue(PublicKeyKey, keyBase64);
            }
	    }

		public static byte[] PrivateKey
		{
			get {
				var keyBase64 = GetValueOrDefault(PrivateKeyKey, PrivateKeyDefault);
				return Convert.FromBase64String(keyBase64);
			}
			set { 
				var keyBase64 = Convert.ToBase64String(value);
				AddOrUpdateValue(PrivateKeyKey, keyBase64);
			}
		}

	    private static bool AddOrUpdateValue<T>(string key, T value)
	    {
            lock (cachedValues)
            {
                cachedValues[key] = value;
                return AppSettings.AddOrUpdateValue(key, value);
            }
	    }

	    private static T GetValueOrDefault<T>(string key, T defaultValue = default(T))
	    {
	        lock (cachedValues)
	        {
                object valueFromCache;
                if (cachedValues.TryGetValue(key, out valueFromCache))
                {
                    return (T)valueFromCache;
                }
	            T value = AppSettings.GetValueOrDefault(key, defaultValue);
	            cachedValues[key] = value;
	            return value;
	        }
	    }


		public static bool IsLoggedIn
		{
			get { return !string.IsNullOrWhiteSpace (UserDeviceId); }
		}

		public static Task Logout()
		{
			UserDeviceId = UserDeviceIdDefault;
			UserDeviceLoginId = UserDeviceLoginIdDefault;
			PublicKey = Convert.FromBase64String(PublicKeyDefault);
			PrivateKey = Convert.FromBase64String(PrivateKeyDefault);
			KeyValidUntil = KeyValidUntilDefault;
			AccessToken = string.Empty;
			Email = string.Empty;
			PhoneNumber = string.Empty;
			NickName = string.Empty;
			MyId = -1;
			FirstRun = true;
			HubRegistrationId = string.Empty;
			AccessToken = string.Empty;
			KinderPoints = 500;
			KinderPointsPending = 0;
			FirstRun = FirstRunDefault;
			ParentEmail = ParentEmailDefault;
			LinkedToParent = LinkedToParentDefault;
			Avatar = AvatarDefault;
			CustomAvatar = CustomAvatarDefault;
			CustomAvatarId = CustomAvatarIdDefault;
			App.Logout ();
			return App.DataManager.DropDatabase ();
		}
	}
}
