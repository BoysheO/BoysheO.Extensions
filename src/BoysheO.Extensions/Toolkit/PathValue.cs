using System;
using System.IO;

namespace BoysheO.Toolkit
{
    /// <summary>
    /// It means a file path or a directory path
    /// </summary>
    public readonly struct PathValue
    {
        public readonly string Value;

        public PathValue(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Same as <see cref="Path.Combine(string,string)"/>
        /// ex. "a/b" + "c.txt" -> "a/b/c.txt"
        /// </summary>
        public PathValue Combine(PathValue value)
        {
            return new PathValue(Path.Combine(Value, value.Value));
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
        /// Same as <see cref="Path.GetDirectoryName"/>
        /// ex. "a/b/c.txt" -> "a/b"
        /// </summary>
        public PathValue? GetDirectoryName()
        {
            var value = Path.GetDirectoryName(Value);
            return value == null ? (PathValue?) null : Path.GetDirectoryName(Value);
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
        public static PathValue? ChangeExtension(string? path, string? extension)
        {
            var res = Path.ChangeExtension(path, extension);
            return res == null ? (PathValue?) null : new PathValue(res);
        }

        public static implicit operator PathValue(string value)
        {
            return new PathValue(value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}