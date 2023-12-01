using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Base62;
using BoysheO.Util;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class StringExtensionsTests
{
    /// <summary>
    /// 确认结果等价
    /// </summary>
    [Test]
    public void ToRawBytes2()
    {
        var raw = RandomUtil.Int.ToString();
        var bytes = new byte[raw.Length];
        for (int i = 0; i < raw.Length; i++)
        {
            bytes[i] = (byte)raw[i];
        }

        var result = raw.ToRawBytes();

        Assert.AreEqual(bytes, result);
    }

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
        return buff.AsSpan().AsReadOnly().ToHexText();
    }


    [TestCase("123", ExpectedResult = "31 32 33")]
    public string ToRawBytesUsingBuff(string raw)
    {
        var buff = new byte[raw.Length];
        raw.ToRawBytes(buff, 0, raw.Length);
        return buff.AsSpan().AsReadOnly().ToHexText();
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
        return span.ParseToPositiveInt();
    }

    [Test]
    public void ToHexString()
    {
        var rand = Random.Shared.Next();
        var bytes = rand.AsMemoryByteSpan().ToArray().ToList();
        var expert = bytes.AsEnumerable().ToHexText();
        var res = bytes.ToHexText();
        Assert.AreEqual(expert, res);
    }

    [Test]
    public void MemoryToString()
    {
        var bytes = new byte[32];
        new Random().NextBytes(bytes);
        var str = Convert.ToBase64String(bytes);
        var srcBytes = Encoding.Unicode.GetBytes(str);
        Console.WriteLine(srcBytes.ToHexText());
        var raw = srcBytes.AsSpan().AsReadOnly().MemoryToString();
        Assert.AreEqual(str, raw);
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

    // [TestCase("1233214565", "1")]
    // [TestCase("1233214562", "2")]
    // [TestCase("1233214562", "4")]
    // [TestCase("121", "1")]
    // [TestCase("11", "1")]
    // [TestCase("22", "1")]
    // [TestCase("212", "1")]
    // [TestCase("11", "11")]
    // [TestCase("112", "11")]
    // [TestCase("11211", "11")]
    // [TestCase("2", "11")]
    // [TestCase("2112", "11")]
    // [TestCase("1111", "11")]
    // [TestCase("121121", "11")]
    // [TestCase("3", "1")]
    // [TestCase("3", "11")]
    // public void SplitAsPooledChars(string str, string sp)
    // {
    //     var sys = str.Split(sp);
    //     var charsCount = str.AsSpan().SplitAsPooledChars(sp, out (int start, int count)[] chars);
    //     var mySplit = chars.Take(charsCount).Select(v => str.Substring(v.start, v.count)).ToArray();
    //     Console.WriteLine(sys.Join());
    //     Console.WriteLine(mySplit.Join());
    //     ArrayPool<(int start, int count)>.Shared.Return(chars);
    //     Assert.IsTrue(sys.SequenceEqual(mySplit));
    // }

    // [TestCase]
    // public void SplitAsPooledCharsRandomTest()
    // {
    //     var digitCount = RandomUtil.Int % 2 + 1;
    //     var rand = Enumerable.Range(0, digitCount).Select(v => (RandomUtil.Int % 10).ToString()).Join("");
    //     var sb = new StringBuilder();
    //     for (int i = 0; i < 10; i++) sb.Append(RandomUtil.Long);
    //     var randStr = sb.ToString();
    //     Console.WriteLine(randStr);
    //     Console.WriteLine(rand);
    //     SplitAsPooledChars(randStr, rand);
    // }

    [TestCase("abc",ExpectedResult = "abc")]
    [TestCase("Abc",ExpectedResult = "abc")]
    [TestCase("AbcA",ExpectedResult = "abcA")]
    public string MakeFirstCharLowerOrNot(string str)
    {
        return str.MakeFirstCharLowerOrNot();
    }

    [TestCase("abc",ExpectedResult = "Abc")]
    [TestCase("Abc",ExpectedResult = "Abc")]
    [TestCase("abbA",ExpectedResult = "AbbA")]
    public string MarkFirstCharUpperOrNot(string str)
    {
        return str.MakeFirstCharUpperOrNot();
    }
}