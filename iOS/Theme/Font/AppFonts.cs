using System;

using UIKit;

namespace KinderChat.iOS
{
	public static class AppFonts
	{
		public static class Comic
		{
			#region Regular

			public static UIFont Regular9 {
				get {
					return ComicNeue (11.5f);
				}
			}

			public static UIFont Regular10 {
				get {
					return ComicNeue (10f);
				}
			}

			public static UIFont Regular13 {
				get {
					return ComicNeue (13f);
				}
			}

			public static UIFont Regular14 {
				get {
					return ComicNeue (14f);
				}
			}

			public static UIFont Regular11_5 {
				get {
					return ComicNeue (11.5f);
				}
			}

			public static UIFont Regular16_5 {
				get {
					return ComicNeue (16.5f);
				}
			}

			public static UIFont Regular40 {
				get {
					return ComicNeue (40f);
				}
			}

			#endregion 

			#region Bold

			public static UIFont Bold12_5 {
				get {
					return ComicNeueBold (12.5f);
				}
			}

			public static UIFont Bold16_5 {
				get {
					return ComicNeueBold (16.5f);
				}
			}

			#endregion

			public static UIFont ComicNeue(float size)
			{
				return UIFont.FromName ("ComicNeue", size);
			}

			public static UIFont ComicNeueBold(float size)
			{
				return UIFont.FromName ("ComicNeue-Bold", size);
			}
		}

		public static class Times
		{
			#region Regular

			public static UIFont Regular9 {
				get {
					return AppFonts.Times.TimesNewRoman (9f);
				}
			}

			public static UIFont Regular14 {
				get {
					return AppFonts.Times.TimesNewRoman (14f);
				}
			}

			public static UIFont Regular10 {
				get {
					return AppFonts.Times.TimesNewRoman (10f);
				}
			}

			public static UIFont Regular11_5 {
				get {
					return AppFonts.Times.TimesNewRoman (11.5f);
				}
			}

			public static UIFont Regular13 {
				get {
					return AppFonts.Times.TimesNewRoman (13f);
				}
			}

			public static UIFont Regular16_5 {
				get {
					return AppFonts.Times.TimesNewRoman (16.5f);
				}
			}

			public static UIFont Regular40 {
				get {
					return AppFonts.Times.TimesNewRoman (40f);
				}
			}

			#endregion

			#region Bold

			public static UIFont Bold12_5 {
				get {
					return AppFonts.Times.TimesNewRomanBold (12.5f);
				}
			}

			public static UIFont Bold16_5 {
				get {
					return AppFonts.Times.TimesNewRomanBold (16.5f);
				}
			}

			#endregion

			public static UIFont TimesNewRoman(float size)
			{
				return UIFont.FromName ("TimesNewRomanPSMT", size);
			}

			public static UIFont TimesNewRomanBold(float size)
			{
				return UIFont.FromName ("TimesNewRomanPS-BoldMT", size);
			}
		}

		public static class Varela
		{
			#region Regular

			public static UIFont Regular9 {
				get {
					return AppFonts.Varela.VarelaRoundRegular (9f);
				}
			}

			public static UIFont Regular10 {
				get {
					return AppFonts.Varela.VarelaRoundRegular (10f);
				}
			}

			public static UIFont Regular11_5 {
				get {
					return AppFonts.Varela.VarelaRoundRegular (11.5f);
				}
			}

			public static UIFont Regular12_5 {
				get {
					return AppFonts.Varela.VarelaRoundRegular (12.5f);
				}
			}

			public static UIFont Regular13 {
				get {
					return AppFonts.Varela.VarelaRoundRegular (13f);
				}
			}

			public static UIFont Regular14 {
				get {
					return AppFonts.Varela.VarelaRoundRegular (14f);
				}
			}

			public static UIFont Regular16_5 {
				get {
					return AppFonts.Varela.VarelaRoundRegular (16.5f);
				}
			}

			public static UIFont Regular40 {
				get {
					return AppFonts.Varela.VarelaRoundRegular (40f);
				}
			}

			#endregion

			public static UIFont VarelaRoundRegular(float size)
			{
				return UIFont.FromName ("VarelaRound-Regular", size);
			}
		}
	}
}
