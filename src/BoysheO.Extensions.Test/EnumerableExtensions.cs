using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using BoysheO.Util;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class EnumerableExtensions
{
    [TestCase]
    public void PooledGroupBy()
    {
        var src = Enumerable.Range(0, 10).Select(v => RandomUtil.Int).ToArray();
        Console.WriteLine($"src={src.Select(v => v.ToString()).JoinAsOneString()}");


        Dictionary<int, List<int>> sysGroup = src.GroupBy(v => v % 10)
            .ToDictionary(v => v.Key, v => v.ToList());
        var json = JsonSerializer.Serialize(sysGroup);
        Console.WriteLine(json);

        Extensions.EnumerableExtensions.PooledGroupBy(src,
            v => v % 10,
            out var keys, out var keysCount,
            out var valueIndex, out var valueIndexCount,
            out var values, out var valuesCount);
        var lstKeys = keys.Take(keysCount).ToArray();
        var lstValueIndex = valueIndex.Take(valueIndexCount).ToArray();
        var lstValues = values.Take(valuesCount).ToArray();
        var dic = new Dictionary<int, List<int>>();
        for (int i = 0; i < lstKeys.Length; i++)
        {
            var key = lstKeys[i];
            var idx = lstValueIndex[i];
            var nextIdx = i + 1 < lstValueIndex.Length ? lstValueIndex[i + 1] : lstValues.Length - 1;
            var count = nextIdx - idx;
            dic.Add(key, lstValues.AsSpan(idx,count).ToArray().ToList());
        }

        var json2 = JsonSerializer.Serialize(dic);
        Console.WriteLine(json2);

        foreach (var sysGroupValue in sysGroup.Values)
        {
            sysGroupValue.Sort();
        }

        foreach (var dicValue in dic.Values)
        {
            dicValue.Sort();
        }

        var a = sysGroup.ToList();
        a.Sort((a, b) => a.Key - b.Key);

        var b = dic.ToList();
        b.Sort((a, b) => a.Key - b.Key);

        var aj = JsonSerializer.Serialize(a);
        var bj = JsonSerializer.Serialize(b);
        Assert.AreEqual(aj, bj);
        // Assert.AreEqual(sysGroup,dic);
    }
}