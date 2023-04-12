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
    public static class OperationExtensions
    {
        public static PooledListBuffer<TTar> PooledSelect<TSrc, TTar>(this PooledListBuffer<TSrc> source,
            Func<TSrc, TTar> selector)
        {
            var buff = PooledListBuffer<TTar>.Rent();
            for (int index = 0, count = source.Count; index < count; index++)
            {
                var src = source[index];
                var tar = selector(src);
                buff.Add(tar);
            }

            source.Dispose();
            return buff;
        }

        // public static PooledBuffer<TDestination> SelectMany<TElement, TDestination>(
        //     this PooledBuffer<PooledBuffer<TElement>> source, Func<TElement, TDestination> selector)
        // {
        //     var buff = PooledBuffer<TDestination>.Rent();
        //     for (int index = 0, count = buff.Count; index < count; index++)
        //     {
        //         var enumerableSrc = source[index];
        //         var itor = enumerableSrc.GetEnumerator();
        //         while (itor.MoveNext())
        //         {
        //             buff.Add(selector(itor.Current));
        //         }
        //
        //         itor.Dispose();
        //         enumerableSrc.Dispose();
        //     }
        //
        //     source.Dispose();
        //     return buff;
        // }

        public static PooledListBuffer<T> PooledWhere<T>(this PooledListBuffer<T> source, Func<T, bool> filter)
        {
            var buff = PooledListBuffer<T>.Rent();
            var span = source.Span;
            for (int index = 0, count = source.Count; index < count; index++)
            {
                var src = span[index];
                if (filter(src)) buff.Add(src);
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

        public static PooledListBuffer<T> ToPooledListBuffer<T>(this IEnumerable<T> source)
        {
            var buff = PooledListBuffer<T>.Rent();
            foreach (var x1 in source)
            {
                buff.Add(x1);
            }

            return buff;
        }

        public static PooledListBuffer<KeyValuePair<TK, TV>> ToPooledListBuffer<TK, TV>(
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

        public static PooledDictionaryBuffer<TK, TV> ToPooledDictionaryBuffer<TS, TK, TV>(
            this IEnumerable<TS> source,
            Func<TS, TK> keySelector,
            Func<TS, TV> valueSelector)
        {
            var buff = PooledDictionaryBuffer<TK, TV>.Rent();
            foreach (var x1 in source)
            {
                buff.Add(keySelector(x1), valueSelector(x1));
            }

            return buff;
        }

        public static PooledDictionaryBuffer<TK, TV> ToPooledDictionaryBuffer<TS, TK, TV>(
            this PooledListBuffer<TS> source,
            Func<TS, TK> keySelector,
            Func<TS, TV> valueSelector)
        {
            var buff = PooledDictionaryBuffer<TK, TV>.Rent();
            foreach (var x1 in source)
            {
                buff.Add(keySelector(x1), valueSelector(x1));
            }

            source.Dispose();
            return buff;
        }

        public static int PooledSum<T>(this PooledListBuffer<T> buffer, Func<T, int> selector)
        {
            int sum = 0;
            for (int i = 0, count = buffer.Count; i < count; i++)
            {
                sum += selector(buffer[i]);
            }

            return sum;
        }

        public static T PooledFirst<T>(this PooledListBuffer<T> buffer)
        {
            if (buffer.Count == 0) throw new Exception("PooledBuffer is empty");
            var ele = buffer[0];
            buffer.Dispose();
            return ele;
        }

        public static T PooledFirstOrDefault<T>(this PooledListBuffer<T> buffer)
        {
            if (buffer.Count == 0) return default(T);
            var ele = buffer[0];
            buffer.Dispose();
            return ele;
        }
    }
}