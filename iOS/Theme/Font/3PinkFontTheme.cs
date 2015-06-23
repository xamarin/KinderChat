using System;

using UIKit;

namespace KinderChat.iOS
{
	public class PinkFontTheme : IFontTheme
	{
		public static PinkFontTheme Instance { get; private set; }

		private PinkFontTheme()
		{
		}

		static PinkFontTheme()
		{
			Instance = new PinkFontTheme ();
		}

		public UIFont ScreenTitleFont {
			get {
				return AppFonts.Comic.Bold16_5;
			}
		}

		public UIFont NavigationButtonTitleFont {
			get {
				return AppFonts.Comic.Regular16_5;
			}
		}

		public UIFont SausageContrinueButtonFont {
			get {
				return AppFonts.Comic.Bold16_5;
			}
		}

		public UIFont SausageSwitchIdentityButtonFont {
			get {
				return AppFonts.Comic.Bold12_5;
			}
		}

		public UIFont TabBarItemTitle {
			get {
				return AppFonts.Comic.Regular10;
			}
		}

		public UIFont FriendNameFont {
			get {
				return AppFonts.Comic.Bold16_5;
			}
		}

		public UIFont LastMessageTextFont {
			get {
				return AppFonts.Comic.Regular11_5;
			}
		}

		public UIFont DateMessageLabelFont {
			get {
				return AppFonts.Comic.Regular9;
			}
		}

		public UIFont MessageFont {
			get {
				return AppFonts.Comic.Regular14;
			}
		}

		public UIFont TextTitleFont {
			get {
				return AppFonts.Comic.Regular13;
			}
		}

		public UIFont BadgeValueFont {
			get {
				return AppFonts.Comic.Regular40;
			}
		}
	}
}

