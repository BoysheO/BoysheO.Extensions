using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static class CharExtensions
    {
        //48－57 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once InconsistentNaming
        public static bool Is0to9(this char c)
        {
            return c <= 57 && c >= 48;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int To0To9(this char c)
        {
            if (!Is0to9(c)) throw new ArgumentOutOfRangeException($"not a number:{c}");
            return c - 48;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAtoZ(this char c)
        {
            return c >= 97 && c <= 122;
        }
    }
}