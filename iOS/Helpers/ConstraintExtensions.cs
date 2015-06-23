using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;

namespace KinderChat.iOS
{
	public static class ConstraintExtensions
	{
		#region Width Height

		public static void Height (this UIView view, nfloat height)
		{
			AssertNotNull (view);
			view.AddConstraint (NSLayoutConstraint.Create (view, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, height));
		}

		public static void Width (this UIView view, nfloat width)
		{
			AssertNotNull (view);
			view.AddConstraint (NSLayoutConstraint.Create (view, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, width));
		}

		public static void Size (this UIView view,  CGSize size)
		{
			Width (view, size.Width);
			Height (view, size.Height);
		}

		#endregion

		#region Center

		public static void CenterX (this UIView first)
		{
			UIView superview = AssertSuperview (first);
			CenterX (first, superview);
		}

		public static void CenterX (this UIView first, UIView second)
		{
			InstallEqual (NSLayoutAttribute.CenterX, first, second);
		}

		public static void CenterY (this UIView first)
		{
			UIView superview = AssertSuperview (first);
			CenterY (first, superview);
		}

		public static void CenterY (this UIView first, UIView second)
		{
			InstallEqual (NSLayoutAttribute.CenterY, first, second);
		}

		public static void CenterXY (this UIView first)
		{
			var superview = AssertSuperview (first);
			CenterX (first, superview);
			CenterY (first, superview);
		}

		public static void CenterXY (this UIView first, UIView second)
		{
			CenterX (first, second);
			CenterY (first, second);
		}

		#endregion

		static void InstallEqual(NSLayoutAttribute attribute, UIView first, UIView second)
		{
			UIView commonAncestor = AssertCommonAncestor (first, second);

			var c = CreateEqual (attribute, first, second);
			commonAncestor.AddConstraint (c);
		}

		static NSLayoutConstraint CreateEqual(NSLayoutAttribute attribute, UIView first, UIView second)
		{
			return NSLayoutConstraint.Create (first, attribute, NSLayoutRelation.Equal, second, attribute, 1, 0);
		}

		static UIView FindCommonAncestor (UIView first, UIView second)
		{
			AssertNotNull (first);
			AssertNotNull (second);

			HashSet<UIView> map = new HashSet<UIView> ();

			do {
				if (!map.Add (first))
					return first;
				if (!map.Add (second))
					return second;

				first = first.Superview;
				second = second.Superview;
			} while (first != null || second != null);

			return null;
		}

		#region Assert

		static void AssertNotNull (UIView view)
		{
			if (view == null)
				throw new ArgumentNullException ();
		}

		static UIView AssertSuperview (UIView view)
		{
			AssertNotNull (view);

			var superview = view.Superview;
			if (superview == null)
				throw new ArgumentException ("Parent view is null");

			return superview;
		}

		static UIView AssertCommonAncestor (UIView first, UIView second)
		{
			UIView commonAncestor = FindCommonAncestor (first, second);
			if (commonAncestor == null)
				throw new Exception ("view don't have common ancestor");

			return commonAncestor;
		}

		#endregion

	}
}