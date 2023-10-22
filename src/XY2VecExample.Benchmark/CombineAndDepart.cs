using BenchmarkDotNet.Attributes;
using XY2VecExample;

namespace Benchmark.StringExtensions;

public class CombineAndDepart
{
    private int x = Random.Shared.Next() * (Random.Shared.NextSingle() > 0.5f ? -1 : 1);
    private int y = Random.Shared.Next() * (Random.Shared.NextSingle() > 0.5f ? -1 : 1);


    [Benchmark(Baseline = true)]
    public (int, int) UnsafeCombine()
    {
        var a = Helper.UnsafeCombine2(x, y);
        var xy = Helper.UnsafeDepart2(a);
        return xy;
    }

    [Benchmark]
    public (int, int) BitCombine()
    {
        var a = Helper.UnsafeCombine2(x, y);
        var xy = Helper.UnsafeDepart2(a);
        return xy;
    }

    [Benchmark]
    public (int, int) CastCombine()
    {
        var a = Helper.UnsafeCombine2(x, y);
        var xy = Helper.UnsafeDepart2(a);
        return xy;
    }

    [Benchmark]
    public (int, int) UnsafeCombine2()
    {
        var a = Helper.UnsafeCombine2(x, y);
        var xy = Helper.UnsafeDepart2(a);
        return xy;
    }

    [Benchmark]
    public (int, int) Fastest()
    {
        var a = Helper.BitCombine(x, y);
        var xy = Helper.UnsafeDepart2(a);
        return xy;
    }
}