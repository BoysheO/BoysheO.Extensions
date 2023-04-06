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
            Value = value;
        }

        /// <summary>
        /// Same as Path.Combine
        /// </summary>
        public PathValue Combine(PathValue value)
        {
            return new PathValue(Path.Combine(Value, value.Value));
        }

        /// <summary>
        /// Same as Path.GetFileName
        /// </summary>
        public string GetFileName()
        {
            return Path.GetFileName(Value);
        }

        /// <summary>
        /// Same as Path.GetDirectoryName
        /// </summary>
        public PathValue GetDirectoryName()
        {
            return Path.GetDirectoryName(Value);
        }

        /// <summary>
        /// Same as Path.GetFileNameWithoutExtension
        /// </summary>
        public string GetFileNameWithoutExt()
        {
            return Path.GetFileNameWithoutExtension(Value);
        }

        /// <summary>
        /// Same as Path.GetExtension
        /// </summary>
        /// <returns></returns>
        public string GetExtension()
        {
            return Path.GetExtension(Value);
        }

        public static implicit operator PathValue(string value)
        {
            return new PathValue(value);
        }
    }
}