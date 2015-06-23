using System;

namespace KinderChat.iOS
{
	public interface ISelectableSource
	{
		event EventHandler<NSIndexPathEventArgs> Selected;
	}
}

