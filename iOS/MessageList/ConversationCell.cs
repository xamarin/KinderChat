using System;

using UIKit;
using Foundation;

using MonoTouch.Dialog.Utilities;

namespace KinderChat.iOS
{
	public partial class ConversationCell : UITableViewCell, IImageUpdated
	{
		public string Name {
			get {
				return NameLbl.Text;
			}
			set {
				NameLbl.Text = value;
			}
		}

		public string MessageText {
			get {
				return MessageLbl.Text;
			}
			set {
				MessageLbl.Text = value;
			}
		}

		public string DateText {
			get {
				return DateLbl.Text;
			}
			set {
				DateLbl.Text = value;
			}
		}

		string photoUrl;
		public string PhotoUrl {
			get {
				return photoUrl;
			}
			set {
				photoUrl = value;
				TrySetAvatar (force:true);
			}
		}

		public ConversationCell (IntPtr handle)
			: base(handle)
		{
			SeparatorInset = UIEdgeInsets.Zero;

			SelectedBackgroundView = new UIView ();

			// reset separator on iOS8
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				PreservesSuperviewLayoutMargins = false;
				LayoutMargins = UIEdgeInsets.Zero;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			AvatarImg.Image = ImageUtils.ConversationAvatarPlaceholder;
			AvatarImg.Layer.CornerRadius = AvatarImg.Frame.Width / 2;
			AvatarImg.ClipsToBounds = true;
			AvatarImg.ContentMode = UIViewContentMode.ScaleAspectFill;
			ApplyCurrentTheme ();
		}

		void TrySetAvatar(bool force)
		{
            if (string.IsNullOrEmpty(photoUrl))
                return;
			
            UIImage img = ImageLoader.DefaultRequestImage(new Uri (photoUrl), this);
			if (img == null)
				return;

			if (force) {
				AvatarImg.Image = Theme.Current.ApplyEffects(img);
			} else {
				UIView.Transition(AvatarImg,0.7, UIViewAnimationOptions.TransitionCrossDissolve, () => {
					AvatarImg.Image = Theme.Current.ApplyEffects(img);
				}, null);
			}
		}

		public void UpdatedImage (Uri uri)
		{
			if (uri.AbsoluteUri == photoUrl)
				TrySetAvatar (force: false);
		}

		public void ApplyCurrentTheme()
		{
			NameLbl.Font = Theme.Current.FriendNameFont;
			NameLbl.TextColor = Theme.Current.FriendNameColor;

			MessageLbl.Font = Theme.Current.LastMessageTextFont;
			MessageLbl.TextColor = Theme.Current.LastMessageTextColor;

			DateLbl.Font = Theme.Current.DateMessageLabelFont;
			DateLbl.TextColor = Theme.Current.DateMessageLabelColor;

			SelectedBackgroundView.BackgroundColor = Theme.Current.ConversationSelectedCellColor;
		}
	}
}