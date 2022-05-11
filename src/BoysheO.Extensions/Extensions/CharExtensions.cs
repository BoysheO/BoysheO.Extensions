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

        /// <summary>
        /// 是否小写a-z
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Isatoz(this char c)
        {
            return c >= 97 && c <= 122;
        }

        /// <summary>
        /// 是否大写A-Z
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAtoZ(this char c)
        {
            return c >= 65 && c <= 90;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnglishLetter(this char c)
        {
            return c.Isatoz() || c.IsAtoZ();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToUpper(this char c)
        {
            if (IsAtoZ(c)) return c;
            if (Isatoz(c)) return (char) (c - 32);
            throw new Exception($"{c} is not english letter");
        } 
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToLower(this char c)
        {
            if (Isatoz(c)) return c;
            if (IsAtoZ(c)) return (char) (c + 32);
            throw new Exception($"{c} is not english letter");
        }

    }
}