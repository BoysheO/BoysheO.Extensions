using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BoysheO.Extensions;

namespace Benchmark.StringExtensions;

[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net461)]
[RPlotExporter]
[MemoryDiagnoser]
public class ToIntNumber
{
    [Params("123", "2346728")] public string Source;

    [Benchmark(Baseline = true)]
    public int SystemIntParser()
    {
        return int.Parse(Source);
    }
    
    [Benchmark]
    public int IntParser()
    {
        return Source.ToIntNumber();
    }

    [Benchmark]
    public int AsSpanToNumber()
    {
        return Source.AsSpan().ToPositiveInt();
    }
}