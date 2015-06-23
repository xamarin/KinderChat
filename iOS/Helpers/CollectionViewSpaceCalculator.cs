using System;
using UIKit;
using CoreGraphics;

namespace KinderChat.iOS
{
	// The purpose of this class is not allow to cut Cells with collection edges
	// Calculator sets insets and spaces to show whole number of cell in content area
	public class CollectionViewSpaceCalculator
	{
		class LayoutInfo
		{
			public nfloat VGap;
			public nfloat HGap;

			public int Rows;
			public int Columns;
		}

		UICollectionView collectionView;
		CGSize cellSize;

		public CollectionViewSpaceCalculator (UICollectionView collectionView, CGSize cellSize)
		{
			this.collectionView = collectionView;
			this.cellSize = cellSize;
		}

		public void SetInsets()
		{
			Console.WriteLine (collectionView);

			var layout = (UICollectionViewFlowLayout)collectionView.CollectionViewLayout;
			layout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
			layout.ItemSize = cellSize;

			var lp = CalcLayoutParameters (collectionView.Frame.Size);
			layout.MinimumLineSpacing = NMath.Floor (lp.VGap);
			layout.MinimumInteritemSpacing = NMath.Floor (lp.HGap);
			layout.SectionInset = new UIEdgeInsets (0, lp.HGap, 0, lp.HGap);
		}

		LayoutInfo CalcLayoutParameters(CGSize size)
		{
			nfloat vGap = 0;
			nfloat hGap = 0;

			// no intest with edges, just row's gap
			int rows = (int)(size.Height / cellSize.Height);
			nfloat vExtraSpace = size.Height - rows * cellSize.Height;
			vGap = rows > 1 ? vExtraSpace / (rows - 1) : 0;

			// equal gap between edges and columns 
			int cols = (int)(size.Width / cellSize.Width);
			nfloat hExtraSpace = size.Width - cols * cellSize.Width;
			hGap = hExtraSpace / (cols + 1);

			return new LayoutInfo {
				HGap = hGap,
				VGap = vGap,
				Columns = cols,
				Rows = rows
			};
		}
	}
}

