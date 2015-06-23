using System;
using Android.App;
using Android.Content;
using Android.OS;
using com.refractored;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Util;
using Android.Content.PM;

namespace KinderChat
{
	[Activity (Label = "@string/app_name", Icon = "@drawable/ic_launcher", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
	public class MainActivity : BaseActivity, ViewPager.IOnPageChangeListener
	{

		protected override int LayoutResource {
			get { return Resource.Layout.activity_main; }
		}

		private MyPagerAdapter adapter;
		private ViewPager pager;
		private PagerSlidingTabStrip tabs;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			adapter = new MyPagerAdapter (this, SupportFragmentManager);
			pager = FindViewById<ViewPager> (Resource.Id.pager);
			tabs = FindViewById<PagerSlidingTabStrip> (Resource.Id.tabs);
			pager.Adapter = adapter;
			tabs.SetViewPager (pager);
			tabs.OnPageChangeListener = this;
			var pageMargin = (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, 4, Resources.DisplayMetrics);
			pager.PageMargin = pageMargin;
			pager.OffscreenPageLimit = 4;
			if (Settings.FirstRun) {
				pager.CurrentItem = 2;
				App.MessageDialog.SendMessage (Resources.GetString(Resource.String.get_started_welcome), Resources.GetString(Resource.String.welcome_to_kinderchat));
			} else {
				pager.CurrentItem = 0;
			}

			SupportActionBar.SetDisplayHomeAsUpEnabled (false);
			SupportActionBar.SetHomeButtonEnabled (false);

		
			// Register for GCM
			KinderGcmService.Register (this);
		}

		public void OnPageScrollStateChanged (int state)
		{
		}
		public void OnPageScrolled (int position, float positionOffset, int positionOffsetPixels)
		{
			
		}
		public void OnPageSelected (int position)
		{
			var tag = "android:switcher:" + pager.Id + ":" + position;
			var frag = SupportFragmentManager.FindFragmentByTag (tag) as BaseFragment;
			if (frag != null)
				frag.Init ();	
		}

		public class MyPagerAdapter : FragmentPagerAdapter
		{
			private string[] Titles;

			public MyPagerAdapter (Context context, Android.Support.V4.App.FragmentManager fm)
				: base (fm)
			{
				Titles = context.Resources.GetStringArray (Resource.Array.sections);
			}

			public override Java.Lang.ICharSequence GetPageTitleFormatted (int position)
			{
				return new Java.Lang.String (Titles [position]);
			}

			#region implemented abstract members of PagerAdapter

			public override int Count {
				get {
					return Titles.Length;
				}
			}

			#endregion

			#region implemented abstract members of FragmentPagerAdapter

			public override Android.Support.V4.App.Fragment GetItem (int position)
			{
				switch (position) {
				case 0:
					var frag = ConversationsFragment.NewInstance ();
					return frag;
				case 1:
					var frag2 = FriendsFragment.NewInstance ();
					return frag2;
				case 2:
					var frag3 = ProfileFragment.NewInstance ();
					return frag3;
				case 3:
					var frag4 = KinderPointsFragment.NewInstance ();
					return frag4;
				}

				return null;
			}

			#endregion
		}
	}
}

