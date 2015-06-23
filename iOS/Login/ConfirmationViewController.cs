using System;

using UIKit;
using Foundation;
using BigTed;

namespace KinderChat.iOS
{
	public partial class ConfirmationViewController : UIViewController
	{
		FirstResponderResigner resigner;

		#if DEBUG
		#pragma warning disable 0414

		TapGestureAttacher debugGesture;

		#pragma warning restore
		#endif

		public SignUpViewModel ViewModel { get; set; }

		public ConfirmationViewController (IntPtr handle)
			: base (handle)
		{
			
		}

		#region Life cycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			#if DEBUG
//			debugGesture = new TapGestureAttacher (View, 3, ChangeThemeProps);
			debugGesture = new TapGestureAttacher (View, 3, Theme.SetNextTheme);
			#endif

			resigner = new FirstResponderResigner (View, Input);

			SausageButtons.SetUp (ContinueBtn);

			Input.EditingChanged += InputChangedHandler;
			ContinueBtn.TouchUpInside += OnContinueClicked;
			UpdateUserInterface ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			ContinueBtn.Enabled = ViewModel.ValidatePinEnabled;
			SausageButtons.UpdateBackgoundColor (ContinueBtn);

			ViewModel.PropertyChanged += OnViewModelPropertyChanged;

			ApplyTheme ();
			Theme.ThemeChanged += OnThemeChanged;
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
			Theme.ThemeChanged -= OnThemeChanged;
		}

		#endregion

		void OnThemeChanged (object sender, EventArgs e)
		{
			ApplyTheme ();
		}

		void ApplyTheme ()
		{
			View.BackgroundColor = Theme.Current.BackgroundColor;

			ThemeUtils.ApplyCurrentFont (Input);

			DescriptionTopLbl.Font = Theme.Current.MessageFont;
			DescriptionTopLbl.TextColor = Theme.Current.DescriptionDimmedColor;

			DescriptionBottomlbl.Font = Theme.Current.MessageFont;
			DescriptionBottomlbl.TextColor = Theme.Current.TitleTextColor;

			SausageButtons.ApplyCurrentTheme (ContinueBtn);
			SausageButtons.UpdateBackgoundColor (ContinueBtn);
		}

		void OnViewModelPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			InvokeOnMainThread (() => {
				switch (e.PropertyName) {
					case SignUpViewModel.ValidatePinEnabledPropertyName:
						ContinueBtn.Enabled = ViewModel.ValidatePinEnabled;
						SausageButtons.UpdateBackgoundColor(ContinueBtn);
						break;

					case BaseViewModel.IsBusyPropertyName:
						if(ViewModel.IsBusy)
							BTProgressHUD.Show(null, -1, ProgressHUD.MaskType.Black);
						else
							BTProgressHUD.Dismiss();
						break;

					case SignUpViewModel.CanProgressPropertyName:
						if(ViewModel.CanProgress)
							GoToMainScreen();
						break;
				}
			});
		}

		void InputChangedHandler (object sender, EventArgs e)
		{
			ViewModel.Pin = Input.Text;
		}

		void OnContinueClicked (object sender, EventArgs e)
		{
			if (ViewModel.ValidatePinEnabled) {
				resigner.TryResignFirstResponder ();
				ViewModel.ExecuteValidatePinCommand ();
			}
		}

		void UpdateUserInterface()
		{
			Title = ViewModel.Identifier;

			var isMail = ViewModel.Identity == SignUpIdentity.Email;
			var topDesc = isMail ? Strings.ConfirmationScreen.DescriptionEmailTop : Strings.ConfirmationScreen.DescriptionPhoneTop;
			var bottomDesc = isMail ? Strings.ConfirmationScreen.DescriptionEmailBottom : Strings.ConfirmationScreen.DescriptionPhoneBottom;
			DescriptionTopLbl.Text = topDesc;
			DescriptionBottomlbl.Text = bottomDesc;
		}

		void GoToMainScreen()
		{
			((AppDelegate)UIApplication.SharedApplication.Delegate).GoToMainScreen ();
		}

		void ChangeThemeProps ()
		{
			if (Theme.Current.IsDirty)
				Theme.Current.ResetToDefaults ();
			else
				Theme.Current.BackgroundColor = UIColor.Cyan; // actual theme change
		}
	}
}