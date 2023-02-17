using BenchmarkDotNet.Attributes;
using BoysheO.Extensions;

namespace Benchmark.ByteExtensions;

[MemoryDiagnoser]
public class ToBinTextBenchmark
{
    private readonly byte[] randBytes = new byte[10];

    public ToBinTextBenchmark()
    {
        var rand = new Random();
        rand.NextBytes(randBytes);
    }

    [Benchmark(Baseline = true)]
    public string ByteToBinTextByConvert()
    {
        var b = randBytes[0];
        var text = Convert.ToString(b, 2).PadLeft(8, '0');
        return text;
    }
    
    [Benchmark]
    public string ByteToBinTextByExtension()
    {
        var b = randBytes[0];
        var text = b.ToBinText();
        return text;
    }
}