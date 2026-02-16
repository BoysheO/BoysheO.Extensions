using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace BoysheO.Util
{
    /// <summary>
    /// <see cref="System.Random"/> 的线程安全镜像 API
    /// </summary>
    public static class RandomUtil
    {
#if NET6_0_OR_GREATER
#else
        // 保证每个线程拿到不同 seed（避免同一时间创建 Random 导致序列相同）
        private static int _seed = Environment.TickCount;

        // 低版本：ThreadStatic Random
        [ThreadStatic] private static Random? __random;

        // 用于 NextBytes 的线程内缓冲（避免每次分配）
        [ThreadStatic] private static byte[]? __bytes4;
        [ThreadStatic] private static byte[]? __bytes8;

        // 低版本线程 Random（仅在无法用 Random.Shared 时启用）
        private static Random ThreadRandom
            => __random ??= new Random(Interlocked.Increment(ref _seed));

        private static byte[] Bytes4 => __bytes4 ??= new byte[4];
        private static byte[] Bytes8 => __bytes8 ??= new byte[8];
#endif
        // 高版本（net6+）优先：Random.Shared 是线程安全、无锁/低锁优化实现
        private static Random Rng
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if NET6_0_OR_GREATER
                return Random.Shared;
#else
                return ThreadRandom;
#endif
            }
        }

        /// <summary>
        /// 获取当前线程的随机数生成器（注意 async 上下文切换时不要缓存该引用）
        /// </summary>
        public static Random Random
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Rng;
        }

        #region value

        /// <summary> [0, 1) </summary>
        public static float Float
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)Rng.NextDouble();
        }

        /// <summary> 64-bit 随机 long（包含负数） </summary>
        public static long Long
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => NextInt64();
        }

        /// <summary> [0, short.Max) </summary>
        public static short Short
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (short)Rng.Next(0, short.MaxValue);
        }

        /// <summary> int.MaxValue 范围内的非负 int </summary>
        public static int Int
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Rng.Next();
        }

        public static bool Boolean
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Rng.Next() & 1) == 1; // 比 Next(2) 少一次取模/边界处理
        }

        #endregion

        #region Range

        [Obsolete(
            "Due to floating-point issues, the range of the mathematical computation result is [0, 1), but the actual output may deviate to [0, 1].")]
        public static float MoreEqualMinLessEqualMax(float min, float max)
        {
            if (!(min < max)) throw new ArgumentOutOfRangeException(nameof(min)); //这里用反向判断代替 min >= max，以顺便阻止NaN值
#if NET6_0_OR_GREATER
            float v = Rng.NextSingle() * (max - min) + min;
#else
            float v = (float)(Rng.NextDouble() * (max - min) + min);
#endif
            return v;
        }

        /// <summary>
        /// [min,max)
        /// </summary>
        public static float MoreEqualMinLessMax(float min, float max)
        {
            // 这个写法能把 NaN 也拦住
            if (!(min < max)) throw new ArgumentOutOfRangeException(nameof(min));
#if NET6_0_OR_GREATER
            float v = Rng.NextSingle() * (max - min) + min;
            if (v >= max) v = MathF.BitDecrement(max);
#else
            float v = (float)(Rng.NextDouble() * (max - min) + min);
            if (v >= max) v = FloatBitCompat.BitDecrement(max);
#endif
            return v;
        }

        /// <summary> [min, max) </summary>
        public static int MoreEqualMinLessMax(int min, int max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException(nameof(min));
            return Rng.Next(min, max);
        }

        /// <summary> [min, max) </summary>
        public static long MoreEqualMinLessMax(long min, long max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException(nameof(min));

#if NET6_0_OR_GREATER
            return Rng.NextInt64(min, max);
#else
            ulong range = (ulong)max - (ulong)min; // 不能 max-min，否则可能溢出
            ulong offset = NextUInt64(range);
            return unchecked((long)(offset + (ulong)min));
#endif
        }

        /// <summary> [min, max) </summary>
        public static uint MoreEqualMinLessMax(uint min, uint max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException(nameof(min));
            return NextUInt32(max - min) + min;
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long NextInt64()
        {
#if NET6_0_OR_GREATER
            return Rng.NextInt64(long.MinValue, long.MaxValue);
#else
            var bytes = Bytes8;
            ThreadRandom.NextBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong NextUInt64()
        {
#if NET6_0_OR_GREATER
            Span<byte> tmp = stackalloc byte[8];
            Rng.NextBytes(tmp);
            return MemoryMarshal.Read<ulong>(tmp);
#else
            var bytes = Bytes8;
            ThreadRandom.NextBytes(bytes);
            return BitConverter.ToUInt64(bytes, 0);
#endif
        }

        /// <summary>
        /// 返回 [0, maxExclusive) 的均匀 ulong（拒绝采样避免取模偏差）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong NextUInt64(ulong maxExclusive)
        {
            if (maxExclusive == 0) return 0;

            // limit = floor(ulong.MaxValue / maxExclusive) * maxExclusive
            ulong limit = ulong.MaxValue - (ulong.MaxValue % maxExclusive);
            ulong r;
            do
            {
                r = NextUInt64();
            } while (r >= limit);

            return r % maxExclusive;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint NextUInt32()
        {
#if NET6_0_OR_GREATER
            // stackalloc 4字节 + MemoryMarshal.Read<uint>（更少检查/分支）
            Span<byte> tmp = stackalloc byte[4];
            Rng.NextBytes(tmp);
            return MemoryMarshal.Read<uint>(tmp);
#else
            var bytes = Bytes4;
            ThreadRandom.NextBytes(bytes);
            return BitConverter.ToUInt32(bytes, 0);
#endif
        }

        /// <summary>
        /// 返回 [0, maxExclusive) 的均匀 uint（拒绝采样避免取模偏差）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint NextUInt32(uint maxExclusive)
        {
            if (maxExclusive == 0) return 0;

            uint limit = uint.MaxValue - (uint.MaxValue % maxExclusive);
            uint r;
            do
            {
                r = NextUInt32();
            } while (r >= limit);

            return r % maxExclusive;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short NextInt16()
        {
#if NET6_0_OR_GREATER
            // stackalloc 2字节即可（原实现借用4字节也行）
            Span<byte> tmp = stackalloc byte[2];
            Rng.NextBytes(tmp);
            return MemoryMarshal.Read<short>(tmp);
#else
            var bytes = Bytes4; // 借用前2字节
            ThreadRandom.NextBytes(bytes);
            return BitConverter.ToInt16(bytes, 0);
#endif
        }
    }
}