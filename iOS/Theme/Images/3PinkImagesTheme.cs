using System;

using UIKit;

namespace KinderChat.iOS
{
	public class PinkImagesTheme : IImagesTheme
	{
		public static PinkImagesTheme Instance { get; private set; }

		static PinkImagesTheme()
		{
			Instance = new PinkImagesTheme ();
		}

		private PinkImagesTheme()
		{

		}

		public UIImage SignUpIcon {
			get {
				return UIImage.FromBundle ("bearIcon");
			}
		}

		public UIImage ApplyEffects (UIImage image)
		{
			return image;
		}
	}
}

