using System;

using UIKit;
using Foundation;
using CoreGraphics;
using KinderChat.ServerClient.Entities;

namespace KinderChat.iOS
{
	[Register("AvatarCollectionViewCell")]
	public class AvatarCollectionViewCell : UICollectionViewCell
	{
		public static readonly CGSize CellSize = new CGSize (74, 74); // by design
		public static readonly NSString CellId = new NSString("AvatarCollectionViewCell");

		UIView strokeEmulator;
		UIImageView avatar;

		NSLayoutConstraint avatarCenterY, strokeEmulatorCenterY;
		NSLayoutConstraint avatarWidth, avatarHeight;
		NSLayoutConstraint strokeEmulatorWidth, strokeEmulatorHeight;

		string avatarUrl;
		public string AvatarUrl {
			get {
				return avatarUrl;
			}
			set {
				avatarUrl = value;
				if (string.IsNullOrWhiteSpace (avatarUrl))
					avatar.Image = ImageUtils.FriendAvatarPlaceholder;
				else
					ImageUtils.SetImage (avatar, avatarUrl, url => avatarUrl == url);
			}
		}

		public AvatarCollectionViewCell (IntPtr handle)
			: base(handle)
		{
			Initialize ();
		}

		public AvatarCollectionViewCell ()
		{
			Initialize ();
		}

		void Initialize()
		{
			strokeEmulator = new UIView {
				TranslatesAutoresizingMaskIntoConstraints = false,
				BackgroundColor = UIColor.White
			};
			strokeEmulator.Layer.MasksToBounds = false;
			strokeEmulator.Layer.ShadowOffset = new CGSize (0, 0.5f);
			strokeEmulator.Layer.ShadowRadius = 0;
			strokeEmulator.Layer.ShadowOpacity = 0.2f;
			strokeEmulator.Layer.ShadowColor = UIColor.Black.CGColor;

			avatar = new UIImageView {
				TranslatesAutoresizingMaskIntoConstraints = false,
				ClipsToBounds = true,
				ContentMode = UIViewContentMode.ScaleAspectFill
			};
			ContentView.AddSubviews (strokeEmulator, avatar);

			avatarCenterY = NSLayoutConstraint.Create (avatar, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Top, 1, 0);
			avatarWidth = Layout.Width (avatar, 0);
			avatarHeight = Layout.Height (avatar, 0);
			ContentView.AddConstraints (new NSLayoutConstraint[]{ avatarCenterY, avatarWidth, avatarHeight});

			strokeEmulatorCenterY = NSLayoutConstraint.Create (strokeEmulator, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Top, 1, 0);
			strokeEmulatorWidth = Layout.Width (strokeEmulator, 0);
			strokeEmulatorHeight = Layout.Height (strokeEmulator, 0);
			ContentView.AddConstraints (new NSLayoutConstraint[]{ strokeEmulatorCenterY, strokeEmulatorWidth, strokeEmulatorHeight});

			avatar.CenterX ();
			strokeEmulator.CenterX ();

			BackgroundView = new UIView ();
			UIImageView cycleView = new UIImageView ();
			cycleView.Image = UIImage.FromBundle ("avatarSelectionMask");
			SelectedBackgroundView = cycleView;
		}

		public void UpdateLayout(AvatarType type)
		{
			nfloat scale = type == AvatarType.User ? 0.592f: 1;
			nfloat avatarDiameter = CellSize.Width * scale;
			nfloat strokeDiameter = avatarDiameter + 4;

			nfloat centerOffsetY = CellSize.Width / 2;
			avatarCenterY.Constant = centerOffsetY;
			strokeEmulatorCenterY.Constant = centerOffsetY;

			avatarWidth.Constant = avatarHeight.Constant = avatarDiameter;
			strokeEmulatorWidth.Constant = strokeEmulatorHeight.Constant = strokeDiameter;
			avatar.Layer.CornerRadius = avatarDiameter / 2;
			strokeEmulator.Layer.CornerRadius = strokeDiameter / 2;

			strokeEmulator.Hidden = type != AvatarType.User;
		}

		public void AnimateAppearance ()
		{
			ContentView.Alpha = 0;
			UIView.Animate (1, () => {
				ContentView.Alpha = 1;
			});
		}
	}
}

