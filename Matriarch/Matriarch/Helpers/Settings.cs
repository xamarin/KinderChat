// Helpers/Settings.cs
using Refractored.Xam.Settings;
using Refractored.Xam.Settings.Abstractions;

namespace Matriarch.Helpers
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
		static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		#region Setting Constants


		const string KinderPointsPendingKey = "kinder_points_pending_key";
		static readonly int KinderPointsPendingDefault = 0;

		const string KinderPointsKey = "kinder_points_key";
		static readonly int KinderPointsDefault = 500;

		const string LinkedToParentKey = "linked_to_parent";
		static readonly bool LinkedToParentDefault = false;

		const string ParentEmailKey = "parent_email";
		static readonly string ParentEmailDefault = string.Empty;

		const string FirstRunKey = "first_run_key";
		static readonly bool FirstRunDefault = true;

		const string PublicKeyKey = "public_key";
		static readonly string PublicKeyDefault = string.Empty;


		const string PrivateKeyKey = "private_key";
		static readonly string PrivateKeyDefault = string.Empty;

		const string AccessTokenKey = "access_token_key";
		static readonly string AccessTokenDefault = "Hy1CrYZYNHkm5OYRQgJby3rRget07wBUYr4evM8l7aQUQKVFNvNTrQRsCjxx0n9m1ncNE_6eTNHVlHmJi1hG4G7Fey9TcU194P78GK0p0waqOf4sOCsTVkE_8_tdAnShWVQrdlEyLjAZHwBoiOeYMfLp3BbmsiNKQWQyd-2CvmW7fj5tWZG3nO0-mFs-MUTn331WxQkbww-QL3GXYv8QI8vcfE_Qp3Hb_fbC2uswlwKxPw2vFWHcVeVMJkWWam1XTfN7I-ZMN3dSQKWhliGKLUJ9_P3HzPK2b-J-itfmKC3hJcCVfixoZsAOvrNz1tyf0NWOEr11rWV9yNVjS4KI2Oc3hyvIqqPzKFLYfSGPzQ8mCs-975xROKYYc1Cpd9KetVLMcgITxPsXmXvTFEaqkrniQ79JBxSKHoZ0RhA0lDhA4LAk2cO7SrgY59NMnZQrv3G8nG5Wd1MzIyUBarKdXYCctvqXxY7hkuw_snIUmri51Y4J2G7LeI2tZ-gzeOCs6QPXlZp-rL9RWKBkOPQZjSts96KD0RVjAodEtLjsleg";
		const string UserDeviceIdKey = "username_key";
		static readonly string UserDeviceIdDefault = "ef40cdd8-564f-4c65-949e-ce20f1719ce7";

		const string UserDeviceLoginIdKey = "username_login_key";
		static readonly string UserDeviceLoginIdDefault = string.Empty;


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
			var id = AppSettings.GetValueOrDefault("notification_id", 0);
			var newId = id + 1;
			AppSettings.AddOrUpdateValue("notification_id", newId);
			return id;
		}

		public static int GenerateTempFriendId()
		{
			var current = AppSettings.GetValueOrDefault(NewFriendIdKey, NewFriendIdDefault);
			current--;
			AppSettings.AddOrUpdateValue(NewFriendIdKey, current);
			return current;
		}

		public static int KinderPointsPending
		{
			get
			{
				return AppSettings.GetValueOrDefault(KinderPointsPendingKey, KinderPointsPendingDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(KinderPointsPendingKey, value);
			}
		}

		public static int KinderPoints
		{
			get
			{
				return AppSettings.GetValueOrDefault(KinderPointsKey, KinderPointsDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(KinderPointsKey, value);
			}
		}

		public static bool LinkedToParent
		{
			get
			{
				return AppSettings.GetValueOrDefault(LinkedToParentKey, LinkedToParentDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(LinkedToParentKey, value);
			}
		}

		public static string ParentEmail
		{
			get
			{
				return AppSettings.GetValueOrDefault(ParentEmailKey, ParentEmailDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(ParentEmailKey, value);
			}
		}

		public static bool FirstRun
		{
			get
			{
				return AppSettings.GetValueOrDefault(FirstRunKey, FirstRunDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(FirstRunKey, value);
			}
		}

		public static int CustomAvatarId
		{
			get
			{
				return AppSettings.GetValueOrDefault(CustomAvatarIdKey, CustomAvatarIdDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(CustomAvatarIdKey, value);
			}
		}

		public static string HubRegistrationId
		{
			get
			{
				return AppSettings.GetValueOrDefault(HubRegistrationIdKey, HubRegistrationIdDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(HubRegistrationIdKey, value);
			}
		}

		public static string CustomAvatar
		{
			get
			{
				return AppSettings.GetValueOrDefault(CustomAvatarKey, CustomAvatarDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(CustomAvatarKey, value);
			}
		}

		public static string Avatar
		{
			get
			{
				return AppSettings.GetValueOrDefault(AvatarKey, AvatarDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(AvatarKey, value);
			}
		}

		public static AppTheme AppTheme
		{
			get
			{
				return (AppTheme)AppSettings.GetValueOrDefault(AppThemeKey, AppThemeDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(AppThemeKey, (int)value);
			}
		}

		public static long KeyValidUntil
		{
			get
			{
				return AppSettings.GetValueOrDefault(KeyValidUntilKey, KeyValidUntilDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(KeyValidUntilKey, value);
			}
		}


		public static string AccessToken
		{
			get
			{
				return AppSettings.GetValueOrDefault(AccessTokenKey, AccessTokenDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(AccessTokenKey, value);
			}
		}

		public static string UserDeviceLoginId
		{
			get
			{
				return AppSettings.GetValueOrDefault(UserDeviceLoginIdKey, UserDeviceLoginIdDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(UserDeviceLoginIdKey, value);
			}
		}

		public static string UserDeviceId
		{
			get
			{
				return AppSettings.GetValueOrDefault(UserDeviceIdKey, UserDeviceIdDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(UserDeviceIdKey, value);
			}
		}

		public static string Email
		{
			get
			{
				return AppSettings.GetValueOrDefault(EmailKey, EmailDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(EmailKey, value);
			}
		}

		public static string PhoneNumber
		{
			get
			{
				return AppSettings.GetValueOrDefault(PhoneNumberKey, PhoneNumberDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(PhoneNumberKey, value);
			}
		}

		public static string NickName
		{
			get
			{
				return AppSettings.GetValueOrDefault(NickNameKey, NickNameDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(NickNameKey, value);
			}
		}

		public static int MyId
		{
			get
			{
				return AppSettings.GetValueOrDefault(MyIdKey, MyIdDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(MyIdKey, value);
			}
		}

		

		public static bool IsLoggedIn
		{
			get { return !string.IsNullOrWhiteSpace(UserDeviceId); }
		}

		public static void Logout()
		{
			UserDeviceId = string.Empty;
			UserDeviceLoginId = string.Empty;
			AccessToken = string.Empty;
			Email = string.Empty;
			PhoneNumber = string.Empty;
			NickName = string.Empty;
			MyId = -1;
			FirstRun = true;
		}
	}
}