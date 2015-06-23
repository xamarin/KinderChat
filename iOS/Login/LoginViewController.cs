using System;

using UIKit;
using Foundation;
using CoreGraphics;
using BigTed;
using System.ComponentModel;

namespace KinderChat.iOS
{
	public partial class LoginViewController : UIViewController
	{
		NSObject keyboardWillHide;
		NSObject keyboardWillShow;

		// If we are goint to compact layout, store height for future restoration
		nfloat blendViewNormalHeight, bubbleTopNormalOffset;
		NSLayoutConstraint[] compactConstraints;
		NSLayoutConstraint bubbleHeightConstraint, bubbleWidhtConstraint;

		FirstResponderResigner resigner;
		SignUpViewModel viewModel;

		#if DEBUG
		#pragma warning disable 0414
		TapGestureAttacher debugGesture;
		#pragma warning restore
		#endif

		UIColor BarTintColor {
			get {
				return NavigationController.NavigationBar.BarTintColor;
			}
		}

		AppDelegate AppDelegate {
			get {
				return (AppDelegate)UIApplication.SharedApplication.Delegate;
			}
		}

		public LoginViewController (IntPtr handle)
			: base(handle)
		{
		}

		#region Life cycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			viewModel = App.SignUpViewModel;

			resigner = new FirstResponderResigner (View, Input);

			#if DEBUG
//			debugGesture = new TapGestureAttacher (View, 3, ChangeThemeProps);
			debugGesture = new TapGestureAttacher (View, 3, Theme.SetNextTheme);
			#endif

			ContinueBtn.TouchUpInside += ContinueHandler;
			SwitchSignUpType.TouchUpInside += SwitchSignUpTypeHandler;
			Input.EditingChanged += InputChangedHandler;
			NickName.EditingChanged += NickNameInputHandler;

			SausageButtons.SetUp (ContinueBtn);
			SausageButtons.SetUp (SwitchSignUpType);
			SwitchSignUpType.Layer.BorderWidth = 1.5f;

			#region Theme switcher

			BoyButton.SetTitle("Blue", UIControlState.Normal);
			GirlButton.SetTitle("Pink", UIControlState.Normal);

			BoyButton.TouchUpInside += BlueThemeSelected;
			GirlButton.TouchUpInside += GirlThemeSelected;

			ThemeSelectorContainerView.BackgroundColor = Theme.Current.BackgroundColor;

			SausageButtons.SetUp (BoyButton);
			SausageButtons.ApplyTheme(AppDelegate.BlueTheme, BoyButton);
			SausageButtons.UpdateBackgoundColor(AppDelegate.BlueTheme, BoyButton);

			SausageButtons.SetUp (GirlButton);
			SausageButtons.ApplyTheme(AppDelegate.PinkTheme, GirlButton);
			SausageButtons.UpdateBackgoundColor(AppDelegate.PinkTheme, GirlButton);
			#endregion

			UpdateText ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SubscribeKeyboardEvents ();

			ContinueBtn.Enabled = viewModel.RegisterEnabled;
			SausageButtons.UpdateBackgoundColor (ContinueBtn);

			viewModel.PropertyChanged += OnViewModelPropertyChanged;

			ApplyCurrentTheme ();
			Theme.ThemeChanged += OnThemeChanged;
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			viewModel.PropertyChanged -= OnViewModelPropertyChanged;

			Theme.ThemeChanged -= OnThemeChanged;

			keyboardWillHide.Dispose ();
			keyboardWillShow.Dispose ();
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion

		void SubscribeKeyboardEvents()
		{
			keyboardWillShow = UIKeyboard.Notifications.ObserveWillShow(KeyboardShowHandler);
			keyboardWillHide = UIKeyboard.Notifications.ObserveWillHide(KeyboardHideHandler);
		}

		void OnThemeChanged (object sender, EventArgs e)
		{
			ApplyCurrentTheme ();
		}

		void ApplyCurrentTheme()
		{
			NavBarBlendView.BackgroundColor = Theme.Current.MainColor;

			ThemeUtils.ApplyCurrentFont (Input);
			ThemeUtils.ApplyCurrentFont (NickName);

			SausageButtons.ApplyCurrentTheme (ContinueBtn);
			SausageButtons.UpdateBackgoundColor (ContinueBtn);

			SwitchSignUpType.SetTitleColor (Theme.Current.MainSaturatedColor, UIControlState.Normal);
			SwitchSignUpType.Font = Theme.Current.SausageSwitchIdentityButtonFont;
			SwitchSignUpType.Layer.BorderColor = Theme.Current.MainSaturatedColor.CGColor;

			BubbleImg.Image = Theme.Current.ApplyEffects(Theme.Current.SignUpIcon);
		}

		async void ContinueHandler (object sender, EventArgs e)
		{
			resigner.TryResignFirstResponder ();
			await viewModel.ExecuteRegisterCommand ();
			PerformSegue("ConfirmationSegueId", ContinueBtn);
		}

		void SwitchSignUpTypeHandler (object sender, EventArgs e)
		{
			SwitchSignUpIdentityType();

			UpdateText ();
			Input.Text = string.Empty;
			viewModel.Identifier = string.Empty;
		}

		void InputChangedHandler (object sender, EventArgs e)
		{
			viewModel.Identifier = Input.Text;
		}

		void NickNameInputHandler (object sender, EventArgs e)
		{
			viewModel.NickName = NickName.Text;
		}

		// TODO: Move to shared code
		void SwitchSignUpIdentityType()
		{
			switch (viewModel.Identity) {
				case SignUpIdentity.Email:
					viewModel.Identity = SignUpIdentity.Mobile;
					break;

				case SignUpIdentity.Mobile:
					viewModel.Identity = SignUpIdentity.Email;
					break;

				default:
					throw new NotImplementedException ();
			}
		}

		void UpdateText()
		{
			switch (viewModel.Identity) {
				case SignUpIdentity.Email:
					Input.Placeholder = Strings.LoginScreen.InputEmailPlaceholder;
					Input.KeyboardType = UIKeyboardType.EmailAddress;
					SwitchSignUpType.SetTitle (Strings.LoginScreen.UseMobile, UIControlState.Normal);
					break;

				case SignUpIdentity.Mobile:
					Input.Placeholder = Strings.LoginScreen.InputMobilePlaceholder;
					Input.KeyboardType = UIKeyboardType.PhonePad;
					SwitchSignUpType.SetTitle (Strings.LoginScreen.UseEmail, UIControlState.Normal);
					break;
			}

			// Change keyboard type
			resigner.AdjustKeyboard();
		}

		void OnViewModelPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			InvokeOnMainThread (() => {
				switch(e.PropertyName) {
					case SignUpViewModel.RegisterEnabledPropertyName:
						ContinueBtn.Enabled = viewModel.RegisterEnabled;
						SausageButtons.UpdateBackgoundColor(ContinueBtn);
						break;

					case BaseViewModel.IsBusyPropertyName:
						if(viewModel.IsBusy)
							BTProgressHUD.Show(null, -1, ProgressHUD.MaskType.Black);
						else
							BTProgressHUD.Dismiss();
						break;
				}
			});
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "ConfirmationSegueId") {
				((ConfirmationViewController)segue.DestinationViewController).ViewModel = viewModel;
			} else {
				base.PrepareForSegue (segue, sender);
			}
		}

		void KeyboardHideHandler (object sender, UIKeyboardEventArgs e)
		{
			GoToNormalLayout ();
		}

		void KeyboardShowHandler (object sender, UIKeyboardEventArgs e)
		{
			blendViewNormalHeight = NavBarBlendViewHeightConstraint.Constant;
			var keyboardEdge = View.Bounds.Height - e.FrameEnd.Height;

			// verticat space between Input and ContinueBtn
			var h = ContinueBtn.Frame.GetMinY () - Input.Frame.GetMaxY ();

			if (ContinueBtn.Frame.GetMaxY () + h > keyboardEdge) {
				nfloat continueBtnBottomOfsset = e.FrameEnd.Height + h;
				GoToCompactLayout (continueBtnBottomOfsset);
			}
		}

		void GoToCompactLayout(nfloat bottomOffset)
		{
			// Compress NavBarBlendView and use that space to show controls

			NavBarBlendView.RemoveConstraint (NavBarBlendViewHeightConstraint);

			compactConstraints  = NSLayoutConstraint.FromVisualFormat ("V:[btn]-(offset)-|",
				NSLayoutFormatOptions.DirectionRightToLeft,
				"btn", ContinueBtn,
				"offset", (float)bottomOffset
			);

			View.AddConstraints (compactConstraints);

			View.SetNeedsUpdateConstraints ();
			View.LayoutIfNeeded ();
		}

		void GoToNormalLayout()
		{
			if (compactConstraints == null)
				return;

			View.RemoveConstraints (compactConstraints);
			compactConstraints = null;

			NavBarBlendView.AddConstraint (NavBarBlendViewHeightConstraint);

			View.SetNeedsUpdateConstraints ();
			View.LayoutIfNeeded ();
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			var ratio = NavBarBlendView.Bounds.Height / blendViewNormalHeight;
			if (ratio < 1) {
				if (bubbleWidhtConstraint != null || bubbleHeightConstraint != null)
					return;

				bubbleWidhtConstraint = SetConstant (BubbleImg, NSLayoutAttribute.Width, ratio * BubbleImg.IntrinsicContentSize.Width);
				bubbleHeightConstraint = SetConstant (BubbleImg, NSLayoutAttribute.Height, ratio * BubbleImg.IntrinsicContentSize.Height);

				bubbleTopNormalOffset = BubbleImgTopOffset.Constant;
				BubbleImgTopOffset.Constant -= (blendViewNormalHeight - BubbleImg.IntrinsicContentSize.Height) * (1 - ratio) / 2;
				BubbleImgTopOffset.Constant = NMath.Max (BubbleImgTopOffset.Constant, 0);

			} else {
				if (bubbleWidhtConstraint == null || bubbleHeightConstraint == null)
					return;

				BubbleImg.RemoveConstraint (bubbleWidhtConstraint);
				BubbleImg.RemoveConstraint (bubbleHeightConstraint);
				bubbleWidhtConstraint = bubbleHeightConstraint = null;

				BubbleImgTopOffset.Constant = bubbleTopNormalOffset;
			}

			View.LayoutIfNeeded ();
		}

		NSLayoutConstraint SetConstant(UIView view, NSLayoutAttribute attr, nfloat value)
		{
			var c =  NSLayoutConstraint.Create (view, attr, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0, value);
			view.AddConstraint (c);

			return c;
		}

		void BlueThemeSelected (object sender, EventArgs e)
		{
			SetTheme (AppTheme.Blue, AppDelegate.BlueTheme);
		}

		void GirlThemeSelected (object sender, EventArgs e)
		{
			SetTheme (AppTheme.Pink, AppDelegate.PinkTheme);
		}

		void SetTheme(AppTheme themeType, Theme theme)
		{
			Settings.AppTheme = themeType;
			Theme.Current = theme;
			ThemeSelectorContainerView.Hidden = true;
		}

		void ChangeThemeProps ()
		{
			if (Theme.Current.IsDirty)
				Theme.Current.ResetToDefaults ();
			else
				Theme.Current.MainColor = UIColor.Green; // actual theme change
		}
	}
}

