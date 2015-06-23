using System;

namespace KinderChat
{
	public static class Strings
	{
		public static class LoginScreen
		{
			public static readonly string ContinueButtonTitle = "Continue";

			public static readonly string InputEmailPlaceholder = "Enter your email address";
			public static readonly string InputMobilePlaceholder = "Enter your mobile number";

			public static readonly string UseMobile = "Use your mobile number";
			public static readonly string UseEmail = "Use your email address";

			public static readonly string EmailBackButtonTitle = "Edit email";
			public static readonly string PhoneBackButtonTitle = "Edit phone";
		}

		public static class ConfirmationScreen
		{
			public static readonly string DescriptionEmailTop = "We have sent you an email with a confirmation code to address above.";
			public static readonly string DescriptionEmailBottom = "To complete your email verification, please enter the 6-digit code.";

			public static readonly string DescriptionPhoneTop = "We have sent you an SMS with a confirmation code to the number above.";
			public static readonly string DescriptionPhoneBottom = "To complete your phone number verification, please enter the 6-digit code.";
		}

		public static class Chats
		{
			public static readonly string Title = "Kinder Chats";
			public static readonly string BackButtonTitle = "Chats";
		}

		public static class Messages
		{
			public static readonly string TabBarTitle = "Messages";
		}

		public static class Friends
		{
			public static readonly string TabBarTitle = "Friends";
		}

		public static class Profile
		{
			public static readonly string TabBarTitle = "Profile";
		}
	}
}

