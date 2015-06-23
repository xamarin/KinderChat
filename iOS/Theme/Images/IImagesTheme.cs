using System;

using UIKit;

namespace KinderChat.iOS
{
	public interface IImagesTheme
	{
		UIImage SignUpIcon { get; }
		UIImage ApplyEffects(UIImage image);
	}
}

