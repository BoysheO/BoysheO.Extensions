namespace BoysheO.Toolkit
{
    using System;
    using System.Buffers.Binary;
    using System.Text;


    /// <summary>
    /// 按大端序读取数据
    /// *不会验证Seek是否越界，以获取更多性能。这应该是合理的
    /// </summary>
    public ref struct SpanReaderB
    {
        private readonly ReadOnlySpan<byte> _buffer;
        public int Seek;

        public SpanReaderB(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer;
            Seek = 0;
        }

        private void EnsureCapacity(int length)
        {
            if (Seek + length > _buffer.Length)
                throw new InvalidOperationException("No bytes enough");
        }

        public int ReadInt32()
        {
            EnsureCapacity(sizeof(int));
            int value = BinaryPrimitives.ReadInt32BigEndian(_buffer.Slice(Seek));
            Seek += sizeof(int);
            return value;
        }

        public uint ReadUInt32()
        {
            EnsureCapacity(sizeof(uint));
            uint value = BinaryPrimitives.ReadUInt32BigEndian(_buffer.Slice(Seek));
            Seek += sizeof(uint);
            return value;
        }

        public short ReadInt16()
        {
            EnsureCapacity(sizeof(short));
            short value = BinaryPrimitives.ReadInt16BigEndian(_buffer.Slice(Seek));
            Seek += sizeof(short);
            return value;
        }

        public ushort ReadUInt16()
        {
            EnsureCapacity(sizeof(ushort));
            ushort value = BinaryPrimitives.ReadUInt16BigEndian(_buffer.Slice(Seek));
            Seek += sizeof(ushort);
            return value;
        }

        public long ReadInt64()
        {
            EnsureCapacity(sizeof(long));
            long value = BinaryPrimitives.ReadInt64BigEndian(_buffer.Slice(Seek));
            Seek += sizeof(long);
            return value;
        }

        public ulong ReadUInt64()
        {
            EnsureCapacity(sizeof(ulong));
            ulong value = BinaryPrimitives.ReadUInt64BigEndian(_buffer.Slice(Seek));
            Seek += sizeof(ulong);
            return value;
        }

        /// <summary>
        /// 按'\0'识别字符串结尾，但是输出结果中不含'\0'
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string ReadStringASCII()
        {
            var bytes = ReadBytesUntilNull();
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
            return Encoding.ASCII.GetString(bytes);
#else
            unsafe
            {
                fixed (byte* b = bytes)
                {
                    var result = Encoding.ASCII.GetString(b, bytes.Length);
                    return result;
                }
            }
#endif
        }

        [Obsolete("The API is designed not good enough")]
        public string ReadStringUTF8()
        {
            var len = ReadUInt16();
            if (Seek + len > _buffer.Length) throw new Exception("invalid size"); //字符串长度过大
            unsafe
            {
                fixed (byte* b = _buffer.Slice(Seek))
                {
                    var str = Encoding.UTF8.GetString(b, len);
                    Seek += len;
                    return str;
                }
            }
        }

        public ReadOnlySpan<byte> ReadBytes(int length)
        {
            EnsureCapacity(length);
            var bytes = _buffer.Slice(Seek, length);
            Seek += length;
            return bytes;
        }

        /// <summary>
        /// 读取到下个/0标记，输出/0标记前的所有byte，并且Seek跳到/0符号后一个位置
        /// 没有找到/0标记就会抛异常
        /// 如果/0标记在第一个字符，则输出的Span会是长度0
        /// </summary>
        /// <returns></returns>
        public ReadOnlySpan<byte> ReadBytesUntilNull()
        {
            if (Seek >= _buffer.Length) throw new InvalidOperationException("No bytes can be read");
            var idx = _buffer.Slice(Seek).IndexOf<byte>(0);
            if (idx < 0) throw new InvalidOperationException("No /0 found");
            var start = Seek;
            Seek = start + idx + 1;
            return _buffer.Slice(start, idx);
        }

        public void Reset()
        {
            Seek = 0;
        }
    }
}