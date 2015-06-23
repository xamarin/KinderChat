using System;

using UIKit;
using CoreImage;

namespace KinderChat.iOS
{
	public class BlackImagesTheme : IImagesTheme
	{
		public static BlackImagesTheme Instance { get; private set; }

		CIColorControls saturationFilter;

		static BlackImagesTheme()
		{
			Instance = new BlackImagesTheme ();
		}

		private BlackImagesTheme()
		{
			saturationFilter = new CIColorControls ();
		}

		public UIImage SignUpIcon {
			get {
				return UIImage.FromBundle ("bearIconWithBook");
			}
		}

		public UIImage ApplyEffects (UIImage image)
		{
			saturationFilter.Brightness = 0.2f;
			saturationFilter.Contrast = 1f;
			saturationFilter.Saturation = 0;
			saturationFilter.Image = image.CGImage;
			var img = new UIImage (saturationFilter.OutputImage, image.CurrentScale, image.Orientation);

			return img;
		}
	}
}

