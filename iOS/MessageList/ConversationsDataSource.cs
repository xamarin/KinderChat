using System;

using UIKit;
using Foundation;
using KinderChat.Converters;
using System.Collections.Generic;

namespace KinderChat.iOS
{
	public class ConversationsDataSource : UITableViewSource, ISelectableSource
	{
		public event EventHandler<NSIndexPathEventArgs> Selected;
		static readonly NSString cellId = new NSString("ConversationCellId");

//		ConversationsViewModel viewModel;
		IList<Message> messages;

		public ConversationsDataSource (/*ConversationsViewModel viewModel*/ IList<Message> messages)
		{
//			this.viewModel = viewModel;
			this.messages = messages;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (ConversationCell)tableView.DequeueReusableCell(cellId);
			cell.ApplyCurrentTheme ();
			int row = indexPath.Row;

//			Message message = viewModel.Conversations [row];
			var message = messages[row];
			cell.Name = message.FriendName;
			cell.MessageText = message.Text;
			cell.DateText = new TimestampConverter().Convert(message.Date);
			cell.PhotoUrl = message.FriendPhoto;

			return cell;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
//			return viewModel.Conversations.Count;
			return messages.Count;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var handler = Selected;
			if (handler != null)
				handler (this, new NSIndexPathEventArgs { IndexPath = indexPath });
		}
	}
}

