using System.IO;
using System.Text;

namespace BoysheO.Util
{
    public static class IOUtil
    {
        /// <summary>
        /// Saves text content to the specified file path
        /// </summary>
        /// <param name="text">The text content to save</param>
        /// <param name="filePath">The target file path</param>
        /// <param name="createDirectoryIfNotExists">Whether to create directory if it doesn't exist</param>
        /// <param name="throwIfFileExists">Whether to throw exception if file already exists</param>
        /// <exception cref="DirectoryNotFoundException">Thrown when directory doesn't exist and createDirectoryIfNotExists is false</exception>
        /// <exception cref="IOException">Thrown when file exists and throwIfFileExists is true</exception>
        public static void SaveTextToFile(string text, string filePath,
            bool createDirectoryIfNotExists = true,
            bool throwIfFileExists = false)
        {
            ValidateAndPreparePath(filePath, createDirectoryIfNotExists, throwIfFileExists);

            File.WriteAllText(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// Saves byte array to the specified file path
        /// </summary>
        /// <param name="bytes">The byte array to save</param>
        /// <param name="filePath">The target file path</param>
        /// <param name="createDirectoryIfNotExists">Whether to create directory if it doesn't exist</param>
        /// <param name="throwIfFileExists">Whether to throw exception if file already exists</param>
        /// <exception cref="DirectoryNotFoundException">Thrown when directory doesn't exist and createDirectoryIfNotExists is false</exception>
        /// <exception cref="IOException">Thrown when file exists and throwIfFileExists is true</exception>
        public static void SaveBytesToFile(byte[] bytes, string filePath,
            bool createDirectoryIfNotExists = true,
            bool throwIfFileExists = false)
        {
            ValidateAndPreparePath(filePath, createDirectoryIfNotExists, throwIfFileExists);

            File.WriteAllBytes(filePath, bytes);
        }

        public static void ValidateAndPreparePath(string filePath, bool createDirectoryIfNotExists,
            bool throwIfFileExists)
        {
            string? directory = Path.GetDirectoryName(filePath);

            // 处理目录不存在的情况
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                if (createDirectoryIfNotExists)
                {
                    Directory.CreateDirectory(directory);
                }
                else
                {
                    throw new DirectoryNotFoundException($"Directory not found:{directory}");
                }
            }

            // 处理文件已存在的情况
            if (File.Exists(filePath) && throwIfFileExists)
            {
                throw new IOException($"File already exists: {filePath}");
            }
        }
    }
}