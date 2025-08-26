using System;
using System.Diagnostics;
using System.IO;
using BoysheO.Extensions;

namespace BoysheO.Toolkit
{
    /// <summary>
    /// 轻量级跨平台 Path 结构体：
    /// - 重载 '/' 作为路径拼接（只与 Path 运算；string 需经隐式转换）
    /// - Parent 返回上级目录（无上级则为空路径 ""）
    /// - 内部字符串始终使用 '/' 作为分隔符（自动归一化，折叠多余的斜杠）
    /// - DropFront() 去掉一层前缀目录（a/b/c -> b/c；b/c -> c；c -> ""；"" 继续 DropFront() 仍为 ""）
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public readonly struct PathX : IEquatable<PathX>
    {
        public readonly string Value;

        /// <summary>空路径常量。</summary>
        public static readonly PathX Empty = new PathX("");

        public PathX(string path)
        {
            path.ThrowIfNull();
            Value = Normalize(path);
        }

        /// <summary>是否为空路径。</summary>
        public bool IsEmpty => string.IsNullOrEmpty(Value);

        /// <summary>父级目录；若无则返回空路径。</summary>
        public PathX Parent
        {
            get
            {
                if (IsEmpty) return Empty;

                var s = Value.TrimEnd('/');
                var idx = s.LastIndexOf('/');
                if (idx < 0) return Empty;

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                return new PathX(s[..idx]);
#else
                return new PathX(s.Substring(0, idx));
#endif
            }
        }

        /// <summary>
        /// 去掉一层前缀目录：
        /// "a/b/c" -> "b/c"；"b/c" -> "c"；"c" -> ""；"" -> ""。
        /// </summary>
        public PathX DropFront()
        {
            if (IsEmpty) return Empty;

            var trimmed = Value.Trim('/');
            var idx = trimmed.IndexOf('/');
            if (idx < 0) return Empty;

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
            return new PathX(trimmed[(idx + 1)..]);
#else
            return new PathX(trimmed.Substring(idx + 1));
#endif
        }

        /// <summary>
        /// Same as <see cref="Path.GetFileName"/>
        /// ex. "a/b/c.txt" -> "c.txt"
        /// </summary>
        public string GetFileName()
        {
            return Path.GetFileName(Value);
        }

        /// <summary>
        /// Same as <see cref="Path.GetFileNameWithoutExtension"/>
        /// ex. "a.txt" -> "a"
        /// </summary>
        public string GetFileNameWithoutExt()
        {
            return Path.GetFileNameWithoutExtension(Value);
        }


        /// <summary>
        /// Same as <see cref="Path.GetExtension"/>
        /// ex. "a.txt" -> ".txt"
        /// </summary>
        /// <returns></returns>
        public string GetExtension()
        {
            return Path.GetExtension(Value);
        }
        
        /// <summary>
        /// Same as <see cref="Path.ChangeExtension"/>
        /// ex. a.txt -> a.bytes
        /// </summary>
        public static PathX? ChangeExtension(string? path, string? extension)
        {
            var res = Path.ChangeExtension(path, extension);
            return res == null ? (PathX?) null : new PathX(res);
        }
        
        /// <summary>
        /// Same as <see cref="Path.GetDirectoryName"/>
        /// ex. "a/b/c.txt" -> "a/b"
        /// </summary>
        public PathX? GetDirectoryName()
        {
            var value = Path.GetDirectoryName(Value);
            return value == null ? (PathX?)null : (PathX?)Path.GetDirectoryName(Value);
        }


        /// <summary>与 string 的隐式互转（输出/内部始终使用 '/'）。</summary>
        public static implicit operator PathX(string s) => new PathX(s);

        /// <summary>'/' 运算符用于拼接路径（仅 Path 与 Path 运算；string 需隐式转换）。</summary>
        public static PathX operator /(PathX left, PathX right)
        {
            if (left.IsEmpty) return right;
            if (right.IsEmpty) return left;

            var l = left.Value.TrimEnd('/');
            var r = right.Value.TrimStart('/');
            return new PathX($"{l}/{r}");
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj) => obj is PathX p && Equals(p);
        public bool Equals(PathX other) => string.Equals(Value, other.Value, StringComparison.Ordinal);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

        // ===== 私有工具 =====

        private static string Normalize(string s)
        {
            // 统一分隔符
            s = s.Replace('\\', '/');

            // 折叠多余斜杠（保持前导/和中间/为单个）
            while (s.Contains("//")) s = s.Replace("//", "/");

            // 去掉多余的末尾斜杠（但 "/" 保留为 "/"）
            if (s.Length > 1 && s.EndsWith("/")) s = s.TrimEnd('/');

            return s;
        }
    }
}