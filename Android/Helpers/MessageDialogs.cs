using System;
using Android.Widget;
using Android.Views;

namespace KinderChat
{
	public class MessageDialog : IMessageDialog
	{

		public void SendMessage (string message, string title = null)
		{
			var builder = new Android.Support.V7.App.AlertDialog.Builder (BaseActivity.CurrentActivity);
			builder
				.SetTitle (title ?? string.Empty)
				.SetMessage (message)
				.SetPositiveButton (Android.Resource.String.Ok, delegate {
			});             

			BaseActivity.CurrentActivity.RunOnUiThread (() => {
				var alert = builder.Create ();
				alert.Show ();
			});
		}


		public void SendToast (string message)
		{
			BaseActivity.CurrentActivity.RunOnUiThread (() => Toast.MakeText (BaseActivity.CurrentActivity, message, ToastLength.Long).Show ());
		}

		public void SelectOption (string title, string[] options, Action<int> confirmationAction)
		{
			var builder = new Android.Support.V7.App.AlertDialog.Builder (BaseActivity.CurrentActivity);
			builder.SetTitle (title)
				.SetItems (options, (sender, args) =>
					confirmationAction (args.Which))
				.SetNegativeButton ("Cancel", delegate {
			});
			
			BaseActivity.CurrentActivity.RunOnUiThread (() => builder.Create ().Show ());
		}


		public void SendConfirmation (string message, string title, Action<bool> confirmationAction)
		{
			var builder = new Android.Support.V7.App.AlertDialog.Builder (BaseActivity.CurrentActivity);
			builder
				.SetTitle (title ?? string.Empty)
				.SetMessage (message)
				.SetPositiveButton (Android.Resource.String.Ok, delegate {
				confirmationAction (true);
			})
				.SetNegativeButton (Android.Resource.String.Cancel, delegate {
				confirmationAction (false);
			});


			BaseActivity.CurrentActivity.RunOnUiThread (() => {
				var alert = builder.Create ();
				alert.Show ();
			});
		}

		public void AskForString (string message, string title, Action<string> returnString)
		{

			var builder = new Android.Support.V7.App.AlertDialog.Builder (BaseActivity.CurrentActivity);
			builder.SetIcon (Resource.Drawable.ic_launcher);
			builder.SetTitle (title ?? string.Empty);
			builder.SetMessage (message);
			var view = View.Inflate (BaseActivity.CurrentActivity, Resource.Layout.dialog_ask_text, null);
			builder.SetView (view);

			var textBoxName = view.FindViewById<EditText> (Resource.Id.text);
			builder.SetCancelable (true);
			builder.SetNegativeButton (Android.Resource.String.Cancel, delegate {
				
			});
			builder.SetPositiveButton (Android.Resource.String.Ok, (sender, which) => {

				if (string.IsNullOrWhiteSpace (textBoxName.Text))
					return;

				returnString (textBoxName.Text);
			});

			BaseActivity.CurrentActivity.RunOnUiThread (() => {
				var alertDialog = builder.Create ();
				alertDialog.Show ();
			});


		}
	}
}

