using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BoysheO.Util
{
    public static class ByteUtil
    {
        /// <summary>
        ///     将单个0-9A-Fa-f字符视作Hex转byte
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte HexCharToByte(char c)
        {
            if (c >= 48 && c <= 57) return (byte)(c - 48);
            if (c >= 65 && c <= 70) return (byte)(c - 55);
            if (c >= 97 && c <= 102) return (byte)(c - 87);
            throw new ArgumentOutOfRangeException(nameof(c));
        }

        /// <summary>
        ///     将形如"01""AF"这样的Hex字符对，转换成等义byte
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte HexCharToByte(char c1, char c2)
        {
            return unchecked((byte)((byte)(HexCharToByte(c1) << 4) + HexCharToByte(c2)));
        }

        /// <summary>
        /// Convert the byte as '0A','3F'...<br />
        /// buff's index 0 is the high digit.<br />
        /// Buff must be 2 len
        /// </summary>
        public static void ByteToHexChar(byte src, Span<char> buff)
        {
            if (buff.Length != 2) throw new ArgumentException("buff len must be 2", nameof(buff));
            var low = src & 0x0F;
            if (low <= 9) buff[1] = (char)(low + 48);
            else buff[1] = (char)(low - 10 + 65);
            var high = (src & 0xF0) >> 4;
            if (high <= 9) buff[0] = (char)(high + 48);
            else buff[0] = (char)(high - 10 + 65);
        }

        /// <summary>
        /// Convert the byte to bin numbers.ex.0A=>1010 (numbers' index 0 is 1) <br />
        /// true is 1,false is 0
        /// </summary>
        public static void ByteToBinDigits(byte src, Span<bool> numbers)
        {
            if (numbers.Length != 8) throw new ArgumentException("numbers len must be 8", nameof(numbers));
            numbers[0] = (src & 0b1000_0000) == 0b1000_0000;
            numbers[1] = (src & 0b0100_0000) == 0b0100_0000;
            numbers[2] = (src & 0b0010_0000) == 0b0010_0000;
            numbers[3] = (src & 0b0001_0000) == 0b0001_0000;
            numbers[4] = (src & 0b0000_1000) == 0b0000_1000;
            numbers[5] = (src & 0b0000_0100) == 0b0000_0100;
            numbers[6] = (src & 0b0000_0010) == 0b0000_0010;
            numbers[7] = (src & 0b0000_0001) == 0b0000_0001;
        }

        #region IsGZipHeader

        /// <summary>
        ///     判断前2个字节是否符合gzip标准1f8b<br />
        ///     Get to know is the bytes start with 1f8b GZip header
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGZipHeader(this ReadOnlySpan<byte> bytes)
        {
            return bytes.Length >= 2 && bytes[0] == 0x1f && bytes[1] == 0x8b;
        }

        /// <summary>
        ///     判断前2个字节是否符合gzip标准1f8b<br />
        ///     Get to know is the bytes start with 1f8b GZip header
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGZipHeader(this Span<byte> bytes)
        {
            return ((ReadOnlySpan<byte>)bytes).IsGZipHeader();
        }

        /// <summary>
        ///     判断前2个字节是否符合gzip标准1f8b<br />
        ///     Get to know is the bytes start with 1f8b GZip header
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGZipHeader(this byte[] bytes)
        {
            return ((ReadOnlySpan<byte>)bytes).IsGZipHeader();
        }

        /// <summary>
        ///     判断前2个字节是否符合gzip标准1f8b<br />
        ///     Get to know is the bytes start with 1f8b GZip header
        /// </summary>
        public static bool IsGZipHeader(this IEnumerable<byte> bytes)
        {
            using var enumerator = bytes.GetEnumerator();
            if (!enumerator.MoveNext()) return false;
            if (enumerator.Current != 0x1f) return false;
            if (!enumerator.MoveNext()) return false;
            if (enumerator.Current != 0x8b) return false;
            return true;
        }

        #endregion
    }
}