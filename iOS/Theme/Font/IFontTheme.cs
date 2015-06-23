using System;

using UIKit;

namespace KinderChat.iOS
{
	public interface IFontTheme
	{
		UIFont ScreenTitleFont { get; }
		UIFont NavigationButtonTitleFont { get; }
		UIFont SausageContrinueButtonFont { get; }
		UIFont SausageSwitchIdentityButtonFont { get; }

		UIFont TabBarItemTitle { get; }

		#region Conversations screen

		UIFont FriendNameFont { get; }
		UIFont LastMessageTextFont { get; }
		UIFont DateMessageLabelFont { get; }

		#endregion

		#region Conversation with Friend screen

		UIFont MessageFont { get; }

		#endregion

		UIFont TextTitleFont { get; }

		#region Points

		UIFont BadgeValueFont { get; }

		#endregion
	}
}

