using System;
using System.Buffers;

namespace BoysheO.Util
{
    public static class BaseNUtil
    {
        public const string Base62CharacterSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public const string Base64CharacterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        public static int ToBase62AsPooledChar(ReadOnlySpan<byte> str,out char[] chars)
        {
            var len = str.Length * 256 / 62 + 1;
            var buffer = ArrayPool<char>.Shared.Rent(len);
            var bufferCount = 0;

            int carry = 0;
            for (int i = 0, count = str.Length; i < count; i++)
            {
                var b = str[i];
                var value = b + carry;
                carry = value / 62;
                var mod = value - carry * 62;
                buffer[bufferCount] = Base62CharacterSet[mod];
                bufferCount++;
            }
            //处理最后一个进位
            if (carry > 0)
            {
                buffer[bufferCount] = Base62CharacterSet[carry];
                bufferCount++;
            }

            chars = buffer;
            return bufferCount;
        }

        public static int ToBase64AsPooledChar(ReadOnlySpan<byte> str,out char[] chars)
        {
            var len = str.Length * 256 / 64 + 1;
            var buffer = ArrayPool<char>.Shared.Rent(len);
            var bufferCount = 0;

            int carry = 0;
            for (int i = 0, count = str.Length; i < count; i++)
            {
                var b = str[i];
                var value = b + carry;
                carry = value / 64;
                var mod = value - carry * 64;
                buffer[bufferCount] = Base64CharacterSet[mod];
                bufferCount++;
            }
            //处理最后一个进位
            if (carry > 0)
            {
                buffer[bufferCount] = Base64CharacterSet[carry];
                bufferCount++;
            }

            chars = buffer;
            return bufferCount;
        }
        
        public static ReadOnlySpan<char> ToBase58(ReadOnlySpan<byte> bytes)
        {
            throw new NotImplementedException();
        }
    }
}