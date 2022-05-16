using System;

namespace BoysheO.Extensions
{
    public static partial class CharSpanExtensions
    {
        /// <summary>
        /// 将chars解析为int
        /// *假设chars满足条件：纯数字，值在int正范围内
        /// 比int.parse还要快，也适合应付从字串部分中获得int值的情况
        /// </summary>
        [Obsolete("use ToPositiveInt instead")]
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
        /// 将chars解析为int
        /// *假设chars满足条件：纯数字，值在int正范围内
        /// 比int.parse还要快，也适合应付从字串部分中获得int值的情况
        /// </summary>
        public static int ToPositiveInt(this ReadOnlySpan<char> chars)
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
        /// 将chars解析为long
        /// *假设chars满足条件：纯数字，值在long正范围内
        /// 比long.parse还要快，也适合应付从字串部分中获得int值的情况
        /// </summary>
        [Obsolete("use ToPositiveLong instead")]
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