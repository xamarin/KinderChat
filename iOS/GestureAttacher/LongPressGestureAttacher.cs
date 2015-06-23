using System;

using UIKit;

namespace KinderChat.iOS
{
	public class LongPressGestureAttacher
	{
		readonly UILongPressGestureRecognizer gesture;
		readonly Action<UILongPressGestureRecognizer> handler;

		public LongPressGestureAttacher (UIView view, Action<UILongPressGestureRecognizer> handler)
		{
			this.handler = handler;
			gesture = new UILongPressGestureRecognizer (HandleGesture);
			view.AddGestureRecognizer (gesture);
		}

		void HandleGesture(UILongPressGestureRecognizer gesture)
		{
			if(gesture.State == UIGestureRecognizerState.Began) {
				if (handler != null)
					handler (gesture);
			}
		}
	}
}