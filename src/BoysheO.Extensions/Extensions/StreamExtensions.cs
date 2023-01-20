using System;
using System.IO;

namespace BoysheO.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Seek to 0 and read all bytes.
        /// </summary>
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