using System;

namespace KinderChat
{
	public class KinderTask : EntityBase
	{
		public string Name { get; set; }
		public string IconName { get; set; }
		public bool IsPending { get; set; }
		public int Points { get; set; }
	}
}

