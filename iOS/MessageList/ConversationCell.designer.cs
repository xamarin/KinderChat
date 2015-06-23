// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace KinderChat.iOS
{
	[Register ("ConversationCell")]
	partial class ConversationCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView AvatarImg { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel DateLbl { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView Disclosure { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel MessageLbl { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel NameLbl { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AvatarImg != null) {
				AvatarImg.Dispose ();
				AvatarImg = null;
			}
			if (DateLbl != null) {
				DateLbl.Dispose ();
				DateLbl = null;
			}
			if (Disclosure != null) {
				Disclosure.Dispose ();
				Disclosure = null;
			}
			if (MessageLbl != null) {
				MessageLbl.Dispose ();
				MessageLbl = null;
			}
			if (NameLbl != null) {
				NameLbl.Dispose ();
				NameLbl = null;
			}
		}
	}
}
