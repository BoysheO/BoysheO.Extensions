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
        ///     Get Hex string from byte like "A1","0F".<br />
        ///     *Performance tips:it's same to call ToString("X2").
        ///     If call in loop,suggestion is use <see cref="ByteUtil.ByteToHexChar"/> instead 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexText(this byte byte1)
        {
            return byte1.ToString("X2");
        }

        /// <summary>
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
                buff[buffCount] = charBuff[0];
                buffCount++;
                buff[buffCount] = charBuff[1];
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
        ///     Get Hex string from bytes like "A1 BE C1".
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexText(this byte[] bytes)
        {
            return bytes.AsSpan().AsReadOnly().ToHexText();
        }

        /// <summary>
        ///     Get Hex string from bytes like "A1 BE C1".
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexText(this ArraySegment<byte> bytes)
        {
            return bytes.AsSpan().AsReadOnly().ToHexText();
        }

        #endregion

        #region ToBinText

        /// <summary>
        ///     Get binText from byteSpan.ex."01001"
        /// </summary>
        public static string ToBinText(this ReadOnlySpan<byte> byteSpan)
        {
            const int stackSize = 1024;
            if (byteSpan.Length == 0) return "";
            var charBuffCapacity = checked(byteSpan.Length * 8);
            char[] pooledBuff = null;
            Span<char> charBuff = charBuffCapacity > stackSize
                ? (pooledBuff = ArrayPool<char>.Shared.Rent(charBuffCapacity))
                : stackalloc char[charBuffCapacity];
            var charBuffCount = 0;


            Span<bool> bBuff = stackalloc bool[8];
            foreach (var byte2 in byteSpan)
            {
                ByteUtil.ByteToBinDigits(byte2, bBuff);
                //len of bBufff is immutable.It's algorithm complexity is O(1)
                foreach (var b in bBuff)
                {
                    charBuff[charBuffCount] = b ? '1' : '0';
                    charBuffCount++;
                }
            }

            if (pooledBuff != null)
            {
                ArrayPool<char>.Shared.Return(pooledBuff);
            }

            return charBuff.Slice(0, charBuffCount).AsReadOnly().ToNewString();
        }

        /// <summary>
        ///     Get binText from a byte.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinText(this byte byte1)
        {
            Span<bool> buff = stackalloc bool[8];
            ByteUtil.ByteToBinDigits(byte1, buff);
            Span<char> charBuff = stackalloc char[8];
            for (int i = 0; i < 8; i++)
            {
                charBuff[i] = buff[i] ? '1' : '0';
            }

            return charBuff.AsReadOnly().ToNewString();
        }

        #endregion

        /// <summary>
        ///     Copy the span to a new Array reversed.<br />
        ///     *Performance tips:If you don't need to get a new ary,use <see cref="Array.Reverse(System.Array)"/> better.
        /// </summary>
        public static byte[] ToReverseArray(this ReadOnlySpan<byte> byteSpan)
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

        /// <summary>
        ///     Look through the value as bytes in memory.<br />
        ///     *<b>UNSAFE</b>:Changing the return value causes the parameter value changed.
        ///     If you change the string,the program maybe crash.The suggestion
        ///     is use this for debug only.And,I don't promise the return value
        ///     is same in different hardware environment.Only the base value
        ///     can be the parameters.<br />
        ///     将该值类型转换成内存中byte表示，无复制,仅限基础类型，否则会异常.
        ///     *不安全：修改返回的span数组会修改初始值.在不清楚自己在做什么的时候不要修改返回值
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
    }
}