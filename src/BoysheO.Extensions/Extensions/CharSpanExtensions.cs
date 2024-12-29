using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using BoysheO.Util;

namespace BoysheO.Extensions
{
    public static class CharSpanExtensions
    {
        /// <summary>
        /// Convert chars '123' to int 123.<br />
        /// <b>*UNSAFE</b>:make sure chars is all digit char.Not '-123' '12.3'<br />
        /// This method is faster than <see cref="int.Parse(string)"/> 
        /// </summary>
        public static int ParseToPositiveInt(this ReadOnlySpan<char> chars)
        {
            var res = ParseToPositiveLong(chars);
            if (res < int.MinValue || res > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(chars),
                    $"arg {nameof(chars)}={chars.ToString()} is outOf int range[{int.MinValue},{int.MaxValue}]");
            return unchecked((int)res);
        }

        /// <summary>
        /// Identifies numbers in string.<br />
        /// The char not number and + - will be dealt as separator<br />
        /// NOT SUPPORT any calculations in string<br />
        /// ex."a123b46.21+45-87"=>{123,46,21,45,-87}
        /// </summary>
        /// <param name="chars">source</param>
        /// <param name="initBuffSize">any value in [1,+).</param>
        /// <param name="ints">result</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">initBuffSize not in [1,+) or any math value in chars too big</exception>
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
            var isNegative = false; //是否负数
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
                                    isNegative = false;
                                    state = 1;
                                }
                                else if (*charPointer == '-')
                                {
                                    isNegative = true;
                                    charPointer++;
                                    charOrder++;
                                    state = 1;
                                }
                                else if (*charPointer == '+')
                                {
                                    isNegative = false;
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
                                        throw new ArgumentOutOfRangeException(nameof(chars),
                                            "one more math value inside chars is over than int.Max");
                                    }

                                    buffer[buffCount] *= 10;
                                    buffer[buffCount] += *charPointer - 48;
                                    charPointer++;
                                    charOrder++;
                                }

                                if (isNegative) buffer[buffCount] = -buffer[buffCount];

                                buffCount++;
                                if (buffCount >= bufferLen)
                                {
                                    RefArrayPoolUtil.Resize(ref buffer, buffCount, buffCount + 1);
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
        /// Convert chars "123" to long 123<br />
        /// <b>*UNSAFE</b>:make sure chars is all digit char.Not '-123' '12.3'<br />
        /// This method is faster than <see cref="long.Parse(string)"/> 
        /// </summary>
        public static long ParseToPositiveLong(this ReadOnlySpan<char> chars)
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
        /// Creates a string from characters.
        /// You should use <see cref="ReadOnlySpan{T}.ToString"/> for most cases.
        /// </summary>
        public static string ToNewString(this ReadOnlySpan<char> chars)
        {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
            return new string(chars);
#else
            unsafe
            {
                fixed (char* c = chars)
                {
                    return new string(c, 0, chars.Length);
                }
            }
#endif
        }

        /// <summary>
        /// Slice the source.
        /// ex."HelloWorld".SkipCount("Hello") => "World"
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> SkipCount(this ReadOnlySpan<char> content, string count)
        {
            return content.Slice(count.Length);
        }

        /// <summary>
        /// Slice the source.
        /// ex."HelloWorld".SkipLastCount("World") => "Hello"
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> SkipTailCount(this ReadOnlySpan<char> content, string count)
        {
            return content.Slice(0, content.Length - count.Length);
        }
    }
}