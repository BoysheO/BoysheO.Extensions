using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using XY2VecExample;

namespace Benchmark.StringExtensions;

// [SimpleJob(RuntimeMoniker.Net60)]
// [SimpleJob(RuntimeMoniker.Mono)]
// [SimpleJob(RuntimeMoniker.Net47)]
// [SimpleJob(RuntimeMoniker.MonoAOTLLVM)]
[SimpleJob(RuntimeMoniker.Net461)]
[SimpleJob(RuntimeMoniker.Net60)]
// [SimpleJob(RuntimeMoniker.Mono)]
// [RPlotExporter] //生成png直方图
// [MemoryDiagnoser]
public class XYCombine
{
    private int x = Random.Shared.Next() * (Random.Shared.NextSingle() > 0.5f ? -1 : 1);
    private int y = Random.Shared.Next() * (Random.Shared.NextSingle() > 0.5f ? -1 : 1);


    [Benchmark(Baseline = true)]
    public ulong UnsafeCombine()
    {
        return Helper.UnsafeCombine(x, y);
    }

    [Benchmark]
    public ulong BitCombine()
    {
        return Helper.BitCombine(x, y);
    }
    
    [Benchmark]
    public ulong CastCombine()
    {
        return Helper.CastCombine(x, y);
    }

    [Benchmark]
    public ulong UnsafeCombine2()
    {
        return Helper.UnsafeCombine2(x, y);
    }
}