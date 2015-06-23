using System;
using Android.App;
using Android.Content.PM;
using KinderChat.Infrastructure;

namespace KinderChat
{
	
	[Activity (Label = "@string/app_name", ScreenOrientation = ScreenOrientation.Portrait, MainLauncher = true, NoHistory=true, Icon = "@drawable/ic_launcher", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
	public class WelcomeActivity : BaseActivity
	{
		protected override int LayoutResource {
			get {
				return Resource.Layout.activity_welcome;
			}
		}

		SignUpViewModel viewModel;
		protected override void OnCreate (Android.OS.Bundle bundle)
		{
			base.OnCreate (bundle);

            ServiceContainer.Register<IUIThreadDispacher>(() => new UIThreadDispacher());
            ServiceContainer.Register<ILiveConnection>(() => new WebSocketConnection());
			App.Init ();

			//don't need to do this every time unless testing
			if (Settings.IsLoggedIn && !App.ForceSignup) {
				StartActivity (typeof(MainActivity));
				return;
			}

			// Create the transaction
			var fts = SupportFragmentManager.BeginTransaction();
			fts.SetCustomAnimations(Resource.Animation.enter, Resource.Animation.exit, Resource.Animation.pop_enter, Resource.Animation.pop_exit);

			// Replace the content of the container
			fts.Replace(Android.Resource.Id.Content, WelcomeFragment.NewInstance()); 
			// Commit the changes
			fts.Commit();

			SupportActionBar.Title = "Welcome";
			SupportActionBar.SetDisplayHomeAsUpEnabled (false);
			SupportActionBar.SetHomeButtonEnabled (false);

			viewModel = App.SignUpViewModel;
			viewModel.PropertyChanged += ViewModelPropertyChanged;
		}

		void ViewModelPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			RunOnUiThread (() => {
				switch (e.PropertyName) {
				case BaseViewModel.IsBusyPropertyName:
					if (viewModel.IsBusy)
						AndroidHUD.AndHUD.Shared.Show (this, maskType: AndroidHUD.MaskType.Clear);
					else
						AndroidHUD.AndHUD.Shared.Dismiss (this);
					break;
				}
			});
		}

		public void GoToGetStarted()
		{
			// Create the transaction
			var fts = SupportFragmentManager.BeginTransaction();
			fts.SetCustomAnimations(Resource.Animation.enter, Resource.Animation.exit, Resource.Animation.pop_enter, Resource.Animation.pop_exit);

			// Replace the content of the container
			fts.Replace(Android.Resource.Id.Content, GetStartedFragment.NewInstance()); 
			// Append this transaction to the backstack
			fts.AddToBackStack("get_started");
			// Commit the changes
			fts.Commit();

			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetHomeButtonEnabled (true);

			SupportActionBar.Title = "Sign Up";
		}

		public void GoToConfirmation()
		{
			// Create the transaction
			var fts = SupportFragmentManager.BeginTransaction();
			fts.SetCustomAnimations(Resource.Animation.enter, Resource.Animation.exit, Resource.Animation.pop_enter, Resource.Animation.pop_exit);

			// Replace the content of the container
			fts.Replace(Android.Resource.Id.Content, ConfirmationFragment.NewInstance()); 
			// Append this transaction to the backstack
			fts.AddToBackStack("confirmation");
			// Commit the changes
			fts.Commit();

			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetHomeButtonEnabled (true);
		}

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			var showUp = SupportFragmentManager.BackStackEntryCount > 0;
			SupportActionBar.SetDisplayHomeAsUpEnabled (showUp);
			SupportActionBar.SetHomeButtonEnabled (showUp);
		}

		public override bool OnOptionsItemSelected (Android.Views.IMenuItem item)
		{
			if (item.ItemId == Android.Resource.Id.Home) {
				if (SupportFragmentManager.BackStackEntryCount > 0) {
					SupportFragmentManager.PopBackStack ();	
				}


				var showUp = SupportFragmentManager.BackStackEntryCount > 0;
				SupportActionBar.SetDisplayHomeAsUpEnabled (showUp);
				SupportActionBar.SetHomeButtonEnabled (showUp);
			}
			return base.OnOptionsItemSelected (item);
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			if(viewModel != null)
				viewModel.PropertyChanged -= ViewModelPropertyChanged;
		}
	}
}

