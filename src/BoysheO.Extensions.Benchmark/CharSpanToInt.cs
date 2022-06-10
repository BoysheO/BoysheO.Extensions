using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BoysheO.Extensions.Benchmark;

[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
[MemoryDiagnoser]
public class CharSpanToInt
{
    [Params("123", "2346728")] public string Source;

    [Benchmark]
    public int AsSpanToIntNumber() => Source.AsSpan().ToPositiveInt();

    [Benchmark]
    public int ToIntNumber() => Source.ToIntNumber();

    [Benchmark]
    public int IntParse() => int.Parse(Source);
}
/*
|            Method |  Source |      Mean |     Error |    StdDev | Allocated |
|------------------ |-------- |----------:|----------:|----------:|----------:|
| AsSpanToIntNumber |     123 |  2.503 ns | 0.0209 ns | 0.0185 ns |         - |
|       ToIntNumber |     123 |  8.012 ns | 0.0553 ns | 0.0490 ns |         - |
|          IntParse |     123 |  8.240 ns | 0.1001 ns | 0.0937 ns |         - |
| AsSpanToIntNumber | 2346728 |  4.962 ns | 0.0667 ns | 0.0624 ns |         - |
|       ToIntNumber | 2346728 | 10.927 ns | 0.1264 ns | 0.1182 ns |         - |
|          IntParse | 2346728 | 11.019 ns | 0.0621 ns | 0.0581 ns |         - |
*/