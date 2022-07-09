using System;
using System.Buffers;
using System.Collections.Generic;
using Extensions;

namespace BoysheO.Extensions
{
    public static partial class CharSpanExtensions
    {
        /// <summary>
        /// 将chars解析为int
        /// *假设chars满足条件：纯数字，值在int正范围内
        /// 比int.parse还要快，也适合应付从字串部分中获得int值的情况
        /// </summary>
        [Obsolete("use ToPositiveInt instead to avoid Semantic trap", true)]
        public static int ToIntNumber(this ReadOnlySpan<char> chars)
        {
            int result = 0;
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
        /// 将chars解析为int<br />
        /// *假设chars满足条件：纯数字，值在int正范围内<br />
        /// 比int.parse还要快，也适合应付从字串部分中获得int值的情况<br />
        /// </summary>
        public static int ToPositiveInt(this ReadOnlySpan<char> chars)
        {
            var res = ToPositiveLong(chars);
            if (res < int.MinValue || res > int.MaxValue) throw new Exception($"arg {nameof(chars)}={chars.ToString()} is outOf int range[{int.MinValue},{int.MaxValue}]");
            return unchecked((int)res);
        }

        /// <summary>
        /// 识别字串中的数字。任意符号均视为分割符，会识别表示正负的+-符号。但是不支持四则运算 
        /// </summary>
        /// <param name="str">source</param>
        /// <param name="initBuffSize">初始大小，至少为1。函数内部不再检验，如果少于1会报错</param>
        /// <returns>结果 结果大小与initBuffSize无关</returns>
        public static int[] SplitAsIntArray(this ReadOnlySpan<Char> str, int initBuffSize)
        {
            var buf = SplitAsIntPoolArray(str, initBuffSize);
            // ReSharper disable once InvokeAsExtensionMethod
            var ary = BoysheO.Extensions.EnumerableExtensions.ToArray(buf);
            ArrayPool<int>.Shared.Return(buf.Array);
            return ary;
        }

        /// <summary>
        /// 识别字串中的数字。任意符号均视为分割符，会识别表示正负的+-符号。但是不支持四则运算 
        /// </summary>
        /// <param name="str">source</param>
        /// <param name="initBuffSize">初始大小，至少为1。函数内部不再检验，如果少于1会报错</param>
        /// <param name="buff">用于承载结果的列表</param>
        public static void SplitAsIntArray(this ReadOnlySpan<Char> str, int initBuffSize, IList<int> buff)
        {
            buff.Clear();
            var result = SplitAsIntPoolArray(str, initBuffSize);
            var span = result.AsSpan();
            foreach (var i in span)
            {
                buff.Add(i);
            }

            ArrayPool<int>.Shared.Return(result.Array);
        }

        /// <summary>
        /// 识别字串中的数字。任意符号均视为分割符，会识别表示正负的+-符号。不支持四则运算 
        /// </summary>
        /// <param name="str">source</param>
        /// <param name="initBuffSize">初始大小，至少为1。函数内部不再检验，如果少于1会报错</param>
        /// <returns>返回结果，此结果在使用完毕后需要归还到ArrayPool</returns>
        /// <exception cref="Exception"></exception>
        private static ArraySegment<int> SplitAsIntPoolArray(ReadOnlySpan<char> str, int initBuffSize)
        {
            const int mm = int.MaxValue / 10;
            var buffer = ArrayPool<int>.Shared.Rent(initBuffSize);
            var bufferLen = buffer.Length;
            var bufPointer = 0;
            buffer[0] = 0; //init

            var strLen = str.Length;
            var charOrder = 0;
            // int idx = -1;
            var state = 0; //0-findNumberAndSign 1:readingNumber 
            var isM = false; //是否负数
            unsafe
            {
                fixed (char* p1 = str)
                {
                    var charPointer = p1;
                    while (charOrder < strLen)
                        switch (state)
                        {
                            case 0:
                                if (*charPointer >= '0' && *charPointer <= '9')
                                {
                                    isM = false;
                                    state = 1;
                                }
                                else if (*charPointer == '-')
                                {
                                    isM = true;
                                    charPointer++;
                                    charOrder++;
                                    state = 1;
                                }
                                else if (*charPointer == '+')
                                {
                                    isM = false;
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
                                while (charOrder < strLen && *charPointer >= '0' && *charPointer <= '9')
                                {
                                    if (buffer[bufPointer] > mm) throw new Exception($"至少一个数字超过int.Max大小");
                                    buffer[bufPointer] *= 10;
                                    buffer[bufPointer] += *charPointer - 48;
                                    charPointer++;
                                    charOrder++;
                                }

                                if (isM) buffer[bufPointer] = -buffer[bufPointer];

                                bufPointer++;
                                if (bufPointer >= bufferLen)
                                {
                                    buffer = ArrayPoolUtil.Resize(buffer, bufPointer + 1);
                                    bufferLen = buffer.Length;
                                }

                                buffer[bufPointer] = 0;

                                state = 0;

                                break;
                        }

                    return new ArraySegment<int>(buffer, 0, bufPointer);
                }
            }
        }


        /// <summary>
        /// 将chars解析为long
        /// *假设chars满足条件：纯数字，值在long正范围内
        /// 比long.parse还要快，也适合应付从字串部分中获得int值的情况
        /// </summary>
        [Obsolete("use ToPositiveLong instead to avoid Semantic trap", true)]
        public static long ToLongNumber(this ReadOnlySpan<char> chars)
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
    }
}