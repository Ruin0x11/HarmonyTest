using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Extensions
{
    public static class LinqExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> iterator, Action<T> action)
		{
			foreach (T item in iterator)
			{
				action(item);
			}
		}

		public static void ForEachWithIndex<T>(this IEnumerable<T> ie, Action<T, int> action)
		{
			var i = 0;
			foreach (var e in ie) action(e, i++);
		}

		public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> ie) => ie.Select((x, index) => (x, index));

		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> iterator)
		{
			foreach (var item in iterator)
			{
				if (item != null)
					yield return item;
			}
		}

		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dic, IDictionary<TKey, TValue> dicToAdd)
		{
			foreach (var x in dicToAdd)
            {
				dic.Add(x.Key, x.Value);
			}
		}
	}
}
