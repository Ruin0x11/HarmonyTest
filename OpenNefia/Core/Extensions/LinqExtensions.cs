using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Extensions
{
    public static class LinqExtensions
    {
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
			foreach (var x in dicToAdd)
            {
				dic.Add(x.Key, x.Value);
			}
		}
	}
}
