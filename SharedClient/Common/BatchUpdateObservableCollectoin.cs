using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace KinderChat
{
	public class BatchUpdateObservableCollectoin<T> : ObservableCollection<T>
	{
		public event EventHandler UpdatesBegin;
		public event EventHandler UpdatesEnded;

		public bool IsBatchUpdates { get; private set; }

		public BatchUpdateObservableCollectoin (IEnumerable<T> collection)
			: base(collection)
		{
		}

		public BatchUpdateObservableCollectoin ()
		{
		}

		public IDisposable UpdatesBlock ()
		{
            return new DisposableContext(BeginUpdates, EndUpdates);
		}

		void BeginUpdates ()
		{
			IsBatchUpdates = true;
			var handler = UpdatesBegin;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		void EndUpdates ()
		{
			IsBatchUpdates = false;
			var handler = UpdatesEnded;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}
	}
}