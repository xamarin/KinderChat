using System;
using Xamarin;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace KinderChat
{
	public class Logger
	{
		public void Report(Exception ex, IDictionary extraData = null, ReportSeverity warningLevel = ReportSeverity.Warning)
		{
			Debug.WriteLine ("KINDER: Exception occurred: " + ex); 

			if(!Insights.IsInitialized)
				return;
			
			Insights.Report (ex, extraData, warningLevel);

		}

		public void Track (string trackIdentifier, IDictionary<string, string> table = null)
		{
			Debug.WriteLine ("KINDER: Event:" + trackIdentifier); 

			if(!Insights.IsInitialized)
				return;
			
			Insights.Track (trackIdentifier, table);
		}

		public IDisposable TrackTimeContext (string identifier)
		{
			var handler = Insights.TrackTime (identifier);
            return new DisposableContext(handler.Start, handler.Stop);
		}
	}
}

