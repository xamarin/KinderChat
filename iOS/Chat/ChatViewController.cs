using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreGraphics;

using JSQMessagesViewController;

using KinderChat;
using System.Collections.Specialized;
using System.ComponentModel;
using CoreFoundation;
using KinderChat.ViewModels.Messages;

namespace KinderChat.iOS
{
	[Register("ChatViewController")]
	public class ChatViewController : MessagesViewController
	{
		IMessageBubbleImageDataSource incomingBubbleImageData;
		IMessageBubbleImageDataSource outgoingBubbleImageData;

		ConversationViewModel viewModel;

		public long RecipientId { get; set; }

		public ChatViewController(IntPtr handle)
			: base(handle)
		{
			var bubbleFactory = new MessagesBubbleImageFactory ();

			incomingBubbleImageData = bubbleFactory.CreateIncomingMessagesBubbleImage (Theme.Current.BackgroundColor);
			outgoingBubbleImageData = bubbleFactory.CreateOutgoingMessagesBubbleImage (Theme.Current.MainSaturatedColor);

			var border = UIImage.FromBundle ("bubble_stroked");
			var bubble = UIImage.FromBundle ("bubble_regular");

			var bubbleSrc = bubbleFactory.CreateIncomingMessagesBubbleImage (Theme.Current.BackgroundColor);
			var img = CreateBubbleWithBorder (bubble, Theme.Current.BackgroundColor, border, Theme.Current.IncomingBubbleStroke);
			var highlighedImg = bubbleSrc.MessageBubbleHighlightedImage;
			incomingBubbleImageData = new MessagesBubbleImage (img,	highlighedImg);

			HidesBottomBarWhenPushed = true;
		}

		#region Stroke utils

		UIImage CreateBubbleWithBorder(UIImage bubbleImg, UIColor bubbleColor, UIImage borderImg, UIColor borderColor)
		{
			bubbleImg = bubbleImg.ImageMaskedWithColor(bubbleColor);
			borderImg = borderImg.ImageMaskedWithColor (borderColor);

			CGSize size = bubbleImg.Size;
			UIEdgeInsets caps = CenterPointEdgeInsetsForImageSize (size);

			UIGraphics.BeginImageContextWithOptions (size, false, 0);
			var rect = new CGRect (CGPoint.Empty, size);
			bubbleImg.Draw (rect);
			borderImg.Draw (rect);

			var result = UIGraphics.GetImageFromCurrentImageContext ();
			result = Mirror (result);
			UIGraphics.EndImageContext ();

			result = result.CreateResizableImage (caps);
			return result;
		}

		UIImage Mirror(UIImage image)
		{
			var flipOrientation = UIImageOrientation.UpMirrored;
			if (image.Orientation == UIImageOrientation.Down)
				flipOrientation = UIImageOrientation.DownMirrored;

			return new UIImage (image.CGImage, image.CurrentScale, flipOrientation);
		}

		UIEdgeInsets CenterPointEdgeInsetsForImageSize (CGSize size)
		{
			CGPoint p = new CGPoint (size.Width / 2f, size.Height / 2f);
			return new UIEdgeInsets (p.Y, p.X, p.Y, p.X);
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			viewModel = new ConversationViewModel (RecipientId);

			/**
		     *  You MUST set your senderId and display name
		     */
			SenderId = Settings.MyId.ToString();
			SenderDisplayName = Settings.NickName;

			/**
		     *  Load up our fake data for the demo
		     */
			CollectionView.CollectionViewLayout.IncomingAvatarViewSize = CoreGraphics.CGSize.Empty;
			CollectionView.CollectionViewLayout.OutgoingAvatarViewSize = CoreGraphics.CGSize.Empty;
			CollectionView.CollectionViewLayout.MessageBubbleFont = Theme.Current.MessageFont;


			ShowLoadEarlierMessagesHeader = false;


			this.InputToolbar.ContentView.LeftBarButtonItem = null;
			/*NavigationItem.RightBarButtonItem = new UIBarButtonItem (
				BubbleImageFromBundleWithName ("typing"), UIBarButtonItemStyle.Bordered, ReceiveMessagePressed);*/
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			viewModel.PropertyChanged += OnPropertyChanged;
			viewModel.Messages.CollectionChanged += OnMessagesCollectionChanged;
			viewModel.ExecuteLoadMessagesCommand ();
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear (animated);

			viewModel.PropertyChanged -= OnPropertyChanged;
			viewModel.Messages.CollectionChanged -= OnMessagesCollectionChanged;
		}

		void OnMessagesCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				return;


			if (!viewModel.IsAddingLocalMessages) {
				DispatchQueue.MainQueue.DispatchAsync (() => { 
					var msg = viewModel.Messages [e.NewStartingIndex];
					if (msg.IsIncoming) {
							SystemSoundPlayer.PlayMessageReceivedSound ();
							FinishReceivingMessage (true);

					} else {
						
							SystemSoundPlayer.PlayMessageSentSound ();
							FinishSendingMessage (true);
						}

				});
			}
            //InvokeOnMainThread won't help here
			DispatchQueue.MainQueue.DispatchAsync (() => {
				CollectionView.ReloadData();
				ScrollToBottom (false);
			});
		}

		void OnPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			InvokeOnMainThread (() => {
				switch (e.PropertyName) {
				case BaseViewModel.IsBusyPropertyName:
					if (!viewModel.IsBusy) {
						if (viewModel.Friend != null)
							Title = viewModel.Friend.Name;
					}
					break;
				}
			});
		}



		public override async void PressedSendButton (UIButton button, string text, string senderId, string senderDisplayName, NSDate date)
		{
			SystemSoundPlayer.PlayMessageSentSound ();
			await viewModel.SendMessage(text);
		}

		UIImage BubbleImageFromBundleWithName (string name)
		{
			var bundle = NSBundle.FromClass (new ObjCRuntime.Class (typeof(MessagesViewController)));

			string bundleResourcePath = bundle.ResourcePath;
			string assetPath = System.IO.Path.Combine (bundleResourcePath, "JSQMessagesAssets.bundle");
			var messagesAssetBundle = NSBundle.FromPath (assetPath);


			var path = messagesAssetBundle.PathForResource (name, "png", "Images");
			return UIImage.FromFile (path);
		}

		#region CollectionView

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath) 
		{
			var cell =  base.GetCell (collectionView, indexPath) as MessagesCollectionViewCell;

			var message = viewModel.Messages [indexPath.Row];

			var tv = cell.TextView;
			var isIncoming = message.IsIncoming;
			tv.TextColor = isIncoming ? Theme.Current.IncomingTextColor : Theme.Current.OutgoingTextColor;

			return cell;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return viewModel.Messages.Count;
		}

		public override IMessageData GetMessageData (MessagesCollectionView collectionView, NSIndexPath indexPath)
		{
			var msg = viewModel.Messages [indexPath.Row];
		    var msgAsText = msg as TextMessageViewModel;
		    var msgAsImage = msg as ImageMessageViewModel;

			string senderId = msg.IsIncoming ? viewModel.Friend.FriendId.ToString () : SenderId;
            string senderName = msg.IsIncoming ? viewModel.Friend.Name : SenderDisplayName;

			if (string.IsNullOrWhiteSpace (senderName))
				senderName = string.Empty;

			NSDate msgDate = (NSDate)(new DateTime (msg.Timestamp.Ticks, DateTimeKind.Utc));
			IMessageData result = new JSQMessagesViewController.Message (senderId, senderName, msgDate, msgAsImage != null ? "Image message" : msgAsText.Text);
			return result;
		}


		public override IMessageBubbleImageDataSource GetMessageBubbleImageData (MessagesCollectionView collectionView, NSIndexPath indexPath)
		{
			var message = viewModel.Messages [indexPath.Row];
			return message.IsIncoming ? incomingBubbleImageData : outgoingBubbleImageData;
		}

		public override IMessageAvatarImageDataSource GetAvatarImageData (MessagesCollectionView collectionView, NSIndexPath indexPath)
		{
			return null;
		}

		#endregion

		#region Debuging

		static int msgNumber = 0;
		async void ReceiveMessagePressed (object sender, EventArgs args)
		{
			await SimulateDelayedMessageReceived (string.Format ("Another one {0}", ++msgNumber));
		}

		async Task SimulateDelayedMessageReceived (string message)
		{
			ShowTypingIndicator = true;
			ScrollToBottom (true);

			await System.Threading.Tasks.Task.Delay (1500);

            //viewModel.ReceiveMessage(new Message {
            //    Date = DateTime.UtcNow,
            //    Sender = viewModel.Friend.FriendId,
            //    Text = message,
            //    RecipientName = SenderDisplayName,
            //    Recipient = Settings.MyId
            //});
		}

		#endregion
	}
}
