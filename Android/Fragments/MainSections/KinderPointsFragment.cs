using Android.OS;
using Android.Views;
using Android.Widget;

namespace KinderChat
{
	class KinderPointsFragment : BaseFragment
    {
		public static KinderPointsFragment NewInstance()
        {
			var f = new KinderPointsFragment();
            var b = new Bundle();
            f.Arguments = b;
            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			RetainInstance = true;
        }


		TextView points, pendingPoints;
		bool tapped;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			var root = inflater.Inflate(Resource.Layout.fragment_kinderpoints, container, false);
			points = root.FindViewById<TextView> (Resource.Id.kinder_points);
			pendingPoints = root.FindViewById<TextView> (Resource.Id.kinder_points_pending);

			var list = root.FindViewById<GridView> (Resource.Id.grid);
			list.Adapter = new ItemAdapter (Activity);

			list.ItemClick += (sender, e) => {
				if(Settings.KinderPointsPending > 0){
					App.MessageDialog.SendToast("Points pending, please try again later.");
					return;
				}
				Settings.KinderPointsPending = Settings.KinderPointsPending + 100;
				UpdatePoints();
			};
            return root;
        }

		private void UpdatePoints()
		{
			points.Text = Settings.KinderPoints.ToString ();
			pendingPoints.Text = Settings.KinderPointsPending.ToString ();
		}

		public override void OnResume ()
		{
			base.OnResume ();
			UpdatePoints ();
		}

		public override void Init ()
		{
			base.Init ();
			UpdatePoints ();
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate (Resource.Menu.profile, menu);
			base.OnCreateOptionsMenu (menu, inflater);
		}

	

    }
}