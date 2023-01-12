using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using BoysheO.Util;
using Extensions;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class ArrayPoolUtilTests
{
    private const int SizeMax = 1000000;
    private const int TimesMax = 1000;

    [TestCase]
    public void RandomAdd()
    {
        var random = new Random();
        var lst = new List<byte>();
        var buff = ArrayPool<byte>.Shared.Rent(random.Next(1, SizeMax));
        var elementCount = 0;
        var times = random.NextInt64() % TimesMax;
        Span<byte> b = stackalloc byte[1];
        for (int i = 0; i < times; i++)
        {
            random.NextBytes(b);
            elementCount = ArrayPoolUtil.Add(buff, elementCount, b[0], out buff);
            lst.Add(b[0]);
        }

        Assert.AreEqual(lst.Count, elementCount);
        Assert.AreEqual(lst.ToArray(), buff.Take(elementCount).ToArray());
        ArrayPool<byte>.Shared.Return(buff);
    }

    [TestCase]
    public void RandomAddRange()
    {
        var random = new Random();
        var lst = new List<byte>();
        var buff = ArrayPool<byte>.Shared.Rent(random.Next(1, SizeMax));
        var elementCount = 0;
        var times = random.NextInt64() % TimesMax;
        byte[] b = new byte[223];
        for (int i = 0; i < times; i++)
        {
            random.NextBytes(b);
            elementCount = ArrayPoolUtil.AddRange(buff, elementCount, b, 0, b.Length, out buff);
            lst.AddRange(b);
        }

        Assert.AreEqual(lst.Count, elementCount);
        Assert.AreEqual(lst.ToArray(), buff.Take(elementCount).ToArray());
        ArrayPool<byte>.Shared.Return(buff);
    }

    [TestCase]
    public void InsertCase()
    {
        var source = Enumerable.Range(0, 4).Select(v => RandomUtil.Int % 10).ToArray();
        var insertValue = RandomUtil.Int % 10;
        var insertIdx = RandomUtil.Int % source.Length;
        Console.WriteLine(
            $"source={source.Select(v => v.ToString()).JoinAsOnString(",")} insertValue={insertValue} insertIndex={insertIdx}");

        int[] poolResult;
        {
            var buf = ArrayPool<int>.Shared.Rent(source.Length);
            for (int i = 0; i < source.Length; i++)
            {
                buf[i] = source[i];
            }

            var bufCount = source.Length;
            bufCount = ArrayPoolUtil.Insert(buf, bufCount, insertValue, insertIdx, out buf);
            poolResult = buf.Take(bufCount).ToArray();
            Console.WriteLine(buf.AsSpan(0, bufCount).ToArray().Select(v => v.ToString()).JoinAsOnString());
            ArrayPool<int>.Shared.Return(buf);
        }

        int[] sysResult;
        {
            var lst = new List<int>(source);
            lst.Insert(insertIdx, insertValue);
            sysResult = lst.ToArray();
        }

        Console.WriteLine($"pool={poolResult.Select(v=>v.ToString()).JoinAsOnString()} sys={sysResult.Select(v=>v.ToString()).JoinAsOnString()}");
        Assert.AreEqual(sysResult, poolResult);
    }
}