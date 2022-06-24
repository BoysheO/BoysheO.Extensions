using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Other;

[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
[MemoryDiagnoser]
public sealed class ForeachForSpanArray
{
    [Params("1234567890")] public string Source;

    
    [Benchmark]
    public int ArrayFor()
    {
        int count = 0;
        var len = Source.Length;
        for (var i = 0; i < Source.Length; i++)
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
        for (var index = 0; index < cs.Length; index++)
        {
            var c = cs[index];
            count += c;
        }

        return count;
    }
}