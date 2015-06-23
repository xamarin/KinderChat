using System;
using UIKit;

namespace KinderChat.iOS
{
	public static class RefresherControl
	{
		public static void ApplyCurrentTheme(UIControl refresher)
		{
			refresher.BackgroundColor = Theme.Current.MainColor;
			refresher.TintColor = UIColor.White;
		}
	}
}

