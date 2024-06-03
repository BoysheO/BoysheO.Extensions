using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BoysheO.Toolkit;
using Extensions;

namespace BoysheO.Extensions
{
    public static class EnumerableExtensions
    {
        //useless
        // public static bool TryGetElementAt<T>(this IEnumerable<T> source, int offset, out T value)
        // {
        //     if (source == null) throw new ArgumentNullException(nameof(source));
        //     switch (source)
        //     {
        //         case T[] ary:
        //             return TryGetElementAt(ary, offset, out value);
        //     }
        //
        //     var count = -1;
        //     foreach (var item in source)
        //     {
        //         count++;
        //         if (count == offset)
        //         {
        //             value = item;
        //             return true;
        //         }
        //     }
        //
        //     value = default!;
        //     return false;
        // }

        //useless
        // public static bool TryGetElementAt<T>(this T[] source, int idx, out T value)
        // {
        //     if (source == null) throw new ArgumentNullException(nameof(source));
        //     if (idx >= source.Length)
        //     {
        //         value = default!;
        //         return false;
        //     }
        //
        //     value = source[idx];
        //     return true;
        // }

        public static SortedList<TKey, TRes> ToSortedList<TKey, TRes, TSource>(
            this IEnumerable<TSource> sources,
            Func<TSource, TKey> keySelector,
            Func<TSource, TRes> valueSelector,
            IComparer<TKey> comparer)
        {
            if (sources == null || keySelector == null || valueSelector == null || comparer == null)
                throw new ArgumentNullException();
            var sortedList = new SortedList<TKey, TRes>(comparer);
            foreach (var item in sources) sortedList.Add(keySelector(item), valueSelector(item));
            return sortedList;
        }

        /// <summary>
        /// Determines whether all elements of <paramref name="another"/> are included in <paramref name="source"/> or if both collections are equal.
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
        /// <c>true</c> if <paramref name="source"/> includes all elements of <paramref name="another"/> or if both collections are equal; otherwise, <c>false</c>.
        /// </returns>
        [Obsolete("Use IsSupersetOf instead.")]
        public static bool IsInclude<T>(this IEnumerable<T> source, IEnumerable<T> another)
        {
            return IsSupersetOf(source, another);
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
            var rest = another.Except(source);
            using var enumerator = rest.GetEnumerator();
            return !enumerator.MoveNext();
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
            using var itor = enumerable.GetEnumerator();
            return !itor.MoveNext();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this T[] source)
        {
            return source.Length == 0;
        }

        /// <summary>
        ///     Order by key.
        /// </summary>
        [Obsolete(
            "This method is deprecated due to low usage and limited functionality. Use the built-in OrderBy method instead.")]
        public static IOrderedEnumerable<TSource> OrderBy<TSource, TCompareKey>
        (this IEnumerable<TSource> sources,
            Func<TSource, TCompareKey> keySelector)
            where TCompareKey : IComparer<TCompareKey>
        {
            return sources.OrderBy(v => v,
                Comparer<TSource>.Create((a, b) =>
                {
                    var akey = keySelector(a);
                    var bkey = keySelector(b);
                    return akey.Compare(akey, bkey);
                }));
        }

        /// <summary>
        ///     Same as SelectMany(v=>v).
        /// </summary>
        [Obsolete("not useful enough.use internal api instead")]
        public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> source)
        {
            foreach (var item in source)
            foreach (var initem in item)
                yield return initem;
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

        // /// <summary>
        // /// <code>
        // /// |keys|valueIndex|value|
        // /// |____|__________|_____|
        // /// | a  |     0    | a1  |
        // /// |    |          | a2  |
        // /// |____|__________|_____|
        // /// | b  |     2    | b1  |
        // /// |    |          | b2  |
        // /// |    |          | b3  |
        // /// |____|__________|_____|
        // /// | c  |     5    | c1  |
        // /// </code>
        // /// </summary>
        // /// <param name="source"></param>
        // /// <param name="keys"></param>
        // /// <param name="valueIndex"></param>
        // /// <param name="values"></param>
        // /// <typeparam name="TK"></typeparam>
        // /// <typeparam name="TValue"></typeparam>
        // [Obsolete("未完工")]
        // public static void PooledGroupBy<TK, TValue>(
        //     IEnumerable<TValue> source,
        //     Func<TValue, TK> keySelector,
        //     out TK[] keys, out int keysCount,
        //     out int[] valueIndex, out int valueIndexCount,
        //     out TValue[] values, out int valuesCount)
        // {
        //     keys = ArrayPool<TK>.Shared.Rent(1);
        //     keysCount = 0;
        //     valueIndex = ArrayPool<int>.Shared.Rent(1);
        //     valueIndexCount = 0;
        //     values = ArrayPool<TValue>.Shared.Rent(1);
        //     valuesCount = 0;
        //     foreach (var value in source)
        //     {
        //         var key = keySelector(value);
        //         var keyIdx = Array.IndexOf(keys, key, 0, keysCount);
        //         if (keyIdx < 0)
        //         {
        //             keyIdx = keysCount;
        //             keysCount = ArrayPoolUtil.Add(keys, keysCount, key, out keys);
        //         }
        //
        //         int curValueIdx;
        //         if (keyIdx >= valueIndexCount)
        //         {
        //             curValueIdx = valuesCount;
        //             valueIndexCount = ArrayPoolUtil.Add(valueIndex, valueIndexCount, curValueIdx, out valueIndex);
        //             // valueIndex = ArrayPoolUtil.Resize(valueIndex, valueIndex.Length + 1);
        //         }
        //         else curValueIdx = valueIndex[keyIdx];
        //
        //         valuesCount = ArrayPoolUtil.Insert(values, valuesCount, value, curValueIdx, out values);
        //         var valueIndexSpan = valueIndex.AsSpan();
        //         for (int i = keyIdx + 1; i < valueIndexCount; i++)
        //         {
        //             valueIndexSpan[i]++;
        //         }
        //     }
        // }
    }
}