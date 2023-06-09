using System;

namespace BoysheO.Util
{
    public static class StringUtil
    {
        /// <summary>
        ///     Convert hex text ot bytes.<br />
        ///     ex."0AFEAA23" => {0A,FE,AA,23}
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">source must has 2*n chars which in [0-9A-Z].</exception>
        public static byte[] HexTextToBytes(this string source)
        {
            var bufferLen = source.Length / 2;
            var isAsByteArray = bufferLen >= 1024;
            var bytes = isAsByteArray ? stackalloc byte[bufferLen] : new byte[bufferLen];
            for (var i = 0; i < bytes.Length; i++) bytes[i] = ByteUtil.HexCharToByte(source[2 * i], source[2 * i + 1]);

            return bytes.ToArray();
        }
    }
}