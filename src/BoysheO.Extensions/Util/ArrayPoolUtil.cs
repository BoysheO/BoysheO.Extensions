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
            if (elementCount >= buff.Length)
            {
                buff = Resize(buff, (int)(elementCount * 1.2));
            }

            buff[elementCount] = addValue;
            outBuff = buff;
            return elementCount + 1;
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
            if (elementCount + addValueAryCount > buff.Length)
            {
                buff = Resize(buff, elementCount + addValueAryCount);
            }

            var buffSpan = buff.AsSpan(elementCount, addValueAryCount);
            addValueAry.AsSpan(addValueAryOffset, addValueAryCount).CopyTo(buffSpan);
            outBuff = buff;
            return elementCount + addValueAryCount;
        }
    }
}