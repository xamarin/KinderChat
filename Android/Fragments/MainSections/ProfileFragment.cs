using Android.OS;
using Android.Views;
using Android.Widget;
using KinderChat.ServerClient.Ws.Proxy;

namespace KinderChat
{
	class ProfileFragment : BaseFragment
    {
		public static ProfileFragment NewInstance()
        {
			var f = new ProfileFragment();
            var b = new Bundle();
            f.Arguments = b;
            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			RetainInstance = true;
			HasOptionsMenu = true;
        }


		readonly ProfileViewModel viewModel = App.ProfileViewModel;
		EditText nickName;
		ImageView avatar;
		GridView avatarGrid;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			var root = inflater.Inflate(Resource.Layout.fragment_profile, container, false);
           

			avatarGrid = root.FindViewById<GridView> (Resource.Id.grid);
			avatarGrid.Adapter = new AvatarAdapter (Activity, viewModel);

			avatarGrid.ItemClick += (sender, e) => {
				var selected = viewModel.Avatars[e.Position];
				viewModel.Avatar = selected.Location;
			};


			nickName = root.FindViewById<EditText> (Resource.Id.nickname);
			avatar = root.FindViewById<ImageView> (Resource.Id.avatar);
			avatar.Clickable = true;
			avatar.Click += (sender, e) => App.MessageDialog.SelectOption ("New Avatar", new[] {
				"Pick Photo from Gallery",
				"Take Photo"
			}, which => {
				if (which == 0)
					viewModel.PickPhoto ();
				else if(which == 1)
					viewModel.TakePhoto ();
			});

			nickName.Text = viewModel.NickName;
			nickName.TextChanged += (sender, e) => 
			{
				viewModel.NickName = nickName.Text;
			};

			Koush.UrlImageViewHelper.SetUrlDrawable (avatar, viewModel.AvatarUrl, Resource.Drawable.ic_launcher);
			viewModel.PropertyChanged += ViewModelPropertyChanged;
            return root;
        }

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.profile, menu);
			base.OnCreateOptionsMenu (menu, inflater);
		}

		public override void OnResume ()
		{
			base.OnResume ();
			if (Settings.FirstRun) {
				Settings.FirstRun = false;
				if (!viewModel.Initialized) {
					viewModel.Initialized = true;
					viewModel.ExecuteLoadAvatarsCommand ();
				}
			}
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			viewModel.PropertyChanged -= ViewModelPropertyChanged;
		}

		public override void Init ()
		{
			base.Init ();
			if (!viewModel.Initialized) {
				viewModel.Initialized = true;
				viewModel.ExecuteLoadAvatarsCommand ();
			}
		}


		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.action_link_to_parent:
				viewModel.ExecuteLinkToParentCommand ();
				break;
			case Resource.Id.action_save:
				viewModel.ExecuteSaveProfileCommand ();
				break;
			case Resource.Id.action_logout:
				App.MessageDialog.SendConfirmation ("Are you sure you want to logout?", "Logout?", (logoff) => {
					if(logoff)
					{
						Settings.Logout();
                        ServiceContainer.Resolve<ConnectionManager>().ForceClose();
						Activity.StartActivity(typeof(WelcomeActivity));
						Activity.Finish();
					}
				});
				break;
			}
			return base.OnOptionsItemSelected (item);
		}
       

		void ViewModelPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Activity.RunOnUiThread (() => {
				switch (e.PropertyName) {
				case BaseViewModel.IsBusyPropertyName:
					if (viewModel.IsBusy)
						AndroidHUD.AndHUD.Shared.Show (Activity, viewModel.LoadingMessage, maskType: AndroidHUD.MaskType.Clear);
					else
						AndroidHUD.AndHUD.Shared.Dismiss (Activity);

					break;
				case ProfileViewModel.AvatarUrlName:


					Koush.UrlImageViewHelper.SetUrlDrawable (avatar, viewModel.AvatarUrl, Resource.Drawable.ic_launcher);

					break;
				}
			});
		}

    }
}