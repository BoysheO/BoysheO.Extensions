using System;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class StringExtensionsTests
{
    [TestCase("123", ExpectedResult = "31 32 33")]
    public string ToRawBytes(string raw)
    {
        return raw.ToRawBytes().ToHexText();
    }

    [TestCase("123", ExpectedResult = "31 32 33")]
    public string ToRawBytesUsingBuff(string raw)
    {
        var buff = new ArraySegment<byte>(new byte[raw.Length]);
        raw.ToRawBytes(buff);
        return buff.AsSpan().ToHexText();
    }

    /// <summary>
    /// ToRawBytes的相等性测试
    /// </summary>
    /// <param name="source"></param>
    [TestCase("123")]
    [TestCase("321")]
    [TestCase("1233214565")]
    [TestCase("1233214562")]
    [TestCase("1233214562")]
    public void EquivalenceBetweenToRawBytesAndToRawBytesUsingBuff(string source)
    {
        var raw1 = source.ToRawBytes();
        var raw2 = new ArraySegment<byte>(new byte[source.Length]);
        source.ToRawBytes(raw2);
        Assert.AreEqual(raw1.ToHexText(), raw2.ToHexText());
    }

    [Test]
    [TestCase("123", ExpectedResult = 123)]
    [TestCase("321", ExpectedResult = 321)]
    [TestCase("1233214565", ExpectedResult = 1233214565)]
    [TestCase("1233214562", ExpectedResult = 1233214562)]
    [TestCase("1233214562", ExpectedResult = 1233214562)]
    public int ToIntNumber2(string raw)
    {
        var span = raw.AsSpan();
        return span.ToPositiveInt();
    }
}