using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public static class Extensions
    {
		public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
		{
			foreach (T item in enumeration)
			{
				action(item);
			}
		}

		public static IEnumerable<T> WhereNonNull<T>(this IEnumerable<T?> items)
		{
			foreach (var item in items)
			{
				if (item != null)
					yield return item;
			}
		}
	}
}
