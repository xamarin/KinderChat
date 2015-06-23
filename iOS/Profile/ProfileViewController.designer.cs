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
	[Register ("ProfileViewController")]
	partial class ProfileViewController
	{
		[Outlet]
		UIKit.UIImageView AvatarImg { get; set; }

		[Outlet]
		UIKit.UIView BlendNavBarView { get; set; }

		[Outlet]
		UIKit.UICollectionView CollectionView { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView Spinner { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (AvatarImg != null) {
				AvatarImg.Dispose ();
				AvatarImg = null;
			}

			if (BlendNavBarView != null) {
				BlendNavBarView.Dispose ();
				BlendNavBarView = null;
			}

			if (CollectionView != null) {
				CollectionView.Dispose ();
				CollectionView = null;
			}

			if (Spinner != null) {
				Spinner.Dispose ();
				Spinner = null;
			}
		}
	}
}
