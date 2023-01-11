using System;
using System.IO;

namespace BoysheO.Extensions
{
    public static class MemoryExtensions
    {
        /// <summary>
        ///     正数代表左移，负数代表右移；即当count=1时，the last在数组末端
        /// </summary>
        /// <param name="span"></param>
        /// <param name="defaultT">从offset的位置左移1位-1，右移1位+1</param>
        /// <param name="count">正数代表左移，负数代表右移</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentException"></exception>
        public static void Panning<T>(this Span<T> span, int count = 1, Func<int, T>? defaultT = null)
            // where T : notnull
        {
            var len = span.Length;
            if (count == 0) throw new ArgumentException(nameof(count));
            defaultT ??= _ => default!;

            if (count < 0)
                for (var i = len - 1; i >= 0; i--)
                {
                    var valueNeed = i + count;
                    span[i] = valueNeed >= 0 && valueNeed < len ? span[valueNeed] : defaultT(valueNeed);
                }
            else
                for (var i = 0; i < len; i++)
                {
                    var valueNeed = i + count;
                    span[i] = valueNeed >= 0 && valueNeed < len ? span[valueNeed] : defaultT(valueNeed);
                }
        }

        // /// <summary>
        // ///     返回mem中的0-Position部分
        // /// </summary>
        // public static ArraySegment<byte> AsSegment(this MemoryStream mem)
        // {
        //     if (!mem.TryGetBuffer(out var buffer)) throw new Exception("MemoryStream must can be read and write.");
        //
        //     return buffer.Slice(0, checked((int) mem.Position));
        // }
    }
}