using System;

namespace KinderChat
{
	public class DisposableContext : IDisposable
	{
		readonly Action endContext;

        public DisposableContext(Action beginContext, Action endContext)
		{
			if (beginContext == null)
				throw new ArgumentNullException ();
			if (endContext == null)
				throw new ArgumentNullException ();

			this.endContext = endContext;

			beginContext ();
		}

		public void Dispose ()
		{
			endContext ();
		}
	}
}