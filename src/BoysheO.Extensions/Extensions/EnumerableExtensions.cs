using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static class EnumerableExtensions
    {
        public static SortedList<TKey, TRes> ToSortedList<TKey, TRes, TSource>(
            this IEnumerable<TSource> sources,
            Func<TSource, TKey> keySelector,
            Func<TSource, TRes> valueSelector,
            IComparer<TKey> comparer) where TKey : notnull
        {
            if (sources == null || keySelector == null || valueSelector == null || comparer == null)
                throw new ArgumentNullException();
            var sortedList = new SortedList<TKey, TRes>(comparer);
            foreach (var item in sources) sortedList.Add(keySelector(item), valueSelector(item));
            return sortedList;
        }

        /// <summary>
        /// Determines whether <paramref name="source"/> is a superset of <paramref name="another"/> or if both collections are equal.
        /// </summary>
        /// <remarks>
        /// Performance tips:
        /// This method uses LINQ. The <paramref name="source"/> collection will be converted to a set immediately.
        /// The <paramref name="another"/> collection may be fully traversed.
        /// </remarks>
        /// <typeparam name="T">The type of elements in the collections.</typeparam>
        /// <param name="source">The collection to check against.</param>
        /// <param name="another">The collection to check for inclusion in <paramref name="source"/>.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="source"/> is a superset of all elements of <paramref name="another"/> or if both collections are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> another)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (another == null) throw new ArgumentNullException(nameof(another));
            return !another.Except(source).Any();
        }

        /// <summary>
        /// Read elements in arg:source and write to arg:span until arg:span or arg:source end.
        /// </summary>
        public static void CopyTo<T>(this IEnumerable<T> source, Span<T> span)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            using var itor = source.GetEnumerator();
            var itor2 = span.GetEnumerator();
            while (itor.MoveNext() && itor2.MoveNext()) itor2.Current = itor.Current;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        public static T[] ToArray<T>(this ArraySegment<T> source)
        {
            var res = new T[source.Count];
            source.AsSpan().CopyTo(res.AsSpan());
            return res;
        }

        public static ArraySegment<T> Slice<T>(this ArraySegment<T> source, int offset, int count)
        {
            if (source.Array == null) throw new ArgumentOutOfRangeException(nameof(source));
            if (source.Offset + offset + count > source.Array.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "offset or count is out of source");
            var newOffset = source.Offset + offset;
            return new ArraySegment<T>(source.Array!, newOffset, count);
        }

        /// <summary>
        /// Find the element and return index.<br />
        /// return -1 if not found.
        /// </summary>
        public static int FirstOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> pre, out T item)
        {
            var index = -1;
            foreach (var ele in enumerable)
            {
                index++;
                if (pre(ele))
                {
                    item = ele;
                    return index;
                }
            }

            item = default!;
            return -1;
        }

        /// <summary>
        /// use list as stack
        /// </summary>
        public static bool TryPop<T>(this IReadOnlyList<T> lst, out T output)
        {
            if (lst.Count <= 0)
            {
                output = default!;
                return false;
            }

            output = lst[lst.Count - 1];
            return true;
        }

        /// <summary>
        /// use list as stack
        /// </summary>
        public static void Push<T>(this IList<T> lst, T item)
        {
            lst.Add(item);
        }

        public static IEnumerable<T[]> Chunk<T>(this IEnumerable<T> source, int size)
        {
#if !NET6_0_OR_GREATER //.net6开始有官方Chunk实现
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), "chunkSize must greater than 0");

            var buffer = new List<T>(size);
            foreach (var item in source)
            {
                buffer.Add(item);
                if (buffer.Count == size)
                {
                    yield return buffer.ToArray();
                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
                yield return buffer.ToArray();
#else
            return System.Linq.Enumerable.Chunk(source, size);
#endif
        }
    }
}