using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Other;

[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
[MemoryDiagnoser]
public class OtherBenchmark
{
    [Params(1, 1234567890)]
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
// BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1288 (21H2)
// 11th Gen Intel Core i5-11400 2.60GHz, 1 CPU, 12 logical and 6 physical cores
// .NET SDK=6.0.300
//   [Host]   : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
//   .NET 6.0 : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
//
// Job=.NET 6.0  Runtime=.NET 6.0  
//
// |         Method |     Source |      Mean |     Error |    StdDev | Allocated |
// |--------------- |----------- |----------:|----------:|----------:|----------:|
// |   CountByWhile |          1 | 0.2266 ns | 0.0102 ns | 0.0090 ns |         - |
// |      CountByIf |          1 | 3.0818 ns | 0.0693 ns | 0.0648 ns |         - |
// | CountBySwitch1 |          1 | 1.0087 ns | 0.0155 ns | 0.0145 ns |         - |
// | CountBySwitch2 |          1 | 1.0111 ns | 0.0252 ns | 0.0236 ns |         - |
// |   CountByWhile |         12 | 0.2400 ns | 0.0125 ns | 0.0117 ns |         - |
// |      CountByIf |         12 | 2.8095 ns | 0.0649 ns | 0.0607 ns |         - |
// | CountBySwitch1 |         12 | 0.9881 ns | 0.0193 ns | 0.0171 ns |         - |
// | CountBySwitch2 |         12 | 1.0073 ns | 0.0219 ns | 0.0205 ns |         - |
// |   CountByWhile |        123 | 0.2318 ns | 0.0138 ns | 0.0129 ns |         - |
// |      CountByIf |        123 | 2.5064 ns | 0.0398 ns | 0.0372 ns |         - |
// | CountBySwitch1 |        123 | 1.2536 ns | 0.0242 ns | 0.0226 ns |         - |
// | CountBySwitch2 |        123 | 1.2603 ns | 0.0348 ns | 0.0326 ns |         - |
// |   CountByWhile |       1234 | 0.2368 ns | 0.0091 ns | 0.0085 ns |         - |
// |      CountByIf |       1234 | 2.2526 ns | 0.0317 ns | 0.0281 ns |         - |
// | CountBySwitch1 |       1234 | 1.2455 ns | 0.0138 ns | 0.0122 ns |         - |
// | CountBySwitch2 |       1234 | 1.2462 ns | 0.0228 ns | 0.0214 ns |         - |
// |   CountByWhile |      12345 | 0.2264 ns | 0.0095 ns | 0.0089 ns |         - |
// |      CountByIf |      12345 | 2.0911 ns | 0.0246 ns | 0.0218 ns |         - |
// | CountBySwitch1 |      12345 | 1.2586 ns | 0.0164 ns | 0.0145 ns |         - |
// | CountBySwitch2 |      12345 | 1.2534 ns | 0.0181 ns | 0.0169 ns |         - |
// |   CountByWhile |     123456 | 0.2179 ns | 0.0116 ns | 0.0103 ns |         - |
// |      CountByIf |     123456 | 1.4977 ns | 0.0376 ns | 0.0333 ns |         - |
// | CountBySwitch1 |     123456 | 0.7663 ns | 0.0266 ns | 0.0236 ns |         - |
// | CountBySwitch2 |     123456 | 0.7813 ns | 0.0414 ns | 0.0442 ns |         - |
// |   CountByWhile |    1234567 | 0.2268 ns | 0.0121 ns | 0.0107 ns |         - |
// |      CountByIf |    1234567 | 1.2139 ns | 0.0238 ns | 0.0223 ns |         - |
// | CountBySwitch1 |    1234567 | 0.7531 ns | 0.0184 ns | 0.0163 ns |         - |
// | CountBySwitch2 |    1234567 | 0.7796 ns | 0.0204 ns | 0.0191 ns |         - |
// |   CountByWhile |   12345678 | 0.2213 ns | 0.0114 ns | 0.0106 ns |         - |
// |      CountByIf |   12345678 | 0.6959 ns | 0.0288 ns | 0.0241 ns |         - |
// | CountBySwitch1 |   12345678 | 0.4151 ns | 0.0147 ns | 0.0130 ns |         - |
// | CountBySwitch2 |   12345678 | 0.7070 ns | 0.0180 ns | 0.0160 ns |         - |
// |   CountByWhile |  123456789 | 0.2269 ns | 0.0229 ns | 0.0214 ns |         - |
// |      CountByIf |  123456789 | 0.1869 ns | 0.0278 ns | 0.0273 ns |         - |
// | CountBySwitch1 |  123456789 | 0.3787 ns | 0.0234 ns | 0.0195 ns |         - |
// | CountBySwitch2 |  123456789 | 0.6775 ns | 0.0336 ns | 0.0315 ns |         - |
// |   CountByWhile | 1234567890 | 0.2183 ns | 0.0291 ns | 0.0335 ns |         - |
// |      CountByIf | 1234567890 | 0.0000 ns | 0.0000 ns | 0.0000 ns |         - |
// | CountBySwitch1 | 1234567890 | 0.1994 ns | 0.0278 ns | 0.0232 ns |         - |
// | CountBySwitch2 | 1234567890 | 0.4354 ns | 0.0118 ns | 0.0098 ns |         - |