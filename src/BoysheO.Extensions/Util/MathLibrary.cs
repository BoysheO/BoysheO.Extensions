using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using BoysheO.Extensions;

namespace BoysheO.Util
{
    public static class MathLibrary
    {
        #region Distance

        /// <summary>
        ///     计算两坐标曼哈顿距离（溢出抛异常)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanDistance(int x1, int y1, int x2, int y2)
        {
            checked
            {
                return (x1 - x2).Abs() + (y1 - y2).Abs();
            }
        }

        #endregion

        #region remap

        /// <summary>
        /// 重映射区间[vMin,vMax]的值v到[newMin,newMax]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Remap(double v, double vMin, double vMax, double newMin, double newMax)
        {
            //对于0长区间，特殊处理
            if (vMin == vMax)
            {
                if (newMin == newMax)
                {
                    var d = v - vMin;
                    var newValue = newMin + d;
                    return newValue;
                }

                throw new ArgumentOutOfRangeException(nameof(vMin), "vMin equals vMax");
            }

            var newSpan = newMax - newMin;
            var oldSpan = vMax - vMin;
            var a = (v - vMin) / oldSpan * newSpan + newMin;
            return a;
        }

        /// <summary>
        /// 重映射区间[vMin,vMax]的值v到[newMin,newMax]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Remap(decimal v, decimal vMin, decimal vMax, decimal newMin, decimal newMax)
        {
            //对于0长区间，特殊处理
            if (vMin == vMax)
            {
                if (newMin == newMax)
                {
                    var d = v - vMin;
                    var newValue = newMin + d;
                    return newValue;
                }

                throw new ArgumentOutOfRangeException(nameof(vMin), "vMin equals vMax");
            }

            var newSpan = newMax - newMin;
            var oldSpan = vMax - vMin;
            var a = (v - vMin) / oldSpan * newSpan + newMin;
            return a;
        }

        /// <summary>
        /// 重映射区间[vMin,vMax]的值v到[newMin,newMax]，使用四舍五入算法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Remap(long v, long vMin, long vMax, long newMin, long newMax)
        {
            checked
            {
                //对于0长区间，特殊处理
                if (vMin == vMax)
                {
                    if (newMin == newMax)
                    {
                        var d = v - vMin;
                        var newValue = newMin + d;
                        return newValue;
                    }

                    throw new ArgumentOutOfRangeException(nameof(vMin), "vMin equals vMax");
                }

                var newSpan = newMax - newMin;
                var oldSpan = vMax - vMin;
                var delta = ((double)v - vMin) / oldSpan * newSpan;
                var iDelta = (long)Math.Round(delta, MidpointRounding.AwayFromZero);
                var result = iDelta + newMin; //这里要先转v到double，防止出现int.MinValue-n这样的bug
                return result;
            }
        }

        #endregion

        #region Draw

        #region Primitives

        /// <summary>
        ///     等概率抽取1个元素
        /// </summary>
        public static (int Idx, T Item) Draw<T>(this ReadOnlySpan<T> source)
        {
            if (source.Length == 0) throw new ArgumentException("can not draw anything in empty pool");
            var rand = RandomUtil.MoreEqualMinLessMax(0, source.Length);
            return (rand, source[rand]);
        }

        /// <summary>
        ///     等概率抽取1个元素
        /// </summary>
        public static (int Idx, T Item) Draw<T>(this IReadOnlyList<T> source)
        {
            if (source.Count == 0) throw new ArgumentException("can not draw anything in empty pool");
            var rand = RandomUtil.MoreEqualMinLessMax(0, source.Count);
            return (rand, source[rand]);
        }
        
        /// <summary>
        ///     加权抽取1个元素（items 与 weights 视为 zip）
        ///     *注意的点：1.出于预期性能考虑，约定weight传入的就是正整数
        /// </summary>
        public static (int Idx, T Item) Draw<T>(
            this IReadOnlyList<T> items,
            IReadOnlyList<int> weights)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));
            if (weights is null) throw new ArgumentNullException(nameof(weights));
            if (items.Count == 0) throw new ArgumentOutOfRangeException(nameof(items), "empty source detected");
            if (items.Count != weights.Count) throw new ArgumentException("items and weights length must be equal");

            long sum = 0;
            for (var i = 0; i < weights.Count; i++)
            {
                var w = weights[i];
                sum = checked(sum + w);
            }

            if (sum <= 0) throw new InvalidOperationException("total weight must be > 0");

            var rand = RandomUtil.MoreEqualMinLessMax(0, sum) + 1;

            for (var i = 0; i < items.Count; i++)
            {
                rand -= weights[i];
                if (rand <= 0) return (i, items[i]);
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// 遍历 source；每个元素以 hitProbability 命中；返回第一个命中的元素。
        /// 对无穷 IEnumerable 也适用（p>0 时几乎必然终止）。
        /// </summary>
        public static bool Draw<T>(
            this IEnumerable<T> source,
            double hitProbability,
#if NET6_0_OR_GREATER
            [NotNullWhen(true)]
#else
#endif
            out T picked,
            CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled)
                throw new ArgumentOutOfRangeException(nameof(cancellationToken),
                    "CancellationToken must be cancelable to avoid infinite loop.");
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (hitProbability < 0 || hitProbability > 1) throw new ArgumentOutOfRangeException(nameof(hitProbability));
            if (hitProbability == 0)
            {
                picked = default!;
                return false;
            }

            foreach (var item in source)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (RandomUtil.Random.NextDouble() < hitProbability)
                {
                    picked = item;
                    return true;
                }
            }

            picked = default!;
            return false; // 只有在 source 真的枚举完（有限序列）才会到这
        }

        #endregion

        #endregion

        #region Lottery Permutation

        /// <summary>
        ///     等概率抽取n个元素<br/>
        ///     蓄水池算法<br/>
        ///     *最终输出结果的顺序既有可能是顺序的，也有可能是乱序的
        /// </summary>
        /// <param name="count">抽取多少个元素</param>
        /// <param name="poolSize">奖池大小</param>
        /// <returns>池化数组，用完记得归还<see cref="ArrayPool{T}"/>，返回一组从集合[0,poolSize)中随机抽取的count个元素的排列</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数不正确</exception>
        public static int[] DrawToPooledArray(int poolSize, int count)
        {
            if (poolSize < 0)
                throw new ArgumentOutOfRangeException(nameof(poolSize), $"{nameof(poolSize)}={poolSize} should =>0");
            if (!count.IsInRange(0, poolSize))
                throw new ArgumentOutOfRangeException(nameof(count),
                    $"{nameof(count)}={count} should belong [0,{nameof(poolSize)}={poolSize}]");
            var buff = ArrayPool<int>.Shared.Rent(count);
            var span = buff.AsSpan(0, count);
            for (int i = 0; i < count; i++)
            {
                span[i] = i;
            }

            for (int i = count; i < poolSize; i++)
            {
                var rand = RandomUtil.Random.Next(i + 1);
                if (rand < count)
                {
                    span[rand] = i;
                }
            }

            return buff;
        }

        /// <summary>
        ///     等概率抽取n个元素（排列）<br/>
        ///     蓄水池有序改良算法<br/>
        ///     最终结果保持增序排列<br />
        /// </summary>
        /// <param name="count">抽取多少个元素</param>
        /// <param name="poolSize">奖池大小</param>
        /// <returns>池化数组，用完记得归还<see cref="ArrayPool{T}"/>，返回一组从集合[0,poolSize)中随机抽取的count个元素的排列</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数不正确</exception>
        public static int[] DrawToPooledArraySorted(int poolSize, int count)
        {
            if (poolSize < 0)
                throw new ArgumentOutOfRangeException(nameof(poolSize), $"{nameof(poolSize)}={poolSize} should >=0");
            if (!count.IsInRange(0, poolSize))
                throw new ArgumentOutOfRangeException(nameof(count),
                    $"{nameof(count)}={count} should belong [0,{nameof(poolSize)}={poolSize}]");
            var buff = ArrayPool<int>.Shared.Rent(count);
            var span = buff.AsSpan(0, count);
            var spanCount = 0;
            for (int i = 0; i < poolSize; i++)
            {
                var chance = (double)(count - spanCount) / (poolSize - i);
                var rand = RandomUtil.Random.NextDouble();
                if (!(rand <= chance)) continue;
                span[spanCount] = i;
                spanCount++;
                if (spanCount == count) break;
            }

            return buff;
        }

        #endregion
        
        #region CombinationAndPermutaion

        /// <summary>
        /// 从n中组合k个元素组合的组合数
        /// 只能算很小的值
        /// 很大的值考虑使用大数库
        /// </summary>
        public static int Combination(int n, int k)
        {
            // if (!n.IsInRange(1, 12)) throw new ArgumentOutOfRangeException(nameof(n), "n∈[1,12]");
            // if (!k.IsInRange(1, n)) throw new ArgumentOutOfRangeException(nameof(n), "k∈[1,n]");
            var a = Factorial1(n);
            var b = Factorial1(n - k);
            var c = Factorial1(k);
            return checked(a / (b * c));
        }

        /// <summary>
        /// 计算P(n,k)，从n个里面抽取k个排列的排列数
        /// 只能算很小的值(n∈[1,12],k∈[1,n])
        /// 很大的值考虑使用大数库
        /// </summary>
        public static int Permutation(int n, int k)
        {
            if (!n.IsInRange(1, 12)) throw new ArgumentOutOfRangeException(nameof(n), "n∈[1,12]");
            if (!k.IsInRange(1, n)) throw new ArgumentOutOfRangeException(nameof(n), "k∈[1,n]");
            var a = Factorial1(n);
            var b = Factorial1(n - k);
            return a / b;
        }

        #endregion

        /// <summary>
        /// 提供基本款阶乘(只能计算1-12）
        /// 很大的值考虑使用大数库
        /// </summary>
        public static int Factorial1(int n)
        {
            if (n == 0) return 1;
            if (!n.IsInRange(1, 12)) throw new ArgumentException($"n={n} is too big,n∈[1,12]");
            var res = 1;
            while (n > 1)
            {
                res *= n;
                n--;
            }

            return res;
        }

        /// <summary>
        /// 提供基本款阶乘(只能计算1-20）
        /// 很大的值考虑使用大数库
        /// </summary>
        public static long Factorial2(int n)
        {
            if (n == 0) return 1;
            if (!n.IsInRange(1, 20)) throw new ArgumentException($"n={n} is too big,n∈[1,20]");
            long res = 1;
            while (n > 1)
            {
                res *= n;
                n--;
            }

            return res;
        }
    }
}