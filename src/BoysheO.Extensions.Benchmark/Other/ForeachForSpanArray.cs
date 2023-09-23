using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Other;

[SimpleJob(RuntimeMoniker.Mono)]
[SimpleJob(RuntimeMoniker.Net60)]
// [RPlotExporter]
[MemoryDiagnoser]
public class ForeachForSpanArray
{
    public int[] Source;

    [GlobalSetup]
    public void ii()
    {
        Source = Enumerable.Range(1, 1000).ToArray();
    }

    /// <summary>
    /// 性能提示：比其他3个都慢一倍
    /// </summary>
    /// <returns></returns>
    [Benchmark]
    public int ArrayFor()
    {
        int count = 0;
        var src = Source;
        var len = src.Length;
        for (var i = 0; i < len; i++)
        {
            unchecked
            {
                count += src[i];
            }
        }

        return count;
    }

    [Benchmark]
    public int ArrayForeach()
    {
        int count = 0;
        var src = Source;
        foreach (var c in src)
        {
            unchecked
            {
                count += c;
            }
        }

        return count;
    }

    [Benchmark]
    public int SpanForeach()
    {
        int count = 0;
        var src = Source;
        foreach (var c in src.AsSpan())
        {
            unchecked
            {
                count += c;
            }
        }

        return count;
    }

    [Benchmark]
    public int SpanFor()
    {
        int count = 0;
        var src = Source;
        var cs = src.AsSpan();
        var len = cs.Length;
        for (var index = 0; index < len; index++)
        {
            unchecked
            {
                count += cs[index];
            }
        }

        return count;
    }

    [Benchmark]
    public int ArrayFor33()
    {
        int max = 0;
        var offset = 3;
        var count = 3 + offset;
        var src = Source;
        for (var i = offset; i < count; i++)
        {
            unchecked
            {
                max += src[i];
            }
        }

        return count;
    }

    [Benchmark]
    public int ArrayForeach33()
    {
        var offset = 3;
        var count = 3;
        int max = 0;
        var src = Source;
        foreach (var c in src)
        {
            if (offset >= 0)
            {
                offset--;
                continue;
            }

            if (count <= 0) return max;
            max += c;
            count--;
        }

        return max;
    }

    [Benchmark]
    public int SpanForeach33()
    {
        int count = 0;
        foreach (var c in Source.AsSpan(3, 3))
        {
            count += c;
        }

        return count;
    }

    [Benchmark]
    public int SpanFor33()
    {
        int count = 0;
        var cs = Source.AsSpan(3, 3);
        var len = cs.Length;
        for (var index = 0; index < len; index++)
        {
            var c = cs[index];
            count += c;
        }

        return count;
    }
}
/*
 * 其中一次运行
|         Method |      Job |  Runtime |          Mean |      Error |     StdDev |        Median | Allocated |
|--------------- |--------- |--------- |--------------:|-----------:|-----------:|--------------:|----------:|
|       ArrayFor | .NET 6.0 | .NET 6.0 |   217.2125 ns |  1.0477 ns |  0.8749 ns |   217.0490 ns |         - |
|   ArrayForeach | .NET 6.0 | .NET 6.0 |   217.8536 ns |  0.7814 ns |  0.6927 ns |   217.8916 ns |         - |
|    SpanForeach | .NET 6.0 | .NET 6.0 |   219.1812 ns |  1.4399 ns |  1.3469 ns |   219.2356 ns |         - |
|        SpanFor | .NET 6.0 | .NET 6.0 |   218.4413 ns |  1.4964 ns |  1.3265 ns |   218.1631 ns |         - |
|     ArrayFor33 | .NET 6.0 | .NET 6.0 |     0.9036 ns |  0.0187 ns |  0.0175 ns |     0.8960 ns |         - |
| ArrayForeach33 | .NET 6.0 | .NET 6.0 |     5.1462 ns |  0.0320 ns |  0.0299 ns |     5.1328 ns |         - |
|  SpanForeach33 | .NET 6.0 | .NET 6.0 |     0.6699 ns |  0.0133 ns |  0.0124 ns |     0.6680 ns |         - |
|      SpanFor33 | .NET 6.0 | .NET 6.0 |     0.6483 ns |  0.0160 ns |  0.0142 ns |     0.6452 ns |         - |
|       ArrayFor |     Mono |     Mono |   436.2396 ns |  3.0415 ns |  2.6962 ns |   436.8743 ns |         - |
|   ArrayForeach |     Mono |     Mono |   496.2832 ns |  2.5579 ns |  2.2675 ns |   495.7494 ns |         - |
|    SpanForeach |     Mono |     Mono | 1,096.5291 ns | 10.9542 ns | 10.2465 ns | 1,097.2127 ns |         - |
|        SpanFor |     Mono |     Mono | 1,802.2391 ns | 10.2827 ns |  9.6184 ns | 1,800.1060 ns |         - |
|     ArrayFor33 |     Mono |     Mono |     2.0732 ns |  0.0709 ns |  0.1145 ns |     2.1360 ns |         - |
| ArrayForeach33 |     Mono |     Mono |     7.1962 ns |  0.1711 ns |  0.2037 ns |     7.2725 ns |         - |
|  SpanForeach33 |     Mono |     Mono |     9.6423 ns |  0.0693 ns |  0.0648 ns |     9.6392 ns |         - |
|      SpanFor33 |     Mono |     Mono |     9.1406 ns |  0.1117 ns |  0.1045 ns |     9.1031 ns |         - |
 */
