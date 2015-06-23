using System;

using UIKit;
using CoreGraphics;
using System.Collections.Generic;

namespace KinderChat.iOS
{
	public static class Layout
	{
		public static NSLayoutConstraint[] PinLeftRightEdges(UIView view, float left = 0, float right = 0)
		{
			return new NSLayoutConstraint[] {
				CreateEqual(NSLayoutAttribute.Left, view.Superview, view, left),
				CreateEqual(NSLayoutAttribute.Right, view.Superview, view, -right),
			};
		}

		public static NSLayoutConstraint[] PinLeftRightMargins(UIView view)
		{
			return new NSLayoutConstraint[] {
				PinLeftMargin(view.Superview, view),
				PinRightMargin(view.Superview, view)
			};
		}

		public static NSLayoutConstraint PinLeftMargin(UIView refView, UIView view)
		{
			return NSLayoutConstraint.Create (view, NSLayoutAttribute.Left, NSLayoutRelation.Equal, refView, NSLayoutAttribute.LeftMargin, 1, 0);
		}

		public static NSLayoutConstraint PinRightMargin(UIView refView, UIView view)
		{
			return NSLayoutConstraint.Create (view, NSLayoutAttribute.Right, NSLayoutRelation.Equal, refView, NSLayoutAttribute.RightMargin, 1, 0);
		}

		public static NSLayoutConstraint PinLeftEdge(UIView refView, UIView view, float left = 0)
		{
			return CreateEqual (NSLayoutAttribute.Left, refView, view, -left);
		}

		public static NSLayoutConstraint PinRightEdge(UIView refView, UIView view, float right = 0)
		{
			return CreateEqual (NSLayoutAttribute.Right, refView, view, right);
		}

		public static NSLayoutConstraint PinTopEdge(UIView refView, UIView view, float top = 0)
		{
			return CreateEqual (NSLayoutAttribute.Top, refView, view, -top);
		}

		public static NSLayoutConstraint PinBottomEdge(UIView refView, UIView view, float bottom = 0)
		{
			return CreateEqual (NSLayoutAttribute.Bottom, refView, view, bottom);
		}

		public static NSLayoutConstraint Width(UIView view, nfloat width)
		{
			return SetEqual (NSLayoutAttribute.Width, view, width);
		}

		public static NSLayoutConstraint WidthMin(UIView view, nfloat width)
		{
			return SetGE (NSLayoutAttribute.Width, view, width);
		}

		public static NSLayoutConstraint Height(UIView view, nfloat height)
		{
			return SetEqual (NSLayoutAttribute.Height, view, height);
		}

		public static NSLayoutConstraint[] SetCenter(UIView refView, UIView view, CGPoint centerPoint)
		{
			return new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (view, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, refView, NSLayoutAttribute.Left, 1, centerPoint.X),
				NSLayoutConstraint.Create (view, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, refView, NSLayoutAttribute.Top, 1, centerPoint.Y)
			};
		}

//		public static NSLayoutConstraint CenterX(UIView refView, UIView view)
//		{
//			return CreateEqual (NSLayoutAttribute.CenterX, refView, view);
//		}

		public static NSLayoutConstraint VerticalSpacing(UIView above, UIView below)
		{
			return VerticalSpacing (above, below, 0);
		}

		public static NSLayoutConstraint VerticalSpacing(UIView above, UIView below, nfloat space)
		{
			return NSLayoutConstraint.Create (below, NSLayoutAttribute.Top, NSLayoutRelation.Equal, above, NSLayoutAttribute.Bottom, 1, space);
		}

		static NSLayoutConstraint SetEqual(NSLayoutAttribute attribute, UIView view, nfloat value)
		{
			return NSLayoutConstraint.Create (view, attribute, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, value);
		}

		static NSLayoutConstraint SetLE(NSLayoutAttribute attribute, UIView view, nfloat value)
		{
			return NSLayoutConstraint.Create (view, attribute, NSLayoutRelation.LessThanOrEqual, null, NSLayoutAttribute.NoAttribute, 0, value);
		}

		static NSLayoutConstraint SetGE(NSLayoutAttribute attribute, UIView view, nfloat value)
		{
			return NSLayoutConstraint.Create (view, attribute, NSLayoutRelation.GreaterThanOrEqual, null, NSLayoutAttribute.NoAttribute, 0, value);
		}

		static NSLayoutConstraint CreateEqual(NSLayoutAttribute attribute, UIView refView, UIView view, float constant = 0)
		{
			return NSLayoutConstraint.Create (view, attribute, NSLayoutRelation.Equal, refView, attribute, 1, constant);
		}

		static NSLayoutConstraint Set(NSLayoutAttribute attribute, UIView first, UIView second, float constant)
		{
			return NSLayoutConstraint.Create (first, attribute, NSLayoutRelation.Equal, second, attribute, 1, constant);
		}
	}
}

