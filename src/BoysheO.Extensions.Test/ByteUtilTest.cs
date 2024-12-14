using System;
using BoysheO.Util;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class ByteUtilTest
{
    [TestCase]
    public void ByteToHexChar()
    {
        for (int v = byte.MinValue; v <= byte.MaxValue; v++)
        {
            var b = (byte)v;
            Span<char> buf = new char[2];
            ByteUtil.ByteToHexChar(b, buf);
            Assert.AreEqual(v.ToString("X2"),buf.AsReadOnly().ToNewString());
        }
    }
}