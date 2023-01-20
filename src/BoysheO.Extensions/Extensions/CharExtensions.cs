using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static class CharExtensions
    {
        /// <summary>
        /// Determine the char is in '0'-'9' without culture.<br />
        /// <see cref="char.IsDigit(char)"/>has more logic.
        /// *Performance tips:very fast in net48,but fail to against char.IsDigital() in net6.0 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is0to9(this char c)
        {
            return c <= 57 && c >= 48;
        }

        /// <summary>
        /// Convert '0'-'9' to byte 0-9
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte To0To9(this char c)
        {
            if (!c.Is0to9()) throw new ArgumentOutOfRangeException($"not a number:{c}");
            return unchecked((byte)(c - 48));
        }

        /// <summary>
        /// Determine the char is in 'a'-'z' without culture.<br />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once IdentifierTypo
        public static bool Isatoz(this char c)
        {
            return c >= 97 && c <= 122;
        }

        /// <summary>
        /// Determine the char is in 'A'-'Z' without culture.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAtoZ(this char c)
        {
            return c >= 65 && c <= 90;
        }

        /// <summary>
        /// Determine the char is in [a-zA-Z] without culture.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnglishLetter(this char c)
        {
            return c.Isatoz() || c.IsAtoZ();
        }

        /// <summary>
        /// Convert [a-zA-Z] to [A-Z]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToUpper(this char c)
        {
            if (IsAtoZ(c)) return c;
            if (Isatoz(c)) return (char)(c - 32);
            throw new Exception($"{c} is not english letter");
        }

        /// <summary>
        /// Convert [a-zA-Z] to [a-z]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToLower(this char c)
        {
            if (Isatoz(c)) return c;
            if (IsAtoZ(c)) return (char)(c + 32);
            throw new Exception($"{c} is not english letter");
        }

        /// <summary>
        /// Convert char[a-zA-Z] to [1,26]<br />
        /// It's useful for convert Excel col number ABC.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte CovertAZazTo26(this char c)
        {
            if (c.Isatoz()) return (byte)(c - 96);
            if (c.IsAtoZ()) return (byte)(c - 64);
            throw new ArgumentOutOfRangeException(nameof(c), $"c={c} not the EnglishLetter");
        }
    }
}