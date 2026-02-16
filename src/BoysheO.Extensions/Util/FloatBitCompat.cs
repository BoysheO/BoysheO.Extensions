using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Util
{
    public static class FloatBitCompat
    {
        /// <summary>
        /// 兼容性API,等价于 MathF.BitDecrement(x)：
        /// 返回严格小于 x 的“下一个可表示 float”（NaN 返回自身）。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float BitDecrement(float x)
        {
#if NET6_0_OR_GREATER
            return MathF.BitDecrement(x);
#else

            // NaN：按 .NET 行为，返回 NaN（原样）
            if (float.IsNaN(x)) return x;

            // -Infinity：没有更小的可表示值
            if (x == float.NegativeInfinity) return x;

            // +Infinity：下一个更小的是最大有限值
            if (x == float.PositiveInfinity) return float.MaxValue;

            // 包括 +0 和 -0：下一个更小的是最小负非零（-float.Epsilon）
            if (x == 0f) return -float.Epsilon;

            uint bits = *(uint*)&x;

            // IEEE754 按位序：
            // 正数：bits - 1 => 更小
            // 负数：bits + 1 => 更小（更负）
            bits = (bits & 0x8000_0000u) == 0u ? (bits - 1u) : (bits + 1u);

            return *(float*)&bits;
#endif
        }
    }
}