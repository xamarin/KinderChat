using System;

using UIKit;
using CoreGraphics;
using KinderChat.ViewModels.Messages;

namespace KinderChat.iOS
{
	public abstract class BubbleCell : UITableViewCell, IThemeable
	{
		public UIImageView BubbleView { get; private set; }
		public UILabel MessageLbl { get; private set; }

		public UIImage BubbleImg { get; set; }
		public UIImage BubbleHighlightedImage { get; set; }

        MessageViewModel msg;
		public MessageViewModel Message {
			get {
				return msg;
			}
			set {
				msg = value;
				BubbleView.Image = BubbleImg;
				BubbleView.HighlightedImage = BubbleHighlightedImage;

			    var msgAsText = msg as TextMessageViewModel;
			    var msgAsImage = msg as ImageMessageViewModel;

				MessageLbl.Text = msgAsImage != null ? "Image message" : msgAsText.Text;

				MessageLbl.UserInteractionEnabled = true;
				BubbleView.UserInteractionEnabled = false;
			}
		}

		public BubbleCell (IntPtr handle)
			: base(handle)
		{
			Initialize ();
		}

		public BubbleCell()
		{
			Initialize ();
		}

		void Initialize()
		{
			BubbleView = new UIImageView {
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			MessageLbl = new UILabel {
				TranslatesAutoresizingMaskIntoConstraints = false,
				Lines = 0,
				PreferredMaxLayoutWidth = 220
			};

			ContentView.AddSubview(BubbleView);
			ContentView.AddSubview(MessageLbl);
		}

		public override void SetSelected (bool selected, bool animated)
		{
			base.SetSelected (selected, animated);
			BubbleView.Highlighted = selected;
		}

		protected static UIImage CreateColoredImage(UIColor color, UIImage mask)
		{
			var rect = new CGRect (CGPoint.Empty, mask.Size);
			UIGraphics.BeginImageContextWithOptions (mask.Size, false, mask.CurrentScale);
			CGContext context = UIGraphics.GetCurrentContext ();
			mask.DrawAsPatternInRect (rect);
			context.SetFillColor (color.CGColor);
			context.SetBlendMode (CGBlendMode.SourceAtop);
			context.FillRect (rect);
			UIImage result = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return result;
		}

		public void ApplyCurrentTheme ()
		{
			MessageLbl.Font = Theme.Current.MessageFont;
		}
	}
}