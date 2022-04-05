using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Util
{
    public static class ByteUtil
    {
        /// <summary>
        ///     将0-9A-F字符转换成同义byte
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte HexCharToByte(char c)
        {
            if (c >= 48 && c <= 57) return (byte) (c - 48);
            if (c >= 65 && c <= 70) return (byte) (c - 55);
            if (c >= 97 && c <= 102) return (byte) (c - 87);
            throw new ArgumentOutOfRangeException(nameof(c));
        }

        /// <summary>
        ///     将形如"01""AF"这样的Hex字符对，转换成等义byte
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte HexCharToByte(char c1, char c2)
        {
            return unchecked((byte) ((byte) (HexCharToByte(c1) << 4) + HexCharToByte(c2)));
        }
    }
}