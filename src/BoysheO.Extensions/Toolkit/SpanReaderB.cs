namespace BoysheO.Toolkit
{
    using System;
    using System.Buffers.Binary;
    using System.Text;


    /// <summary>
    /// 按大端序读取数据
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
                throw new InvalidOperationException("Buffer overflow.");
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
            var idx = _buffer.Slice(Seek).IndexOf((byte)0);

            if (idx == -1)
                throw new InvalidOperationException("Null terminator not found for ASCII string.");
            if (idx == 0) return "";

            unsafe
            {
                fixed (byte* b = _buffer.Slice(Seek))
                {
                    var result = Encoding.ASCII.GetString(b, idx);
                    Seek += idx; // 跳过\0
                    return result;
                }
            }
        }

        public string ReadStringUTF8()
        {
            var len = ReadUInt16();
            if (Seek + len > _buffer.Length) throw new Exception("invalid size");//字符串长度过大
            unsafe
            {
                fixed (byte* b = _buffer.Slice(Seek))
                { 
                    var str =Encoding.UTF8.GetString(b, len);
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

        public void Reset()
        {
            Seek = 0;
        }
    }
}