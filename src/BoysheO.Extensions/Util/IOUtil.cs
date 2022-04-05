using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BoysheO.Extensions;

namespace BoysheO.Util
{
    public static class IOUtil
    {
        /// <summary>
        ///     用于读取文件的文件流
        ///     以只读方式打开，不锁定
        /// </summary>
        public static FileStream? ReadStream(string file)
        {
            return !File.Exists(file)
                ? null
                : new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        /// <summary>
        ///     用于创建文件的文件流,会自动创建不存在的文件夹
        /// </summary>
        public static FileStream? CreatStream(string file)
        {
            var dir = Path.GetDirectoryName(file)?.Replace('\\', '/');
            if (dir == null) return null;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
        }

        /// <summary>
        ///     批量选取文件打开
        ///     TODO 优化GC
        /// </summary>
        /// <returns>FileStream的Creator</returns>
        public static IEnumerable<Func<FileStream?>> ReadFolderStream(
            string folder,
            string? searchPattern = null,
            string? regularPatternPath = null,
            string? regularPatternFilename = null,
            bool toLower = false
        )
        {
            var regexPath = regularPatternPath == null ? null : new Regex(regularPatternPath, RegexOptions.Compiled);
            var regexFilename = regularPatternFilename == null
                ? null
                : new Regex(regularPatternFilename, RegexOptions.Compiled);


            var files = Directory.EnumerateFiles(folder, searchPattern ?? "*", SearchOption.AllDirectories)
                .Where(v =>
                {
                    v = toLower ? v.Replace('\\', '/').ToLowerInvariant() : v.Replace('\\', '/');
                    var isPassPathRegx = regexPath == null || v.IsMatch(regexPath);
                    var filename = v.GetPathFileName();
                    var isPassFilename = regexFilename == null || filename.IsMatch(regexFilename);
                    return isPassPathRegx && isPassFilename;
                });
            return files.Select(v => new Func<FileStream?>(() => ReadStream(v)));
        }

        /// <summary>
        ///     清空目录，有任何异常返回false，但仍会清空所有其他正常的文件和目录，不抛异常
        /// </summary>
        public static bool ClearDir(string folder,
            Action<FileInfo>? onDelFail = null,
            Action<DirectoryInfo>? onDelDirFail = null)
        {
            var dir = new DirectoryInfo(folder);
            var isEr = false;
            foreach (var file in dir.EnumerateFiles("*", SearchOption.AllDirectories))
                try
                {
                    file.Delete();
                }
                catch
                {
                    onDelFail?.Invoke(file);
                    isEr = true;
                }

            foreach (var indir in dir.EnumerateDirectories())
                try
                {
                    indir.Delete();
                }
                catch
                {
                    onDelDirFail?.Invoke(indir);
                    isEr = true;
                }

            return isEr;
        }

        /// <summary>
        ///     从0写入bytes
        /// </summary>
        public static FileStream Write(this FileStream fs, byte[] bytes)
        {
            if (fs == null) throw new ArgumentNullException(nameof(fs));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            fs.Write(bytes, 0, bytes.Length);
            return fs;
        }

        public static FileStream Write(this FileStream fs, ArraySegment<byte> bytes)
        {
            if (fs == null) throw new ArgumentNullException(nameof(fs));
            if (bytes.Array == null) throw new ArgumentOutOfRangeException(nameof(bytes), "bytes have no array");
            fs.Write(bytes.Array, bytes.Offset, bytes.Count);
            return fs;
        }

        /// <summary>
        ///     无提示,只有cwlog
        /// </summary>
        public static bool SaveFileCollection(
            IDictionary<string, byte[]> fullFileNameFileBytes,
            string defaultFolder,
            bool warpFolder = false,
            bool creatDefault = true)
        {
            if (creatDefault && !Directory.Exists(defaultFolder)) Directory.CreateDirectory(defaultFolder);

            var selectFolder = defaultFolder;

            if (warpFolder) ClearDir(selectFolder);

            var isEr = false;
            // ReSharper disable once InconsistentNaming
            foreach (var fullname_bytes in fullFileNameFileBytes)
                try
                {
                    using var fs = CreatStream(fullname_bytes.Key) ?? throw new Exception($"invalid path {fullname_bytes.Key}");
                    fs.Write(fullname_bytes.Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"[error]{fullname_bytes.Key}写入失败。\n异常消息{ex}\n文件前10byte内容如下：\n{fullname_bytes.Value.Take(10).ToHexText()}");
                    isEr = true;
                }

            return isEr;
        }
    }
}