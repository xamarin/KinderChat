using System;

using UIKit;
using System.Collections.Generic;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using KinderChat.ViewModels.Messages;

namespace KinderChat.iOS
{
	public class ChatSource : UITableViewSource
	{
		readonly ConversationViewModel viewModel;
		IList<MessageViewModel> Messages {
			get {
				return viewModel.Messages;
			}
		}

		readonly BubbleCell[] sizingCells;

		public ChatSource(ConversationViewModel viewModel)
		{
			this.viewModel = viewModel;
			sizingCells = new BubbleCell[2];
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return viewModel.Messages.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			BubbleCell cell = null;
			var msg = viewModel.Messages [indexPath.Row];

			cell = (BubbleCell)tableView.DequeueReusableCell (GetReuseId (msg));
			cell.ApplyCurrentTheme ();
            cell.MessageLbl.TextColor = msg.IsIncoming ? Theme.Current.IncomingTextColor : Theme.Current.OutgoingTextColor;
			cell.Message = msg;

			return cell;
		}

		public override NSIndexPath WillSelectRow (UITableView tableView, NSIndexPath indexPath)
		{
			return null; // Reserve row selection #CopyMessage
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			var msg = Messages [indexPath.Row];
			return CalculateHeightFor (msg, tableView);
		}

		public override nfloat EstimatedHeight (UITableView tableView, NSIndexPath indexPath)
		{
			var msg = Messages [indexPath.Row];
			return CalculateHeightFor (msg, tableView);
		}

		nfloat CalculateHeightFor(MessageViewModel msg, UITableView tableView)
		{
            var index = msg.IsIncoming ? 0 : 1;
			BubbleCell cell = sizingCells [index];
			if (cell == null)
				cell = sizingCells [index] = (BubbleCell)tableView.DequeueReusableCell (GetReuseId(msg));

			cell.ApplyCurrentTheme ();
			cell.Message = msg; 

			cell.SetNeedsLayout ();
			cell.LayoutIfNeeded ();
			CGSize size = cell.ContentView.SystemLayoutSizeFittingSize (UIView.UILayoutFittingCompressedSize);
			return NMath.Ceiling (size.Height) + 1;
		}

		NSString GetReuseId(MessageViewModel message)
		{
			return message.IsIncoming ? IncomingCell.CellId : OutgoingCell.CellId;
		}
	}
}