using System;

using UIKit;
using Foundation;
using System.Collections.Generic;

namespace KinderChat.iOS
{
	public class AvatarsCollectionViewSource : UICollectionViewSource, ISelectableSource
	{
		public event EventHandler<NSIndexPathEventArgs> Selected;

		readonly List<AvatarItem> avatars;

		public bool ShouldAnimateAppearance { get; set; }

		public AvatarsCollectionViewSource (List<AvatarItem> avatars)
		{
			this.avatars = avatars;
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return avatars.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			int index = indexPath.Row;
			var cell = (AvatarCollectionViewCell)collectionView.DequeueReusableCell(AvatarCollectionViewCell.CellId, indexPath);
			AvatarItem avatar = avatars [index];
			cell.AvatarUrl = avatar.ImageUrl;
			cell.ContentMode = UIViewContentMode.ScaleAspectFill;
			cell.UpdateLayout (avatar.AvatarType);

			if (ShouldAnimateAppearance)
				cell.AnimateAppearance ();

			return cell;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var handler = Selected;
			if (handler != null)
				Selected (this, new NSIndexPathEventArgs { IndexPath = indexPath });
		}
	}
}

