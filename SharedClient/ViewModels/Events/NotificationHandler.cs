using System;

namespace KinderChat
{
	public static class NotificationHandler
	{
		public static void OnNotification(object sender, NotificationEventArgs e)
		{
			if (string.IsNullOrEmpty (e.Title)) {
				App.MessageDialog.SendToast (e.Message);
				return;
			}

			App.MessageDialog.SendMessage (e.Message, e.Title);
		}
	}
}