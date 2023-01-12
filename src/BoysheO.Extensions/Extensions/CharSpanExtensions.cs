using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Extensions;

namespace BoysheO.Extensions
{
    public static class CharSpanExtensions
    {
        /// <summary>
        /// 将chars解析为int<br />
        /// *假设chars满足条件：纯数字，值在int正范围内<br />
        /// 比int.parse还要快，也适合应付从字串部分中获得int值的情况<br />
        /// </summary>
        public static int ParserToPositiveInt(this ReadOnlySpan<char> chars)
        {
            var res = ToPositiveLong(chars);
            if (res < int.MinValue || res > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(chars),
                    $"arg {nameof(chars)}={chars.ToString()} is outOf int range[{int.MinValue},{int.MaxValue}]");
            return unchecked((int)res);
        }

        /// <summary>
        /// Identifies numbers in string.<br />
        /// The char not number and + - will be dealt as separator<br />
        /// NOT SUPPORT any calculations in string<br />
        /// 识别字串中的数字。任意符号均视为分割符，会识别表示正负的+-符号。不支持运算 <br />
        /// </summary>
        /// <param name="chars">source</param>
        /// <param name="initBuffSize"></param>
        /// <param name="ints"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="Exception"></exception>
        public static int SplitAsIntPoolArray(this ReadOnlySpan<char> chars, int initBuffSize, out int[] ints)
        {
            if (initBuffSize <= 0) throw new ArgumentOutOfRangeException(nameof(initBuffSize));
            const int mm = int.MaxValue / 10;
            var buffer = ArrayPool<int>.Shared.Rent(initBuffSize);
            var bufferLen = buffer.Length;
            var buffCount = 0;
            buffer[0] = 0; //init

            var charsLength = chars.Length;
            var charOrder = 0;
            var state = 0; //0-findNumberAndSign 1:readingNumber 
            var isNegative  = false; //是否负数
            unsafe
            {
                fixed (char* p1 = chars)
                {
                    var charPointer = p1;
                    while (charOrder < charsLength)
                        switch (state)
                        {
                            case 0:
                                if (*charPointer >= '0' && *charPointer <= '9')
                                {
                                    isNegative  = false;
                                    state = 1;
                                }
                                else if (*charPointer == '-')
                                {
                                    isNegative  = true;
                                    charPointer++;
                                    charOrder++;
                                    state = 1;
                                }
                                else if (*charPointer == '+')
                                {
                                    isNegative  = false;
                                    charPointer++;
                                    charOrder++;
                                    state = 1;
                                }
                                else
                                {
                                    charPointer++;
                                    charOrder++;
                                }

                                break;
                            case 1:
                                while (charOrder < charsLength && *charPointer >= '0' && *charPointer <= '9')
                                {
                                    if (buffer[buffCount] > mm)
                                    {
                                        ArrayPool<int>.Shared.Return(buffer);
                                        throw new Exception($"至少一个数字超过int.Max大小");
                                    }
                                    buffer[buffCount] *= 10;
                                    buffer[buffCount] += *charPointer - 48;
                                    charPointer++;
                                    charOrder++;
                                }

                                if (isNegative ) buffer[buffCount] = -buffer[buffCount];

                                buffCount++;
                                if (buffCount >= bufferLen)
                                {
                                    buffer = ArrayPoolUtil.Resize(buffer, buffCount + 1);
                                    bufferLen = buffer.Length;
                                }

                                buffer[buffCount] = 0;

                                state = 0;

                                break;
                        }

                    ints = buffer;
                    return buffCount;
                }
            }
        }

        /// <summary>
        /// 将chars解析为long
        /// *假设chars满足条件：纯数字，值在long正范围内
        /// 比long.parse还要快，也适合应付从字串部分中获得int值的情况
        /// </summary>
        public static long ToPositiveLong(this ReadOnlySpan<char> chars)
        {
            long result = 0;
            int count = chars.Length;
            unsafe
            {
                fixed (char* p = chars)
                {
                    var r = p;
                    while (count > 0)
                    {
                        result *= 10;
                        result += *r - 48;
                        r++;
                        count--;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// creat string from chars
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string CreatString(this ReadOnlySpan<char> chars)
        {
            unsafe
            {
                fixed (char* c = chars)
                {
                    return new string(c);
                }
            }
        }
        
        /// <summary>
        /// 在头部截去对应数量的字符
        /// </summary>
        /// <param name="content"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> WithoutHeadCount(this ReadOnlySpan<char> content, string count)
        {
            return content.Slice(count.Length);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> WithoutTailCount(this ReadOnlySpan<char> content, string count)
        {
            return content.Slice(0, content.Length - count.Length);
        }
    }
}