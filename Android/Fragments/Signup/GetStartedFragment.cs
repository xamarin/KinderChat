using System;
using Android.OS;
using Android.Widget;
using Android.Util;
using Android.Accounts;

namespace KinderChat
{
	public class GetStartedFragment : BaseFragment
	{
		
		public static GetStartedFragment NewInstance ()
		{
			var f = new GetStartedFragment ();
			var b = new Bundle ();
			f.Arguments = b;
			return f;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RetainInstance = true;
		}


		public string GetEmail ()
		{
			var emailPattern = Patterns.EmailAddress; // API level 8+
			var accounts = AccountManager.Get (Activity).GetAccounts ();
			foreach (var account in accounts) {
				if (emailPattern.Matcher (account.Name).Matches ()) {
					return account.Name;
				}
			}

			return string.Empty;
		}
	
		readonly SignUpViewModel viewModel = App.SignUpViewModel;
		Button buttonContinue, havePin;
		public override Android.Views.View OnCreateView (Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
		{
			var root = inflater.Inflate (Resource.Layout.fragment_get_started, container, false);

			var switchIdentity = root.FindViewById<Button> (Resource.Id.switch_identity);
			havePin = root.FindViewById<Button> (Resource.Id.have_pin);
			havePin.Click += (sender, e) => {
				Settings.NickName = viewModel.NickName;
				Settings.Email = viewModel.Identifier;
				((WelcomeActivity)Activity).GoToConfirmation ();
			};
			var identifier = root.FindViewById<EditText> (Resource.Id.identifier);
			identifier.Text = GetEmail ();
			identifier.TextChanged += (sender, e) => {
				viewModel.Identifier = identifier.Text.Trim ();
			};

			var nickname = root.FindViewById<EditText> (Resource.Id.nickname);
			nickname.Text = viewModel.NickName;

			nickname.TextChanged += (sender, e) => {
				viewModel.NickName = nickname.Text.Trim ();
			};

			switchIdentity.Click += (sender, e) => {
				if (viewModel.Identity == SignUpIdentity.Email) {
					viewModel.Identity = SignUpIdentity.Mobile;
					identifier.InputType = Android.Text.InputTypes.ClassPhone;
					identifier.SetHint(Resource.String.input_mobile_placeholder);
					switchIdentity.SetText(Resource.String.use_email);
					identifier.Text = string.Empty;

				} else {
					viewModel.Identity = SignUpIdentity.Email;
					identifier.InputType = Android.Text.InputTypes.TextVariationEmailAddress;
					identifier.SetHint(Resource.String.input_email_placeholder);
					switchIdentity.SetText(Resource.String.use_mobile);
					identifier.Text = string.Empty;
				}
			};

			buttonContinue = root.FindViewById<Button> (Resource.Id.continue_button);
			buttonContinue.Enabled = viewModel.RegisterEnabled;
			buttonContinue.Click += (sender, e) => viewModel.ExecuteRegisterCommand ();
			viewModel.Identifier = identifier.Text;
			buttonContinue.Enabled = viewModel.RegisterEnabled;
			return root;
		}

		void ViewModelPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Activity.RunOnUiThread (() => {
				switch (e.PropertyName) {
				case SignUpViewModel.RegisterEnabledPropertyName:
					buttonContinue.Enabled = havePin.Enabled = viewModel.RegisterEnabled;
					break;
				case SignUpViewModel.CanProgressPropertyName:
					if(viewModel.CanProgress)
						((WelcomeActivity)Activity).GoToConfirmation ();
					break;	
				}
			});
		}

		public override void OnResume ()
		{
			base.OnResume ();
			((WelcomeActivity)Activity).SupportActionBar.SetTitle (Resource.String.signup);
			viewModel.PropertyChanged += ViewModelPropertyChanged;
		}

		public override void OnStop ()
		{
			base.OnStop ();
			viewModel.PropertyChanged -= ViewModelPropertyChanged;
		}

	}
}

