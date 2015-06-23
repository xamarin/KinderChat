using System;

using UIKit;
using Foundation;
using CoreGraphics;
using System.Collections.Generic;

namespace KinderChat.iOS
{
	public class TaskCollectionViewSource : UICollectionViewSource, ISelectableSource
	{
		public event EventHandler<NSIndexPathEventArgs> Selected;
		public event EventHandler<NSIndexPathEventArgs> Deselected;

		readonly IList<KinderTask> tasks;

		public bool ShouldAnimateAppearance { get; set; }

		public TaskCollectionViewSource (IList<KinderTask> tasks)
		{
			this.tasks = tasks;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var handler = Selected;
			if (handler != null)
				handler (this, new NSIndexPathEventArgs { IndexPath = indexPath });
		}

		public override void ItemDeselected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var handler = Deselected;
			if (handler != null)
				handler (this, new NSIndexPathEventArgs { IndexPath = indexPath });
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return tasks.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, Foundation.NSIndexPath indexPath)
		{
			TaskCell cell = (TaskCell)collectionView.DequeueReusableCell (TaskCell.CellId, indexPath);
			cell.ApplyCurrentTheme ();

			KinderTask task = tasks [indexPath.Row];
			cell.TaskName = task.Name;
			cell.TaskIcon = UIImage.FromBundle (task.IconName);
			cell.SetCellSize (GetCellSize (collectionView));

			cell.Selected = task.IsPending;
			if (task.IsPending)
				collectionView.SelectItem (indexPath, false, UICollectionViewScrollPosition.None);
			else
				collectionView.DeselectItem (indexPath, false);

			if (ShouldAnimateAppearance)
				cell.AnimateIconAppearance ();

			return cell;
		}

		[Export ("collectionView:layout:sizeForItemAtIndexPath:")]
		public CGSize GetSizeForItem (UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			return GetCellSize (collectionView);
		}

		CGSize GetCellSize(UICollectionView collectionView)
		{
			// 3 friends per row
			var width = collectionView.Frame.Width / 3;
			var height = width + 27;

			return new CGSize (width, height);
		}
	}
}

