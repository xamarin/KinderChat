using System;

using UIKit;

namespace KinderChat.iOS
{
	public class TapGestureAttacher
	{
		public TapGestureAttacher (UIView view, int tapCount, Action handler)
		{
			var tap = new UITapGestureRecognizer (handler);
			tap.NumberOfTapsRequired = (uint)tapCount;

			view.AddGestureRecognizer (tap);
		}
	}
}

