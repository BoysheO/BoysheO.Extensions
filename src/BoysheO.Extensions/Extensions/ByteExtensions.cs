using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using BoysheO.Util;

namespace BoysheO.Extensions
{
    public static class ByteExtensions
    {
        #region ToHexText

        /// <summary>
        ///     根据集合元素，输出集合的hex字符串表达，形如“A1 BE C1”<br />
        ///     Get Hex string from bytes like "A1 BE C1".<br />
        ///     *Performance tips:this method use stringBuilder,and not very fast.
        /// </summary>
        public static string ToHexText(this IEnumerable<byte> bytes, StringBuilder? usingStringBuilder = null)
        {
            switch (bytes)
            {
                case byte[] byteAry:
                    return byteAry.ToHexText();
                case IList<byte> lst:
                    return lst.ToHexText();
            }

            //4=hex:2 blank:1
            var sb = usingStringBuilder ?? new StringBuilder();
            usingStringBuilder?.Clear();
            foreach (var byte1 in bytes)
            {
                sb.Append(byte1.ToHexText());
                sb.Append(' ');
            }

            if (sb.Length == 0) return "";
            sb.Remove(sb.Length - 1, 1);
            var res = sb.ToString();
            usingStringBuilder?.Clear();
            return res;
        }

        /// <summary>
        ///     根据集合元素，输出集合的hex字符串表达，形如“A1 BE C1”<br />
        ///     Get Hex string from bytes like "A1 BE C1".
        /// </summary>
        private static string ToHexText(this IList<byte> bytes)
        {
            const int stackSize = 1024;
            if (bytes.Count == 0) return "";
            var len = bytes.Count * 3;
            char[]? ary = null;
            Span<char> buff = len > stackSize ? ary = ArrayPool<char>.Shared.Rent(len) : stackalloc char[len];
            int buffCount = 0;
            Span<char> charBuff = stackalloc char[2];
            for (int i = 0, count = bytes.Count; i < count; i++)
            {
                var b = bytes[i];
                ByteUtil.ByteToHexChar(b, charBuff);
                buff[buffCount] = charBuff[1];
                buffCount++;
                buff[buffCount] = charBuff[0];
                buffCount++;
                buff[buffCount] = ' ';
                buffCount++;
            }

            //slice to ignore the last empty char
            string res = buff.Slice(0, buffCount - 1).AsReadOnly().ToNewString();

            if (ary != null)
            {
                ArrayPool<char>.Shared.Return(ary);
            }

            return res;
        }

        /// <summary>
        ///     输出byte的hex字符串表达，形如"A1","01"<br />
        ///     Get Hex string from byte like "A1".<br />
        ///     *Performance tips:it's same to call ToString("X2").
        ///     If call in loop,suggestion is use <see cref="ByteUtil.ByteToHexChar"/> instead 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexText(this byte byte1)
        {
            return byte1.ToString("X2");
        }

        /// <summary>
        ///     根据集合元素，输出集合的hex字符串表达，形如“A1 BE C1”<br />
        ///     Get Hex string from bytes like "A1 BE C1".
        /// </summary>
        public static string ToHexText(this ReadOnlySpan<byte> bytes)
        {
            const int stackSize = 1024;
            if (bytes.Length == 0) return "";
            var len = bytes.Length * 3;
            char[]? ary = null;
            Span<char> buff = len > stackSize ? ary = ArrayPool<char>.Shared.Rent(len) : stackalloc char[len];
            int buffCount = 0;
            Span<char> charBuff = stackalloc char[2];
            for (int i = 0, count = bytes.Length; i < count; i++)
            {
                var b = bytes[i];
                ByteUtil.ByteToHexChar(b, charBuff);
                buff[buffCount] = charBuff[1];
                buffCount++;
                buff[buffCount] = charBuff[0];
                buffCount++;
                buff[buffCount] = ' ';
                buffCount++;
            }

            //slice to ignore the last empty char
            string res = buff.Slice(0, buffCount - 1).AsReadOnly().ToNewString();

            if (ary != null)
            {
                ArrayPool<char>.Shared.Return(ary);
            }

            return res;
        }

        /// <summary>
        ///     根据集合元素，输出集合的hex字符串表达，形如“A1 BE C1”<br />
        ///     Get Hex string from bytes like "A1 BE C1".
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexText(this byte[] bytes)
        {
            return bytes.AsSpan().AsReadOnly().ToHexText();
        }

        /// <summary>
        ///     根据集合元素，输出集合的hex字符串表达，形如“A1 BE C1”<br />
        ///     Get Hex string from bytes like "A1 BE C1".
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexText(this ArraySegment<byte> bytes)
        {
            return bytes.AsSpan().AsReadOnly().ToHexText();
        }

        #endregion

        #region ToBinText

        //primitives
        /// <summary>
        ///     输出字节块的二进制字符串表达，形如"01001"
        /// </summary>
        public static string ToBinText(this ReadOnlySpan<byte> byteSpan, StringBuilder? usingStringBuilder = null)
        {
            if (byteSpan.Length == 0) return "";
            var sb = usingStringBuilder ?? new StringBuilder(byteSpan.Length * 9); //9=8 bit and 1 blank
            usingStringBuilder?.Clear();
            foreach (var byte2 in byteSpan)
            {
                sb.Append(byte2.ToBinText());
                sb.Append(' ');
            }

            sb.Remove(sb.Length - 1, 1);
            usingStringBuilder?.Clear();
            return sb.ToString();
        }

        /// <summary>
        ///     输出字节块的二进制字符串表达，形如"01001"
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinText(this Span<byte> span, StringBuilder? usingStringBuilder = null)
        {
            return ((ReadOnlySpan<byte>)span).ToBinText(usingStringBuilder);
        }

        /// <summary>
        ///     输出字节块的二进制字符串表达，形如"01001"
        /// </summary>
        public static string ToBinText(this byte[] bytes, StringBuilder? usingStringBuilder = null)
        {
            if (bytes.Length == 0) return "";
            var sb = usingStringBuilder ?? new StringBuilder(bytes.Length * 9); //9=8 bit and 1 blank
            usingStringBuilder?.Clear();
            foreach (var byte2 in bytes)
            {
                sb.Append(byte2.ToBinText());
                sb.Append(' ');
            }

            sb.Remove(sb.Length - 1, 1);
            usingStringBuilder?.Clear();
            return sb.ToString();
        }

        //primitives
        /// <summary>
        ///     输出byte的bin字符串表达，形如"00010000","11001000"
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinText(this byte byte1)
        {
            return Convert.ToString(byte1, 2).PadLeft(8, '0');
        }

        /// <summary>
        ///     输出字节块的二进制字符串表达，形如"01001"
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinText(this ReadOnlyMemory<byte> memory, StringBuilder? usingStringBuilder = null)
        {
            return ToBinText(memory.Span, usingStringBuilder);
        }

        /// <summary>
        ///     输出字节块的二进制字符串表达，形如"01001"
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinText(this Memory<byte> memory, StringBuilder? usingStringBuilder = null)
        {
            return ToBinText(memory.Span, usingStringBuilder);
        }

        #endregion

        /// <summary>
        ///     输出字节块的反转数组
        ///     即时演绎，非延时
        ///     *当不需要复制时，使用<see cref="System.Array.Reverse"/>更好
        /// </summary>
        public static byte[] ReverseAndToArray(this ReadOnlySpan<byte> byteSpan)
        {
            // ReSharper disable once UseArrayEmptyMethod
            if (byteSpan.Length == 0) return new byte[0];
            var len = byteSpan.Length;
            var res = new byte[len];
            var p = len - 1;
            foreach (var item in byteSpan)
            {
                res[p] = item;
                p--;
            }

            return res;
        }

        //primitives
        /// <summary>
        ///     将该值类型转换成内存中byte表示，无复制
        ///     仅限基础类型，否则会异常
        ///     *修改返回的span数组会修改初始值
        /// </summary>
        public static Span<byte> AsMemoryByteSpan<T>(this ref T value) where T : unmanaged
        {
            unsafe
            {
                fixed (T* p = &value)
                {
                    var span = new Span<byte>(p, sizeof(T));
                    // Console.WriteLine(span.ToHexString());
                    return span;
                }
            }
        }


        /// <summary>
        ///     将值视作二进制输出hexString
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToMemoryHexText<T>(this T value, Span<char> charBuffer = default)
            where T : unmanaged
        {
            return ((ReadOnlySpan<byte>)value.AsMemoryByteSpan()).ToHexText(charBuffer);
        }
    }
}