using System;
using System.Collections.Generic;
using System.Linq;
using BoysheO.Util;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class MemoryExtensionsTest
{
    [TestCase]
    public void Pinning()
    {
        var len = 10;
        var digits = Enumerable.Range(0, len).Select(v => RandomUtil.Int % 10).ToArray();
        var copy = new List<int>(digits);
        copy.Insert(0,default);

        var copy2 = new List<int>(digits).ToArray();
        copy2.AsSpan().Panning(-1);

        Console.WriteLine(digits.Select(v=>v.ToString()).JoinAsOneString());
        Console.WriteLine(copy.Select(v=>v.ToString()).JoinAsOneString());
        Console.WriteLine(copy2.Select(v=>v.ToString()).JoinAsOneString());
        Assert.AreEqual(copy.SkipLast(1),copy2);
    }
}