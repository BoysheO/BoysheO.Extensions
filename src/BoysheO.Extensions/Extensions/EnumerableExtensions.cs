using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public static bool TryPop<T>(this IList<T> lst,
#if NETSTANDARD2_1_OR_GREATER
            [NotNullWhen(returnValue: true)]
#endif
            out T output)
        {
            if (lst.Count <= 0)
            {
                output = default!;
                return false;
            }

            var lastIdx = lst.Count - 1;
            output = lst[lastIdx]!;
            lst.RemoveAt(lastIdx);
            return true;
        }
        
        /// <summary>
        /// use list as stack
        /// </summary>
        public static void Push<T>(this IList<T> lst, T item)
        {
            lst.Add(item);
        }

        /// <summary>
        /// Try to remove the first element that match the predicate.
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="predicate"></param>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryRemoveFirst<T>(this IList<T> lst, Predicate<T> predicate,
#if NETSTANDARD2_1_OR_GREATER
            [NotNullWhen(returnValue: true)]
#endif
            out T item)
        {
            for (int i = 0, count = lst.Count; i < count; i++)
            {
                item = lst[i]!;
                if (predicate(item))
                {
                    lst.RemoveAt(i);
                    return true;
                }
            }

            item = default!;
            return false;
        }
        
        /// <summary>
        /// Split the elements of a sequence into chunks of size at most <paramref name="size"/>.
        /// </summary>
        /// <remarks>
        /// Every chunk except the last will be of size <paramref name="size"/>.
        /// The last chunk will contain the remaining elements and may be of a smaller size.
        /// </remarks>
        /// <param name="source">
        /// An <see cref="IEnumerable{T}"/> whose elements to chunk.
        /// </param>
        /// <param name="size">
        /// Maximum size of each chunk.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the elements of source.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> that contains the elements of the input sequence split into chunks of size <paramref name="size"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="size"/> is below 1.
        /// </exception>
        public static IEnumerable<TSource[]> Chunk1<TSource>(this IEnumerable<TSource> source, int size)
        {
#if !NET6_0_OR_GREATER //.net6开始有官方Chunk实现
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (size < 1)
                throw new ArgumentOutOfRangeException(nameof(size));

            return EnumerateChunks();

            IEnumerable<TSource[]> EnumerateChunks()
            {
                using (IEnumerator<TSource> sourceEnumerator = source.GetEnumerator())
                {
                    if (!sourceEnumerator.MoveNext())
                        yield break;

                    int chunkCapacity = Math.Min(size, 4);

                    while (true)
                    {
                        TSource[] chunk = new TSource[chunkCapacity];
                        chunk[0] = sourceEnumerator.Current;
                        int chunkCount = 1;

                        while (chunkCount < size && sourceEnumerator.MoveNext())
                        {
                            // Grow gradually so we do not allocate "size" upfront for short sequences.
                            if (chunkCount == chunk.Length)
                            {
                                chunkCapacity = (int)Math.Min((uint)size, 2u * (uint)chunkCount);
                                Array.Resize(ref chunk, chunkCapacity);
                            }

                            chunk[chunkCount] = sourceEnumerator.Current;
                            chunkCount++;
                        }

                        if (chunkCount != chunk.Length)
                            Array.Resize(ref chunk, chunkCount);

                        yield return chunk;

                        if (chunkCount < size || !sourceEnumerator.MoveNext())
                            yield break;
                    }
                }
            }
#else
            return System.Linq.Enumerable.Chunk(source, size);
#endif
        }
    }
}
