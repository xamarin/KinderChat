using System;
using Foundation;

namespace KinderChat.iOS
{
	public class MoveEventArgs : EventArgs
	{
		public NSIndexPath From { get; set; }
		public NSIndexPath To { get; set; }
	}
}
