using System;
using UIKit;

namespace KinderChat.iOS
{
	public class BlueImagesTheme : IImagesTheme
	{
		public static BlueImagesTheme Instance { get; private set; }

		static BlueImagesTheme()
		{
			Instance = new BlueImagesTheme ();
		}

		private BlueImagesTheme()
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

