using System;
using Foundation;

namespace KinderChat.iOS
{
	public class AddRemoveReplaceEventArgs : EventArgs
	{
		public NSIndexPath[] IndexPaths { get; set; }
	}
}