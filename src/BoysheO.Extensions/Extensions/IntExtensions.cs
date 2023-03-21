using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static partial class IntExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int i)
        {
            return Math.Abs(i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(this int value, int another)
        {
            return Math.Min(value, another);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(this int value, int another)
        {
            return Math.Max(value, another);
        }

        /// <summary>
        /// Split int 123=>0,0,0,0,0,0,0,1,2,3
        /// </summary>
        /// <param name="value">source</param>
        /// <param name="buffer">buffer len must be 10</param>
        /// <returns>count of bytes write</returns>
        public static void PositiveIntegerToEachDigit(this int value, Span<int> buffer)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "need >0");
            if (buffer.Length != 10) throw new ArgumentOutOfRangeException(nameof(buffer), "buffer len must be 10");
            var p = buffer.Length - 1;
            while (value > 0)
            {
                if (p < 0) return;
                buffer[p] = value % 10;
                p--;
                value /= 10;
            }

            while (p >= 0)
            {
                buffer[p] = 0;
                p--;
            }
        }

        /// <summary>
        /// value &gt; 0 &amp;&amp; value &lt; collection.Count
        /// </summary>
        public static bool IsValidIndex<T>(this int value, ICollection<T> collection)
        {
            return value > 0 && value < collection.Count;
        }
    }
}