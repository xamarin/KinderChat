using System;

using UIKit;

namespace KinderChat.iOS
{
	public class BlackFontTheme : IFontTheme
	{
		public static BlackFontTheme Instance { get; private set; }

		private BlackFontTheme()
		{
		}

		static BlackFontTheme()
		{
			Instance = new BlackFontTheme ();
		}

		public UIFont ScreenTitleFont {
			get {
				return AppFonts.Times.Bold16_5;
			}
		}

		public UIFont NavigationButtonTitleFont {
			get {
				return AppFonts.Times.Regular16_5;
			}
		}

		public UIFont SausageContrinueButtonFont {
			get {
				return AppFonts.Times.Bold16_5;
			}
		}

		public UIFont SausageSwitchIdentityButtonFont {
			get {
				return AppFonts.Times.Bold12_5;
			}
		}

		public UIFont TabBarItemTitle {
			get {
				return AppFonts.Times.Regular10;
			}
		}

		public UIFont FriendNameFont {
			get {
				return AppFonts.Times.Bold16_5;
			}
		}

		public UIFont LastMessageTextFont {
			get {
				return AppFonts.Times.Regular11_5;
			}
		}

		public UIFont DateMessageLabelFont {
			get {
				return AppFonts.Times.Regular9;
			}
		}

		public UIFont MessageFont {
			get {
				return AppFonts.Times.Regular14;
			}
		}

		public UIFont TextTitleFont {
			get {
				return AppFonts.Times.Regular13;
			}
		}

		public UIFont BadgeValueFont {
			get {
				return AppFonts.Times.Regular40;
			}
		}
	}
}

