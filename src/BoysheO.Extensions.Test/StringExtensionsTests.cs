using System;
using System.Buffers;
using System.Linq;
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
    public string ToRawBytesUsingSeg(string raw)
    {
        var buff = new ArraySegment<byte>(new byte[raw.Length]);
        raw.ToRawBytes(buff);
        return buff.AsSpan().ToHexText();
    }


    [TestCase("123", ExpectedResult = "31 32 33")]
    public string ToRawBytesUsingBuff(string raw)
    {
        var buff = new byte[raw.Length];
        raw.ToRawBytes(buff, 0, raw.Length);
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
    public void EquivalenceRawBytesAPIs(string source)
    {
        var raw1 = source.ToRawBytes();
        var raw2 = new ArraySegment<byte>(new byte[source.Length]);
        source.ToRawBytes(raw2);
        var raw3 = new byte[source.Length];
        source.ToRawBytes(raw3, 0, source.Length);
        Assert.AreEqual(raw1.ToHexText(), raw2.ToHexText());
        Assert.AreEqual(raw2.ToHexText(), raw3.ToHexText());
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

    // // [TestCase(@"12332\n14\\n562")]//暂不考虑转义
    // [TestCase(@"123\n321\n4562")]
    // [TestCase(@"123n321\4562")]
    // public void ReplaceNToLine(string src)
    // {
    //     var src1 = src.Replace("\\n", "\n");
    //     Console.WriteLine(src1);
    //     var src2 = src.ReplaceNToLine();
    //     Console.WriteLine(src2);
    //     Assert.AreEqual(src1, src2);
    // }

    [TestCase("1233214565", "1")]
    [TestCase("1233214562", "2")]
    [TestCase("1233214562", "4")]
    public void SplitAsPooledChars(string str,string sp)
    {
        var sys = str.Split(sp);
        var charsCount = str.AsSpan().SplitAsPooledChars(sp,out (int start, int count)[] chars);
        var mySplit = chars.Take(charsCount).Select(v => str.Substring(v.start, v.count)).ToArray();
        ArrayPool<(int start, int count)>.Shared.Return(chars);
        Assert.IsTrue(sys.SequenceEqual(mySplit));
    }
}