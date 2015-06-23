using System;

namespace KinderChat
{
	public class NotificationEventArgs : EventArgs
	{
		public string Title { get; set; }
		public string Message { get; set; }
	}
}