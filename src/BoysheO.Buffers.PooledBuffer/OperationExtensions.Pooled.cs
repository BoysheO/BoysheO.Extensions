using System;
using System.Collections.Generic;

namespace BoysheO.Buffers.PooledBuffer.Linq
{
    /// <summary>
    /// Every operation will dispose the source PooledBuff and rent another PooledBuff to return
    /// Rules:
    ///  1. don't use the source PooledBuff after operation.
    ///  2. every customer operation should dispose the source PooledBuff and return another PooledBuff.
    /// It's faster and lowGc 
    /// </summary>
    public static partial class OperationExtensions
    {
        public static PooledListBuffer<TTar> PooledSelect<TSrc, TTar>(this PooledListBuffer<TSrc> source,
            Func<TSrc, TTar> selector)
        {
            var buff = PooledListBuffer<TTar>.Rent();
            foreach (var src in source.Span)
            {
                var tar = selector(src);
                buff.Add(tar);
            }

            source.Dispose();
            return buff;
        }

        public static PooledListBuffer<TTar> PooledSelect<TSrc, TTar>(this PooledListBuffer<TSrc> source,
            Func<int, TSrc, TTar> selector)
        {
            var buff = PooledListBuffer<TTar>.Rent();
            var len = source.Span.Length;
            for (var index = 0; index < len; index++)
            {
                var src = source.Span[index];
                var tar = selector(index, src);
                buff.Add(tar);
            }

            source.Dispose();
            return buff;
        }
        
        public static PooledListBuffer<T> PooledWhere<T>(this PooledListBuffer<T> source, Func<T, bool> predicate)
        {
            var buff = PooledListBuffer<T>.Rent();
            foreach (var x1 in source.Span)
            {
                if (predicate(x1)) buff.Add(x1);
            }

            source.Dispose();
            return buff;
        }

        public static PooledListBuffer<T> PooledSlice<T>(this PooledListBuffer<T> source, int start, int count)
        {
            var buf = PooledListBuffer<T>.Rent();
            var destination = buf.GetSpanAdding(count);
            source.Span.Slice(start, count).CopyTo(destination);
            source.Dispose();
            return buf;
        }

        public static PooledListBuffer<PooledListBuffer<T>> PooledChunk<T>(this PooledListBuffer<T> source, int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            var buff = PooledListBuffer<PooledListBuffer<T>>.Rent();
            var count = source.Count;
            for (int i = 0; i < count; i += size)
            {
                var subBuff = PooledListBuffer<T>.Rent();
                var destination = subBuff.GetSpanAdding(size);
                source.Span.Slice(i, size).CopyTo(destination);
                buff.Add(subBuff);
            }

            source.Dispose();
            return buff;
        }

        public static PooledListBuffer<T> PooledSelectMany<T>(this PooledListBuffer<PooledListBuffer<T>> source)
        {
            var buff = PooledListBuffer<T>.Rent();
            foreach (var x1 in source.Span)
            {
                buff.AddRange(x1.Span);
                x1.Dispose();
            }

            source.Dispose();
            return buff;
        }

        public static PooledListBuffer<KeyValuePair<TK, TV>> PooledToPooledListBuffer<TK, TV>(
            this PooledDictionaryBuffer<TK, TV> source)
        {
            var buff = PooledListBuffer<KeyValuePair<TK, TV>>.Rent();
            foreach (var x1 in source)
            {
                buff.Add(x1);
            }

            source.Dispose();
            return buff;
        }

        public static PooledListBuffer<KeyValuePair<TK, TV>> PooledToPooledListBuffer<TK, TV>(
            this PooledSortedListBuffer<TK, TV> source)
        {
            var buff = PooledListBuffer<KeyValuePair<TK, TV>>.Rent();
            var span = buff.GetSpanAdding(source.Count);
            int index = 0;
            foreach (var x1 in source)
            {
                span[index] = x1;
                index++;
            }

            source.Dispose();
            return buff;
        }

        public static PooledDictionaryBuffer<TK, TV> PooledToPooledDictionaryBuffer<TS, TK, TV>(
            this PooledListBuffer<TS> source,
            Func<TS, TK> keySelector,
            Func<TS, TV> valueSelector)
        {
            var buff = PooledDictionaryBuffer<TK, TV>.Rent();
            foreach (var x1 in source.Span)
            {
                buff.Add(keySelector(x1), valueSelector(x1));
            }

            source.Dispose();
            return buff;
        }

        public static PooledSortedListBuffer<TK, TV> PooledToPooledSortedListBuffer<TS, TK, TV>(
            this PooledListBuffer<TS> source,
            Func<TS, TK> keySelector,
            Func<TS, TV> valueSelector,
            IComparer<TK> keyComparer)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));
            if (keyComparer == null) throw new ArgumentNullException(nameof(keyComparer));
            var buff = PooledSortedListBuffer<TK, TV>.Rent(keyComparer);
            foreach (var x1 in source.Span)
            {
                buff.Add(keySelector(x1), valueSelector(x1));
            }

            source.Dispose();
            return buff;
        }

        public static T PooledFirst<T>(this PooledListBuffer<T> buffer)
        {
            var span = buffer.Span;
            if (span.Length == 0) throw new Exception("PooledBuffer is empty");
            var ele = span[0];
            buffer.Dispose();
            return ele;
        }

        public static T PooledFirstOrDefault<T>(this PooledListBuffer<T> buffer)
        {
            var span = buffer.Span;
            if (span.Length == 0) return default(T);
            var ele = span[0];
            buffer.Dispose();
            return ele;
        }

        public static int PooledSum<T>(this PooledListBuffer<T> buffer, Func<T, int> selector)
        {
            int sum = 0;
            var span = buffer.Span;
            foreach (var x in span)
            {
                sum += selector(x);
            }

            buffer.Dispose();
            return sum;
        }
        
        public static T PooledLastOrDefault<T>(this PooledListBuffer<T> buffer)
        {
            var span = buffer.Span;
            if (span.Length == 0) return default(T);
            var ele = span[span.Length - 1];
            buffer.Dispose();
            return ele;
        }
        
        public static T PooledLast<T>(this PooledListBuffer<T> buffer)
        {
            var span = buffer.Span;
            if (span.Length == 0) throw new Exception("PooledBuffer is empty");
            var ele = span[span.Length - 1];
            buffer.Dispose();
            return ele;
        }
    }
}