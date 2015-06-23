using System;

using UIKit;
using Foundation;
using CoreGraphics;
using KinderChat.ServerClient.Entities;

namespace KinderChat.iOS
{
	[Register("UICollectionViewFriendCell")]
	public class UICollectionViewFriendCell : UICollectionViewCell
	{
		public static readonly NSString CellId = new NSString("UICollectionViewFriendCell");

		UIView strokeEmulator;
		UIImageView avatar;
		UILabel nickname;

		NSLayoutConstraint avatarCenterY, strokeEmulatorCenterY;
		NSLayoutConstraint avatarWidth, avatarHeight;
		NSLayoutConstraint strokeEmulatorWidth, strokeEmulatorHeight;
		NSLayoutConstraint nickNameTopSpacing;

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

		public string Name {
			get {
				return nickname.Text;
			}
			set {
				nickname.Text = value;
			}
		}

		public UICollectionViewFriendCell (IntPtr handle)
			: base(handle)
		{
			Initialize ();
		}

		public UICollectionViewFriendCell ()
		{
			Initialize();
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
			nickname = new UILabel {
				TranslatesAutoresizingMaskIntoConstraints = false,
				TextAlignment = UITextAlignment.Center,
				Lines = 2
			};
			ContentView.AddSubviews (strokeEmulator, avatar, nickname);

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

			nickNameTopSpacing = NSLayoutConstraint.Create (nickname, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Top, 1, 0);
			ContentView.AddConstraint (nickNameTopSpacing);
			ContentView.AddConstraints (Layout.PinLeftRightEdges(nickname, 0, 0));
		}

		public void ApplyCurrentTheme()
		{
			nickname.Font = Theme.Current.TextTitleFont;
			nickname.TextColor = UIColor.Black.ColorWithAlpha (0.8f);
		}

		public void UpdateLayout(CGSize cellSize, AvatarType type)
		{
			nfloat scale = (type == AvatarType.User || type == AvatarType.None) ? 0.592f: 1;
			nfloat avatarDiameter = cellSize.Width * scale;
			nfloat strokeDiameter = avatarDiameter + 4;

			nfloat centerOffsetY = cellSize.Width / 2;
			avatarCenterY.Constant = centerOffsetY;
			strokeEmulatorCenterY.Constant = centerOffsetY;

			avatarWidth.Constant = avatarHeight.Constant = avatarDiameter;
			strokeEmulatorWidth.Constant = strokeEmulatorHeight.Constant = strokeDiameter;
			avatar.Layer.CornerRadius = avatarDiameter / 2;
			strokeEmulator.Layer.CornerRadius = strokeDiameter / 2;

			nickNameTopSpacing.Constant = cellSize.Width - 10;
			nickname.PreferredMaxLayoutWidth = cellSize.Width;

			strokeEmulator.Hidden = type != AvatarType.User;
		}
	}
}

