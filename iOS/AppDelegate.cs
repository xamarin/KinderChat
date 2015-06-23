using System;
using Foundation;
using UIKit;
using System.IO;
using System.Linq;

namespace KinderChat.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		const string StartViewControllerId = "StartViewControllerId";
		const string SignUpViewControllerId = "SignUpViewControllerId";

		public Theme BlueTheme { get; private set; }

		public Theme PinkTheme { get; private set; }

		public Theme BlackTheme { get; private set; }

		UIWindow window;

		UIStoryboard MainStoryboard {
			get {
				return UIStoryboard.FromName ("MainStoryboard", NSBundle.MainBundle);
			}
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			Xamarin.Insights.Initialize (App.InsightsKey);
			Xamarin.Insights.ForceDataTransmission = true;
			ServiceContainer.Register<IMessageDialog> (() => new MessageDialog ());
			var dbLocation = "kinder.db3";
			var docsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var libraryPath = Path.Combine (docsPath, "../Library/");
			dbLocation = Path.Combine (libraryPath, dbLocation);
			KinderDatabase.DatabaseLocation = dbLocation;

			ServiceContainer.Register<IUIThreadDispacher> (() => new UiThreadDispacher ());
			ServiceContainer.Register<ILiveConnection> (() => new WebSocketConnection ());
			App.Init ();

			SetupAppearance ();
			Theme.ThemeChanged += OnThemeChanged;

			window.RootViewController = InitStartViewController ();
			window.MakeKeyAndVisible ();

		    App.ConnectionManager.TryKeepConnectionAsync();

			return false;
		}

		void SetupAppearance ()
		{
			SetupCurrentTheme ();
			ApplyCurrentTheme ();
		}

		void OnThemeChanged (object sender, EventArgs e)
		{
			ApplyCurrentTheme ();
		}

		void SetupCurrentTheme ()
		{
			Theme.AvailableThemes.Add (BlueTheme = CreateBlueTheme ());
			Theme.AvailableThemes.Add (BlackTheme = CreateBlackTheme ());
			Theme.AvailableThemes.Add (PinkTheme = CreatePinkTheme ());

			switch (Settings.AppTheme) {
			case AppTheme.Blue:
				Theme.Current = BlueTheme;
				break;

			case AppTheme.Pink:
				Theme.Current = PinkTheme;
				break;

			case AppTheme.Black:
				Theme.Current = BlackTheme;
				break;

			default:
				Theme.Current = BlueTheme;
				break;
			}
		}

		Theme CreateBlueTheme ()
		{
			IColorTheme colorTheme = BlueColorTheme.Instance;
			IFontTheme fontTheme = BlueFontTheme.Instance;
			IImagesTheme imgTheme = BlueImagesTheme.Instance;

			return new Theme (colorTheme, fontTheme, imgTheme);
		}

		Theme CreateBlackTheme ()
		{
			IColorTheme colorTheme = BlackColorTheme.Instance;
			IFontTheme fontTheme = BlackFontTheme.Instance;
			IImagesTheme imgTheme = BlackImagesTheme.Instance;

			return new Theme (colorTheme, fontTheme, imgTheme);
		}

		Theme CreatePinkTheme ()
		{
			IColorTheme colorTheme = PinkColorTheme.Instance;
			IFontTheme fontTheme = PinkFontTheme.Instance;
			IImagesTheme imgTheme = PinkImagesTheme.Instance;

			return new Theme (colorTheme, fontTheme, imgTheme);
		}

		void ApplyCurrentTheme ()
		{
			UIApplication.SharedApplication.StatusBarStyle = Theme.Current.StatusBarStyle;

			UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes {
				ForegroundColor = Theme.Current.ScreenTitleColor,
				Font = Theme.Current.ScreenTitleFont
			};
			// Set back button chevron color
			UINavigationBar.Appearance.TintColor = Theme.Current.ScreenTitleColor;

			UIBarButtonItem.Appearance.SetTitleTextAttributes (new UITextAttributes {
				TextColor = Theme.Current.ScreenTitleColor,
				Font = Theme.Current.NavigationButtonTitleFont
			}, UIControlState.Normal);

			UITabBarItem.Appearance.SetTitleTextAttributes (new UITextAttributes {
				Font = Theme.Current.TabBarItemTitle
			}, UIControlState.Normal);

			var old = window.RootViewController;
			window.RootViewController = null;
			window.RootViewController = old;
		}

		UIViewController InitStartViewController ()
		{
			if (!Settings.IsLoggedIn || App.ForceSignup) {
				return MainStoryboard.InstantiateViewController (SignUpViewControllerId);
			} else {
				return MainStoryboard.InstantiateViewController (StartViewControllerId);
			}
		}

		public void GoToMainScreen ()
		{
			var vc = MainStoryboard.InstantiateViewController (StartViewControllerId);
			window.RootViewController = vc;
		}

		public override void OnResignActivation (UIApplication application)
		{
		}

		public override void DidEnterBackground (UIApplication application)
		{
		}

		public override void WillEnterForeground (UIApplication application)
		{
		}

		public override void WillTerminate (UIApplication application)
		{
		}

		public async override void HandleWatchKitExtensionRequest (UIApplication application, NSDictionary userInfo, Action<NSDictionary> reply)
		{
			if (userInfo ["Profile"] != null && userInfo ["Profile"].ToString () == "Profile") {
				var conversationsViewModel = App.ConversationsViewModel;
				await conversationsViewModel.ExecuteLoadConversationsCommand ();
				var conversationsList = conversationsViewModel.Conversations.ToList ();
				ConversationsStore.Save (conversationsList);

				foreach (var conversation in conversationsList) {
					var conversationViewModel = new ConversationViewModel (conversation.Sender);
					await conversationViewModel.ExecuteLoadMessagesCommand ();
					ConversationsStore.Save (conversationViewModel.Messages.Select(i => i.UnderlyingMessage).ToList (), string.Format ("conversation-{0}.xml", conversation.Sender));
				}

				var viewModel = App.ProfileViewModel;
				NSData avatarImage = !string.IsNullOrEmpty (viewModel.AvatarUrl) ? ImageUtils.GetImage (viewModel.AvatarUrl, url => url == viewModel.AvatarUrl).AsPNG () : new NSData ();

				reply (new NSDictionary ("Username", viewModel.NickName, "Avatar", avatarImage));
			}

			if (userInfo ["TextInput"] != null && userInfo ["TextInput"].ToString () != string.Empty) {
				var number = userInfo ["SenderID"] as NSNumber;
				var conversationViewModel = new ConversationViewModel (number.Int32Value);
				await conversationViewModel.ExecuteLoadMessagesCommand ();
				await conversationViewModel.SendMessage (userInfo ["TextInput"].ToString ());

				reply (new NSDictionary ("Confirmation", "Text was received"));
			}
		}
	}
}
