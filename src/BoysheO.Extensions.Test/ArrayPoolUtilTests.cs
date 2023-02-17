using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
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
            buff = ArrayPoolUtil.Add(buff, elementCount, b[0], out elementCount);
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
}