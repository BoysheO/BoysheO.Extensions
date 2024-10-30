using System.Linq;
using BoysheO.Toolkit;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

[TestFixture]
public class SpanWriterBTests
{
    private byte[] _buffer;

    [SetUp]
    public void Setup()
    {
        _buffer = new byte[50];
    }

    [Test]
    public void WriteInt32_Test()
    {
        var _writer = new SpanWriterB(_buffer);
        _writer.WriteInt32(1);
        Assert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x01 }, _buffer[..4].ToArray());
    }

    [Test]
    public void WriteUInt32_Test()
    {
        var _writer = new SpanWriterB(_buffer);
        _writer.WriteUInt32(2);
        Assert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x02 }, _buffer[..4].ToArray());
    }

    [Test]
    public void WriteInt16_Test()
    {
        var _writer = new SpanWriterB(_buffer);
        _writer.WriteInt16(3);
        Assert.AreEqual(new byte[] { 0x00, 0x03 }, _buffer[..2].ToArray());
    }

    [Test]
    public void WriteUInt16_Test()
    {
        var _writer = new SpanWriterB(_buffer);
        _writer.WriteUInt16(4);
        Assert.AreEqual(new byte[] { 0x00, 0x04 }, _buffer[..2].ToArray());
    }

    [Test]
    public void WriteInt64_Test()
    {
        var _writer = new SpanWriterB(_buffer);
        _writer.WriteInt64(5);
        Assert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05 }, _buffer[..8].ToArray());
    }

    [Test]
    public void WriteUInt64_Test()
    {
        var _writer = new SpanWriterB(_buffer);
        _writer.WriteUInt64(6);
        Assert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06 }, _buffer[..8].ToArray());
    }

    [Test]
    public void WriteStringASCII_Test()
    {
        var _writer = new SpanWriterB(_buffer);
        _writer.WriteStringASCII("Hello");
        Assert.AreEqual(new byte[] { (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o', 0x00 },
            _buffer[..6].ToArray());
    }

    [Test]
    public void WriteStringUTF8_Test()
    {
        var _writer = new SpanWriterB(_buffer);
        _writer.WriteStringUTF8("World!");
        Assert.AreEqual(new byte[] { 0x00, 0x06, (byte)'W', (byte)'o', (byte)'r', (byte)'l', (byte)'d', (byte)'!' },
            _buffer[..8].ToArray());
    }

    [Test]
    public void WriteBytes_Test()
    {
        byte[] data = { 0x01, 0x02, 0x03 };
        var _writer = new SpanWriterB(_buffer);
        _writer.WriteBytes(data);
        Assert.AreEqual(data, _buffer[..3].ToArray());
    }

    [Test]
    public void Reset_Test()
    {
        var _writer = new SpanWriterB(_buffer);
        _writer.WriteInt32(1);
        _writer.Reset();
        _writer.WriteInt32(2);
        Assert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x02 }, _buffer[..4].ToArray());
    }
}