using System;
using UIKit;

namespace KinderChat.iOS
{
	public interface IColorTheme
	{
		UIStatusBarStyle StatusBarStyle { get; }
		UIColor ScreenTitleColor { get; }

		UIColor MainColor { get; }
		UIColor MainSaturatedColor { get; }
		UIColor DisabledButtonColor { get; }

		UIColor TitleTextColor { get; }
		UIColor DescriptionDimmedColor { get; }

		UIColor BackgroundColor { get; }
		UIColor NavigationBarButtonColor { get; }

		#region Conversations screen

		UIColor FriendNameColor { get; }
		UIColor LastMessageTextColor { get; }
		UIColor DateMessageLabelColor { get; }
		UIColor ConversationSelectedCellColor { get; }

		#endregion

		#region Conversation with Friend screen

		UIColor IncomingBubbleStroke { get; }
		UIColor IncomingTextColor { get; }
		UIColor OutgoingTextColor { get; }

		#endregion

		#region Points

		UIColor BadgeTitleColor { get; }

		#endregion
	}
}

