using System.Collections.Generic;
using UnityEngine;

namespace BoysheO.Extensions.Unity3DCore
{
    public static class ColorExtensions
    {
        /// <summary>
        ///     字节表示Color(固定按RGBA顺序表示，不遵循大小字节序）
        /// </summary>
        public static IEnumerable<byte> AsEnumerable(this Color32 color32)
        {
            yield return color32.r;
            yield return color32.g;
            yield return color32.b;
            yield return color32.a;
        }

        //C#9语法糖
        public static IEnumerator<byte> GetEnumerator(this Color32 color32)
        {
            return color32.AsEnumerable().GetEnumerator();
        }

        public static string ToHexColorString(this Color32 color32, bool sign = true, bool includeAlpha = true)
        {
            return
                $"{(sign ? "#" : "")}{color32.r:X2}{color32.g:X2}{color32.b:X2}{(includeAlpha ? color32.a.ToString("X2") : "")}";
        }

        public static Color32 ToColor32(this Color color)
        {
            return color;
        }

        public static bool IsSameRGB(this Color32 color32, Color32 another)
        {
            return color32.r == another.r && color32.g == another.g && color32.b == another.b;
        }

        // ReSharper disable once InconsistentNaming
        public static bool IsSameRGBA(this Color32 color32, Color32 another)
        {
            return IsSameRGB(color32, another) && color32.a == another.a;
        }
    }
}