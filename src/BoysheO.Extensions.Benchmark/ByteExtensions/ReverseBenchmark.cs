using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BoysheO.Extensions;

namespace Benchmark.ByteExtensions;

[SimpleJob(RuntimeMoniker.Net48)]
[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
[MemoryDiagnoser]
public class ReverseBenchmark
{
    [Params(1, 12, 123, 1234, 12345, 123456, 1234567, 12345678, 123456789, 1234567890)]
    public int Source;

    [Benchmark]
    public void LinqReverse()
    {
        var bytes = Prepare();
        bytes = bytes.Reverse().ToArray();
    }

    [Benchmark]
    public void SpanReverse()
    {
        var bytes = Prepare();
        bytes.AsSpan().Reverse();
    }

    [Benchmark]
    public void ArrayReverse()
    {
        var bytes = Prepare();
        Array.Reverse(bytes);
    }

    [Benchmark]
    public void MemoryExtensionsReverse()
    {
        var bytes = Prepare();
        System.MemoryExtensions.Reverse(bytes.AsSpan());
    }

    private byte[] Prepare()
    {
        return Source.AsMemoryByteSpan().ToArray();
    }
}