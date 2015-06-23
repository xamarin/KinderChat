using Foundation;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;

using UIKit;
using CoreGraphics;

using MonoTouch.Dialog.Utilities;
using BigTed;
using System.Threading.Tasks;

namespace KinderChat.iOS
{
	partial class ProfileViewController : UIViewController
	{
		AvatarsCollectionViewSource source;

		ProfileViewModel viewModel;
		CollectionViewSpaceCalculator spaceCalculator;

		#pragma warning disable 0414
		LongPressGestureAttacher gestureAttacher;
		#pragma warning restore

		bool isLayoutInitialized = false;

		List<AvatarItem> avatars;

		public ProfileViewController (IntPtr handle)
			: base (handle)
		{
			TabBarItem.Image = UIImage.FromBundle ("tabIconProfile");
			TabBarItem.Title = Strings.Profile.TabBarTitle;

			var icon = UIImage.FromBundle ("toolbarLinkIcon");
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (icon, UIBarButtonItemStyle.Plain, OnLinkToParentClicked);
		}

		#region Life cycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			viewModel = App.ProfileViewModel;
			SetUpNickNameControls ();
			SetUpCollectionView ();
			SetUpAvatarControl ();

			spaceCalculator = new CollectionViewSpaceCalculator (CollectionView, AvatarCollectionViewCell.CellSize);
			gestureAttacher = new LongPressGestureAttacher (AvatarImg, TakeAvatar);
		}

		public async override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			BindNicName ();
			BindAvatar ();

			ApplyCurrentTheme ();
			Theme.ThemeChanged += OnThemeChanged;

			viewModel.OnNotification += NotificationHandler.OnNotification;
			viewModel.PropertyChanged += OnPropertyChanged;
			viewModel.Avatars.CollectionChanged += OnAvatarsCollectionChanged;
			viewModel.Avatars.UpdatesEnded += OnAvatarsUpdatesEnded;
			UpdateSpinnerState ();

			if (!viewModel.Initialized | viewModel.Avatars.Count == 0) {
				await viewModel.ExecuteLoadAvatarsCommand ().ConfigureAwait (false);
				viewModel.Initialized = true;
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			Theme.ThemeChanged -= OnThemeChanged;
			viewModel.OnNotification -= NotificationHandler.OnNotification;
			viewModel.PropertyChanged -= OnPropertyChanged;
			viewModel.Avatars.CollectionChanged -= OnAvatarsCollectionChanged;
			viewModel.Avatars.UpdatesEnded -= OnAvatarsUpdatesEnded;
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			if (!isLayoutInitialized) {
				isLayoutInitialized = true;
				TryUpdateViewCollectionLayout ();
			}
		}

		#endregion

		#region Initialization

		void SetUpNickNameControls()
		{
			CreateNickNameView ();
			nickNameInput.Hidden = true;
			NavigationItem.TitleView = titleContainer;
		}

		void SetUpCollectionView ()
		{
			avatars = new List<AvatarItem> ();
			source = new AvatarsCollectionViewSource (avatars);
			source.Selected += OnAvatarSelected;
			CollectionView.Source = source;
			CollectionView.RegisterClassForCell (typeof(AvatarCollectionViewCell), AvatarCollectionViewCell.CellId);

			// Visual stuff
			CollectionView.BackgroundColor = UIColor.Clear;
		}

		void SetUpAvatarControl ()
		{
			AvatarImg.Layer.CornerRadius = ImageUtils.CurrentAvatarSize.Width / 2;
			AvatarImg.ClipsToBounds = true;
			AvatarImg.Image = ImageUtils.CurrentAvatarPlaceholder;
			AvatarImg.ContentMode = UIViewContentMode.ScaleAspectFill;
			AvatarImg.UserInteractionEnabled = true; // by default false for ImageView
		}

		#endregion

		#region Theme

		void OnThemeChanged (object sender, EventArgs e)
		{
			ApplyCurrentTheme ();
		}

		void ApplyCurrentTheme ()
		{
			View.BackgroundColor = Theme.Current.BackgroundColor;
			BlendNavBarView.BackgroundColor = Theme.Current.MainColor;

			nickNameLbl.TextColor = Theme.Current.ScreenTitleColor;
		}

		#endregion

		void OnPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			InvokeOnMainThread (() => {
				switch (e.PropertyName) {
					case ProfileViewModel.AvatarUrlName:
						BindAvatar ();
						break;

					case ProfileViewModel.NickNamePropertyName:
						BindNicName ();
						break;

					case BaseViewModel.IsBusyPropertyName:
						UpdateSpinnerState ();
						break;
				}
			});
		}

		async void OnAvatarsCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (viewModel.Avatars.IsBatchUpdates)
				return;

			await UpdateAvatars ().ConfigureAwait (false);
			InvokeOnMainThread (TryUpdateViewCollectionLayout);
		}

		async void OnAvatarsUpdatesEnded (object sender, EventArgs e)
		{
			bool isFirstAppearance = !viewModel.Initialized;

			await UpdateAvatars ().ConfigureAwait (false);

			InvokeOnMainThread (()=> {
				source.ShouldAnimateAppearance = isFirstAppearance;
				TryUpdateViewCollectionLayout();
				CollectionView.LayoutIfNeeded ();
				source.ShouldAnimateAppearance = false;
			});
		}

		async Task UpdateAvatars ()
		{
			await viewModel.AvatarsSemaphore.WaitAsync ();

			avatars.Clear ();
			avatars.AddRange (viewModel.Avatars);

			viewModel.AvatarsSemaphore.Release ();
		}

		void TryUpdateViewCollectionLayout()
		{
			spaceCalculator.SetInsets ();
			CollectionView.ReloadData ();
		}

		void UpdateSpinnerState()
		{
			if (viewModel.IsBusy && viewModel.Avatars.Count == 0)
				Spinner.StartAnimating ();
			else
				Spinner.StopAnimating ();
		}

		void BindAvatar()
		{
			ImageUtils.SetImage(AvatarImg, viewModel.AvatarUrl, url => url == viewModel.AvatarUrl);
		}

		void BindNicName()
		{
			nickNameLbl.Text = viewModel.NickName;
			nickNameInput.Text = viewModel.NickName;
		}

		void OnAvatarSelected (object sender, NSIndexPathEventArgs e)
		{
			viewModel.Avatar = viewModel.Avatars [e.IndexPath.Row].Location;
			viewModel.ExecuteSaveProfileCommand ();
		}

		void OnLinkToParentClicked(object sender, EventArgs e)
		{
			viewModel.ExecuteLinkToParentCommand ();
		}

		#region Nick Name

		UITapGestureRecognizer tap;

		UIView titleContainer;
		UILabel nickNameLbl;
		UITextField nickNameInput;
		UIImageView nickNameEditIcon;

		void CreateNickNameView()
		{
			titleContainer = new UIView (new CGRect(0, 0, 200, 20));

			nickNameLbl = new UILabel {
				TranslatesAutoresizingMaskIntoConstraints = false,
				TextAlignment = UITextAlignment.Center,
			};

			nickNameEditIcon = new UIImageView {
				TranslatesAutoresizingMaskIntoConstraints = false,
				Image = UIImage.FromBundle ("editNickName"),
			};

			nickNameInput = new UITextField {
				TranslatesAutoresizingMaskIntoConstraints = false,
				ShouldReturn = ShouldReturn,
				TextAlignment = UITextAlignment.Center,
				BorderStyle = UITextBorderStyle.RoundedRect,
				BackgroundColor = UIColor.White,
			};
			nickNameInput.Ended += NickNameEditingEnded;

			titleContainer.AddSubviews (nickNameLbl, nickNameInput, nickNameEditIcon);
			Center(nickNameLbl);
			titleContainer.AddConstraint (PlaceOnTheRight (nickNameLbl, nickNameEditIcon, 10));
			CenterVertically (nickNameEditIcon);
			PinLeftRightEdges (nickNameInput);
			CenterVertically (nickNameInput);

			tap = new UITapGestureRecognizer (StartEditNickName) {
				NumberOfTapsRequired = 1
			};
			titleContainer.AddGestureRecognizer (tap);

			ShowNormalNickNameVisualState ();
		}

		void TakeAvatar (UILongPressGestureRecognizer gesture)
		{
			App.MessageDialog.SelectOption ("New Avatar", new[] {
				"Pick Photo from Gallery",
				"Take Photo"
			}, which => {
				if (which == 0)
					viewModel.PickPhoto ();
				else if(which == 1)
					viewModel.TakePhoto ();
			});
		}

		void PinLeftRightEdges(UIView view)
		{
			var c = NSLayoutConstraint.FromVisualFormat ("H:|[view]|",
				NSLayoutFormatOptions.DirectionLeftToRight,
				"view", view
			);
			view.Superview.AddConstraints (c);
		}

		NSLayoutConstraint PlaceOnTheRight(UIView refView, UIView view, float constant = 0f)
		{
			return NSLayoutConstraint.Create (view, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, refView, NSLayoutAttribute.Trailing, 1, constant);
		}

		void Center(UIView view)
		{
			CenterVertically (view);
			CenterHorizontally (view);
		}

		void CenterHorizontally(UIView view)
		{
			var c = MakeEqual (view, view.Superview, NSLayoutAttribute.CenterX);
			view.Superview.AddConstraint (c);
		}

		void CenterVertically(UIView view)
		{
			var c = MakeEqual (view, view.Superview, NSLayoutAttribute.CenterY);
			view.Superview.AddConstraint (c);
		}

		NSLayoutConstraint MakeEqual(UIView first, UIView second, NSLayoutAttribute attribute)
		{
			return NSLayoutConstraint.Create (first, attribute, NSLayoutRelation.Equal, second, attribute, 1, 0);
		}

		void StartEditNickName()
		{
			nickNameLbl.Hidden = true;
			nickNameEditIcon.Hidden = true;
			nickNameInput.Hidden = false;

			// do not allow avatar selection during nickName editing
			View.UserInteractionEnabled = false;
			nickNameInput.BecomeFirstResponder ();
		}

		void ShowNormalNickNameVisualState()
		{
			nickNameLbl.Hidden = false;
			nickNameEditIcon.Hidden = false;
			nickNameInput.Hidden = true;
		}

		void NickNameEditingEnded (object sender, EventArgs e)
		{
			viewModel.NickName = nickNameInput.Text;
			ShowNormalNickNameVisualState ();
		}

		bool ShouldReturn (UITextField textField)
		{
			viewModel.ExecuteSaveProfileCommand ();
			nickNameInput.ResignFirstResponder ();
			View.UserInteractionEnabled = true;
			return false;
		}

		#endregion
	}
}
