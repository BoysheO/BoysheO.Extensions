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
public class XYDepart
{
    private ulong xy = unchecked((ulong) Random.Shared.NextInt64(long.MinValue, long.MaxValue));

    [Benchmark(Baseline = true)]
    public (int x, int y) UnsafeDepart()
    {
        return Helper.UnsafeDepart(xy);
    }

    [Benchmark]
    public (int x, int y) UnsafeDepart2()
    {
        return Helper.UnsafeDepart2(xy);
    }


    [Benchmark]
    public (int x, int y) BitDepart()
    {
        return Helper.BitDepart(xy);
    }

    [Benchmark]
    public (int x, int y) CastDepart()
    {
        return Helper.CastDepart(xy);
    }
}