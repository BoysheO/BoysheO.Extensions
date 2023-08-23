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
