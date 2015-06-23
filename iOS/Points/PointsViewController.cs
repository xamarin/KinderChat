using System;

using UIKit;
using Foundation;
using CoreGraphics;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace KinderChat.iOS
{
	public partial class PointsViewController : UIViewController, IThemeable
	{
		PointsViewModel viewModel;
		TaskCollectionViewSource dataSource;

		// We need local cache because ViewModel can change Tasks collection on another thread
		List<KinderTask> tasks;

		public PointsViewController (IntPtr handle)
			: base(handle)
		{
			Title = "Points";
			TabBarItem.Image = UIImage.FromBundle ("tabIconPoints");
			TabBarItem.Title = "Points";
		}

		#region Life cycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			viewModel = App.PointsViewModel;

			InitCollectionView ();
			InitBadges ();
		}

		public async override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			ApplyCurrentTheme ();
			Theme.ThemeChanged += OnThemeChanged;

			viewModel.PropertyChanged += OnPropertyChanged;
			viewModel.Tasks.UpdatesEnded += OnCollectionUpdatesEnded;

			BindPoints ();
			BindPendingPoints ();

			if (!viewModel.Initialized | viewModel.Tasks.Count == 0) {
				await viewModel.ExecuteLoadKinderTasksCommand ().ConfigureAwait (false);
				viewModel.Initialized = true;
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			Theme.ThemeChanged -= OnThemeChanged;
			viewModel.PropertyChanged -= OnPropertyChanged;
			viewModel.Tasks.UpdatesEnded -= OnCollectionUpdatesEnded;
		}

		#endregion

		#region IThemeable implementation

		public void ApplyCurrentTheme ()
		{
			BlendNavBarView.BackgroundColor = Theme.Current.MainColor;

			LeftBadgeDescriptionLbl.TextColor = Theme.Current.BadgeTitleColor;
			LeftBadgeDescriptionLbl.Font = Theme.Current.TextTitleFont;

			RightBadgeDescriptionLbl.TextColor = Theme.Current.BadgeTitleColor;
			RightBadgeDescriptionLbl.Font = Theme.Current.TextTitleFont;

			LeftBadgeValueLbl.Font = Theme.Current.BadgeValueFont;
			LeftBadgeValueLbl.TextColor = Theme.Current.MainSaturatedColor;

			RightBadgeValueLbl.Font = Theme.Current.BadgeValueFont;
			RightBadgeValueLbl.TextColor = Theme.Current.MainSaturatedColor;

			CollectionViewTitle.Font = Theme.Current.TextTitleFont;
			CollectionViewTitle.TextColor = Theme.Current.TitleTextColor;
		}

		#endregion

		#region Initialization

		void InitCollectionView()
		{
			tasks = new List<KinderTask> ();
			dataSource = new TaskCollectionViewSource (tasks);
			dataSource.Selected += OnItemSelected;
			dataSource.Deselected += OnItemDeselected;
			CollectionView.Source = dataSource;
			CollectionView.RegisterClassForCell (typeof(TaskCell), TaskCell.CellId);
			CollectionView.AllowsMultipleSelection = true;
			CollectionView.BackgroundColor = UIColor.Clear;

			var layout = (UICollectionViewFlowLayout)CollectionView.CollectionViewLayout;
			layout.MinimumLineSpacing = 0;
			layout.MinimumInteritemSpacing = 0;
			layout.SectionInset = UIEdgeInsets.Zero;
		}

		void OnItemSelected (object sender, NSIndexPathEventArgs e)
		{
			HandleSelectionDeselection (e.IndexPath);
		}

		void OnItemDeselected (object sender, NSIndexPathEventArgs e)
		{
			HandleSelectionDeselection (e.IndexPath);
		}

		void HandleSelectionDeselection (NSIndexPath path)
		{
			int index = (int)path.Row;
			KinderTask task = tasks [index];
			viewModel.Apply (task);
		}

		void InitBadges ()
		{
			MakeCircleBadge (LeftBadgeContainer);
			MakeCircleBadge (RightBadgeContainder);

			LeftBadgeDescriptionLbl.Text = "Points";
			RightBadgeDescriptionLbl.Text = "Pending";

			LeftBadgeValueLbl.AdjustsFontSizeToFitWidth = true;
			RightBadgeValueLbl.AdjustsFontSizeToFitWidth = true;
		}

		void MakeCircleBadge(UIView badge)
		{
			badge.Layer.CornerRadius = 105f / 2f;
			badge.Layer.ShadowOffset = new CGSize (0, 0.5f);
			badge.Layer.ShadowRadius = 0;
			badge.Layer.ShadowOpacity = 0.2f;
			badge.Layer.ShadowColor = UIColor.Black.CGColor;
		}

		#endregion

		void OnThemeChanged (object sender, EventArgs e)
		{
			CollectionView.ReloadData ();
		}

		void OnPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			BeginInvokeOnMainThread (() => {
				switch(e.PropertyName) {
					case BaseViewModel.IsBusyPropertyName:
						UpdateSpinnerState ();
						break;

					case PointsViewModel.PointsPropertyName:
						BindPoints ();
						break;

					case PointsViewModel.PendingPointsName:
						BindPendingPoints ();
						break;
				}
			});
		}

		void OnCollectionUpdatesEnded (object sender, EventArgs e)
		{
			bool isFirstAppearance = !viewModel.Initialized;
			BeginInvokeOnMainThread (async () => {
				await viewModel.TaskSemaphore.WaitAsync ();
				tasks.Clear ();
				tasks.AddRange (viewModel.Tasks);
				viewModel.TaskSemaphore.Release ();

				dataSource.ShouldAnimateAppearance = isFirstAppearance;
				CollectionView.ReloadData ();
				CollectionView.LayoutIfNeeded ();
				dataSource.ShouldAnimateAppearance = false;
			});
		}

		void BindPoints ()
		{
			LeftBadgeValueLbl.Text = viewModel.Points.ToString ();
		}

		void BindPendingPoints ()
		{
			RightBadgeValueLbl.Text = viewModel.PendingPoints.ToString ();
		}

		void UpdateSpinnerState ()
		{
			if (viewModel.IsBusy)
				Spinner.StartAnimating ();
			else
				Spinner.StopAnimating ();
		}
	}
}