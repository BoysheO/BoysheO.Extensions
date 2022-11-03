using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using BoysheO.Extensions;
using BoysheO.Util;

namespace Benchmark.StringExtensions;

// [SimpleJob(RuntimeMoniker.Net461)]
// [SimpleJob(RuntimeMoniker.Net60)]
public class SplitAsPooledChars
{
    public string Source;

    public char Sp;

    private const int loop = 10000;

    [Benchmark]
    public List<int> StringSplit()
    {
        List<int> res = new List<int>();
        for (int j = 0; j < loop; j++)
        {
            var sp = Source.Split();
            var count = sp.Length;
            for (int i = 0; i < count; i++)
            {
                var s = sp[i];
                var num = s.AsSpan().ToPositiveInt();
                res.Add(num);
            }
        }

        return res;
    }

    [Benchmark]
    public List<int> SplitAsPooledChars1()
    {
        List<int> res = new List<int>();
        Span<char> span = stackalloc char[1];
        for (int j = 0; j < loop; j++)
        {
            span[0] = Sp;
            var count = Source.AsSpan().SplitAsPooledChars(span, out var result);
            for (int i = 0; i < count; i++)
            {
                var r = result[i];
                var num = Source.AsSpan(r.start, r.count).ToPositiveInt();
                res.Add(num);
            }

            ArrayPool<(int, int)>.Shared.Return(result);
        }

        return res;
    }

    [GlobalSetup]
    void Setup()
    {
        var digitCount = RandomUtil.Int % 2 + 1;
        // Sp = Enumerable.Range(0, digitCount).Select(v => (RandomUtil.Int % 10).ToString()).JoinAsOneString("");
        Sp = (RandomUtil.Int % 10).ToString()[0];
        var sb = new StringBuilder();
        for (int i = 0; i < 10; i++) sb.Append(RandomUtil.Long);
        Source = sb.ToString();
    }
}