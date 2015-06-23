using System;
using UIKit;

namespace KinderChat.iOS
{
	public static class SausageButtons
	{
		public static void SetUp(UIButton button)
		{
			button.Layer.CornerRadius = 15;
			button.TitleEdgeInsets = new UIEdgeInsets (0, 0, 1, 0);
		}

		public static void ApplyCurrentTheme(UIButton button)
		{
			ApplyTheme (Theme.Current, button);
		}

		public static void ApplyTheme(Theme theme, UIButton button)
		{
			button.SetTitleColor (UIColor.White, UIControlState.Normal);
			button.SetTitleColor (UIColor.White, UIControlState.Disabled);
			button.Font = theme.SausageContrinueButtonFont;
		}

		public static void UpdateBackgoundColor(UIButton button)
		{
			UpdateBackgoundColor (Theme.Current, button);
		}

		public static void UpdateBackgoundColor(Theme theme, UIButton button)
		{
			button.BackgroundColor = button.Enabled ? theme.MainSaturatedColor : theme.DisabledButtonColor;
		}
	}
}

