using System;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class DateTimeOffsetExtensionsTests
{
    [TestCase("5/23/2022 9:46:12 AM +08:00", ExpectedResult = "5/23/2022 12:00:00 AM +08:00")]
    public string ToCurrentDay12Am(string dateTimeOffset)
    {
        var dt = DateTimeOffset.Parse(dateTimeOffset);
        var v = dt.CurrentDay12Am();
        return v.ToString();
    }

    [TestCase("5/23/2022 9:46:12 AM +08:00", ExpectedResult = "5/23/2022 9:00:00 AM +08:00")]
    public string ToCurrentHour(string dat)
    {
        var dt = DateTimeOffset.Parse(dat);
        var v = dt.CurrentHour0Min();
        return v.ToString();
    }
}