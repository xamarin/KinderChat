using System;
using UIKit;
using Foundation;
using System.Collections.Specialized;
using CoreGraphics;
using System.ComponentModel;
using System.Text;
using BigTed;

namespace KinderChat.iOS
{
	[Register("FriendsViewContoller")]
	public partial class FriendsViewContoller : UIViewController, IThemeable
	{
		const string FriendConversationSegueId = "FriendConversationSegueId";

		FriendsViewModel viewModel;
		FriendsSource friendsSource;
		UIRefreshControl refresher;

		#pragma warning disable 0414
		LongPressGestureAttacher gestureAttacher;
		#pragma warning restore

		Friend selectedFriend;

		public FriendsViewContoller (IntPtr handle)
			: base(handle)
		{
			TabBarItem.Image = UIImage.FromBundle ("tabIconBear");
			TabBarItem.Title = Strings.Friends.TabBarTitle;
		}

		#region Life cycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			viewModel = App.FriendsViewModel;
			SetUpCollectionView ();
			SetUpPullToRefresh ();

			var layout = (UICollectionViewFlowLayout)CollectionView.CollectionViewLayout;
			layout.MinimumLineSpacing = 0;
			layout.MinimumInteritemSpacing = 0;
			layout.SectionInset = new UIEdgeInsets (10, 0, 0, 0);

			CollectionView.RegisterClassForCell (typeof(UICollectionViewFriendCell), UICollectionViewFriendCell.CellId);

			gestureAttacher = new LongPressGestureAttacher (CollectionView, (gesture)=>
				{
					var p = gesture.LocationInView(CollectionView);
					var indexPath = CollectionView.IndexPathForItemAtPoint(p);
					if(indexPath == null)
						return;
					var friend = viewModel.Friends[indexPath.Row];
					if(friend == null)
						return;
					viewModel.ExecuteFlagFriendCommand(friend.FriendId, friend.Name);
				});

			var addFriend = new UIBarButtonItem (UIBarButtonSystemItem.Add, delegate {
				App.MessageDialog.SelectOption("Add Friend", new[]{"Enter Email", "Pick Contact"}, (which) =>
					{
						if(which == 0)
						{
							App.MessageDialog.AskForString("Enter friend's email:", "Add Friend", (email)=>
								{
									viewModel.ExecuteSearchForFriendCommand(email);
								});
						}
						else if(which == 1)
						{
							PresentViewController (AddressBookManager.PeoplePicker, true, null);
						}
					}
				);
			});
			NavigationItem.RightBarButtonItem = addFriend;
		}

		AddressBookManager addressBookManager;
		AddressBookManager AddressBookManager {
			get {
				if (addressBookManager == null) {
					addressBookManager = new AddressBookManager ();
					addressBookManager.EmailPicked += OnEmailPicked;
				}
				return addressBookManager;
			}
		}

		#region Search for friend

		void OnEmailPicked (object sender, PersonEventArgs e)
		{
			viewModel.ExecuteSearchForFriendCommand (e.Email);
		}

		#endregion

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			Title = "Friends";
			selectedFriend = null;

			ApplyCurrentTheme ();
			Theme.ThemeChanged += OnThemeChanged;

			viewModel.OnNotification += NotificationHandler.OnNotification;
			viewModel.Friends.CollectionChanged += OnFriendsCollectionChanged;
			viewModel.Friends.UpdatesEnded += OnFriendsUpdatesEnded;

			viewModel.PropertyChanged += OnPropertyChanged;
			UpdateSpinnerState ();

			if (!viewModel.Initialized || viewModel.Friends.Count == 0)
				viewModel.ExecuteLoadFriendsCommand (false);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			Theme.ThemeChanged -= OnThemeChanged;
			viewModel.OnNotification -= NotificationHandler.OnNotification;
			viewModel.Friends.CollectionChanged -= OnFriendsCollectionChanged;
			viewModel.Friends.UpdatesEnded -= OnFriendsUpdatesEnded;
			viewModel.PropertyChanged -= OnPropertyChanged;
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			CollectionView.ContentInset = UIEdgeInsets.Zero;
		}

		#endregion

		#region Initialization

		void SetUpCollectionView()
		{
			friendsSource = new FriendsSource (viewModel);
			friendsSource.Selected += OnFriendSelected;
			CollectionView.Source = friendsSource;
		}

		void SetUpPullToRefresh()
		{
			refresher = new UIRefreshControl ();
			refresher.ValueChanged += OnControllValueChanged;

			CollectionView.AlwaysBounceVertical = true;
			CollectionView.AddSubview (refresher);
		}

		#endregion

		#region Theme

		void OnThemeChanged (object sender, EventArgs e)
		{
			ApplyCurrentTheme ();
		}

		public void ApplyCurrentTheme ()
		{
			View.BackgroundColor = Theme.Current.BackgroundColor;
			CollectionView.BackgroundColor = UIColor.Clear;
			RefresherControl.ApplyCurrentTheme (refresher);
		}

		#endregion

		void OnPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			InvokeOnMainThread (() => {
				switch (e.PropertyName) {
					case BaseViewModel.IsBusyPropertyName:
						UpdateSpinnerState ();
						break;
				}
			});
		}

		void UpdateSpinnerState ()
		{
			if (viewModel.IsBusy)
				StartSpinner ();
			else
				StopSpinner ();
		}

		void StartSpinner ()
		{
			// already shows pull to refresh activity indicator
			if (refresher.Refreshing)
				return;

			if(viewModel.Friends.Count == 0)
				Spinner.StartAnimating ();
		}

		void StopSpinner ()
		{
			Spinner.StopAnimating ();
			if (refresher.Refreshing)
				refresher.EndRefreshing ();
		}

		void OnFriendsCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (viewModel.Friends.IsBatchUpdates)
				return;

			// Friends collection updates as whole
			InvokeOnMainThread (CollectionView.ReloadData);
		}

		void OnFriendsUpdatesEnded (object sender, EventArgs e)
		{
			InvokeOnMainThread (CollectionView.ReloadData);
		}

		void OnControllValueChanged (object sender, EventArgs e)
		{
			viewModel.ExecuteLoadFriendsCommand ();
		}

		void OnFriendSelected (object sender, NSIndexPathEventArgs e)
		{
			selectedFriend = viewModel.Friends [e.IndexPath.Row];
			PerformSegue (FriendConversationSegueId, this);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if(segue.Identifier == FriendConversationSegueId)
				((ConversationViewController)segue.DestinationViewController).RecipientId = selectedFriend.FriendId;
			else
				base.PrepareForSegue (segue, sender);
		}
	}
}