using System;

using UIKit;
using Foundation;
using CoreGraphics;

using KinderChat.iOS;

namespace KinderChat.iOS
{
	public partial class OutgoingCell : BubbleCell
	{
		static readonly UIImage normalBubbleImage;
		static readonly UIImage highlightedBubbleImage;

		public static readonly NSString CellId = new NSString("Outgoing");

		static OutgoingCell()
		{
			UIImage mask = UIImage.FromBundle ("MessageBubble");

			var cap = new UIEdgeInsets {
				Top = 17,
				Left = 21,
				Bottom = (float)17.5,
				Right = (float)26.5
			};

			normalBubbleImage = CreateColoredImage (Theme.Current.MainSaturatedColor, mask).CreateResizableImage (cap);

			var highlightedColor = UIColor.FromRGB (32, 96, 200);
			highlightedBubbleImage = CreateColoredImage (highlightedColor, mask).CreateResizableImage (cap);
		}

		public OutgoingCell (IntPtr handle)
			: base(handle)
		{
			Initialize ();
		}

		public OutgoingCell()
		{
			Initialize ();
		}

		void Initialize ()
		{
			BubbleHighlightedImage = highlightedBubbleImage;
			BubbleImg = normalBubbleImage;

			ContentView.AddConstraint (Layout.PinRightEdge (ContentView, BubbleView));
			ContentView.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|-2-[bubble]-2-|",
				NSLayoutFormatOptions.DirectionLeftToRight,
				"bubble", BubbleView
			));
			BubbleView.AddConstraint (Layout.WidthMin (BubbleView, 48));

			var vSpaceTop = NSLayoutConstraint.Create (MessageLbl, NSLayoutAttribute.Top, NSLayoutRelation.Equal, BubbleView, NSLayoutAttribute.Top, 1, 10);
			ContentView.AddConstraint (vSpaceTop);

			var vSpaceBottom = NSLayoutConstraint.Create (MessageLbl, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, BubbleView, NSLayoutAttribute.Bottom, 1, -10);
			ContentView.AddConstraint (vSpaceBottom);

			var msgTrailing = NSLayoutConstraint.Create (MessageLbl, NSLayoutAttribute.Trailing, NSLayoutRelation.LessThanOrEqual, BubbleView, NSLayoutAttribute.Trailing, 1, -16);
			ContentView.AddConstraint (msgTrailing);

			var msgCenter = NSLayoutConstraint.Create (MessageLbl, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, BubbleView, NSLayoutAttribute.CenterX, 1, -3);
			ContentView.AddConstraint (msgCenter);
		}
	}
}