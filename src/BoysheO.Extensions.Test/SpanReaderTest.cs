using System;
using System.Linq;
using BoysheO.Toolkit;
using BoysheO.Util;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

using System;
using System.Text;
using NUnit.Framework;

[TestFixture]
public class SpanReaderBTests
{
    private byte[] buffer;

    [SetUp]
    public void Setup()
    {
        // 设置一个测试缓冲区，包含各种类型的数据
        buffer = new byte[]
        {
            0x00, 0x00, 0x00, 0x01, // Int32 - 1
            0x00, 0x00, 0x00, 0x02, // UInt32 - 2
            0x00, 0x03, // Int16 - 3
            0x00, 0x04, // UInt16 - 4
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, // Int64 - 5
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, // UInt64 - 6
            (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o', 0x00, // ASCII string "Hello"
            0x00, 0x06, (byte)'W', (byte)'o', (byte)'r', (byte)'l', (byte)'d', (byte)'!' // UTF8 string "World!"
        };
    }

    [Test]
    public void ReadInt32_Test()
    {
        var _reader = new SpanReaderB(buffer);
        Assert.AreEqual(1, _reader.ReadInt32());
    }

    [Test]
    public void ReadUInt32_Test()
    {
        var _reader = new SpanReaderB(buffer);
        _reader.ReadInt32(); // Move seek
        Assert.AreEqual(2, _reader.ReadUInt32());
    }

    [Test]
    public void ReadInt16_Test()
    {
        var _reader = new SpanReaderB(buffer);
        _reader.ReadInt32();
        _reader.ReadUInt32();
        Assert.AreEqual(3, _reader.ReadInt16());
    }

    [Test]
    public void ReadUInt16_Test()
    {
        var _reader = new SpanReaderB(buffer);
        _reader.ReadInt32();
        _reader.ReadUInt32();
        _reader.ReadInt16();
        Assert.AreEqual(4, _reader.ReadUInt16());
    }

    [Test]
    public void ReadInt64_Test()
    {
        var _reader = new SpanReaderB(buffer);
        _reader.ReadInt32();
        _reader.ReadUInt32();
        _reader.ReadInt16();
        _reader.ReadUInt16();
        Assert.AreEqual(5, _reader.ReadInt64());
    }

    [Test]
    public void ReadUInt64_Test()
    {
        var _reader = new SpanReaderB(buffer);
        _reader.ReadInt32();
        _reader.ReadUInt32();
        _reader.ReadInt16();
        _reader.ReadUInt16();
        _reader.ReadInt64();
        Assert.AreEqual(6, _reader.ReadUInt64());
    }

    [Test]
    public void ReadStringASCII_Test()
    {
        var _reader = new SpanReaderB(buffer);
        _reader.Seek = 28; // ASCII string offset
        Assert.AreEqual("Hello", _reader.ReadStringASCII());
    }

    [Test]
    public void ReadStringUTF8_Test()
    {
        var _reader = new SpanReaderB(buffer);
        _reader.Seek = 34; // UTF8 string offset
        Assert.AreEqual("World!", _reader.ReadStringUTF8());
    }

    [Test]
    public void ReadBytes_Test()
    {
        var _reader = new SpanReaderB(buffer);
        _reader.Seek = 28;
        var bytes = _reader.ReadBytes(5);
        Assert.AreEqual(new byte[] { (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o' }, bytes.ToArray());
    }

    [Test]
    public void Reset_Test()
    {
        var _reader = new SpanReaderB(buffer);
        _reader.ReadInt32();
        _reader.Reset();
        Assert.AreEqual(1, _reader.ReadInt32());
    }

    [Test]
    public void WriteAsciiAndRead()
    {
        //随机生成ascii字符串
        var bytes = new byte[1024];
        Random.Shared.NextBytes(bytes);
        var str = Convert.ToBase64String(bytes); //可以保证是ascii编码
        var count = Encoding.ASCII.GetByteCount(str);
        bytes = new byte[count + 1];
        var writer = new SpanWriterB(bytes);
        writer.WriteStringASCII(str);

        var reader = new SpanReaderB(bytes);
        var ascii = reader.ReadStringASCII();
        Assert.AreEqual(str, ascii);
    }


    /// <summary>
    /// 测试是否正确写入结尾字符'\0'
    /// </summary>
    /// <param name="str"></param>
    [TestCase("")]
    [TestCase("\r\n")]
    [TestCase("\n")]
    public void WriteAsciiAndRead(string str)
    {
        var count = Encoding.ASCII.GetByteCount(str);
        var bytes = new byte[count + 1];
        var writer = new SpanWriterB(bytes);
        writer.WriteStringASCII(str);
        var mem = str.ToRawBytes();
        Console.WriteLine(mem.ToHexText());
        Console.WriteLine(bytes.ToHexText());
        

        var reader = new SpanReaderB(bytes);
        var ascii = reader.ReadStringASCII();
        if (str == "\0") str = "";
        Assert.AreEqual(str, ascii);
    }

    /// <summary>
    /// 测试奇异字符是否能够报错
    /// </summary>
    [TestCase("\0")]
    public void WriteAsciiAndRead2(string str)
    {
        var count = Encoding.ASCII.GetByteCount(str);
        var bytes = new byte[count + 1];
        var writer = new SpanWriterB(bytes);
        try
        {
            writer.WriteStringASCII(str);
        }
        catch (ArgumentOutOfRangeException e)
        {
            Assert.Pass();
        }
        Assert.Fail();
    }
}