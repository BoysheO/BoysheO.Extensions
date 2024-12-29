using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using BoysheO.Extensions;

namespace BoysheO.Toolkit
{
    /// <summary>
    /// 按大端序写入数据
    /// </summary>
    public ref struct SpanWriterB
    {
        private readonly Span<byte> _buffer;
        public int Seek;

        public SpanWriterB(Span<byte> buffer)
        {
            _buffer = buffer;
            Seek = 0;
        }

        private void EnsureCapacity(int length)
        {
            if (Seek + length > _buffer.Length)
                throw new InvalidOperationException("Buffer overflow.");
        }

        public void WriteInt32(int value)
        {
            EnsureCapacity(sizeof(int));
            BinaryPrimitives.WriteInt32BigEndian(_buffer.Slice(Seek), value);
            Seek += sizeof(int);
        }

        public void WriteUInt32(uint value)
        {
            EnsureCapacity(sizeof(uint));
            BinaryPrimitives.WriteUInt32BigEndian(_buffer.Slice(Seek), value);
            Seek += sizeof(uint);
        }

        public void WriteInt16(short value)
        {
            EnsureCapacity(sizeof(short));
            BinaryPrimitives.WriteInt16BigEndian(_buffer.Slice(Seek), value);
            Seek += sizeof(short);
        }

        public void WriteUInt16(ushort value)
        {
            EnsureCapacity(sizeof(ushort));
            BinaryPrimitives.WriteUInt16BigEndian(_buffer.Slice(Seek), value);
            Seek += sizeof(ushort);
        }

        public void WriteInt64(long value)
        {
            EnsureCapacity(sizeof(long));
            BinaryPrimitives.WriteInt64BigEndian(_buffer.Slice(Seek), value);
            Seek += sizeof(long);
        }

        public void WriteUInt64(ulong value)
        {
            EnsureCapacity(sizeof(ulong));
            BinaryPrimitives.WriteUInt64BigEndian(_buffer.Slice(Seek), value);
            Seek += sizeof(ulong);
        }

        //ascii码写入约定以\0作为结尾
        public void WriteStringASCII(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length > 0 && value[value.Length - 1] == '\0')
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value can not ends with \\0");
            }

            int byteCount = Encoding.ASCII.GetByteCount(value);
            EnsureCapacity(byteCount + 1);
            int count;
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
            count = Encoding.ASCII.GetBytes(value, _buffer[Seek..]);
#else
            unsafe
            {
                fixed (char* c = value)
                {
                    fixed (byte* b = _buffer.Slice(Seek))
                    {
                        count = Encoding.ASCII.GetBytes(c, value.Length, b, _buffer.Length - Seek);
                    }
                }
            }
#endif
            Seek += count;
            //查看字符串是否以\0结尾
            if (value.Length > 0 && value[value.Length - 1] != '\0')
            {
                _buffer[Seek] = (byte)'\0';
                Seek++;
            }
        }

        //utf8使用长度头+内容
        [Obsolete("The API is designed not good enough")]
        public void WriteStringUTF8(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length > ushort.MaxValue)
                throw new Exception($"string is too big than UInt16.MaxValue={ushort.MaxValue}");
            int byteCount = Encoding.UTF8.GetByteCount(value);
            EnsureCapacity(sizeof(ushort) + byteCount);
            WriteUInt16((ushort)byteCount);
            unsafe
            {
                fixed (char* c = value)
                {
                    fixed (byte* b = _buffer.Slice(Seek))
                    {
                        int count = Encoding.UTF8.GetBytes(c, value.Length, b, _buffer.Length - Seek);
                        Seek += count;
                    }
                }
            }
        }

        /// <summary>
        /// 只写入，不写入长度
        /// </summary>
        public void WriteBytes(ReadOnlySpan<byte> bytes)
        {
            EnsureCapacity(bytes.Length);
            bytes.CopyTo(_buffer.Slice(Seek));
            Seek += bytes.Length;
        }

        public void Reset()
        {
            Seek = 0;
        }
    }
}