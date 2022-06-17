using System;
using System.Linq;
using BoysheO.Util;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class IntExtensionsTests
{
    [TestCase(0, ExpectedResult = "")]
    [TestCase(1, ExpectedResult = "1")]
    [TestCase(12, ExpectedResult = "12")]
    [TestCase(123, ExpectedResult = "123")]
    [TestCase(1234, ExpectedResult = "1234")]
    [TestCase(12345, ExpectedResult = "12345")]
    [TestCase(123456, ExpectedResult = "123456")]
    [TestCase(1234567, ExpectedResult = "1234567")]
    [TestCase(12345678, ExpectedResult = "12345678")]
    [TestCase(123456789, ExpectedResult = "123456789")]
    [TestCase(1234567890, ExpectedResult = "1234567890")]
    public string PositiveIntegerToEachDigit(int src)
    {
        Span<int> buff = stackalloc int[10];
        for (var index = 0; index < buff.Length; index++)
        {
            buff[index] = RandomUtil.Int;
        }

        src.PositiveIntegerToEachDigit(buff);
        var str = buff.ToArray().Select(v => v.ToString()).JoinAsOneString().Replace(",","").TrimStart('0');
        return str;
    }
}