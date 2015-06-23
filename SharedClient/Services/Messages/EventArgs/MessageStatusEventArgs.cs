using System;

namespace KinderChat
{
	public class MessageStatusEventArgs : EventArgs
	{
		public Guid MessageToken { get; set; }
		public MessageStatus Status { get; set; }
	}
}

