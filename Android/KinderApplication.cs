using System;
using Android.App;
using System.IO;

namespace KinderChat
{
	[Application]
	public class KinderApplication: Application
	{
		
		public KinderApplication(IntPtr handle, global::Android.Runtime.JniHandleOwnership transer)
			:base(handle, transer)
		{

		}

		public override void OnCreate()
		{
			base.OnCreate();

			Xamarin.Insights.Initialize (App.InsightsKey, this);
			Xamarin.Insights.ForceDataTransmission = true;
			ServiceContainer.Register<IMessageDialog> (() => new MessageDialog ());
			var dbLocation = "kinder.db3";

			var library = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			dbLocation = Path.Combine(library, dbLocation);
			KinderDatabase.DatabaseLocation = dbLocation;
		}
	}
}

