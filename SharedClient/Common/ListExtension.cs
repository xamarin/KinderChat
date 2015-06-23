using System;
using System.Collections.Generic;

namespace KinderChat
{
	public static class ListExtension
	{
		public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
		{
			foreach (var element in items) {
				list.Add (element);
			}
		}
	}
}

