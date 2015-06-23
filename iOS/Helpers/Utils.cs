using System;
using UIKit;
using Foundation;

namespace KinderChat
{
	public static class Utils
	{
		public static NSObject Invoker;

		/// <summary>
		/// Ensures the invoked on main thread.
		/// </summary>
		/// <param name="action">Action to run on main thread.</param>
		public static void EnsureInvokedOnMainThread(Action action)
		{
			if (NSThread.Current.IsMainThread)
			{
				action();
				return;
			}
			if (Invoker == null)
				Invoker = new NSObject();

			Invoker.BeginInvokeOnMainThread(() => action());
		}

		public static bool IsiOS7
		{
			get
			{
				return UIDevice.CurrentDevice.CheckSystemVersion(7, 0);
			}
		}

		public static bool IsiOS8
		{
			get
			{
				return UIDevice.CurrentDevice.CheckSystemVersion(8, 0);
			}
		}

		public static bool IsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		//Check if taller than iPhone 4
		public static bool IsTallPhone
		{
			get
			{
				return IsPhone && (UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.Scale >= 1136);
			}
		}
	}

}

