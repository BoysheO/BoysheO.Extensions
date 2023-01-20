using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BoysheO.Extensions;
using BoysheO.Util;

namespace Benchmark.ByteExtensions;

[SimpleJob(RuntimeMoniker.Net48)]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Mono)]
[MemoryDiagnoser]
public class Is0to9Benchmark
{
    private const string Digits = "0123456789";
    private char Character;

    public Is0to9Benchmark()
    {
        Character = Digits[RandomUtil.Int % Digits.Length];
    }
    
    [Benchmark(Baseline = true)]
    public bool IsDigital()
    {
        return char.IsDigit(Character);
    }
    
    [Benchmark]
    public bool Is0to9()
    {
        return Character.Is0to9();
    }
}