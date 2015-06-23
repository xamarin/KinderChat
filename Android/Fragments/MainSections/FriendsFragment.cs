using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using com.refractored.fab;
using Android.Support.V4.Widget;
using Android.Content;
using Android.App;

namespace KinderChat
{
	class FriendsFragment : BaseFragment, IDialogInterfaceOnClickListener
	{
		public static FriendsFragment NewInstance ()
		{
			var f = new FriendsFragment ();
			var b = new Bundle ();
			f.Arguments = b;
			return f;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RetainInstance = true;
		}

		readonly FriendsViewModel viewModel = App.FriendsViewModel;
		SwipeRefreshLayout refresher;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var root = inflater.Inflate (Resource.Layout.fragment_friends, container, false);
           

			var friendGrid = root.FindViewById<GridView> (Resource.Id.grid);
			friendGrid.ItemClick += FriendClicked;
			friendGrid.Adapter = new FriendAdapter (Activity, viewModel);

			friendGrid.LongClickable = true;
			friendGrid.ItemLongClick += (sender, e) => 
			{

				var friend = viewModel.Friends[e.Position];
				viewModel.ExecuteFlagFriendCommand(friend.FriendId, friend.Name);
			};

			var fab = root.FindViewById<FloatingActionButton> (Resource.Id.fab);
			fab.AttachToListView (friendGrid);
			fab.Click += (sender, e) => {
				var builder = new Android.Support.V7.App.AlertDialog.Builder (Activity);
				builder.SetTitle (Resource.String.add_friend)
					.SetItems (new [] {
					Resources.GetString (Resource.String.add_by_email),
					Resources.GetString (Resource.String.pick_contact)
				}, this);
				builder.Create ().Show ();
			};

			refresher = root.FindViewById<SwipeRefreshLayout> (Resource.Id.refresher);
			refresher.Refresh += (sender, e) => viewModel.ExecuteLoadFriendsCommand ();


			return root;
		}

		public void OnClick (IDialogInterface dialog, int which)
		{
			if (which == 0) {
				App.MessageDialog.AskForString (Resources.GetString (Resource.String.enter_email),
					Resources.GetString (Resource.String.add_by_email),
					email => viewModel.ExecuteSearchForFriendCommand (email));
			} else if (which == 1) {
				var pickContactIntent = new Intent (Intent.ActionPick, Android.Net.Uri.Parse ("content://contacts"));
				pickContactIntent.SetType (Android.Provider.ContactsContract.CommonDataKinds.Email.ContentType); // Show user only contacts w/ phone numbers
				StartActivityForResult (pickContactIntent, 1);
			}
		}

		public override void OnActivityResult (int requestCode, int resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (requestCode != 1)
				return;

			if (resultCode != (int)Result.Ok)
				return;

			var contactData = data.Data;


			var cursor = Activity.ContentResolver.Query (contactData, null, null, null, null);
			if (cursor.MoveToFirst ()) {

				int column = cursor.GetColumnIndex (Android.Provider.ContactsContract.CommonDataKinds.Email.Address);
				var email = cursor.GetString (column);
				viewModel.ExecuteSearchForFriendCommand(email);
			}
		}


		void FriendClicked (object sender, AdapterView.ItemClickEventArgs e)
		{
			var friend = viewModel.Friends [e.Position];
			var intent = new Intent (Activity, typeof(ConversationActivity));
			intent.PutExtra (ConversationActivity.RecipientId, friend.FriendId);
			Activity.StartActivity (intent);
		}

	
		void ViewModelPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Activity.RunOnUiThread (() => {
				switch (e.PropertyName) {
				case BaseViewModel.IsBusyPropertyName:
					refresher.Refreshing = viewModel.IsBusy;
					if (viewModel.IsBusy)
						AndroidHUD.AndHUD.Shared.Show (Activity, viewModel.LoadingMessage, maskType: AndroidHUD.MaskType.Clear);
					else
						AndroidHUD.AndHUD.Shared.Dismiss (Activity);
					break;
				}
			});
		}


		public override void OnResume ()
		{
			base.OnResume ();
			viewModel.PropertyChanged += ViewModelPropertyChanged;
		}

		public override void OnStop ()
		{
			base.OnStop ();
			viewModel.PropertyChanged -= ViewModelPropertyChanged;
		}

		public override void Init ()
		{
			base.Init ();
			if (!viewModel.Initialized) {
				viewModel.Initialized = true;
				viewModel.ExecuteLoadFriendsCommand (false);
			}
		}
	}
}