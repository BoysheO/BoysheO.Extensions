using System;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class CharSpanExtensionsTests
{
    // [SetUp]
    // public void Setup()
    // {
    // }

    [Test]
    [TestCase("123", ExpectedResult = 123)]
    [TestCase("321", ExpectedResult = 321)]
    [TestCase("1233214565", ExpectedResult = 1233214565)]
    [TestCase("1233214562", ExpectedResult = 1233214562)]
    [TestCase("1233214562", ExpectedResult = 1233214562)]
    public int ToIntNumber2(string raw)
    {
        var span = raw.AsSpan();
        return span.ToIntNumber();
    }
    
    // [Test]
    // [TestCase("123", ExpectedResult = 123)]
    // [TestCase("321", ExpectedResult = 321)]
    // [TestCase("1233214565", ExpectedResult = 1233214565)]
    // public int ToIntNumber1(string raw)
    // {
    //     var span = raw.AsSpan();
    //     return span.ToIntNumber();
    // }
}