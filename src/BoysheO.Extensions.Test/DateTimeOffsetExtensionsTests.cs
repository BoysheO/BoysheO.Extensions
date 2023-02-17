using System;
using System.Globalization;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class DateTimeOffsetExtensionsTests
{
    [TestCase("5/23/2022 9:46:12 AM +08:00", ExpectedResult = "05/23/2022 00:00:00 +08:00")]
    [TestCase("5/23/2022 9:13:12 AM +08:00", ExpectedResult = "05/23/2022 00:00:00 +08:00")]
    [TestCase("5/13/2022 10:21:12 AM +08:00", ExpectedResult = "05/13/2022 00:00:00 +08:00")]
    public string ToCurrentDay12Am(string dateTimeOffset)
    {
        var dt = DateTimeOffset.Parse(dateTimeOffset);
        var v = dt.GetCurDay0Am();
        return v.ToString(CultureInfo.InvariantCulture);
    }

    [TestCase("5/23/2022 9:46:12 AM +08:00", ExpectedResult = "05/23/2022 09:00:00 +08:00")]
    [TestCase("5/13/2022 9:59:59 AM +08:00", ExpectedResult = "05/13/2022 09:00:00 +08:00")]
    public string ToCurrentHour(string dat)
    {
        var dt = DateTimeOffset.Parse(dat);
        var v = dt.GetCurHour0();
        return v.ToString(CultureInfo.InvariantCulture);
    }

    [TestCase("5/23/2022 9:46:12 AM +08:00", ExpectedResult = "05/24/2022 00:00:00 +08:00")]
    [TestCase("5/23/2022 9:13:12 AM +08:00", ExpectedResult = "05/24/2022 00:00:00 +08:00")]
    [TestCase("5/13/2022 10:21:12 AM +08:00", ExpectedResult = "05/14/2022 00:00:00 +08:00")]
    public string ToNextDay12Am(string dat)
    {
        var dt = DateTimeOffset.Parse(dat);
        var v = dt.GetNextDay0Am();
        return v.ToString(CultureInfo.InvariantCulture);
    }
}