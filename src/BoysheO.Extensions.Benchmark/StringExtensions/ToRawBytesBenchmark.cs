using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BoysheO.Extensions;

namespace Benchmark.StringExtensions;

[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
[MemoryDiagnoser]
public class ToRawBytesBenchmark
{
    [Params("123", "2346728")] public string Source;

    [Benchmark]
    public byte[] ToRawBytesWithBuffer()
    {
        var array = ArrayPool<byte>.Shared.Rent(Source.Length);
        var seg = new ArraySegment<byte>(array, 0, Source.Length);
        Source.ToRawBytes(seg);
        var res = seg.ToArray();
        ArrayPool<byte>.Shared.Return(array);
        return res;
    }

    [Benchmark]
    public byte[] ToRawBytes()
    {
        var src = Source.ToRawBytes();
        var res = new byte[src.Length];
        Array.Copy(src,res,src.Length);
        return res;
    }
}