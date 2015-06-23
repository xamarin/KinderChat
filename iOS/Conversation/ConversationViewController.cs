using System;
using System.Drawing;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using AudioToolbox;
using System.ComponentModel;
using System.Collections.Specialized;
using CoreFoundation;
using KinderChat.ViewModels.Messages;

namespace KinderChat.iOS
{
	[Register ("ConversationViewController")]
	public class ConversationViewController : UIViewController
	{
		NSObject willShowToken;
		NSObject willHideToken;
		NSObject willHideMenuToken;

		ChatSource tableSource;

		// We need dummy input for keeping keyboard visible during showing menu
		DummyInput hiddenInput;

		UITableView tableView;
		UIToolbar toolbar;

		NSLayoutConstraint toolbarBottomConstraint;
		NSLayoutConstraint toolbarHeightConstraint;

		ConversationViewModel viewModel;

		ChatInputView chatInputView;
		UIButton SendButton {
			get {
				return chatInputView.SendButton;
			}
		}

		UITextView TextView {
			get {
				return chatInputView.TextView;
			}
		}

		public long RecipientId { get; set; }

		public ConversationViewController (IntPtr handle)
			: base (handle)
		{
			HidesBottomBarWhenPushed = true;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			viewModel = new ConversationViewModel (RecipientId);

			hiddenInput = new DummyInput(this) {
				Hidden = true
			};
			View.AddSubview (hiddenInput);

			SetUpTableView ();
			SetUpToolbar ();

			SendButton.TouchUpInside += OnSendClicked;
			TextView.Changed += OnTextChanged;
			TextView.Started += OnTextViewStarted;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			willShowToken = UIKeyboard.Notifications.ObserveWillShow (KeyboardWillShowHandler);
			willHideToken = UIKeyboard.Notifications.ObserveWillHide (KeyboardWillHideHandler);
			willHideMenuToken = UIMenuController.Notifications.ObserveWillHideMenu (MenuWillHide);

			UpdateTableInsets ();
			UpdateButtonState ();

			viewModel.PropertyChanged += OnPropertyChanged;
			viewModel.Messages.CollectionChanged += OnMessagesCollectionChanged;
			viewModel.ExecuteLoadMessagesCommand ();

			ScrollToBottom (false);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			AddObservers ();
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			UpdateTableInsets ();
		}

		#region Initialization

		void SetUpTableView()
		{
			tableView = new UITableView {
				TranslatesAutoresizingMaskIntoConstraints = false,
				AllowsSelection = false
			};
			tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			tableView.RegisterClassForCellReuse (typeof(IncomingCell), IncomingCell.CellId);
			tableView.RegisterClassForCellReuse (typeof(OutgoingCell), OutgoingCell.CellId);
			View.AddSubview (tableView);

			var pinLeft = NSLayoutConstraint.Create (tableView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0);
			View.AddConstraint (pinLeft);

			var pinRight = NSLayoutConstraint.Create (tableView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, 0);
			View.AddConstraint (pinRight);

			var pinTop = NSLayoutConstraint.Create (tableView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, TopLayoutGuide, NSLayoutAttribute.Bottom, 1, 0);
			View.AddConstraint (pinTop);

			var pinBottom = NSLayoutConstraint.Create (tableView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0);
			View.AddConstraint (pinBottom);

			tableSource = new ChatSource (viewModel);
			tableView.Source = tableSource;
		}

		void SetUpToolbar ()
		{
			toolbar = new UIToolbar {
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			chatInputView = new ChatInputView {
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			View.AddSubview (toolbar);

			var pinLeft = NSLayoutConstraint.Create (toolbar, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0);
			View.AddConstraint (pinLeft);

			var pinRight = NSLayoutConstraint.Create (toolbar, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, 0);
			View.AddConstraint (pinRight);

			toolbarBottomConstraint = NSLayoutConstraint.Create (View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, toolbar, NSLayoutAttribute.Bottom, 1, 0);
			View.AddConstraint (toolbarBottomConstraint);

			toolbarHeightConstraint = NSLayoutConstraint.Create (toolbar, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, 44);
			View.AddConstraint (toolbarHeightConstraint);

			toolbar.AddSubview (chatInputView);

			var c1 = NSLayoutConstraint.FromVisualFormat ("H:|[chat_container_view]|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"chat_container_view", chatInputView
			);
			var c2 = NSLayoutConstraint.FromVisualFormat ("V:|[chat_container_view]|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"chat_container_view", chatInputView
			);
			toolbar.AddConstraints (c1);
			toolbar.AddConstraints (c2);
		}

		#endregion

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

		void OnMessagesCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				return;

			if (!viewModel.IsAddingLocalMessages) {
				DispatchQueue.MainQueue.DispatchAsync (() => { 
					var msg = viewModel.Messages [e.NewStartingIndex];
                    if (msg.IsIncoming)
                    {
//						SystemSoundPlayer.PlayMessageReceivedSound ();
//						FinishReceivingMessage (true);
					} else {
//						SystemSoundPlayer.PlayMessageSentSound ();
//						FinishSendingMessage (true);
					}

				});
			}
			//InvokeOnMainThread won't help here
			DispatchQueue.MainQueue.DispatchAsync (() => {
				tableView.ReloadData();
				ScrollToBottom (false);
			});
		}

		void ScrollToBottom (bool animated)
		{
			if (tableView.NumberOfSections() == 0)
				return;

			int items = (int)tableView.NumberOfRowsInSection (0);
			if (items == 0)
				return;

			nfloat contentHeight = tableView.ContentSize.Height;
			bool isTooSmall = contentHeight < tableView.Bounds.Height;

			if (isTooSmall) {
				// From JSQMessanger
				//  workaround for the first few messages not scrolling
				//  when the collection view content size is too small, `scrollToItemAtIndexPath:` doesn't work properly
				//  this seems to be a UIKit bug, see #256 on GitHub
				tableView.ScrollRectToVisible(new CGRect(0, contentHeight - 1, 1, 1), animated);
				return;
			}

			int finalRow = (int)NMath.Max (0, tableView.NumberOfRowsInSection (0) - 1);
			NSIndexPath finalIndexPath = NSIndexPath.FromRowSection (finalRow, 0);
			tableView.ScrollToRow(finalIndexPath, UITableViewScrollPosition.Top, animated);
		}

		void AddObservers()
		{
			TextView.AddObserver(this,"contentSize", NSKeyValueObservingOptions.OldNew, IntPtr.Zero);
		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (keyPath == "contentSize") {
				OnSizeChanged (new NSObservedChange (change));
			} else {
				base.ObserveValue (keyPath, ofObject, change, context);
			}
		}

		void OnSizeChanged(NSObservedChange change)
		{
			CGSize oldValue = ((NSValue)change.OldValue).CGSizeValue;
			CGSize newValue = ((NSValue)change.NewValue).CGSizeValue;

			var dy = newValue.Height - oldValue.Height;
			AdjustInputToolbarOnTextViewSizeChanged (dy);
		}

		void AdjustInputToolbarOnTextViewSizeChanged(nfloat dy)
		{
			bool isIncreasing = dy > 0;
			if (IsInputToolbarHasReachedMaximumHeight () && isIncreasing) {
				// TODO: scroll to bottom
				return;
			}

			nfloat oldY = toolbar.Frame.GetMinY ();
			nfloat newY = oldY - dy;
			if (newY < TopLayoutGuide.Length)
				dy = oldY - TopLayoutGuide.Length;

			AdjustInputToolbar (dy);
		}

		bool IsInputToolbarHasReachedMaximumHeight()
		{
			return toolbar.Frame.GetMinY () == TopLayoutGuide.Length;
		}

		void AdjustInputToolbar(nfloat change)
		{
			toolbarHeightConstraint.Constant += change;

			if (toolbarHeightConstraint.Constant < ChatInputView.ToolbarMinHeight)
				toolbarHeightConstraint.Constant = ChatInputView.ToolbarMinHeight;

			Console.WriteLine (toolbarHeightConstraint.Constant);

			View.SetNeedsUpdateConstraints ();
			View.LayoutIfNeeded ();
		}

		void KeyboardWillShowHandler (object sender, UIKeyboardEventArgs e)
		{
			UpdateButtomLayoutConstraint (e);
		}

		void KeyboardWillHideHandler (object sender, UIKeyboardEventArgs e)
		{
			UpdateButtomLayoutConstraint (e);
		}

		void UpdateButtomLayoutConstraint(UIKeyboardEventArgs e)
		{
			UIViewAnimationCurve curve = e.AnimationCurve;
			UIView.Animate (e.AnimationDuration, 0, ConvertToAnimationOptions (e.AnimationCurve), () => {
//				SetToolbarContstraint(tableView.Frame.GetMaxY () - e.FrameEnd.GetMinY ());
				SetToolbarContstraint(e.FrameEnd.Height);

				// Move content with keyboard
				/*
				var oldOverlap = CalcContentOverlap();
				UpdateTableInsets ();
				var newOverlap = CalcContentOverlap();

				var offset = tableView.ContentOffset;
				offset.Y += newOverlap - oldOverlap;
				offset.Y = NMath.Max(offset.Y, 0);
				tableView.ContentOffset = offset;
				*/
			}, null);
		}

		void SetToolbarContstraint(nfloat constant)
		{
			toolbarBottomConstraint.Constant = constant;
			View.SetNeedsUpdateConstraints();
			View.LayoutIfNeeded ();

			UpdateTableInsets ();
		}

		void UpdateTableInsets()
		{
			nfloat bottom = tableView.Frame.GetMaxY () - toolbar.Frame.GetMinY ();
			UIEdgeInsets insets = new UIEdgeInsets (0, 0, bottom, 0);
			tableView.ContentInset = insets;
			tableView.ScrollIndicatorInsets = insets;
		}

		nfloat CalcContentOverlap()
		{
			var onScreenHeight = CalcOnScreenContentHeight(); 	// >= 0

			// chat's input view with or without keyboard
			var obstacleHeight = tableView.ContentInset.Bottom; // >= 0

			var overlap = NMath.Max(onScreenHeight + obstacleHeight - tableView.Frame.Height, 0);
			return overlap;
		}

		nfloat CalcOnScreenContentHeight()
		{
			// Content which rendered on screen can't be bigger than table's height
			return NMath.Min (tableView.ContentSize.Height - tableView.ContentOffset.Y, tableView.Frame.Height);
		}

		/*
		void UpdateTableInsets()
		{
			UIEdgeInsets oldInset = tableView.ContentInset;
			CGPoint oldOffset = tableView.ContentOffset;

			nfloat hiddenHeight = tableView.Frame.GetMaxY () - toolbar.Frame.GetMinY();

			UIEdgeInsets newInset = oldInset;
			newInset.Bottom = hiddenHeight;

			tableView.ContentInset = newInset; // this may change ContentOffset property implicitly
			tableView.ScrollIndicatorInsets = newInset;
		}
		*/

		bool IsTableContentOverlapped()
		{
			return tableView.ContentSize.Height > tableView.Frame.Height - tableView.ContentInset.Bottom;
		}

		UIViewAnimationOptions ConvertToAnimationOptions(UIViewAnimationCurve curve)
		{
			// Looks like a hack. But it is correct.
			// UIViewAnimationCurve and UIViewAnimationOptions are shifted by 16 bits
			// http://stackoverflow.com/questions/18870447/how-to-use-the-default-ios7-uianimation-curve/18873820#18873820
			return (UIViewAnimationOptions)((int)curve << 16);
		}

		async void OnSendClicked (object sender, EventArgs e)
		{
			var text = TextView.Text;
			TextView.Text = string.Empty; // this will not generate change text event
			UpdateButtonState ();

//			SystemSoundPlayer.PlayMessageSentSound ();
			await viewModel.SendMessage(text);

			/*
			if (string.IsNullOrWhiteSpace (text))
				return;

			var msg = new Message {
				Type = MessageType.Outgoing,
				Text = text.Trim ()
			};

			messages.Add (msg);

			var indexPaths = new NSIndexPath[] { LastIndexPath };
			tableView.InsertRows(indexPaths, UITableViewRowAnimation.None);
			tableView.ScrollToRow (LastIndexPath, UITableViewScrollPosition.Bottom, true);
			*/
		}

		void OnTextChanged (object sender, EventArgs e)
		{
            viewModel.HandleTyping(TextView.Text);
			UpdateButtonState ();
		}

		void OnTextViewStarted (object sender, EventArgs e)
		{
			ScrollToBottom (true);
		}

		void UpdateButtonState()
		{
			SendButton.Enabled = !string.IsNullOrWhiteSpace (TextView.Text);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			willShowToken.Dispose ();
			willHideToken.Dispose ();
			willHideMenuToken.Dispose ();
		}

		[Export("messageCopyTextAction:")]
		internal void CopyMessage(NSObject sender)
		{
			var selected = tableView.IndexPathForSelectedRow;
			var msg = viewModel.Messages [selected.Row];
		    var msgAsText = msg as TextMessageViewModel;
		    var msgAsImage = msg as ImageMessageViewModel;
		    UIPasteboard.General.String = msgAsImage != null ? "Image message" : msgAsText.Text;
		}

		public override bool CanBecomeFirstResponder {
			get {
				return true;
			}
		}

		void MenuWillHide (object sender, NSNotificationEventArgs e)
		{
			var selected = tableView.IndexPathForSelectedRow;
			tableView.DeselectRow (selected, false);
		}

		void PlayMessageSentSound()
		{
			throw new NotImplementedException ();
		}
	}

	public class DummyInput : UITextView
	{
		readonly ConversationViewController viewController;

		public DummyInput (ConversationViewController vc)
		{
			viewController = vc;
		}

		public override bool CanPerform (ObjCRuntime.Selector action, NSObject withSender)
		{
			return action.Name == "messageCopyTextAction:";
		}

		[Export("messageCopyTextAction:")]
		void CopyMessage(NSObject sender)
		{
			viewController.CopyMessage (sender);
		}
	}
}