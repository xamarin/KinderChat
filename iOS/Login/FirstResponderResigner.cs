using System;
using UIKit;

namespace KinderChat.iOS
{
	public class FirstResponderResigner
	{
		UIControl control;

		public FirstResponderResigner (UIView view, UIControl control)
		{
			this.control = control;

			var tap = new UITapGestureRecognizer (() => {
				control.ResignFirstResponder ();
			});
			tap.NumberOfTapsRequired = 1;

			view.AddGestureRecognizer (tap);
		}

		public void TryResignFirstResponder()
		{
			if (control.IsFirstResponder)
				control.ResignFirstResponder ();
		}

		// TODO: Wrong responsibility. move from here
		public void AdjustKeyboard()
		{
			if (control.IsFirstResponder) {
				control.ResignFirstResponder ();
				control.BecomeFirstResponder ();
			}
		}
	}
}

