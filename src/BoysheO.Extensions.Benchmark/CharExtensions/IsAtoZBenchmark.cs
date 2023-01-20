using BenchmarkDotNet.Attributes;
using BoysheO.Extensions;
using BoysheO.Util;

namespace Benchmark.ByteExtensions;

public class IsAtoZBenchmark
{
    private const string Digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private char Character;
    
    public IsAtoZBenchmark()
    {
        Character = Digits[RandomUtil.Int % Digits.Length];
    }
    
    [Benchmark(Baseline = true)]
    public bool IsUpper()
    {
        return char.IsUpper(Character);
    }
    
    [Benchmark]
    public bool IsAtoZ()
    {
        return Character.IsAtoZ();
    }
}