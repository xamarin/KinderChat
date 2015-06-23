using System;
using UIKit;
using KinderChat;
using Foundation;
using CoreGraphics;
using KinderChat.ServerClient.Entities;

namespace KinderChat.iOS
{
	public class FriendsSource : UICollectionViewSource, ISelectableSource, IUICollectionViewDelegateFlowLayout
	{
		public event EventHandler<NSIndexPathEventArgs> Selected;

		FriendsViewModel viewModel;

		public FriendsSource (FriendsViewModel viewModel)
		{
			this.viewModel = viewModel;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return viewModel.Friends.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (UICollectionViewFriendCell)collectionView.DequeueReusableCell (UICollectionViewFriendCell.CellId, indexPath);
			cell.ApplyCurrentTheme ();
			Friend friend = viewModel.Friends [indexPath.Row];

			CGSize size = GetSizeForItem (collectionView, collectionView.CollectionViewLayout, indexPath);
			cell.UpdateLayout(size, friend.AvatarType);
			cell.PrepareForReuse ();
			Bind (friend, cell);

			return cell;
		}

		[Export ("collectionView:layout:sizeForItemAtIndexPath:")]
		public CGSize GetSizeForItem (UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			// 3 friends per row
			var width = collectionView.Frame.Width / 3;
			var height = width + 27;

			return new CGSize (width, height);
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var handler = Selected;
			if (handler != null)
				Selected (this, new NSIndexPathEventArgs { IndexPath = indexPath });
		}

		void Bind(Friend friend, UICollectionViewFriendCell cell)
		{
			cell.AvatarUrl = friend.Photo;
			cell.Name = friend.Name;
		}
	}
}

