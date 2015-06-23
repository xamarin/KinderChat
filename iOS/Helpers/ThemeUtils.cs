using System;
using UIKit;

namespace KinderChat.iOS
{
	public static class ThemeUtils
	{
		public static void ApplyCurrentFont(UITextField input)
		{
			input.Font = Theme.Current.MessageFont;
		}
	}
}

