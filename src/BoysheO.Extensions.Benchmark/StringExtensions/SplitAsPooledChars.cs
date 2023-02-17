using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using BoysheO.Extensions;
using BoysheO.Util;

namespace Benchmark.StringExtensions;

// [SimpleJob(RuntimeMoniker.Net461)]
// [SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class SplitAsPooledChars
{
    public string Source;

    public char Sp;

    private const int loop = 100_0000;

    [Benchmark]
    public List<string> StringSplit()
    {
        List<string> res = new List<string>();
        for (int j = 0; j < loop; j++)
        {
            var sp = Source.Split(Sp);
            var count = sp.Length;
            for (int i = 0; i < count; i++)
            {
                string s = sp[i];
                res.Add(s);
            }
        }

        return res;
    }
    //
    // [Benchmark]
    // public List<string> SplitAsPooledChars1()
    // {
    //     List<string> res = new List<string>();
    //     Span<char> span = stackalloc char[1];
    //     for (int j = 0; j < loop; j++)
    //     {
    //         span[0] = Sp;
    //         var count = Source.AsSpan().SplitAsPooledChars(span, out var result);
    //         for (int i = 0; i < count; i++)
    //         {
    //             (int start, int count) r = result[i];
    //             res.Add(Source.Substring(r.start, r.count));
    //         }
    //
    //         ArrayPool<(int, int)>.Shared.Return(result);
    //     }
    //
    //     return res;
    // }

    [Benchmark]
    public List<string> SplitIndex()
    {
        List<string> res = new List<string>();
        for (int j = 0; j < loop; j++)
        {
            for (int idx = 0, len = Source.Length; idx < len;)
            {
                var afterIdx = Source.IndexOf(Sp, idx);
                if (afterIdx > 0)
                {
                    res.Add(Source.Substring(idx, afterIdx));
                    idx = afterIdx + len;
                }
                else
                {
                    res.Add(Source.Substring(idx, len - (idx + 1)));
                    break;
                }
            }
        }

        return res;
    }

    [GlobalSetup]
    public void Setup()
    {
        var digitCount = RandomUtil.Int % 2 + 1;
        // Sp = Enumerable.Range(0, digitCount).Select(v => (RandomUtil.Int % 10).ToString()).JoinAsOneString("");
        Sp = (RandomUtil.Int % 10).ToString()[0];
        var sb = new StringBuilder();
        for (int i = 0; i < 10; i++) sb.Append(RandomUtil.Long);
        Source = sb.ToString();
    }
}