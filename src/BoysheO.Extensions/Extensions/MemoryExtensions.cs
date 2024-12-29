using System;

namespace BoysheO.Extensions
{
    public static class MemoryExtensions
    {
        /// <summary>
        ///     Move every elements in arg:span to left side.<br />
        ///     ex.int{1,2,3,4,5}.Panning(1) => int{2,3,4,5,0}<br />
        ///     ex.int{1,2,3,4,5}.Panning(-1) => int{0,1,2,3,4}<br />
        /// </summary>
        /// <exception cref="ArgumentException">count not be 0</exception>
        public static void Panning<T>(this Span<T> span, int count = 1, Func<int, T>? defaultT = null)
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
        //     if (!mem.TryGetBuffer(out var buffer)) throw new Exception("MemoryStream must be read and write.");
        //
        //     return buffer.Slice(0, checked((int) mem.Position));
        // }

        public static ReadOnlySpan<T> AsReadOnly<T>(this Span<T> span) => (ReadOnlySpan<T>)span;
    }
}