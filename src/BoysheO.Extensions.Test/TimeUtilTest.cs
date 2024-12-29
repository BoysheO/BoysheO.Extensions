using System;
using System.Collections.Generic;
using System.Linq;
using Cronos;
using DateAndTime;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class TimeUtilTest
{
    // [TestCase("2023/1/17 09:45:12 +8:00", "2023/1/17 09:45:12 +8:00", "06:00:00", ExpectedResult = 0)]
    // [TestCase("2023/1/17 09:45:12 +8:00", "2023/1/18 09:45:12 +8:00", "06:00:00", ExpectedResult = 1)]
    // [TestCase("2023/1/17 09:45:12 +8:00", "2023/1/18 09:45:12 +8:00", "06:00:00", ExpectedResult = 1)]
    // [TestCase("2023/1/17 09:45:12 +8:00", "2023/1/19 09:45:12 +8:00", "06:00:00", ExpectedResult = 2)]
    // [TestCase("2023/1/17 06:00:00 +8:00", "2023/1/19 06:00:00 +8:00", "06:00:00", ExpectedResult = 2)]
    // [TestCase("2023/1/17 06:00:00 +8:00", "2023/1/19 06:00:01 +8:00", "06:00:00", ExpectedResult = 3)]
    // [TestCase("2023/1/17 06:00:00 +8:00", "2023/1/18 06:00:01 +8:00", "06:00:00", ExpectedResult = 2)]
    // public int GetTimesTheTimeAppearsBetween(string start, string end, string timeOfDay)
    // {
    //     var startMs = DateTimeOffset.Parse(start).ToUnixTimeMilliseconds();
    //     var endMs = DateTimeOffset.Parse(end).ToUnixTimeMilliseconds();
    //     var time = TimeSpan.Parse(timeOfDay);
    //     return TimeUtil.GetTimesTheTimeAppearsBetween(startMs, endMs, (int)time.TotalMilliseconds,
    //         (int)TimeSpan.FromHours(8).TotalMilliseconds);
    // }
    
    [TestCase("2023/1/17 09:45:12 +8:00", "2023/1/17 09:45:12 +8:00", "06:00:00", ExpectedResult = 0)]
    [TestCase("2023/1/17 09:45:12 +8:00", "2023/1/18 09:45:12 +8:00", "06:00:00", ExpectedResult = 1)]
    [TestCase("2023/1/17 09:45:12 +8:00", "2023/1/18 09:45:12 +8:00", "06:00:00", ExpectedResult = 1)]
    [TestCase("2023/1/17 09:45:12 +8:00", "2023/1/19 09:45:12 +8:00", "06:00:00", ExpectedResult = 2)]
    [TestCase("2023/1/17 06:00:00 +8:00", "2023/1/19 06:00:00 +8:00", "06:00:00", ExpectedResult = 2)]
    [TestCase("2023/1/17 06:00:00 +8:00", "2023/1/19 06:00:01 +8:00", "06:00:00", ExpectedResult = 3)]
    [TestCase("2023/1/17 06:00:00 +8:00", "2023/1/18 06:00:01 +8:00", "06:00:00", ExpectedResult = 2)]
    [TestCase("2023/1/17 06:00:00 +8:00", "2023/1/17 06:00:00 +8:00", "06:00:00", ExpectedResult = 1)]
    public long GetCountOfTheTimeBetween(string start, string end, string timeOfDay)
    {
        var startMs = DateTimeOffset.Parse(start).ToUnixTimeMilliseconds();
        var endMs = DateTimeOffset.Parse(end).ToUnixTimeMilliseconds();
        var time = TimeSpan.Parse(timeOfDay);
        Console.WriteLine(new {startMs,endMs,time=time.TotalMilliseconds,offset=(int)TimeSpan.FromHours(8).TotalMilliseconds});
        return TimeUtil.GetCountOfTheTimeBetween(startMs, endMs, (int)time.TotalMilliseconds,
            (int)TimeSpan.FromHours(8).TotalMilliseconds);
    }
    
    [TestCase("2023/1/17 09:45:12 +8:00", "2023/1/17 09:45:12 +8:00", "06:00:00", "0 6 * * *")]
    [TestCase("2023/1/17 09:45:12 +8:00", "2023/1/18 09:45:12 +8:00", "06:00:00", "0 6 * * *")]
    [TestCase("2023/1/17 09:45:12 +8:00", "2023/1/18 09:45:12 +8:00", "06:00:00", "0 6 * * *")]
    [TestCase("2023/1/17 09:45:12 +8:00", "2023/1/19 09:45:12 +8:00", "06:00:00", "0 6 * * *")]
    [TestCase("2023/1/17 06:00:00 +8:00", "2023/1/19 06:00:00 +8:00", "06:00:00", "0 6 * * *")]
    [TestCase("2023/1/17 06:00:00 +8:00", "2023/1/19 06:00:01 +8:00", "06:00:00", "0 6 * * *")]
    [TestCase("2023/1/17 06:00:00 +8:00", "2023/1/18 06:00:01 +8:00", "06:00:00", "0 6 * * *")]
    [TestCase("2023/1/17 06:00:00 +8:00", "2023/1/17 06:00:00 +8:00", "06:00:00", "0 6 * * *")]
    public void GetCountOfTheTimeBetween_Cronos(string start, string end, string timeOfDay,string cron)
    {
        var startMs = DateTimeOffset.Parse(start);
        var endMs = DateTimeOffset.Parse(end);
        var cronExp = CronExpression.Parse(cron);
        
        var time = TimeSpan.Parse(timeOfDay);
        Console.WriteLine(new {startMs,endMs,time=time.TotalMilliseconds,offset=(int)TimeSpan.FromHours(8).TotalMilliseconds});
        var count = TimeUtil.GetCountOfTheTimeBetween(startMs.ToUnixTimeMilliseconds(), endMs.ToUnixTimeMilliseconds(), (int)time.TotalMilliseconds,
            (int)TimeSpan.FromHours(8).TotalMilliseconds);

        var cou = cronExp.GetOccurrences(startMs, endMs,TimeZoneInfo.CreateCustomTimeZone("8",TimeSpan.FromHours(8),"ch","ch")).Count();
        Assert.AreEqual(count,cou);
    }

    // [TestCase("2023/1/17 05:59:59 +8:00", "06:00:00", ExpectedResult = "2023/01/16 06:00:00")]
    // [TestCase("2023/1/17 06:00:01 +8:00", "06:00:00", ExpectedResult = "2023/01/17 06:00:00")]
    // [TestCase("2023/1/17 13:00:01 +8:00", "06:00:00", ExpectedResult = "2023/01/17 06:00:00")]
    // [TestCase("2023/1/17 14:00:01 +8:00", "06:00:00", ExpectedResult = "2023/01/17 06:00:00")]
    // [TestCase("2023/1/17 18:00:01 +8:00", "06:00:00", ExpectedResult = "2023/01/17 06:00:00")]
    // [TestCase("2023/1/19 18:00:01 +8:00", "06:00:00", ExpectedResult = "2023/01/19 06:00:00")]
    // public string GetLastTimeMs(string time, string timeOfDay)
    // {
    //     var dtOfTime = DateTimeOffset.Parse(time);
    //     var spanOfDay = TimeSpan.Parse(timeOfDay);
    //     var timeMs = dtOfTime.ToUnixTimeMilliseconds();
    //     var timeMsOfDay = (int)spanOfDay.TotalMilliseconds;
    //     var timeOffset = (int)TimeSpan.FromHours(8).TotalMilliseconds;
    //     var res = TimeUtil.GetLastDay0TimeMs(timeMs, timeMsOfDay, timeOffset);
    //     return DateTimeOffset.FromUnixTimeMilliseconds(res).ToOffset(TimeSpan.FromHours(8))
    //         .ToString("yyyy/MM/dd hh:mm:ss");
    // }


    // [TestCase("2023/1/17 06:00:01 +8:00", ExpectedResult = 19373)]
    // [TestCase("1970/1/1 00:00:01 +0:00", ExpectedResult = 0)]
    // [TestCase("1970/1/2 00:00:01 +0:00", ExpectedResult = 1)]
    // [TestCase("1970/1/3 00:00:01 +0:00", ExpectedResult = 2)]
    // public long GetDays(string time)
    // {
    //     var dt = DateTimeOffset.Parse(time);
    //     var usec = dt.ToUnixTimeMilliseconds();
    //     var span = TimeSpan.FromMilliseconds(usec);
    //     Console.WriteLine(span.TotalDays);
    //     return TimeUtil.GetDays(dt.ToUnixTimeMilliseconds());
    // }
}