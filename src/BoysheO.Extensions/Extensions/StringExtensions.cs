using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using BoysheO.Util;

namespace BoysheO.Extensions
{
    public static partial class StringExtensions
    {
        /// <summary>
        ///     对string数组中所有string运行trim并返回自己（会改变数组内的值）
        /// </summary>
        public static void TrimAllElements(this string[] strAry)
        {
            for (var i = 0; i < strAry.Length; i++) strAry[i] = strAry[i].Trim();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<string> SelectWithoutNullAndWhiteSpace(this IEnumerable<string> strAry)
        {
            return strAry.Where(v => !v.IsNullOrWhiteSpace());
        }

        /// <summary>
        ///     对string数组中所有string删除空白并返回新值
        ///     源字串组内不应有null元素，不会返回null元素
        ///     <para>空白字符为[ \f\n\r\t\v]</para>
        /// </summary>
        public static IEnumerable<string> RemoveSpaces(this IEnumerable<string> strAry)
        {
            var chars = new[] { ' ', '\f', '\n', '\r', '\t', '\v' };
            foreach (var str in strAry)
            {
                if (str == null) throw new NullReferenceException("null element was rejected");
                if (str == "") yield return "";
                yield return str.Replace(chars, "");
            }
        }

        /// <summary>
        ///     将字符串连接成一句
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string JoinAsOneString(this IEnumerable<string> strings, string sp = ",")
        {
            return string.Join(sp, strings);
        }


        /// <summary>
        ///     删除所有空白字符并返回
        ///     <para>删除空白后如果是""，保留""，null元素返回null</para>
        ///     <para>空白字符为[ \f\n\r\t\v]</para>
        /// </summary>
        public static string RemoveSpaces(this string str)
        {
            if (str == null) throw new Exception("str can not be null");
            return str.Replace(new[] { " ", "\f", "\n", "\r", "\t", "\v" }, "");
        }

        /// <summary>
        ///     将字符串视作int列表，此函数包含了忽略非数字字符、以中英文逗号为分割点的功能
        /// </summary>
        public static IEnumerable<int> AsIntEnumerable(this string? str)
        {
            if (str == null) yield break;

            int? num = null;
            foreach (var curChar in str)
                if (curChar.Is0to9())
                {
                    var n = curChar.To0To9();
                    if (num != null)
                    {
                        num = num.Value * 10;
                        num = num.Value + n;
                    }
                    else
                    {
                        num = n;
                    }
                }
                else if (curChar == ',' || curChar == '，')
                {
                    if (num != null)
                    {
                        yield return num.Value;
                        num = null;
                    }
                }
        }

        /// <summary>
        ///     将字符串视作string列表，此函数包含了忽略元素两端空白字符、以中英文逗号为分割点的功能;不忽略双逗号时的""字符串
        ///     实测效率不如replace+split
        /// </summary>
        [Obsolete("use replace+split instead")]
        public static IEnumerable<string> AsStringEnumerable(this string? str)
        {
            if (str == null) yield break;

            var start = -1;
            var len = str.Length;
            for (var i = 0; i < len; i++)
            {
                var c = str[i];
                if (c == ',' || c == '，')
                {
                    if (start + 1 == i)
                        yield return "";
                    else
                        yield return str.Substring((start + 1), i - (start + 1));
                    // yield return str[(start + 1).. i];

                    start = i;
                }
            }

            if (start + 1 == len - 1)
                yield return "";
            else
                yield return str.Substring(start + 1);
        }

        /// <summary>
        ///     等价
        ///     str.ReplaceAllChineseDouHaoToEnglish()
        ///     .ReplaceAllSpace0()
        ///     .Split(',')
        /// </summary>
        public static string[] SplitByDouHao(this string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            return str.ReplaceAllChineseDouHaoToEnglish()
                .RemoveSpaces()
                .Split(',');
        }

        /// <summary>
        ///     对形如(1,2),(4,6)的字串解析。自动替换中英文、忽略杂词
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IEnumerable<(int x, int y)> ToXYs(this string str)
        {
            var standard = str
                .RemoveSpaces()
                .ReplaceAllChineseDouHaoToEnglish()
                .Replace("（", "(")
                .Replace("）", ")");
            var regex = new Regex(@"(?<=\()-?[0-9]+,-?[-0-9]+(?=\))", RegexOptions.Singleline);
            var coll = regex.Matches(standard);
            foreach (Match match in coll)
            {
                var sp = match.Value.Split(',');
                var x = int.Parse(sp[0]);
                var y = int.Parse(sp[1]);
                yield return (x, y);
            }
        }

        /// <summary>
        ///     替换中文逗号为英文逗号
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReplaceAllChineseDouHaoToEnglish(this string str)
        {
            return str.Replace("，", ",");
        }

        /// <summary>
        ///     遍历替换oldstr，结果可能包含""，null(str是null的情况下)，如果参数集合中包含null则会从replace中引发异常
        ///     <para>old值和new值应一一对应</para>
        ///     <para>TODO 优化GC</para>
        /// </summary>
        public static string Replace(this string str, IEnumerable<string> oldstr, IEnumerable<string> newstr)
        {
            if (str.IsNullOrEmpty()) return str;
            var itor = oldstr.GetEnumerator();
            var newitor = newstr.GetEnumerator();
            while (itor.MoveNext())
            {
                var newerstr = newitor.MoveNext() ? newitor.Current : "";
                str = str.Replace(itor.Current, newerstr);
            }

            itor.Dispose();
            newitor.Dispose();
            return str;
        }

        /// <summary>
        ///     遍历替换oldchar，结果可能包含""，null(str是null的情况下)，如果参数集合中包含null则会从replace中引发异常
        ///     <para>old值和new值应一一对应</para>
        ///     <para>TODO 优化GC</para>
        /// </summary>
        public static string Replace(this string str, IEnumerable<char> oldchar, IEnumerable<char> newchar)
        {
            if (str.IsNullOrEmpty()) return str;
            var itor = oldchar.GetEnumerator();
            var newitor = newchar.GetEnumerator();
            while (itor.MoveNext())
                if (newitor.MoveNext())
                    str = str.Replace(itor.Current, newitor.Current);
                else str = str.Remove(itor.Current);
            itor.Dispose();
            newitor.Dispose();
            return str;
        }

        /// <summary>
        ///     遍历替换oldchar，结果可能包含""，null(str是null的情况下)，如果参数集合中包含null则会从replace中引发异常
        ///     <para>TODO 优化GC</para>
        /// </summary>
        public static string Replace(this string str, IEnumerable<char> oldchar, string newstr)
        {
            if (str.IsNullOrEmpty()) return str;
            var sb = new StringBuilder();
            foreach (var chr in str)
                // ReSharper disable once PossibleMultipleEnumeration
                if (oldchar.Contains(chr)) sb.Append(newstr);
                else sb.Append(chr);
            return sb.ToString();
        }

        /// <summary>
        ///     遍历替换oldStrs，结果可能包含""，如果参数集合中包含null则会引发异常
        /// </summary>
        public static string Replace(this string str, IEnumerable<string> oldStrs, string newStr)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            foreach (var oStr in oldStrs) str = str.Replace(oStr, newStr);

            return str;
        }

        /// <summary>
        ///     遍历替换oldchar，结果可能包含""，null(str是null的情况下)，如果参数集合中包含null则会从replace中引发异常
        ///     <para>TODO 优化GC</para>
        /// </summary>
        public static string Replace(this string str, IEnumerable<char> oldchar, char newstr)
        {
            if (str.IsNullOrEmpty()) return str;
            var itor = oldchar.GetEnumerator();
            while (itor.MoveNext()) str = str.Replace(itor.Current, newstr);
            itor.Dispose();
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Regex ToRegex(this string regex)
        {
            return new Regex(regex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Regex ToRegex(this string regex, RegexOptions op)
        {
            return new Regex(regex, op);
        }

        /// <summary>
        ///     见<see cref="Path.Combine(string,string)" />，另外会对\号转成/号
        ///     u3d在可以使用字符串拼接的情况下，优先使用字符串拼接
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string PathCombine(this string patha, string pathb)
        {
            return Path.Combine(patha, pathb).Replace("\\", "/");
        }

        /// <summary>
        ///     <see cref="Path.GetFileName(string)" />的包装形式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetPathFileName(this string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        ///     <see cref="Path.GetDirectoryName(string)" />的包装形式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetPathDir(this string path)
        {
            return Path.GetDirectoryName(path) ?? throw new Exception("no dir");
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RemoveAmountFromStart(this string content, int count)
        {
            return content.Remove(0, count);
        }

        /// <param name="content">原型</param>
        /// <param name="count">特指该字符串参数的长度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RemoveAmountFromStart(this string content, string count)
        {
            return content.Remove(0, count.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RemoveAmountFromEnd(this string content, int count)
        {
            return content.Remove(content.Length - count, count);
        }

        /// <param name="content">原型</param>
        /// <param name="count">特指该字符串参数的长度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RemoveAmountFromEnd(this string content, string count)
        {
            return RemoveAmountFromEnd(content, count.Length);
        }

        /// <summary>
        ///     string转换成utf8并获取base64
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBase64(this string name)
        {
            var bytes = Encoding.UTF8.GetBytes(name);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        ///     将字符串转换成^[_a-zA-Z0-9]$且以_i开头的SusiBase64字符串 (_i开头是便于识别和规避数字开头的命名不能作为变量命名的规则）
        /// </summary>
        public static string ToSusiBase64(this string str)
        {
            var base64 = str.ToBase64();
            return $"_i{base64.Replace("+", "_p").Replace("/", "_s").Replace("=", "_e")}";
        }

        /// <summary>
        ///     将SusiBase64字符串转还原
        /// </summary>
        public static string FromSusiBase64(this string str)
        {
            return str.RemoveAmountFromStart("_i")
                .Replace("_p", "+")
                .Replace("_s", "/")
                .Replace("_e", "=")
                .FromBase64();
        }


        public static string FromBase64(this string str)
        {
            var bytes = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(bytes);
        }

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
        /// 如果首字母不是字母则抛异常
        /// </summary>
        public static string MakeFirstCharUpper(this string str)
        {
            if (str.Length == 0) return "";
            if (str.Length == 1) return str.ToUpper();
            var c = str[0];
            c = c.ToUpper();
            return c + str.Substring(1);
        }

        public static string MakeFirstCharUpper2(this string str)
        {
            return str.Length > 1 && str[0].Isatoz() ? MakeFirstCharUpper(str) : str;
        }

        /// <summary>
        /// 如果首字母不是字母则抛异常
        /// </summary>
        public static string MakeFirstCharLower(this string str)
        {
            if (str.Length == 0) return "";
            if (str.Length == 1) return str.ToLower();
            var c = str[0];
            c = c.ToLower();
            return c + str.Substring(1);
        }

        public static string MakeFirstCharLower2(this string str)
        {
            return str.Length > 1 && str[0].IsAtoZ() ? MakeFirstCharLower(str) : str;
        }

        /// <summary>
        /// 等价于int.Parse(string str)
        /// </summary>
        public static int ToIntNumber(this string str)
        {
            return int.Parse(str);
        }

        /// <summary>
        /// 拥有更多异常信息
        /// </summary>
        /// <exception cref="ArgumentException">空arg或不是number</exception>
        public static int ToIntNumber2(this string str)
        {
            if (str.IsNullOrWhiteSpace()) throw new ArgumentException($"arg is empty");
            if (int.TryParse(str, out int res))
            {
                return res;
            }

            throw new ArgumentException($"{str} can't be number");
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
    }
}