using System;
using UIKit;

namespace KinderChat.iOS
{
	public class BlueColorTheme : IColorTheme
	{
		public static BlueColorTheme Instance { get; private set; }

		private BlueColorTheme()
		{
		}

		static BlueColorTheme()
		{
			Instance = new BlueColorTheme ();
		}

		public UIStatusBarStyle StatusBarStyle {
			get {
				return UIStatusBarStyle.Default;
			}
		}

		public UIColor ScreenTitleColor {
			get {
				return UIColor.Black;
			}
		}

		public UIColor MainColor {
			get {
				return AppColors.FromHex (0xA0CEFF);
			}
		}

		public UIColor MainSaturatedColor {
			get {
				return AppColors.FromHex (0x2185ED);
			}
		}

		public UIColor DisabledButtonColor {
			get {
				return AppColors.FromHex(0xD3D3D3);
			}
		}

		public UIColor NavigationBarButtonColor {
			get {
				return AppColors.FromHex (0x2185ED);
			}
		}

		public UIColor TitleTextColor {
			get {
				return AppColors.FromHex (0x000000).ColorWithAlpha(0.8f);
			}
		}

		public UIColor DescriptionDimmedColor {
			get {
				return AppColors.FromHex (0x000000).ColorWithAlpha (0.5f);
			}
		}

		public UIColor BackgroundColor {
			get {
				return AppColors.FromHex (0xF9F9F9);
			}
		}

		#region Conversations screen

		public UIColor FriendNameColor {
			get {
				return AppColors.FromHex (0x000000).ColorWithAlpha(0.8f);
			}
		}

		public UIColor LastMessageTextColor {
			get {
				return AppColors.FromHex (0x000000).ColorWithAlpha(0.5f);
			}
		}

		public UIColor DateMessageLabelColor {
			get {
				return AppColors.FromHex (0x000000).ColorWithAlpha(0.5f);
			}
		}

		public UIColor ConversationSelectedCellColor {
			get {
				return MainColor.ColorWithAlpha(0.1f);
			}
		}

		#endregion

		#region Conversation with Friend screen

		public UIColor IncomingBubbleStroke {
			get {
				return AppColors.FromHex (0xD9D9D9);
			}
		}

		public UIColor IncomingTextColor {
			get {
				return AppColors.FromHex(0x000000).ColorWithAlpha (0.5f);
			}
		}

		public UIColor OutgoingTextColor {
			get {
				return UIColor.White;
			}
		}

		#endregion

		#region Points

		public UIColor BadgeTitleColor {
			get {
				return AppColors.FromHex (0x929292);
			}
		}

		#endregion
	}
}

