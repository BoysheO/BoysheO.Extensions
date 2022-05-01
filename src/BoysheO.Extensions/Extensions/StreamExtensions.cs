using System;
using System.IO;

namespace BoysheO.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// stream流的容量在一个byte[]数据上限内时，一次性导出所有byte
        /// 总是seek到0再开始导出
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] GetAllBytesFromOffset0(this Stream stream)
        {
            if (!stream.CanSeek) throw new Exception("stream must can seek to 0");
            stream.Seek(0, SeekOrigin.Begin);
            using var mem = new MemoryStream();
            stream.CopyTo(mem);
            return mem.ToArray();
        }
    }
}