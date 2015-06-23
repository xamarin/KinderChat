using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using com.refractored.fab;
using Android.Support.V4.Widget;
using Android.Content;

namespace KinderChat
{
	class ConversationsFragment : BaseFragment
    {
		
        public static ConversationsFragment NewInstance()
        {
            var f = new ConversationsFragment();
            var b = new Bundle();
            f.Arguments = b;
            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			RetainInstance = true;
        }

		readonly ConversationsViewModel viewModel = App.ConversationsViewModel;
		SwipeRefreshLayout refresher;
		View selectFriend;
		FloatingActionButton fab;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			var root = inflater.Inflate(Resource.Layout.fragment_conversations, container, false);
            var list = root.FindViewById<ListView>(Resource.Id.conversations_list);
            list.ItemClick += OnConversationClick;
			list.Adapter = new ConverstationAdapter(Activity, viewModel);

			var friendGrid = root.FindViewById<GridView> (Resource.Id.grid);
			friendGrid.ItemClick += FriendClicked;
			friendGrid.Adapter = new FriendAdapter (Activity, viewModel);

			selectFriend = root.FindViewById<LinearLayout> (Resource.Id.new_conversation);

			var cancelFriends = root.FindViewById<Button> (Resource.Id.cancel);
			cancelFriends.Click += (sender, e) => {
				fab.Show ();
				selectFriend.Visibility = ViewStates.Gone;
			};

            fab = root.FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += OnStartNewConversationClick;
            fab.AttachToListView(list);

			refresher = root.FindViewById<SwipeRefreshLayout> (Resource.Id.refresher);
			refresher.Refresh += (sender, e) => viewModel.ExecuteLoadConversationsCommand ();


            return root;
        }

        void FriendClicked (object sender, AdapterView.ItemClickEventArgs e)
		{
			var friend = viewModel.Friends [e.Position];
			selectFriend.Visibility = ViewStates.Gone;
			fab.Show ();
			var intent = new Intent (Activity, typeof(ConversationActivity));
			intent.PutExtra (ConversationActivity.RecipientId, friend.FriendId);
			Activity.StartActivity(intent);
        }
			

        void OnConversationClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			
			var message = viewModel.Conversations [e.Position];

			var id = message.Sender == Settings.MyId ? message.Recipient : message.Sender;

			if (id == 0)
				return;

			var intent = new Intent (Activity, typeof(ConversationActivity));
			intent.PutExtra (ConversationActivity.RecipientId, id);
			Activity.StartActivity(intent);
        }

        void OnStartNewConversationClick(object sender, EventArgs e)
        {

			selectFriend.Visibility = ViewStates.Visible;
			fab.Hide ();
			if (viewModel.Friends.Count == 0)
				viewModel.ExecuteLoadFriendsCommand (false);
			
		}

	

		void ViewModelPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Activity.RunOnUiThread (() => {
				switch (e.PropertyName) {
				case BaseViewModel.IsBusyPropertyName:
					refresher.Refreshing = viewModel.IsBusy;
					if (viewModel.IsBusy)
						AndroidHUD.AndHUD.Shared.Show (Activity, Resources.GetString(Resource.String.loading), maskType: AndroidHUD.MaskType.Clear);
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
			fab.Show ();
			Init ();
		}

		public override void OnStop ()
		{
			base.OnStop ();
			viewModel.PropertyChanged -= ViewModelPropertyChanged;
		}

		public override void Init ()
		{
			base.Init ();
			if (!viewModel.Initialized || viewModel.IsDirty) {
				viewModel.Initialized = true;
				viewModel.ExecuteLoadConversationsCommand ();
			}
		}
    }
}