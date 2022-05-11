using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BoysheO.Extensions.Benchmark;
[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
[MemoryDiagnoser]
public class CharSpanToInt
{
    [Params("123","2346728")]
    public string Source;

    // [Benchmark]
    // public int ToInt1() => Source.AsSpan().ToIntNumber();

    [Benchmark]
    public int ToIntNumber() => Source.AsSpan().ToIntNumber();
    
    [Benchmark]
    public int IntParse() => int.Parse(Source);
}