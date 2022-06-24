using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Other;

[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
[MemoryDiagnoser]
public class ForeachForSpanArray
{
    [Params("1234567890")] public string Source;

    /// <summary>
    /// 性能提示：比其他3个都慢一倍
    /// </summary>
    /// <returns></returns>
    [Benchmark]
    public int ArrayFor()
    {
        int count = 0;
        var len = Source.Length;
        for (var i = 0; i < len; i++)
        {
            count += Source[i];
        }

        return count;
    }

    [Benchmark]
    public int ArrayForeach()
    {
        int count = 0;
        foreach (var c in Source)
        {
            count += c;
        }

        return count;
    }

    [Benchmark]
    public int SpanForeach()
    {
        int count = 0;
        foreach (var c in Source.AsSpan())
        {
            count += c;
        }

        return count;
    }

    [Benchmark]
    public int SpanFor()
    {
        int count = 0;
        var cs = Source.AsSpan();
        var len = cs.Length;
        for (var index = 0; index < len; index++)
        {
            var c = cs[index];
            count += c;
        }

        return count;
    }

    [Benchmark]
    public int ArrayFor33()
    {
        int max = 0;
        var offset = 3;
        var count = 3 + offset;
        for (var i = offset; i < count; i++)
        {
            max += Source[i];
        }

        return count;
    }

    [Benchmark]
    public int ArrayForeach33()
    {
        var offset = 3;
        var count = 3;
        int max = 0;
        foreach (var c in Source)
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
|         Method |     Source |      Mean |     Error |    StdDev |    Median | Allocated |
|--------------- |----------- |----------:|----------:|----------:|----------:|----------:|
|       ArrayFor | 1234567890 | 4.0556 ns | 0.0091 ns | 0.0080 ns | 4.0545 ns |         - |
|   ArrayForeach | 1234567890 | 2.8145 ns | 0.0092 ns | 0.0081 ns | 2.8112 ns |         - |
|    SpanForeach | 1234567890 | 2.9974 ns | 0.0294 ns | 0.0275 ns | 2.9868 ns |         - |
|        SpanFor | 1234567890 | 3.0172 ns | 0.0353 ns | 0.0330 ns | 3.0429 ns |         - |
|     ArrayFor33 | 1234567890 | 1.4254 ns | 0.0077 ns | 0.0072 ns | 1.4264 ns |         - |
| ArrayForeach33 | 1234567890 | 6.5222 ns | 0.0235 ns | 0.0220 ns | 6.5110 ns |         - |
|  SpanForeach33 | 1234567890 | 0.8115 ns | 0.0039 ns | 0.0036 ns | 0.8136 ns |         - |
|      SpanFor33 | 1234567890 | 0.8118 ns | 0.0051 ns | 0.0048 ns | 0.8138 ns |         - |
 */