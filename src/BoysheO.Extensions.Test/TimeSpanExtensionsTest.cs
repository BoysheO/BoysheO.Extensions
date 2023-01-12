using System;
using System.Globalization;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class TimeSpanExtensionsTest
{
    // [TestCase("00:59:13", ExpectedResult = "59m")]
    // [TestCase("01:59:13", ExpectedResult = "1h")]
    // public string ToSummaryText1(string dat)
    // {
    //     var t = TimeSpan.Parse(dat);
    //     return t.ToSummaryText1("d","h","m","s");
    // }
    //
    // [TestCase("61", ExpectedResult = "1m")]
    // [TestCase("59", ExpectedResult = "59s")]
    // public string ToSummaryText1WhenOverData(string secends)
    // {
    //     var sec = int.Parse(secends);
    //     var t = TimeSpan.FromSeconds(sec);
    //     return t.ToSummaryText1("d","h","m","s");
    // }
}