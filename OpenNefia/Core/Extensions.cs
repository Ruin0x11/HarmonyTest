using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public static class Extensions
    {
		public static void ForEach<T>(this IEnumerable<T> iterator, Action<T> action)
		{
			foreach (T item in iterator)
			{
				action(item);
			}
		}

		public static IEnumerable<T> WhereNonNull<T>(this IEnumerable<T?> iterator)
		{
			foreach (var item in iterator)
			{
				if (item != null)
					yield return item;
			}
		}

		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dic, IDictionary<TKey, TValue> dicToAdd)
		{
			dicToAdd.ForEach(x => dic.Add(x.Key, x.Value));
		}
	}
}
