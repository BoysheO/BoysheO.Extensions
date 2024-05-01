using System;
using System.Buffers;
using BoysheO.Extensions;

namespace Extensions
{
    [Obsolete("use RefArrayPoolUtil instead")]
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

        /// <summary>
        /// 给从ArrayPool中借出的Buff添加元素
        /// ！不可以保留原buff的引用
        /// </summary>
        /// <param name="buff">原始buff</param>
        /// <param name="elementCount">有效元素个数。须确保所有有效元素位于数组前端并连续</param>
        /// <param name="addValue">需要添加的值</param>
        /// <param name="outBuff">新的buff</param>
        /// <returns>添加后的元素个数</returns>
        public static int Add<T>(T[] buff, int elementCount, T addValue, out T[] outBuff)
        {
            return Insert(buff, elementCount, addValue, elementCount, out outBuff);
        }

        /// <summary>
        /// 给从ArrayPool中借出的Buff添加元素
        /// ！不可以保留原buff的引用
        /// </summary>
        /// <param name="buff">原始buff</param>
        /// <param name="elementCount">有效元素个数。须确保所有有效元素位于数组前端并连续</param>
        /// <param name="insertValue">需要添加的值</param>
        /// <param name="index">插入位置</param>
        /// <param name="outBuff">新的buff</param>
        /// <param name="resizeStep">发生resize时容量的期望增加值，实际值大于等于期望值</param>
        /// <returns>添加后的元素个数</returns>
        public static int Insert<T>(T[] buff, int elementCount, T insertValue, int index, out T[] outBuff,
            int resizeStep = 1)
        {
            var locBuf = ArrayPool<T>.Shared.Rent(1);
            locBuf[0] = insertValue;
            var count = InsertRange(buff, elementCount, new ReadOnlySpan<T>(locBuf,0,1), index, out outBuff,resizeStep);
            ArrayPool<T>.Shared.Return(locBuf);
            return count;
        }
        
        /// <summary>
        /// 给从ArrayPool中借出的Buff添加元素
        /// ！不可以保留原buff的引用
        /// </summary>
        /// <param name="buff">原始buff</param>
        /// <param name="elementCount">有效元素个数。须确保所有有效元素位于数组前端并连续</param>
        /// <param name="insertValue">需要添加的值</param>
        /// <param name="index">插入位置</param>
        /// <param name="outBuff">新的buff</param>
        /// <param name="resizeStep">发生resize时容量的期望增加值，实际值大于等于期望值</param>
        /// <returns>添加后的元素个数</returns>
        public static int InsertRange<T>(T[] buff, int elementCount, ReadOnlySpan<T> insertValue, int index, out T[] outBuff,
            int resizeStep = 1)
        {
            if (index > elementCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"index={index} should belong [0,{nameof(elementCount)}]");
            }

            var sizeNeed = elementCount + insertValue.Length;
            if (sizeNeed > buff.Length)
            {
                buff = Resize(buff, sizeNeed + resizeStep);
            }

            var seqCount = elementCount - index + 1;
            if (seqCount > 0)
            {
                buff.AsSpan(index, seqCount).Panning(-insertValue.Length);
            }

            insertValue.CopyTo(buff.AsSpan(index,insertValue.Length));
            outBuff = buff;
            return sizeNeed;
        }

        /// <summary>
        /// 给从ArrayPool中借出的Buff添加元素
        /// ！不可以保留原buff的引用
        /// </summary>
        /// <param name="buff">原始buff</param>
        /// <param name="elementCount">有效元素个数。须确保所有有效元素位于数组前端并连续</param>
        /// <param name="addValueAry">需要添加的值</param>
        /// <param name="addValueAryOffset"></param>
        /// <param name="addValueAryCount"></param>
        /// <param name="outBuff">新的buff</param>
        /// <returns>添加后的元素个数</returns>
        public static int AddRange<T>(T[] buff, int elementCount,
            T[] addValueAry,
            int addValueAryOffset,
            int addValueAryCount,
            out T[] outBuff)
        {
            return InsertRange(buff, elementCount, addValueAry.AsSpan(addValueAryOffset, addValueAryCount),elementCount, out outBuff);
        }
    }
}