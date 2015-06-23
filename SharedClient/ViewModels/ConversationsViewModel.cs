using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using KinderChat.ServerClient.Managers;
using System.Collections.Generic;

namespace KinderChat
{
	public class ConversationsViewModel : FriendsViewModel
	{
		public bool IsDirty { get; set; }

		readonly SemaphoreSlim newMessagesSemaphore = new SemaphoreSlim (1);
		readonly BatchUpdateObservableCollectoin<Message> conversations = new BatchUpdateObservableCollectoin<Message> ();
		readonly TaskCompletionSource<bool> conversationLoadTaskCompletionSource = new TaskCompletionSource<bool> ();

		public ConversationsViewModel ()
		{
			App.MessagesManager.MessageArrived += OnMessageArrived;
		}

		public BatchUpdateObservableCollectoin<Message> Conversations {
			get {
				return conversations;
			}
		}

		ICommand loadConversationsCommand;

		public ICommand LoadConversationsCommand {
			get { return loadConversationsCommand ?? (loadConversationsCommand = new RelayCommand (() => ExecuteLoadConversationsCommand ())); }
		}

		public async Task ExecuteLoadConversationsCommand ()
		{
			if (IsBusy)
				return;

			IsDirty = false;

			using (BusyContext ()) {
				try {
					var conversations = await GetConversations ();
					LoadToCollection (conversations);
				} catch (Exception ex) {
					App.Logger.Report (ex);
					App.MessageDialog.SendMessage ("Unable to gather converstations.", "Error");
				}
			}
			conversationLoadTaskCompletionSource.TrySetResult (false);
			//do stuff here
		}

		async Task<IEnumerable<Message>> GetConversations ()
		{
			List<Message> messages = await DataManager.GetLatestMessagesAsync ();

			foreach (var msg in messages)
				await FillNameAndPhoto (msg);

			return messages;
		}

		void LoadToCollection (IEnumerable<Message> messages)
		{
			using (conversations.UpdatesBlock ()) {
				conversations.Clear ();
				conversations.AddRange (messages);
			}
		}

		async void OnMessageArrived (object sender, MessageEventArgs e)
		{
			await OnMessageArrived (e.Message);
		}

		async Task OnMessageArrived (Message msg)
		{
			try {
				await conversationLoadTaskCompletionSource.Task; //prevent adding a new conversation before all conversations are loaded
				await newMessagesSemaphore.WaitAsync ();
				await UpdateConversationsWith (msg);
				newMessagesSemaphore.Release ();
			} catch (Exception ex) {
				App.Logger.Report (ex);
			}
		}

		async Task UpdateConversationsWith (Message incomingMsg)
		{
			Message existingConversation = conversations.FirstOrDefault (c => c.ConversationId == incomingMsg.ConversationId);
			if (existingConversation != null)
				ReplaceConversation (existingConversation, incomingMsg);
			else
				await AddConversation (incomingMsg);
		}

		void ReplaceConversation (Message existingConversation, Message incomingMsg)
		{
			conversations.Remove (existingConversation);
			CopyNamePhoto (existingConversation, incomingMsg);
			conversations.Insert (0, incomingMsg); //new conversation on the top
		}

		async Task AddConversation (Message incomingMsg)
		{
			await FillNameAndPhoto (incomingMsg);
			conversations.Insert (0, incomingMsg);
		}

		async Task FillNameAndPhoto (Message msg)
		{
			var friend = await GetFriend (msg);
			CopyNamePhoto (friend, msg);
		}

		async Task<Friend> GetFriend (Message msg)
		{
			long id = GetFriendId (msg);
			var friend = await DataManager.GetFriendAsync (id);

			if (friend == null) {
				friend = new Friend { FriendId = id, Name = string.Empty };
				await App.FriendsViewModel.UpdateFriendInfoAsync (friend);
			}

			return friend;
		}

		long GetFriendId (Message msg)
		{
			// This is equivalent to Settings.MyId == msg.Recipient ? msg.Sender : msg.Recipient;
			// but has a simmetry between Sender and msg.Recipient
			return Settings.MyId ^ (msg.Recipient ^ msg.Sender);
		}

		void CopyNamePhoto (Friend src, Message dst)
		{
			dst.FriendPhoto = src.Photo;
			dst.FriendName = src.Name;
		}

		void CopyNamePhoto (Message src, Message dst)
		{
			dst.FriendPhoto = src.FriendPhoto;
			dst.FriendName = src.FriendName;
		}
	}
}