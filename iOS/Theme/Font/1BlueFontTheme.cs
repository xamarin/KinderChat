using System;

using UIKit;

namespace KinderChat.iOS
{
	public class BlueFontTheme : IFontTheme
	{
		public static BlueFontTheme Instance { get; private set; }

		private BlueFontTheme()
		{
		}

		static BlueFontTheme()
		{
			Instance = new BlueFontTheme ();
		}

		public UIFont ScreenTitleFont {
			get {
				return AppFonts.Varela.Regular16_5;
			}
		}

		public UIFont NavigationButtonTitleFont {
			get {
				return AppFonts.Varela.Regular16_5;
			}
		}

		public UIFont SausageContrinueButtonFont {
			get {
				return AppFonts.Varela.Regular16_5;
			}
		}

		public UIFont SausageSwitchIdentityButtonFont {
			get {
				return AppFonts.Varela.Regular12_5;
			}
		}

		public UIFont TabBarItemTitle {
			get {
				return AppFonts.Varela.Regular10;
			}
		}

		public UIFont FriendNameFont {
			get {
				return AppFonts.Varela.Regular16_5;
			}
		}

		public UIFont LastMessageTextFont {
			get {
				return AppFonts.Varela.Regular11_5;
			}
		}

		public UIFont DateMessageLabelFont {
			get {
				return AppFonts.Varela.Regular9;
			}
		}

		public UIFont MessageFont {
			get {
				return AppFonts.Varela.Regular14;
			}
		}

		public UIFont TextTitleFont {
			get {
				return AppFonts.Varela.Regular13;
			}
		}

		public UIFont BadgeValueFont {
			get {
				return AppFonts.Varela.Regular40;
			}
		}
	}
}

