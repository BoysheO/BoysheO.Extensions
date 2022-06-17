using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Other;

[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
[MemoryDiagnoser]
public class OtherBenchmark
{
    [Params(1, 12, 123, 1234, 12345, 123456, 1234567, 12345678, 123456789, 1234567890)]
    public int Source;

    [Benchmark]
    public int CountByWhile()
    {
        var count = 0;
        while (Source > 0)
        {
            Source /= 10;
            count++;
        }

        return count;
    }

    [Benchmark]
    public int CountByIf()
    {
        if (Source >= 1000000000) return 10;
        if (Source >= 100000000) return 9;
        if (Source >= 10000000) return 8;
        if (Source >= 1000000) return 7;
        if (Source >= 100000) return 6;
        if (Source >= 10000) return 5;
        if (Source >= 1000) return 4;
        if (Source >= 100) return 3;
        if (Source >= 10) return 2;
        if (Source >= 1) return 1;
        return 0;
    }

    [Benchmark]
    public int CountBySwitch1()
    {
        switch (Source)
        {
            case >= 1000000000:
                return 10;
            case >= 100000000:
                return 9;
            case >= 10000000:
                return 8;
            case >= 1000000:
                return 7;
            case >= 100000:
                return 6;
            case >= 10000:
                return 5;
            case >= 1000:
                return 4;
            case >= 100:
                return 3;
            case >= 10:
                return 2;
            case >= 1:
                return 1;
            default:
                return 0;
        }
    }

    [Benchmark]
    public int CountBySwitch2()
    {
        return Source switch
        {
            >= 1000000000 => 10,
            >= 100000000 => 9,
            >= 10000000 => 8,
            >= 1000000 => 7,
            >= 100000 => 6,
            >= 10000 => 5,
            >= 1000 => 4,
            >= 100 => 3,
            >= 10 => 2,
            >= 1 => 1,
            _ => 0
        };
    }
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