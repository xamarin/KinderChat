using System;
using Android.OS;
using Android.Widget;

namespace KinderChat
{
	public class ConfirmationFragment : BaseFragment
	{
		public static ConfirmationFragment NewInstance ()
		{
			var f = new ConfirmationFragment ();
			var b = new Bundle ();
			f.Arguments = b;
			return f;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RetainInstance = true;
		}

		readonly SignUpViewModel viewModel = App.SignUpViewModel;
		Button buttonContinue;
		public override Android.Views.View OnCreateView (Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
		{
			var root = inflater.Inflate (Resource.Layout.fragment_confirmation, container, false);
			if (viewModel.Identity == SignUpIdentity.Mobile) {
				root.FindViewById<TextView> (Resource.Id.text_top).SetText (Resource.String.description_phone_top);
				root.FindViewById<TextView> (Resource.Id.text_bottom).SetText (Resource.String.description_phone_bottom);

			}
			var pin = root.FindViewById<EditText> (Resource.Id.pin);
			pin.TextChanged += (sender, e) => {
				viewModel.Pin = pin.Text.Trim();
			};
	
			buttonContinue = root.FindViewById<Button> (Resource.Id.continue_button);
			buttonContinue.Click += (sender, e) => {
				viewModel.ExecuteValidatePinCommand();
			};
			return root;
		}

		public override void OnResume ()
		{
			base.OnResume ();
			((WelcomeActivity)Activity).SupportActionBar.Title = viewModel.Identifier;
			viewModel.PropertyChanged += ViewModelPropertyChanged;
		}
		public override void OnStop ()
		{
			base.OnStop ();
			viewModel.PropertyChanged -= ViewModelPropertyChanged;
		}

		void ViewModelPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Activity.RunOnUiThread (() => {
				switch (e.PropertyName) {
				case SignUpViewModel.ValidatePinEnabledPropertyName:
					buttonContinue.Enabled = viewModel.RegisterEnabled;
					break;
				case SignUpViewModel.CanProgressPropertyName:
					if(viewModel.CanProgress)
						Activity.StartActivity (typeof(MainActivity));
					break;
				}
			});
		}
	}
}

