using System;
using System.IO;
using BoysheO.Toolkit;

namespace BoysheO.Extensions
{
    public static class PathValueExtensions
    {
        /// <summary>
        /// If directory not exist,creat it.
        /// </summary>
        /// <param name="directory">Directory path.</param>
        public static void CreatDirectoryIfNotExist(this PathValue directory)
        {
            if (!Directory.Exists(directory.Value)) Directory.CreateDirectory(directory.Value);
        }

        /// <summary>
        /// Creat directory if not exist and write file.
        /// </summary>
        /// <param name="filePath">file</param>
        public static void WriteAllBytes(this PathValue filePath, byte[] bytes)
        {
            filePath.GetDirectoryName().Value.CreatDirectoryIfNotExist();
            File.WriteAllBytes(filePath.Value, bytes);
        }

        public static DirectoryInfo ToDirectoryInfo(this PathValue directoryPath)
        {
            return new DirectoryInfo(directoryPath.Value);
        }

        public static FileInfo ToFileInfo(this PathValue filePath)
        {
            return new FileInfo(filePath.Value);
        }
    }
}