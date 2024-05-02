using System;
using System.Buffers;
using System.Collections.Generic;

namespace BoysheO.Extensions.Util
{
    public static class RefArrayPoolUtil
    {
        /// <summary>
        /// resize a buff from ArrayPool.Share with ArrayPool.Share
        /// !after resize,buff.Length is more than the size given generally
        /// </summary>
        public static void Resize<T>(ref T[] buff, int size, ArrayPool<T> pool = null)
        {
            if (buff.Length >= size) return;
            if (pool == null) pool = ArrayPool<T>.Shared;
            var newBuff = pool.Rent(size);
            Array.Copy(buff, newBuff, buff.Length);
            pool.Return(buff);
            buff = newBuff;
        }

        /// <summary>
        /// Add value to the buff from ArrayPool.Share
        /// * resize automatically
        /// </summary>
        public static void Add<T>(ref T[] buff, ref int buffCount, T value, ArrayPool<T> pool = null)
        {
            Insert(ref buff, ref buffCount, value, buffCount, pool);
        }

        /// <summary>
        /// Add values to the buff from ArrayPool.Share
        /// * resize automatically
        /// </summary>
        public static void AddRange<T>(ref T[] buff, ref int buffCount, IReadOnlyList<T> values,
            ArrayPool<T> pool = null)
        {
            InsertRange(ref buff, ref buffCount, values, buffCount, pool);
        }

        /// <summary>
        /// Add values to the buff from ArrayPool.Share
        /// * resize automatically
        /// </summary>
        public static void AddRange<T>(ref T[] buff, ref int buffCount, ReadOnlySpan<T> values,
            ArrayPool<T> pool = null)
        {
            InsertRange(ref buff, ref buffCount, values, buffCount, pool);
        }

        /// <summary>
        /// Insert value to the buff from ArrayPool.Share
        /// * resize automatically
        /// </summary>
        public static void Insert<T>(ref T[] buff, ref int buffCount, T value, int index, ArrayPool<T> pool = null)
        {
            if (index > buffCount || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"index={index} should belong [0,{nameof(buffCount)}]");
            }

            if (buff.Length == buffCount)
            {
                Resize(ref buff, buffCount + 1, pool);
            }

            Array.Copy(buff, index, buff, index + 1, buffCount - index);
            buff[index] = value;
            buffCount++;
        }

        /// <summary>
        /// Insert value to the buff from ArrayPool.Share
        /// * resize automatically
        /// </summary>
        public static void InsertRange<T>(ref T[] buff, ref int buffCount, ReadOnlySpan<T> insertValue, int index,
            ArrayPool<T> pool = null)
        {
            if (index > buffCount || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"index={index} should belong [0,{nameof(buffCount)}]");
            }

            var sizeNeed = buffCount + insertValue.Length;
            if (sizeNeed > buff.Length) Resize(ref buff, sizeNeed, pool);
            Array.Copy(buff, index, buff, index + sizeNeed, buffCount - index);
            insertValue.CopyTo(buff.AsSpan(index));
            buffCount += insertValue.Length;
        }

        /// <summary>
        /// Insert value to the buff from ArrayPool.Share
        /// * resize automatically
        /// </summary>
        public static void InsertRange<T>(ref T[] buff, ref int buffCount, IReadOnlyList<T> values, int index,
            ArrayPool<T> pool = null)
        {
            if (index > buffCount || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"index={index} should belong [0,{nameof(buffCount)}]");
            }

            var sizeNeed = buffCount + values.Count;
            if (sizeNeed > buff.Length) Resize(ref buff, sizeNeed, pool);
            Array.Copy(buff, index, buff, index + sizeNeed, buffCount - index);
            int i = index;
            for (int index1 = 0,count1 = values.Count; index1 < count1; index1++)
            {
                var value = values[index1];
                buff[i] = value;
                i++;
            }

            buffCount = sizeNeed;
        }

        public static bool Remove<T>(ref T[] buff, ref int buffCount, T obj)
        {
            var idx = Array.IndexOf(buff, obj, 0, buffCount);
            if (idx < 0) return false;
            for (int i = idx; i < buffCount - 1; i++)
            {
                buff[i] = buff[i + 1];
            }

            buff[buffCount - 1] = default;
            buffCount--;
            return true;
        }

        public static void TrimExcess<T>(ref T[] buff, int buffCount, ArrayPool<T> pool = null)
        {
            //根据对ArrayPool测试结果显示，ArrayPool给出的数列大小为 0,16,32...16*2^(n-1)
            if (pool == null) pool = ArrayPool<T>.Shared;
            if (buffCount == 0 && buff.Length > 0)
            {
                pool.Return(buff);
                buff = pool.Rent(0);
            }
            else if (buff.Length > 16 && buff.Length > buffCount / 2)
            {
                Resize(ref buff, buffCount, pool);
            }
        }
    }
}