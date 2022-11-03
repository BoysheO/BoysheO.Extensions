using System;
using System.Buffers;
using System.Collections.Generic;

namespace BoysheO.Extensions.Math
{
    public static class RandomUtil
    {
        /// <summary>
        /// 重新排列
        /// </summary>
        public static int[] Draw(Random random, ReadOnlySpan<int> buffer,
            out int count)
        {
            //一个数组随机交换多少次后可以视为随机？
            // count = buffer.Length;
            // var buf = ArrayPool<int>.Shared.Rent(count);
            // var bufCount = 0;
            // var burst = ArrayPool<int>.Shared.Rent(count);
            // buffer.CopyTo(burst.AsSpan(0,count));
            // for (int i = count - 1; i > 0; i++)
            // {
            //     var idx = random.Next(i);
            //     
            // }
            throw new NotImplementedException();
        }

        /// <summary>
        /// 组合
        /// </summary>
        public static void Combine(Random random, Span<int> buff, int minInclusive, int maxInclusive)
        {
            throw new NotImplementedException();

        }
    }
}