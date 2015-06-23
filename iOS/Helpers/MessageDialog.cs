using System;
using GCDiscreetNotification;
using UIKit;

namespace KinderChat
{
	public class MessageDialog : IMessageDialog
	{

		public void SendMessage(string message, string title = null)
		{
			Utils.EnsureInvokedOnMainThread (() => {
				var alertView = new UIAlertView (title ?? string.Empty, message, null, "OK");
				alertView.Show ();
			});
		}

		public void SelectOption (string title, string[] options, Action<int> confirmationAction)
		{
			Utils.EnsureInvokedOnMainThread (() => {

				var sheet = new UIActionSheet(title ?? string.Empty);
				foreach(var item in options)
					sheet.AddButton(item);

				sheet.AddButton("Cancel");
				sheet.CancelButtonIndex = sheet.ButtonCount - 1;
				sheet.Dismissed += (sender, e) => confirmationAction ((int)e.ButtonIndex);
				sheet.ShowInView(UIApplication.SharedApplication.KeyWindow);
			});
		}

		public void SendToast(string message)
		{
			var notificationView = new GCDiscreetNotificationView(
				text: message,
				activity: false,
				presentationMode: GCDNPresentationMode.Bottom,
				view: UIApplication.SharedApplication.KeyWindow
			);

			notificationView.ShowAndDismissAfter(4);
		}

		public void SendConfirmation (string message, string title, System.Action<bool> confirmationAction)
		{
			Utils.EnsureInvokedOnMainThread (() => {
				var alertView = new UIAlertView (title ?? string.Empty, message, null, "OK", "Cancel");
				alertView.Clicked += (sender, e) => {
					confirmationAction (e.ButtonIndex == 0);
				};
				alertView.Show ();
			});
		}

		public void AskForString (string message, string title, System.Action<string> returnString)
		{
			Utils.EnsureInvokedOnMainThread (() => {
				var alertView = new UIAlertView (title ?? string.Empty, message, null, "OK", "Cancel");
				alertView.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
				alertView.Clicked += (sender, e) => {
					var text = alertView.GetTextField(0).Text.Trim();
					if(e.ButtonIndex == 0 && !string.IsNullOrWhiteSpace(text))
						returnString (text);
				};
				alertView.Show ();
			});
		}
	}
}

