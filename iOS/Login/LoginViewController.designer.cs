// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace KinderChat.iOS
{
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		UIKit.UIButton BoyButton { get; set; }

		[Outlet]
		UIKit.UIImageView BubbleImg { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint BubbleImgTopOffset { get; set; }

		[Outlet]
		UIKit.UIButton ContinueBtn { get; set; }

		[Outlet]
		UIKit.UIButton GirlButton { get; set; }

		[Outlet]
		UIKit.UITextField Input { get; set; }

		[Outlet]
		UIKit.UIView NavBarBlendView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint NavBarBlendViewHeightConstraint { get; set; }

		[Outlet]
		UIKit.UITextField NickName { get; set; }

		[Outlet]
		UIKit.UIButton SwitchSignUpType { get; set; }

		[Outlet]
		UIKit.UIView ThemeSelectorContainerView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BubbleImg != null) {
				BubbleImg.Dispose ();
				BubbleImg = null;
			}

			if (BubbleImgTopOffset != null) {
				BubbleImgTopOffset.Dispose ();
				BubbleImgTopOffset = null;
			}

			if (ContinueBtn != null) {
				ContinueBtn.Dispose ();
				ContinueBtn = null;
			}

			if (Input != null) {
				Input.Dispose ();
				Input = null;
			}

			if (NavBarBlendView != null) {
				NavBarBlendView.Dispose ();
				NavBarBlendView = null;
			}

			if (NavBarBlendViewHeightConstraint != null) {
				NavBarBlendViewHeightConstraint.Dispose ();
				NavBarBlendViewHeightConstraint = null;
			}

			if (NickName != null) {
				NickName.Dispose ();
				NickName = null;
			}

			if (SwitchSignUpType != null) {
				SwitchSignUpType.Dispose ();
				SwitchSignUpType = null;
			}

			if (ThemeSelectorContainerView != null) {
				ThemeSelectorContainerView.Dispose ();
				ThemeSelectorContainerView = null;
			}

			if (BoyButton != null) {
				BoyButton.Dispose ();
				BoyButton = null;
			}

			if (GirlButton != null) {
				GirlButton.Dispose ();
				GirlButton = null;
			}
		}
	}
}
