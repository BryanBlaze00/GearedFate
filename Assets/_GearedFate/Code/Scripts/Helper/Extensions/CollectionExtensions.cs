using System;
using System.Collections.Generic;
using System.Linq;

namespace BTG
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns a random element from <paramref name="source"></paramref> using UnityEngine.Random
        /// </summary>
        public static T Random<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            if (list.Count == 0)
                throw new InvalidOperationException("No elements in source collection");
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T SelectRandom<T>(params T[] objects)
        {
            return objects.Random();
        }

        /// <summary>
        /// Invokes <paramref name="action"></paramref> on each element in <paramref name="source"></paramref>
        /// </summary>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var e in source)
            {
                action(e);
            }
            return source;
        }

        /// <summary>
        /// Returns the index of <paramref name="toFind"></paramref> if it's found in <paramref name="array"></paramref>, or -1 if it isn't
        /// </summary>
        public static int IndexOf<T>(this T[] array, T toFind) => Array.IndexOf(array, toFind);
        /// <summary>
        /// Returns the index of <paramref name="toFind"></paramref> if it's found in <paramref name="source"></paramref>, or -1 if it isn't
        /// </summary>
        public static int IndexOf<T>(this IEnumerable<T> source, T toFind, IEqualityComparer<T> comparer = null)
        {
            int i = 0;
            comparer ??= EqualityComparer<T>.Default;
            foreach (var item in source)
            {
                if (comparer.Equals(item, toFind))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
    }
}
