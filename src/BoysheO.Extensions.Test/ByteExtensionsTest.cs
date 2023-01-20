using System;
using System.Linq;
using BoysheO.Util;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class ByteExtensionsTest
{
    // [TestCase]
    // public void ToBinText()
    // {
    //     var rand = Random.Shared.Next();
    //     var bytes = rand.AsMemoryByteSpan().ToArray();
    //     var binText = bytes.ToBinText();
    //     var expected = bytes.Select(v => Convert.ToString(v, 2).PadLeft(8, '0'));
    //     Console.WriteLine(new {binText,expected});
    //     Assert.AreEqual(expected,binText);
    // }

    [TestCase]
    public void ByteToBinText()
    {
        var rand = (byte)Random.Shared.Next();
        var binText = rand.ToBinText();
        var expected = Convert.ToString(rand, 2).PadLeft(8, '0');
        Console.WriteLine(new { binText, expected });
        Assert.AreEqual(expected, binText);
    }

    [Test]
    public void ToHexText()
    {
        byte[] bytes = new byte[255];
        new Random().NextBytes(bytes);
        var strItor = bytes.Select(v => v.ToString("X2"));
        var expect = string.Join(' ', strItor);
        var actual = bytes.ToHexText();
        Assert.AreEqual(expect,actual);
    }
}