using Foundation;
using System;

using UIKit;
using BigTed;
using CoreGraphics;
using System.Linq;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace KinderChat.iOS
{
	partial class MessageListViewController : UITableViewController
	{
		const string ConverstationSegueId = "ConversationSegueId";

		ConversationsViewModel viewModel;
		ConversationsDataSource dataSource;

		UIRefreshControl refresher;
		long friendId;

		readonly List<Message> messageCache = new List<Message>();

		CollectionUpdater<Message> tableUpdater;

		#if DEBUG
		#pragma warning disable 0414
		TapGestureAttacher debugGesture;
		#pragma warning restore

		Server debugServer;
		#endif

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

		public MessageListViewController (IntPtr handle)
			: base (handle)
		{
			TabBarItem.Image = UIImage.FromBundle ("tabIconMessages");
			TabBarItem.Title = Strings.Messages.TabBarTitle;
		}

		#region Life cycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			#if DEBUG
			debugServer = new Server();
			debugServer.OnRequest += HandleDebugRequest;
			debugServer.Start("conversations");

			debugGesture = new TapGestureAttacher (View, 3, ChangeThemeProps);
//			debugGesture = new TapGestureAttacher (View, 3, Theme.SetNextTheme);
			#endif

			Title = Strings.Chats.Title;
			//remove for now because we do on new friends list
			NavigationItem.BackBarButtonItem = new UIBarButtonItem (Strings.Chats.BackButtonTitle, UIBarButtonItemStyle.Plain, null, null);
			//NavigationItem.RightBarButtonItem.Clicked += OnFindNewPersonClicked;
			NavigationItem.RightBarButtonItem = null;

			viewModel = App.ConversationsViewModel;

			tableUpdater = new CollectionUpdater<Message> (viewModel.Conversations, messageCache);

			dataSource = new ConversationsDataSource (messageCache);
			TableView.Source = dataSource;
			TableView.SeparatorInset = UIEdgeInsets.Zero;
			TableView.RowHeight = 82;
			TableView.TableFooterView = new UIView (CGRect.Empty);

			InitPullToRefresh ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			friendId = 0;
			ApplyCurrentTheme ();

			Theme.ThemeChanged += OnThemeChanged;
			dataSource.Selected += OnRowSelected;
			viewModel.PropertyChanged += OnViewModelPropertyChanged;
			viewModel.Conversations.UpdatesEnded += OnConversationsUpdatesEnded;
			viewModel.Conversations.CollectionChanged += OnConversationsCollectionChanged;

			tableUpdater.Add += OnMessagesAdd;
			tableUpdater.Move += OnSingleMessageMove;
			tableUpdater.Remove += OnMessagesRemove;
			tableUpdater.Replace += OnMessagesReplace;
			tableUpdater.Reset += OnMessagesReset;

			if (!viewModel.Initialized || viewModel.IsDirty) {
				viewModel.Initialized = true;
				viewModel.ExecuteLoadConversationsCommand ();
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			Theme.ThemeChanged -= OnThemeChanged;
			dataSource.Selected -= OnRowSelected;
			viewModel.PropertyChanged -= OnViewModelPropertyChanged;
			viewModel.Conversations.UpdatesEnded -= OnConversationsUpdatesEnded;
			viewModel.Conversations.CollectionChanged -= OnConversationsCollectionChanged;

			tableUpdater.Add -= OnMessagesAdd;
			tableUpdater.Move -= OnSingleMessageMove;
			tableUpdater.Remove -= OnMessagesRemove;
			tableUpdater.Replace -= OnMessagesReplace;
			tableUpdater.Reset -= OnMessagesReset;
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion

		void ApplyCurrentTheme()
		{
			View.BackgroundColor = Theme.Current.BackgroundColor;
			RefresherControl.ApplyCurrentTheme (refresher);
		}

		void InitPullToRefresh()
		{
			refresher = new UIRefreshControl ();
			refresher.ValueChanged += OnControllValueChanged;

			RefreshControl = refresher;
		}

		void OnControllValueChanged (object sender, EventArgs e)
		{
			viewModel.ExecuteLoadConversationsCommand ();
		}

		void OnThemeChanged (object sender, EventArgs e)
		{
			TableView.ReloadData ();
		}

		void OnViewModelPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			InvokeOnMainThread (() => {
				switch (e.PropertyName) {
					case BaseViewModel.IsBusyPropertyName:
						if (viewModel.IsBusy)
							refresher.BeginRefreshing ();
						else
							refresher.EndRefreshing ();
						break;
				}
			});
		}

		void OnConversationsCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (viewModel.Conversations.IsBatchUpdates)
				return;

			tableUpdater.EnqueCollectionChange (e);
			InvokeOnMainThread (tableUpdater.PerformCollectionUpdate);
		}

		void OnConversationsUpdatesEnded (object sender, EventArgs e)
		{
			tableUpdater.SyncCache ();
			InvokeOnMainThread (TableView.ReloadData);
		}

		#region Table updates

		void OnMessagesReset (object sender, EventArgs e)
		{
			TableView.ReloadData();
		}

		void OnMessagesReplace (object sender, AddRemoveReplaceEventArgs e)
		{
			TableView.ReloadRows(e.IndexPaths, UITableViewRowAnimation.None);
		}

		void OnMessagesRemove (object sender, AddRemoveReplaceEventArgs e)
		{
			TableView.DeleteRows(e.IndexPaths, UITableViewRowAnimation.None);
		}

		void OnSingleMessageMove (object sender, MoveEventArgs e)
		{
			TableView.MoveRow(e.From, e.To);
		}

		void OnMessagesAdd (object sender, AddRemoveReplaceEventArgs e)
		{
			TableView.InsertRows(e.IndexPaths, UITableViewRowAnimation.None);
		}

		#endregion


		void OnRowSelected (object sender, NSIndexPathEventArgs e)
		{
			var message = viewModel.Conversations [e.IndexPath.Row];
			friendId = 0;

			if (message.Sender == Settings.MyId)
				friendId = message.Recipient;
			else
				friendId = message.Sender;

			if (friendId == 0)
				return;

			PerformSegue (ConverstationSegueId, this);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == ConverstationSegueId) {
				((ConversationViewController)segue.DestinationViewController).RecipientId = friendId;
			} else {
				base.PrepareForSegue (segue, sender);
			}
		}

		#region Search for friend

		void OnFindNewPersonClicked (object sender, EventArgs e)
		{
			PresentViewController (AddressBookManager.PeoplePicker, true, null);
		}

		void OnEmailPicked (object sender, PersonEventArgs e)
		{
			viewModel.ExecuteSearchForFriendCommand (e.Email);
		}

		#endregion

		partial void UpdateTheme (NSObject sender)
		{
			Theme.SetNextTheme ();  // actual theme change
		}

		void ChangeThemeProps ()
		{
			if (Theme.Current.IsDirty) {
				Theme.Current.ResetToDefaults ();
			} else {
				// actual theme change
				Theme.Current.FriendNameColor = UIColor.Cyan; 
				Theme.Current.IncomingBubbleStroke = UIColor.Blue;
			}
		}

		#if DEBUG

		void HandleDebugRequest(object sender, ServerEventArgs e)
		{
			string request = e.Request.Trim ();
			var cmds = request.Split (new char[]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var cmd in cmds)
				HandleCmd (cmd);
		}

		void HandleCmd(string cmd)
		{
			if (cmd.StartsWith ("add"))
				HandleAdd (cmd);
			else if (cmd.StartsWith ("update"))
				HandleUpdate (cmd);
			else if (cmd.StartsWith ("move"))
				HandleMove (cmd);
			else if (cmd.StartsWith ("delete"))
				HandleDelete (cmd);
		}

		List<Friend> friends;

		void HandleUpdate(string request)
		{
			var items = request.Split (new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
			int msgIndex;
			if (int.TryParse (items [1], out msgIndex)) {
				var msg = viewModel.Conversations [msgIndex];
				msg.Text = items [2];
				InvokeOnMainThread (() => {
					viewModel.Conversations [msgIndex] = msg;
				});
			}
		}

		async void HandleAdd(string request)
		{
			friends = friends ?? await App.DataManager.GetFriendsAsync ();

			var items = request.Split (new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
			int friednIndex;
			if (int.TryParse (items [1], out friednIndex)) {
				InvokeOnMainThread (() => {
					var msg = new Message(Guid.NewGuid(), Guid.Empty, DateTime.UtcNow, Settings.MyId, friends[friednIndex].FriendId, items[2], null, MessageStatus.Unsent);
					viewModel.Conversations.Insert(0, msg);
				});
			}
		}

		void HandleMove(string request)
		{
			var items = request.Split (new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);

			int oldIndex, newIndex;
			if (int.TryParse (items [1], out oldIndex) && int.TryParse (items [2], out newIndex)) {
				InvokeOnMainThread (() => {
					viewModel.Conversations.Move (oldIndex, newIndex);
				});
			}
		}

		void HandleDelete (string request)
		{
			var items = request.Split (new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);

			int index;
			if (int.TryParse (items [1], out index)) {
				InvokeOnMainThread (() => {
					viewModel.Conversations.RemoveAt(index);
				});
			}
		}

		#endif
	}
}
