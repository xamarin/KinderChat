using System;

using UIKit;
using CoreGraphics;
using MonoTouch.Dialog.Utilities;

namespace KinderChat.iOS
{
	public static class ImageUtils
	{
		static readonly CGSize ConversationAvatarSize = new CGSize (59, 59);
		static readonly CGSize FriendAvatarSize = new CGSize(52, 52);
		public static readonly CGSize CurrentAvatarSize = new CGSize(113, 113);

		static UIImage conversationAvatarPlaceholder;
		public static UIImage ConversationAvatarPlaceholder {
			get {
				conversationAvatarPlaceholder = conversationAvatarPlaceholder ?? RenderSolidImage (UIColor.LightGray, ConversationAvatarSize);
				return conversationAvatarPlaceholder;
			}
		}

		static UIImage currentAvatarPlaceholder;
		public static UIImage CurrentAvatarPlaceholder {
			get {
				currentAvatarPlaceholder = currentAvatarPlaceholder ?? RenderSolidImage(UIColor.LightGray, CurrentAvatarSize);
				return currentAvatarPlaceholder;
			}
		}

		static UIImage friendAvatarPlaceholder;
		public static UIImage FriendAvatarPlaceholder {
			get {
				friendAvatarPlaceholder = friendAvatarPlaceholder ?? RenderSolidImage(UIColor.LightGray, FriendAvatarSize);
				return friendAvatarPlaceholder;
			}
		}

		static UIImage RenderSolidImage(UIColor color, CGSize size)
		{
			UIGraphics.BeginImageContextWithOptions (size, false, 0);
			CGContext context = UIGraphics.GetCurrentContext ();
			color.SetFill ();
			context.FillRect (new CGRect (CGPoint.Empty, size));
			UIImage result = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return result;
		}

		public static UIImage GetImage (string url, Predicate<string> needToUpdate)
		{
			var updater = new ImageUpdater (url, needToUpdate);
			return updater.Img;
		}

		public static void SetImage (UIImageView imgView, string url, Predicate<string> needToUpdate)
		{
			// GC will not collect updater because it will be referenced by ImageLoader
			var updater = new ImageUpdater (imgView, url, needToUpdate);
			updater.SetImage ();
		}
	}

	public class ImageUpdater : IImageUpdated
	{
		string url;
		UIImageView imgView;
		Predicate<string> needToUpdate;

		public UIImage Img { get; set; }

		public ImageUpdater (string url, Predicate<string> needToUpdate)
		{
			this.url = url;
			this.needToUpdate = needToUpdate;
			Img = ImageLoader.DefaultRequestImage (new Uri (url), this);
		}

		public ImageUpdater (UIImageView imgView, string url, Predicate<string> needToUpdate)
		{
			this.imgView = imgView;
			this.url = url;
			this.needToUpdate = needToUpdate;
		}

		public void SetImage ()
		{
			if (string.IsNullOrWhiteSpace (url))
				return;

			Img = ImageLoader.DefaultRequestImage (new Uri (url), this);
			if (Img == null)
				return;

			UIView.Transition (imgView, 0.7, UIViewAnimationOptions.TransitionCrossDissolve, () => {
				imgView.Image = Img;
			}, null);
		}

		public void UpdatedImage (Uri uri)
		{
			if (needToUpdate (uri.AbsoluteUri))
				SetImage ();
		}
	}

}

