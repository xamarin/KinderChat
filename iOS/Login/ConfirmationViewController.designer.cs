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
	[Register ("ConfirmationViewController")]
	partial class ConfirmationViewController
	{
		[Outlet]
		UIKit.UIButton ContinueBtn { get; set; }

		[Outlet]
		UIKit.UILabel DescriptionBottomlbl { get; set; }

		[Outlet]
		UIKit.UILabel DescriptionTopLbl { get; set; }

		[Outlet]
		UIKit.UITextField Input { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DescriptionBottomlbl != null) {
				DescriptionBottomlbl.Dispose ();
				DescriptionBottomlbl = null;
			}

			if (DescriptionTopLbl != null) {
				DescriptionTopLbl.Dispose ();
				DescriptionTopLbl = null;
			}

			if (Input != null) {
				Input.Dispose ();
				Input = null;
			}

			if (ContinueBtn != null) {
				ContinueBtn.Dispose ();
				ContinueBtn = null;
			}
		}
	}
}
