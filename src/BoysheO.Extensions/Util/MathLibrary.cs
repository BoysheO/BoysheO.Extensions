using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using BoysheO.Extensions;

namespace BoysheO.Util
{
    public static class MathLibrary
    {
        #region Distance

        /// <summary>
        ///     计算两坐标曼哈顿距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanDistance(int x1, int y1, int x2, int y2)
        {
            return (x1 - x2).Abs() + (y1 - y2).Abs();
        }

        #endregion

        #region remap

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Remap(int v, int vMin, int vMax, int newMin, int newMax)
        {
            if (vMin == vMax || newMin == newMax) throw new ArgumentException();
            return (v - vMin) / (vMax - vMin) * (newMax - newMin) + newMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Remap(uint v, uint vMin, uint vMax, uint newMin, uint newMax)
        {
            if (vMin == vMax || newMin == newMax) throw new ArgumentException();
            return (v - vMin) / (vMax - vMin) * (newMax - newMin) + newMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Remap(float v, float vMin, float vMax, float newMin, float newMax)
        {
            if (Math.Abs(vMin - vMax) < 0.002f || Math.Abs(newMin - newMax) < 0.002f)
                throw new ArgumentException();
            return (v - vMin) / (vMax - vMin) * (newMax - newMin) + newMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Remap(double v, double vMin, double vMax, double newMin, double newMax)
        {
            if (Math.Abs(vMin - vMax) < 0.002f || Math.Abs(newMin - newMax) > 0.002f)
                throw new ArgumentException();
            return (v - vMin) / (vMax - vMin) * (newMax - newMin) + newMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Remap(decimal v, decimal vMin, decimal vMax, decimal newMin,
            decimal newMax)
        {
            if (vMin == vMax || newMin == newMax) throw new ArgumentException();
            return (v - vMin) / (vMax - vMin) * (newMax - newMin) + newMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Remap(long v, long vMin, long vMax, long newMin, long newMax)
        {
            if (vMin == vMax || newMin == newMax) throw new ArgumentException();
            return (v - vMin) / (vMax - vMin) * (newMax - newMin) + newMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Remap(ulong v, ulong vMin, ulong vMax, ulong newMin, ulong newMax)
        {
            if (vMin == vMax || newMin == newMax) throw new ArgumentException();
            return (v - vMin) / (vMax - vMin) * (newMax - newMin) + newMin;
        }

        #endregion

        #region Draw

        #region Primitives

        /// <summary>
        ///     等概率抽取1个元素
        /// </summary>
        public static (ushort Idx, T Item) Draw<T>(this ReadOnlySpan<T> source)
        {
            if (source.Length == 0) throw new Exception("can not draw anything in empty pool");
            var rand = RandomUtil.MoreEqualMinLessMax(0, source.Length);
            return ((ushort)rand, source[rand]);
        }

        /// <summary>
        ///     等概率抽取1个元素
        /// </summary>
        public static (ushort Idx, T Item) Draw<T>(this IList<T> source)
        {
            Contract.Assert(source.Count > 0, "can not draw anything in empty pool");
            var rand = RandomUtil.MoreEqualMinLessMax(0, source.Count);
            return ((ushort)rand, source[rand]);
        }

        public static (ushort Idx, T Item) Draw<T>(this ICollection<T> source)
        {
            Contract.Assert(source.Count > 0, "can not draw anything in empty pool");
            var rand = RandomUtil.MoreEqualMinLessMax(0, source.Count);
            return ((ushort)rand, source.ElementAt(rand));
        }

        /// <summary>
        ///     加权抽取1个元素
        /// </summary>
        public static (ushort Idx, T Item) Draw<T>(this ICollection<(T Item, uint Weight)> source)
        {
            Contract.Assert(source.Count > 0, "can not draw anything in empty pool");
            long sum = 0;
            foreach (var v in source) sum += v.Weight;

            var rand = RandomUtil.MoreEqualMinLessMax(0, sum) + 1;
            ushort index = 0;
            foreach (var valueTuple in source)
            {
                rand -= valueTuple.Weight;
                if (rand <= 0) return (index, valueTuple.Item);
                index++;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        ///     加权抽取1个元素
        /// </summary>
        public static (ushort Idx, T Item) Draw<T>(this ReadOnlySpan<(T Item, uint Weight)> source)
        {
            Contract.Assert(source.Length > 0, "can not draw anything in empty pool");
            long sum = 0;
            foreach (var v in source) sum += v.Weight;

            var rand = RandomUtil.MoreEqualMinLessMax(0, sum) + 1;
            var len = source.Length;
            for (ushort i = 0; i < len; i++)
            {
                var cur = source[i];
                rand -= cur.Weight;
                if (rand <= 0) return (i, cur.Item);
            }

            throw new InvalidOperationException();
        }

        #endregion

        #region Extension

        /// <summary>
        ///     等概率抽取1个元素
        /// </summary>
        public static (ushort Idx, T Item) Draw<T>(this T[] source)
        {
            Contract.Assert(source.Length > 0, "can not draw anything in empty pool");
            return Draw((ReadOnlySpan<T>)source);
        }

        /// <summary>
        ///     等概率抽取1个元素
        ///     collection必须可数
        /// </summary>
        public static (ushort Idx, T Item) Draw<T>(this Span<T> source)
        {
            Contract.Assert(source.Length > 0, "can not draw anything in empty pool");
            return Draw((ReadOnlySpan<T>)source);
        }

        /// <summary>
        ///     加权抽取1个元素
        /// </summary>
        public static (ushort Idx, T Item) Draw<T>(this (T Item, uint Weight)[] source)
        {
            Contract.Assert(source.Length > 0, "can not draw anything in empty pool");
            return Draw(new ReadOnlySpan<(T, uint)>(source));
        }

        /// <summary>
        ///     加权抽取1个元素
        /// </summary>
        public static (ushort Idx, T Item) Draw<T>(this Span<(T Item, uint Weight)> source)
        {
            Contract.Assert(source.Length > 0, "can not draw anything in empty pool");
            return Draw((ReadOnlySpan<(T, uint)>)source);
        }

        #endregion

        #endregion

        #region Lottery Permutation

        /// <summary>
        ///     等概率抽取n个元素（排列）
        /// </summary>
        public static IEnumerable<(ushort Index, T Item)> Permutation<T>(this T[] source, uint count)
        {
            if (source.Length == 0) throw new Exception("can not draw anything in empty pool");
            if (!(count > 0 && count < source.Length))
                throw new Exception($"count need belong to [1,{nameof(source)}.{nameof(source.Length)}]");
            if (count == 1) yield return source.Draw();
            var len = source.Length;
            // ReSharper disable once SuspiciousTypeConversion.Global
            var indexlst = Enumerable.Range(0, len).Cast<ushort>().ToList();
            for (; count > 0; count--)
            {
                var draw = indexlst.Draw();
                yield return (draw.Item, source[draw.Item]);
                indexlst.RemoveAt(draw.Idx);
            }
        }

        /// <summary>
        ///     等概率抽取n个元素（排列）
        /// </summary>
        public static IEnumerable<(ushort Index, T Item)> Permutation<T>(this IList<T> source, uint count)
        {
            if (source.Count == 0) throw new Exception("can not draw anything in empty pool");
            if (!(count > 0 && count < source.Count))
                throw new Exception($"count need belong to [1,{nameof(source)}.{nameof(source.Count)}]");
            if (count == 1) yield return source.Draw();
            var len = source.Count;
            // ReSharper disable once SuspiciousTypeConversion.Global
            var indexlst = Enumerable.Range(0, len).Cast<ushort>().ToList();
            for (; count > 0; count--)
            {
                var draw = indexlst.Draw();
                yield return (draw.Item, source[draw.Item]);
                indexlst.RemoveAt(draw.Idx);
            }
        }

        /// <summary>
        ///     加权抽取n个元素（排列）(非树）
        /// </summary>
        public static IEnumerable<(ushort Index, T Item)> Permutation<T>(this (uint Weight, T Item)[] source,
            uint count)
        {
            if (source.Length == 0) throw new Exception("can not draw anything in empty pool");
            if (!(count > 0 && count < source.Length))
                throw new Exception($"count need belong to [1,{nameof(source)}.{nameof(source.Length)}]");
            long sum = 0;
            var len = source.Length;
            for (var i = 0; i < len; i++) sum += source[i].Weight;

            //通常count总是远小于weight_item数量的，这里算法按记录已取出索引而不是记录剩余索引做
            var indexSkip = new SortedSet<ushort>(); //记录下已经取出的物品
            for (; count > 0; count--)
            {
                var rand = RandomUtil.MoreEqualMinLessMax(1, sum + 1);
                for (ushort i = 0; i < len; i++)
                {
                    if (indexSkip.Contains(i)) continue; //跳过已经取出的
                    rand -= source[i].Weight;
                    if (rand <= 0)
                    {
                        var item = source[i].Item;
                        yield return (i, item);
                        sum -= source[i].Weight;
                        indexSkip.Add(i); //标记为已经取出
                        break;
                    }
                }

                // if (rand <= 0) throw new Exception("missing yield return"); //断言失败意味着漏抽了元素
            }
        }

        /// <summary>
        ///     加权抽取n个元素（排列）(非树）
        /// </summary>
        public static IEnumerable<(ushort Index, T Item)> Permutation<T>(
            this IList<(uint Weight, T Item)> source, uint count)
        {
            if (source.Count == 0) throw new Exception("can not draw anything in empty pool");
            if (!(count > 0 && count < source.Count))
                throw new Exception($"count need belong to [1,{nameof(source)}.{nameof(source.Count)}]");
            long sum = 0;
            var len = source.Count;
            for (var i = 0; i < len; i++) sum += source[i].Weight;

            //通常count总是远小于weight_item数量的，这里算法按记录已取出索引而不是记录剩余索引做
            var indexSkip = new SortedSet<ushort>(); //记录下已经取出的物品
            for (; count > 0; count--)
            {
                var rand = RandomUtil.MoreEqualMinLessMax(1, sum + 1);
                for (ushort i = 0; i < len; i++)
                {
                    if (indexSkip.Contains(i)) continue; //跳过已经取出的
                    rand -= source[i].Weight;
                    if (rand <= 0)
                    {
                        var item = source[i].Item;
                        yield return (i, item);
                        sum -= source[i].Weight;
                        indexSkip.Add(i); //标记为已经取出
                        break;
                    }
                }

                // if (rand <= 0) throw new Exception("missing yield return"); //断言失败意味着漏抽了元素
            }
        }

        #endregion

        #region Lottery Combination

        /// <summary>
        ///     等概率抽取抽取n个元素（组合）
        /// </summary>
        public static IEnumerable<(ushort Index, T Item)> Combination<T>(this T[] source, uint count)
        {
            if (source.IsEmpty()) throw new Exception("can not draw anything in empty pool");
            for (; count > 0; count--) yield return source.Draw();
        }

        /// <summary>
        ///     等概率抽取抽取n个元素（组合）
        /// </summary>
        public static IEnumerable<(ushort Index, T Item)> Combination<T>(this IList<T> source, uint count)
        {
            if (source.IsEmpty()) throw new Exception("can not draw anything in empty pool");
            for (; count > 0; count--) yield return source.Draw();
        }

        /// <summary>
        ///     加权抽取n个元素（组合）(非树）
        /// </summary>
        public static IEnumerable<(ushort Index, T Item)> Combination<T>(this (uint Weight, T Item)[] source,
            uint count)
        {
            if (source.IsEmpty()) throw new Exception("can not draw anything in empty pool");
            long sum = 0;
            var len = source.Length;
            for (var i = 0; i < len; i++) sum += source[i].Weight;

            for (; count > 0; count--)
            {
                var rand = RandomUtil.MoreEqualMinLessMax(1, sum + 1);
                for (ushort i = 0; i < len; i++)
                {
                    rand -= source[i].Weight;
                    if (rand <= 0)
                    {
                        yield return (i, source[i].Item);
                        break;
                    }
                }

                // Assert(rand <= 0, "missing yield return"); //断言失败意味着漏抽了元素
            }
        }

        /// <summary>
        ///     加权抽取n个元素（组合）(非树）
        /// </summary>
        public static IEnumerable<(ushort Index, T Item)> Combination<T>(
            this IList<(uint Weight, T Item)> source, uint count)
        {
            if (source.IsEmpty()) throw new Exception("can not draw anything in empty pool");
            long sum = 0;
            var len = source.Count;
            for (var i = 0; i < len; i++) sum += source[i].Weight;

            for (; count > 0; count--)
            {
                var rand = RandomUtil.MoreEqualMinLessMax(1, sum + 1);
                for (ushort i = 0; i < len; i++)
                {
                    rand -= source[i].Weight;
                    if (rand <= 0)
                    {
                        yield return (i, source[i].Item);
                        break;
                    }
                }

                // Assert(rand <= 0, "missing yield return"); //断言失败意味着漏抽了元素
            }
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
            if (!n.IsInRange(1, 12)) throw new Exception($"n={n} is too big,n∈[1,12]");
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
            if (!n.IsInRange(1, 20)) throw new Exception($"n={n} is too big,n∈[1,20]");
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