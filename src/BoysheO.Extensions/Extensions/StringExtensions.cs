using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using BoysheO.Util;
using Extensions;

namespace BoysheO.Extensions
{
    public static class StringExtensions
    {
        // /// <summary>
        // ///     对string数组中所有string删除空白并返回新值
        // ///     源字串组内不应有null元素，不会返回null元素
        // ///     <para>空白字符为[ \f\n\r\t\v]</para>
        // /// </summary>
        // public static IEnumerable<string> RemoveSpaces(this IEnumerable<string> strAry)
        // {
        //     foreach (var str in strAry)
        //     {
        //         if (str == null) continue;
        //         yield return str.RemoveSpaces();
        //     }
        // }

        /// <summary>
        ///     将字符串连接成一句，等价于string.Join<br />
        ///     性能提示：系统自带的string.Join比ZString要快而且GC一样，并且string.Join性能在高版本CLR中有大幅提升，见Benchmark.StringExtensions.JoinAsOneString
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string JoinAsOnString(this IEnumerable<string> strings, string sp = ",")
        {
            return string.Join(sp, strings);
        }

        /// <summary>
        ///     删除所有空白字符并返回
        /// </summary>
        public static string RemoveSpaces(this string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            Span<char> res = stackalloc char[str.Length];
            int resCount = 0;
            ReadOnlySpan<char> src = str.AsSpan();
            for (int i = 0, count = src.Length; i < count; i++)
            {
                var c = src[i];
                if (!char.IsWhiteSpace(c))
                {
                    res[resCount] = c;
                    resCount++;
                }
            }

            unsafe
            {
                fixed (char* p = res)
                {
                    return new string(p, 0, resCount);
                }
            }
        }

        //性能不佳
        // /// <summary>
        // ///     将字符串视作int列表，此函数包含了忽略非数字字符、以中英文逗号为分割点的功能
        // /// </summary>
        // public static IEnumerable<int> AsIntEnumerable(this string? str)
        // {
        //     if (str == null) yield break;
        //
        //     int? num = null;
        //     foreach (var curChar in str)
        //         if (curChar.Is0to9())
        //         {
        //             var n = curChar.To0To9();
        //             if (num != null)
        //             {
        //                 num = num.Value * 10;
        //                 num = num.Value + n;
        //             }
        //             else
        //             {
        //                 num = n;
        //             }
        //         }
        //         else if (curChar == ',' || curChar == '，')
        //         {
        //             if (num != null)
        //             {
        //                 yield return num.Value;
        //                 num = null;
        //             }
        //         }
        // }

        /// <summary>
        ///     将字符串视作string列表，此函数包含了忽略元素两端空白字符、以中英文逗号为分割点的功能;不忽略双逗号时的""字符串
        ///     实测效率不如replace+split
        /// </summary>
        // [Obsolete("use replace+split instead")]
        // public static IEnumerable<string> AsStringEnumerable(this string? str)
        // {
        //     if (str == null) yield break;
        //
        //     var start = -1;
        //     var len = str.Length;
        //     for (var i = 0; i < len; i++)
        //     {
        //         var c = str[i];
        //         if (c == ',' || c == '，')
        //         {
        //             if (start + 1 == i)
        //                 yield return "";
        //             else
        //                 yield return str.Substring((start + 1), i - (start + 1));
        //             // yield return str[(start + 1).. i];
        //
        //             start = i;
        //         }
        //     }
        //
        //     if (start + 1 == len - 1)
        //         yield return "";
        //     else
        //         yield return str.Substring(start + 1);
        // }

        // /// <summary>
        // ///     遍历替换oldstr，结果可能包含""，null(str是null的情况下)，如果参数集合中包含null则会从replace中引发异常
        // ///     <para>old值和new值应一一对应</para>
        // ///     <para>TODO 优化GC</para>
        // /// </summary>
        // public static string Replace(this string str, IEnumerable<string> oldstr, IEnumerable<string> newstr)
        // {
        //     if (str.IsNullOrEmpty()) return str;
        //     var itor = oldstr.GetEnumerator();
        //     var newitor = newstr.GetEnumerator();
        //     while (itor.MoveNext())
        //     {
        //         var newerstr = newitor.MoveNext() ? newitor.Current : "";
        //         str = str.Replace(itor.Current, newerstr);
        //     }
        //
        //     itor.Dispose();
        //     newitor.Dispose();
        //     return str;
        // }

        // /// <summary>
        // ///     遍历替换oldchar，结果可能包含""，null(str是null的情况下)，如果参数集合中包含null则会从replace中引发异常
        // ///     <para>old值和new值应一一对应</para>
        // ///     <para>TODO 优化GC</para>
        // /// </summary>
        // public static string Replace(this string str, IEnumerable<char> oldchar, IEnumerable<char> newchar)
        // {
        //     if (str.IsNullOrEmpty()) return str;
        //     var itor = oldchar.GetEnumerator();
        //     var newitor = newchar.GetEnumerator();
        //     while (itor.MoveNext())
        //         if (newitor.MoveNext())
        //             str = str.Replace(itor.Current, newitor.Current);
        //         else str = str.Remove(itor.Current);
        //     itor.Dispose();
        //     newitor.Dispose();
        //     return str;
        // }

        /// <summary>
        ///     遍历替换oldchar，结果可能包含""
        ///     //todo test empty insertStr
        /// </summary>
        public static string Replace(this string srcStr, ReadOnlySpan<char> chars, string insertStr)
        {
            if (srcStr == null) throw new ArgumentNullException(nameof(srcStr));
            if (insertStr == null) throw new ArgumentNullException(nameof(insertStr));
            if (chars.IsEmpty) throw new ArgumentOutOfRangeException(nameof(chars));
            Span<char> res = stackalloc char[srcStr.Length];
            int resCount = 0;
            for (int srcStrIdx = 0, srcStrLen = srcStr.Length; srcStrIdx < srcStrLen; srcStrIdx++)
            {
                var srcChar = srcStr[srcStrIdx];
                bool isCharOld = false;
                for (int oldCharIdx = 0, oldCharLen = chars.Length; oldCharIdx < oldCharLen; oldCharIdx++)
                {
                    var oldChar = chars[oldCharIdx];
                    if (oldChar == srcChar)
                    {
                        isCharOld = true;
                        break;
                    }
                }

                if (isCharOld)
                {
                    insertStr.AsSpan().CopyTo(res.Slice(resCount));
                    resCount += insertStr.Length;
                }
                else
                {
                    res[resCount] = srcChar;
                    resCount++;
                }
            }

            unsafe
            {
                fixed (char* cc = res)
                {
                    return new string(cc, 0, resCount);
                }
            }
        }

        /// <summary>
        ///     遍历替换oldchar，结果可能包含""
        ///     //todo test empty insertStr
        /// </summary>
        public static string Replace(this string srcStr, ReadOnlySpan<char> chars, char insertChar)
        {
            if (srcStr == null) throw new ArgumentNullException(nameof(srcStr));
            if (chars.IsEmpty) throw new ArgumentOutOfRangeException(nameof(chars));
            Span<char> res = stackalloc char[srcStr.Length];
            int resCount = 0;
            for (int srcStrIdx = 0, srcStrLen = srcStr.Length; srcStrIdx < srcStrLen; srcStrIdx++)
            {
                var srcChar = srcStr[srcStrIdx];
                bool isCharOld = false;
                for (int oldCharIdx = 0, oldCharLen = chars.Length; oldCharIdx < oldCharLen; oldCharIdx++)
                {
                    var oldChar = chars[oldCharIdx];
                    if (oldChar == srcChar)
                    {
                        isCharOld = true;
                        break;
                    }
                }

                if (isCharOld)
                {
                    res[resCount] = insertChar;
                    resCount++;
                }
                else
                {
                    res[resCount] = srcChar;
                    resCount++;
                }
            }

            unsafe
            {
                fixed (char* cc = res)
                {
                    return new string(cc, 0, resCount);
                }
            }
        }

        /// <summary>
        ///     遍历替换oldStrs，结果可能包含""，如果参数集合中包含null则会引发异常
        ///     *性能提示：本API等同遍历Replace
        /// </summary>
        public static string Replace(this string str, IEnumerable<string> oldStrs, string newStr)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            foreach (var oStr in oldStrs) str = str.Replace(oStr, newStr);
            return str;
        }

        //实测效率与string.replace几乎一致，因此不用。注释以备忘
        // /// <summary>
        // /// 将文字中的“/n”替换成换行符'\n'
        // /// </summary>
        // /// <param name="source"></param>
        // /// <returns></returns>
        // public static string ReplaceNToLine(this string source)
        // {
        //     Span<char> buf = stackalloc char[source.Length];
        //     int bufCount = 0;
        //     var span = source.AsSpan();
        //     int matchCount = 0; //0:no match 1:\ match 2:\n match 
        //     var p = 0;
        //     var len = source.Length;
        //     while (p < len)
        //     {
        //         var c = span[p];
        //         switch (matchCount)
        //         {
        //             case 0:
        //                 if (c == '\\')
        //                 {
        //                     matchCount++;
        //                 }
        //                 else
        //                 {
        //                     buf[bufCount] = c;
        //                     bufCount++;
        //                 }
        //
        //                 break;
        //             case 1:
        //                 if (c == 'n')
        //                 {
        //                     buf[bufCount] = '\n';
        //                     bufCount++;
        //                 }
        //                 else
        //                 {
        //                     buf[bufCount] = span[p - 1];
        //                     bufCount++;
        //                     buf[bufCount] = c;
        //                     bufCount++;
        //                 }
        //
        //                 matchCount = 0;
        //
        //                 break;
        //         }
        //
        //         p++;
        //     }
        //
        //     unsafe
        //     {
        //         fixed (char* p1 = buf)
        //         {
        //             return new string(p1, 0, bufCount);
        //         }
        //     }
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReplaceByRegex(this string str, string pattern, string value)
        {
            return Regex.Replace(str, pattern, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReplaceByRegex(this string str, Regex regex, MatchEvaluator evaluator)
        {
            return regex.Replace(str, evaluator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string? str)
        {
            return string.IsNullOrEmpty(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        //path 相关api应当封装到独立的结构中
        // /// <summary>
        // ///     见<see cref="Path.Combine(string,string)" />，另外会对\号转成/号
        // ///     u3d在可以使用字符串拼接的情况下，优先使用字符串拼接
        // /// </summary>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static string PathCombine(this string patha, string pathb)
        // {
        //     return Path.Combine(patha, pathb).Replace("\\", "/");
        // }

        // /// <summary>
        // ///     <see cref="Path.GetFileName(string)" />的包装形式
        // /// </summary>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static string GetPathFileName(this string path)
        // {
        //     return Path.GetFileName(path);
        // }

        // /// <summary>
        // ///     <see cref="Path.GetDirectoryName(string)" />的包装形式
        // /// </summary>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static string GetPathDir(this string path)
        // {
        //     return Path.GetDirectoryName(path) ?? throw new Exception("no dir");
        // }

        /// <summary>
        ///     调用<see cref="Regex.IsMatch(string, string, RegexOptions)" />验证,op=
        ///     <see cref="RegexOptions.CultureInvariant" />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMatch(this string input, string pattern)
        {
            return Regex.IsMatch(input, pattern, RegexOptions.CultureInvariant);
        }

        /// <summary>
        ///     调用regex验证
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMatch(this string input, Regex regex)
        {
            return regex.IsMatch(input);
        }

        //使用频率太低
        // /// <summary>
        // ///     string转换成utf8并获取base64
        // /// </summary>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static string ToBase64(this string name)
        // {
        //     var bytes = Encoding.UTF8.GetBytes(name);
        //     return Convert.ToBase64String(bytes);
        // }

        //考虑直接使用base62
        // /// <summary>
        // ///     将字符串转换成^[_a-zA-Z0-9]$且以_i开头的SusiBase64字符串 (_i开头是便于识别和规避数字开头的命名不能作为变量命名的规则）
        // /// </summary>
        // public static string ToSusiBase64(this string str)
        // {
        //     var base64 = str.ToBase64();
        //     return $"_i{base64.Replace("+", "_p").Replace("/", "_s").Replace("=", "_e")}";
        // }

        // /// <summary>
        // ///     将SusiBase64字符串转还原
        // /// </summary>
        // public static string FromSusiBase64(this string str)
        // {
        //     return str.RemoveAmountFromStart("_i")
        //         .Replace("_p", "+")
        //         .Replace("_s", "/")
        //         .Replace("_e", "=")
        //         .FromBase64();
        // }

        ////使用频率太低
        // public static string FromBase64(this string str)
        // {
        //     var bytes = Convert.FromBase64String(str);
        //     return Encoding.UTF8.GetString(bytes);
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToUTF8Bytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToASCIIBytes(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        /// <summary>
        ///     将字符串按char值忠实地转换成byte，对于ASCII编码以外的字符不会转换成问号
        ///     TODO 应该可以使用指针直接转换byte
        /// </summary>
        /// <exception cref="InvalidCastException">字符中包含超出byte范围的字符，超出语义，视作异常</exception>
        public static byte[] ToRawBytes(this string str, bool check = true)
        {
            var len = str.Length;
            var charSpan = str.AsSpan();
            var result = new byte[len];
            for (len--; len >= 0; len--)
            {
                if (check)
                {
                    var c = charSpan[len];
                    if (c < byte.MinValue || c > byte.MaxValue)
                        throw new InvalidCastException($"str={str} idx={len} char={c} is out of byte value range.");
                }

                result[len] = unchecked((byte)charSpan[len]);
            }

            return result;
        }

        /// <summary>
        ///     将字符串按char值忠实地转换成byte，对于ASCII编码以外的字符不会转换成问号
        ///     <para>性能提示：与无buff的ToRawBytes各个性能指标相差不大，可以忽略</para>
        /// </summary>
        /// <exception cref="InvalidCastException">字符中包含超出byte范围的字符，超出语义，视作异常</exception>
        public static void ToRawBytes(this string str, ArraySegment<byte> buff, bool check = true)
        {
            if (buff.Count != str.Length)
                throw new ArgumentException($"buff len need to be str.len {str.Length},but {buff.Count}");
            var len = str.Length;
            var charSpan = str.AsSpan();
            var result = buff.AsSpan();
            for (len--; len >= 0; len--)
            {
                if (check)
                {
                    var c = charSpan[len];
                    if (c < byte.MinValue || c > byte.MaxValue)
                        throw new InvalidCastException($"str={str} idx={len} char={c} is out of byte value range.");
                }

                result[len] = unchecked((byte)charSpan[len]);
            }
        }

        /// <summary>
        ///     将字符串按char值忠实地转换成byte，对于ASCII编码以外的字符不会转换成问号
        ///     <para>性能提示：与无buff的ToRawBytes各个性能指标相差不大，可以忽略</para>
        /// </summary>
        /// <exception cref="InvalidCastException">字符中包含超出byte范围的字符，超出语义，视作异常</exception>
        public static void ToRawBytes(this string str, byte[] buff, int offset, int count, bool check = true)
        {
            if (count != str.Length)
                throw new ArgumentException($"buff len need to be str.len {str.Length},but {count}");
            var len = str.Length;
            var charSpan = str.AsSpan();
            var result = buff.AsSpan(offset, count);
            for (len--; len >= 0; len--)
            {
                if (check)
                {
                    var c = charSpan[len];
                    if (c < byte.MinValue || c > byte.MaxValue)
                        throw new InvalidCastException($"str={str} idx={len} char={c} is out of byte value range.");
                }

                result[len] = unchecked((byte)charSpan[len]);
            }
        }

        /// <summary>
        ///     将byte忠实地按byte转换成char
        /// </summary>
        public static string ToRawString(this ReadOnlySpan<byte> source)
        {
            const int maxStackLimit = 1024;
            var len = source.Length;
            var buffer = len <= maxStackLimit ? stackalloc char[len] : new char[len];
            for (len--; len >= 0; len--) buffer[len] = (char)source[len];

            unsafe
            {
                fixed (char* p = buffer)
                {
                    return new string(p, 0, source.Length);
                }
            }
        }

        /// <summary>
        ///     根据等价char原则，每个byte转换成char再转换成string;warp
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToRawString(this byte[] bytes)
        {
            return ToRawString((ReadOnlySpan<byte>)bytes);
        }

        /// <summary>
        ///     根据等价char原则，每个byte转换成char再转换成string;warp
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToRawString(this ArraySegment<byte> bytes)
        {
            return ToRawString((ReadOnlySpan<byte>)bytes);
        }

        /// <summary>
        ///     根据等价char原则，每个byte转换成char再转换成string;warp
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToRawString(this Span<byte> bytes)
        {
            return ToRawString((ReadOnlySpan<byte>)bytes);
        }

        /// <summary>
        ///     根据string内容转换byte。要求source是稠密无杂质的hexString。未测试
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">输入的source必须是具备偶数个字符0-F的</exception>
        public static byte[] HexTextToBytes(this string source)
        {
            var bufferLen = source.Length / 2;
            var isAsByteArray = bufferLen >= 1024;
            var bytes = isAsByteArray ? stackalloc byte[bufferLen] : new byte[bufferLen];
            for (var i = 0; i < bytes.Length; i++) bytes[i] = ByteUtil.HexCharToByte(source[2 * i], source[2 * i + 1]);

            return bytes.ToArray();
        }

        /// <summary>
        ///     将数据块直接视作string返回（会创建新string）
        ///     强转成string，只有source是按utf16 char才能返回得到C#下的有意义的字段。否则此string的hex就是source的值
        /// </summary>
        public static string MemoryToString(this ReadOnlySpan<byte> source)
        {
            unsafe
            {
                fixed (byte* p = source)
                {
                    var sb = (char*)p;
                    return new string(sb);
                }
            }
        }

        /// <summary>
        ///     强转成string
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MemoryToString(this Span<byte> source)
        {
            return MemoryToString((ReadOnlySpan<byte>)source);
        }

        /// <summary>
        ///     强转成string
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MemoryToString(this byte[] bytes)
        {
            return MemoryToString(bytes.AsSpan());
        }


        /// <summary>
        ///     强制转换成string
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MemoryToString<T>(this ref T source) where T : unmanaged
        {
            return source.AsMemoryByteSpan().MemoryToString();
        }

        /// <summary>
        /// 如果首字符不是字母则原样返回，忽略文化差异
        /// </summary>
        public static string MakeFirstCharUpperOrNot(this string str)
        {
            if (str.Length == 0) return "";
            if (!str[0].Isatoz()) return str;
            if (str.Length == 1) return str.ToUpperInvariant();
            Span<char> buffer = stackalloc char[str.Length];
            buffer[0] = str[0].ToUpper();
            str.AsSpan(1).CopyTo(buffer.Slice(1));
            unsafe
            {
                fixed (char* p = buffer)
                {
                    return new string(p);
                }
            }
        }

        /// <summary>
        /// 如果首字符不是字母则原样返回，忽略文化差异
        /// </summary>
        public static string MakeFirstCharLowerOrNot(this string str)
        {
            if (str.Length == 0) return "";
            if (!str[0].IsAtoZ()) return str;
            if (str.Length == 1) return str.ToLowerInvariant();
            Span<char> buffer = stackalloc char[str.Length];
            buffer[0] = str[0].ToUpper();
            str.AsSpan(1).CopyTo(buffer.Slice(1));
            unsafe
            {
                fixed (char* p = buffer)
                {
                    return new string(p);
                }
            }
        }

        /// <summary>
        /// 等价于int.Parse(string str) <br />
        /// 性能提示：如果能假定str是纯数字，使用ToPositiveInt快4倍
        /// </summary>
        public static int ParserToInt(this string str)
        {
            return int.Parse(str);
        }

        /// <summary>
        /// 纯数字字串转换成int
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ParserToPositiveInt(this string str)
        {
            return str.AsSpan().ParserToPositiveInt();
        }

        /// <summary>
        /// 纯数字字串转换成long
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long ParserToPositiveLong(this string str)
        {
            return str.AsSpan().ToPositiveLong();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(this string str, object a)
        {
            return string.Format(str, a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(this string str, object a, object b)
        {
            return string.Format(str, a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(this string str, object a, object b, object c)
        {
            return string.Format(str, a, b, c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        // /// <summary>
        // /// 虽然正确性可以得到确认，但是比string.Split慢4倍,相差一个数量级
        // /// 测试中gc也失败于string.Split,多4倍gc，此API不再使用
        // /// </summary>
        // [Obsolete]
        // public static int SplitAsPooledChars(this ReadOnlySpan<char> str,
        //     ReadOnlySpan<char> chars,
        //     out (int start, int count)[] pooledResult)
        // {
        //     if (chars.Length == 0) throw new ArgumentException("can not 0 len", nameof(chars));
        //     pooledResult = ArrayPool<(int, int)>.Shared.Rent(1);
        //     int pooledResultCount = 0;
        //     int lp = 0;
        //     int p = 0;
        //     var strLen = str.Length;
        //     var charsLen = chars.Length;
        //     while (p < strLen)
        //     {
        //         bool hasSub;
        //         if (p + charsLen > strLen)
        //         {
        //             hasSub = false;
        //         }
        //         else
        //         {
        //             var slice = str.Slice(p, charsLen);
        //             hasSub = slice.SequenceEqual(chars);
        //         }
        //
        //         if (hasSub)
        //         {
        //             var count = p - lp;
        //             pooledResultCount =
        //                 ArrayPoolUtil.Add(pooledResult, pooledResultCount, (lp, count), out pooledResult);
        //             p += charsLen;
        //             if (p > strLen) p = strLen;
        //             lp = p;
        //             continue;
        //         }
        //
        //         p++;
        //     }
        //
        //     pooledResultCount =
        //         ArrayPoolUtil.Add(pooledResult, pooledResultCount, (lp, str.Length - lp), out pooledResult);
        //     return pooledResultCount;
        // }
    }
}