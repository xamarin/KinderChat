using System;
using Android.OS;
using Android.Widget;

namespace KinderChat
{
	public class WelcomeFragment : BaseFragment
	{
		public static WelcomeFragment NewInstance ()
		{
			var f = new WelcomeFragment ();
			var b = new Bundle ();
			f.Arguments = b;
			return f;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RetainInstance = true;
		}


		RadioButton blue;
		public override Android.Views.View OnCreateView (Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
		{
			var root = inflater.Inflate (Resource.Layout.fragment_welcome, container, false);
			var buttonSignup = root.FindViewById<Button> (Resource.Id.signup);
			blue = root.FindViewById<RadioButton> (Resource.Id.blue);
			buttonSignup.Click += (sender, e) => {
				Settings.AppTheme = blue.Checked ? AppTheme.Blue : AppTheme.Pink;
				((WelcomeActivity)Activity).GoToGetStarted ();
			};
			return root;
		}

		public override void OnResume ()
		{
			base.OnResume ();
			((WelcomeActivity)Activity).SupportActionBar.SetTitle (Resource.String.welcome);
		}
	}
}

