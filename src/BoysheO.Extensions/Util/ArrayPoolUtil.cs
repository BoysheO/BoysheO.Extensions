using System;
using System.Buffers;
using BoysheO.Extensions;

namespace Extensions
{
    public static class ArrayPoolUtil
    {
        /// <summary>
        /// 从ArrayPool中借出新的Buff并且填充老buff的数据；
        /// ！新Buff通常容量比size大
        /// ！ resize后不能再使用原buff
        /// </summary>
        public static T[] Resize<T>(T[] buff, int size)
        {
            var newBuff = ArrayPool<T>.Shared.Rent(size);
            Array.Copy(buff, newBuff, size.Min(buff.Length));
            ArrayPool<T>.Shared.Return(buff);
            return newBuff;
        }
    }
}