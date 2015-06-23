using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KinderChat.Services.Messages;
using KinderChat.ViewModels.Messages;

namespace KinderChat
{
	public class ConversationViewModel : BaseViewModel
	{
        readonly IDataManager dataManager;
		readonly IMessageRepository messageRepository;
        readonly ObservableCollection<MessageViewModel> messages;
	    readonly TaskCompletionSource<bool> messageLoadTaskCompletionSource = new TaskCompletionSource<bool>();
	    readonly OpponentTypingController opponentTypingController;
	    readonly CurrentUserTypingController currentUserTypingController;
	    readonly IUIThreadDispacher uiThreadDispacher;
	    readonly long recipientId;
        bool isTyping;

		public ConversationViewModel (long recipientId)
		{
            messages = new ObservableCollection<MessageViewModel>();
			dataManager = App.DataManager;
		    messageRepository = App.MessageRepository;
		    App.MessagesManager.MessageArrived += OnMessageArrived;
		    App.MessagesManager.MessageStatusChanged += OnMessageStatusChanged; 
		    this.recipientId = recipientId;
		    IsTyping = isTyping;

		    uiThreadDispacher = ServiceContainer.Resolve<IUIThreadDispacher>();
		    var typingManager = ServiceContainer.Resolve<TypingService>();
            opponentTypingController = new OpponentTypingController(uiThreadDispacher, typingManager);
            currentUserTypingController = new CurrentUserTypingController(recipientId, typingManager, App.FriendsViewModel);
            opponentTypingController.Subscribe(recipientId, i => IsTyping = i);
		}

	    private void OnMessageStatusChanged(object sender, MessageStatusEventArgs e)
	    {
            uiThreadDispacher.Dispatch(() => {
                var msg = Messages.FirstOrDefault(m => m.UnderlyingMessage.MessageToken == e.MessageToken);
                if (msg != null)
                    msg.ChangeStatus(e.Status);
            });
	    }

        public ObservableCollection<MessageViewModel> Messages
		{
			get { return messages; }
		}

		public Friend Friend {
			get;
			private set;
		}

        public const string IsTypingPropertyName = "IsTyping";
	    public bool IsTyping
	    {
	        get { return isTyping; }
	        set { SetProperty(ref isTyping, value); }
	    }

	    ICommand loadMessagesCommand;

	    public ICommand LoadMessagesCommand
		{
			get { return loadMessagesCommand ?? (loadMessagesCommand = new RelayCommand (() => ExecuteLoadMessagesCommand ())); }
		}

		/// <summary>
		/// Gets or sets if add messages from local repository
		/// Use this flag to supress playing incoming/outgoing sound
		/// </summary>
		public bool IsAddingLocalMessages { get; private set; }

		public async Task ExecuteLoadMessagesCommand ()
		{
			if (IsBusy)
				return;

			IsBusy = true;
		
			try {
				Friend = await dataManager.GetFriendAsync(recipientId);
				messages.Clear();

				IsAddingLocalMessages = true;
                List<Message> notSeenYet = new List<Message>();
                foreach (var item in await messageRepository.GetMessagesAsync(recipientId))
				{				
					messages.Add(MessageViewModelFactory.Create(item));
				    if (item.Sender != Settings.MyId && item.Status == MessageStatus.Delivered)
				    {
				        notSeenYet.Add(item);
				    }
				}
			    MarkMessagesAsSeenAsync(notSeenYet);

				IsAddingLocalMessages = false;
			} catch (Exception ex) {
				App.Logger.Report (ex);
				App.MessageDialog.SendMessage ("Unable to gather messages.", "Error");
			} finally {
				IsBusy = false;
				IsAddingLocalMessages = false;
			}
		    messageLoadTaskCompletionSource.TrySetResult(true);

		    //do stuff here
		}

	    private async void MarkMessagesAsSeenAsync(List<Message> notSeenYet)
	    {
	        try
            {
                foreach (var message in notSeenYet)
                {
                    await App.MessagesManager.MarkMessageAsSeen(message);
                }
	        }
	        catch (Exception){}
	    }

	    public void HandleTyping(string text)
	    {
	        if (string.IsNullOrEmpty(text))
	        {
	            currentUserTypingController.HandleClear();
	        }
	        else
	        {
	            currentUserTypingController.HandleTyping();
	        }
	    }

		public async Task SendMessage(string text, byte[] image = null)
        {
		    try
			{
                currentUserTypingController.HandleOutgoingMessage();
				Settings.KinderPoints = Settings.KinderPoints + 1;

                await messageLoadTaskCompletionSource.Task;
				App.ConversationsViewModel.IsDirty = true;
                var message = new Message(Guid.NewGuid(), Guid.Empty, DateTime.UtcNow, recipientId, Settings.MyId, text, image, MessageStatus.Unsent);

                Messages.Add(MessageViewModelFactory.Create(message));

                //chatting with fake contacts
				if (recipientId < 0)
                {
					dataManager.AddMessageAsync(message);
					return;
				}

			    var sendMessageResult = await App.MessagesManager.SendMessageAsync(message);
                switch (sendMessageResult)
                {
                    case SendMessageResult.Ok:
                        break;
                    case SendMessageResult.ConnectionError:
                        App.MessageDialog.SendMessage("Internet connection is required", "No service");
                        break;
                    case SendMessageResult.ReceiverUnknown:
                        App.MessageDialog.SendMessage("Receiver was not found on the server", "Error");
                        break;
                    case SendMessageResult.ReceiverAndSenderAreSame:
                        App.MessageDialog.SendMessage("That's you!", "Error");
                        break;
                    case SendMessageResult.UnknownError:
                        App.MessageDialog.SendMessage("Oops... something went wrong", "Error");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


			    App.Logger.Track("SendMessage");
		    }
		    catch (Exception ex)
            {
				App.Logger.Report (ex);
                App.MessageDialog.SendMessage("Message wasn't sent", "Oops..");
		    }
		}

        private void OnMessageArrived(object sender, MessageEventArgs e)
        {
            uiThreadDispacher.Dispatch(() =>
                {
                    opponentTypingController.HandleIncomingMessage();
                    App.ConversationsViewModel.IsDirty = true;
                    Messages.Add(MessageViewModelFactory.Create(e.Message));
                    //if conversation is opened!
                    MarkMessagesAsSeenAsync(new List<Message> { e.Message });
                });
        }

	    public override void OnClose()
        {
            App.MessagesManager.MessageArrived -= OnMessageArrived;
            App.MessagesManager.MessageStatusChanged -= OnMessageStatusChanged; 
	        base.OnClose();
	    }
	}
}

