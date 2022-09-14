using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BoysheO.Toolkit;

namespace BoysheO.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     非索引器使用见 <see cref="Enumerable.ElementAtOrDefault{TSource}" />条目
        ///     <para>另注意:ElementAt会调用索引器(例如IList)，此时有可能发生循环调用导致爆栈（当该集合继承了IList且index索引器是不被支持的不良集合），此时可用此API通过遍历器实现</para>
        ///     优先使用ElementAt
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">offset out of source range</exception>
        public static T GetElementAt<T>(this IEnumerable<T> source, int offset)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.TryGetElementAt(offset, out var res)) return res;
            throw new IndexOutOfRangeException();
        }

        public static bool TryGetElementAt<T>(this IEnumerable<T> source, int offset, out T value)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case T[] ary:
                    return TryGetElementAt(ary, offset, out value);
            }

            var count = -1;
            foreach (var item in source)
            {
                count++;
                if (count == offset)
                {
                    value = item;
                    return true;
                }
            }

            value = default!;
            return false;
        }

        public static bool TryGetElementAt<T>(this T[] source, int idx, out T value)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (idx >= source.Length)
            {
                value = default!;
                return false;
            }

            value = source[idx];
            return true;
        }

        public static SortedList<TKey, TRes> ToSortedList<TKey, TRes, TSource>(this IEnumerable<TSource> sources,
            Func<TSource, TKey> keySelector, Func<TSource, TRes> valueSelector)
        {
            if (sources == null) throw new ArgumentNullException(nameof(sources));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));
            return ToSortedList(sources, keySelector, valueSelector, Comparer<TKey>.Default);
        }

        public static SortedList<TKey, TRes> ToSortedList<TKey, TRes, TSource>(this IEnumerable<TSource> sources,
            Func<TSource, TKey> keySelector, Func<TSource, TRes> valueSelector,
            IComparer<TKey> comparer)
        {
            if (sources == null || keySelector == null || valueSelector == null || comparer == null)
                throw new ArgumentNullException();
            var sortedList = new SortedList<TKey, TRes>(comparer);
            foreach (var item in sources) sortedList.Add(keySelector(item), valueSelector(item));

            return sortedList;
        }

        public static SortedList<TKey, TRes> ToSortedList<TKey, TRes, TSource>(this IEnumerable<TSource> sources,
            Func<TSource, TKey> keySelector, Func<TSource, TRes> valueSelector,
            Func<TKey, TKey, int> comparer)
        {
            if (sources == null) throw new ArgumentNullException(nameof(sources));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            return ToSortedList(sources, keySelector, valueSelector, new ComparerAdapter<TKey>(comparer));
        }

        public static SortedList<TKey, TSource> ToSortedList<TKey, TSource>(this IEnumerable<TSource> sources,
            Func<TSource, TKey> keySelector)
        {
            if (sources == null || keySelector == null) throw new ArgumentNullException();
            var sortedList = new SortedList<TKey, TSource>();
            foreach (var item in sources) sortedList.Add(keySelector(item), item);

            return sortedList;
        }

        /// <summary>
        ///     A集合是否完全包含B集合<br />
        ///     ！集合相等的情况也返回true<br />
        ///     ！根据Except的实现，会立即遍历source构建一个集合，然后遍历another查找相同元素<br />
        ///     *性能提示：本质上是linq的封装
        /// </summary>
        public static bool IsContainsSet<T>(this IEnumerable<T> source, IEnumerable<T> another)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return another.Except(source).IsLessThan(1);
        }

        /// <summary>
        ///     A集合是否完全包含B集合<br />
        ///     ！集合相等的情况也返回true<br />
        ///     ！如果source包含buff的元素，则从buff移除这个元素，最后buff的剩余元素就是source中没有的元素
        ///     ！适合buff较小、大几率在遍历完source前就获得结果的情况。低gc
        /// </summary>
        public static bool IsContainsSetBuff<T>(this IEnumerable<T> source, ISet<T> setBuff)
        {
            foreach (var v in source)
            {
                if (!setBuff.Contains(v)) continue;
                setBuff.Remove(v);
                if (setBuff.Count == 0) return true;
            }

            return false;
        }

        /// <summary>
        /// 将source中的元素复制到span中，在source遍历完或者span完结就会终止数据复制
        /// </summary>
        public static void CopyTo<T>(this IEnumerable<T> source, Span<T> span)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            using var itor = source.GetEnumerator();
            var itor2 = span.GetEnumerator();
            while (itor.MoveNext() && itor2.MoveNext()) itor2.Current = itor.Current;
        }

        /// <summary>
        ///     比较集合大小是否比count大<br />
        ///     ！相对于linq.count()来说这个API不会遍历全集，只数前几个元素<br />
        ///     ！null元素视为有效元素<br />
        ///     ！判断遍历体是否有值时应使用Any()、IsEmpty、Count==0等替代而不是使用这个API<br />
        /// </summary>
        public static bool IsMoreThan<T>(this IEnumerable<T> source, int count)
        {
            switch (source)
            {
                case T[] ary:
                    return ary.Length > count;
                case ICollection collection:
                    return collection.Count > count;
                case ICollection<T> collection1:
                    return collection1.Count > count;
            }

            using var itor = source.GetEnumerator();
            while (itor.MoveNext())
                if (--count < 0)
                    return true;

            return false;
        }

        /// <summary>
        ///     比较集合大小是否比count大或等于<br />
        ///     ！相对于linq.count()来说这个API不会遍历全集，只数前几个元素<br />
        ///     ！null元素视为有效元素<br />
        ///     ！判断遍历体是否有值时应使用Any()、IsEmpty、Count==0等替代而不是使用这个API<br />
        /// </summary>
        public static bool IsMoreThanOrEqual<T>(this IEnumerable<T> source, int count)
        {
            switch (source)
            {
                case T[] ary:
                    return ary.Length >= count;
                case ICollection collection:
                    return collection.Count >= count;
                case ICollection<T> collection1:
                    return collection1.Count >= count;
            }

            using var itor = source.GetEnumerator();
            while (itor.MoveNext())
            {
                count--;
                if (count <= 0) return true;
            }

            return false;
        }

        /// <summary>
        ///     比较集合大小是否比count小<br />
        ///     ！相对于linq.count()来说这个API不会遍历全集，只数前几个元素<br />
        ///     ！null元素视为有效元素<br />
        ///     ！判断遍历体是否有值时应使用Any()、IsEmpty、Count==0等替代而不是使用这个API<br />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThan<T>(this IEnumerable<T> source, int count)
        {
            return !IsMoreThanOrEqual(source, count);
        }

        /// <summary>
        ///     比较集合大小是否比count小或等于<br />
        ///     ！相对于linq.count()来说这个API不会遍历全集，只数前几个元素<br />
        ///     ！null元素视为有效元素<br />
        ///     ！判断遍历体是否有值时应使用Any()、IsEmpty、Count==0等替代而不是使用这个API<br />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThanOrEqual<T>(this IEnumerable<T> source, int count)
        {
            return !IsMoreThan(source, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !IsMoreThan(enumerable, 0);
        }

        /// <summary>
        /// 由于ICollection与ICollection{T}通常一起使用，这会导致API冲突从而降低API使用舒适度。因此这个API带有1尾缀
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty1(this ICollection collection)
        {
            return collection.Count == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this T[] source)
        {
            return source.Length == 0;
        }

        /// <summary>
        /// linq增补
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IOrderedEnumerable<T> OrderBy<TK, T>(this IEnumerable<T> enumerable, Func<T, TK> keySelector,
            Func<TK, TK, int> comparer)
        {
            return enumerable.OrderBy(keySelector, new ComparerAdapter<TK>(comparer));
        }

        /// <summary>
        ///     linq增补,通过指定key进行排序
        /// </summary>
        public static IOrderedEnumerable<TSource> OrderBy
            <TSource, TCompareKey>(this IEnumerable<TSource> sources, Func<TSource, TCompareKey> keySelector)
            where TCompareKey : IComparer<TCompareKey>
        {
            var compare = new ComparerAdapter<TSource>(
                (a, b) =>
                {
                    var akey = keySelector(a);
                    var bkey = keySelector(b);
                    return akey.Compare(akey, bkey);
                });
            return sources.OrderBy(v => v, compare);
        }


        /// <summary>
        ///     选取所有里表元素，等同SelectMany(v => v)
        ///     这个是增补版，节约一个委托
        /// </summary>
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
        /// 匹配这个元素并返回所在位置计数（对可数集合即为index)
        /// 集合元素量不能大于int.Max
        /// -1代表找不到
        /// </summary>
        public static int Find<T>(this IEnumerable<T> enumerable,
            Func<T, bool> pre,
            out T elementFound)
        {
            var index = -1;
            foreach (var ele in enumerable)
            {
                index++;
                if (pre(ele))
                {
                    elementFound = ele;
                    return index;
                }
            }

            elementFound = default!;
            return -1;
        }
    }
}