using System;
using UIKit;

namespace KinderChat.iOS
{
	public static class AppColors
	{
		public static UIColor FromHex(int color)
		{
			byte[] rgb = BitConverter.GetBytes (color);
			return UIColor.FromRGB (rgb [2], rgb [1], rgb [0]);
		}

		public static UIColor FromHex(int color, float alpha)
		{
			var solidColor = FromHex(color);
			return solidColor.ColorWithAlpha (alpha);
		}
	}
}

