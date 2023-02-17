using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BoysheO.Extensions;
using Cysharp.Text;

namespace Benchmark.StringExtensions;

// [SimpleJob(RuntimeMoniker.Net60)]
// [SimpleJob(RuntimeMoniker.Mono)]
// [SimpleJob(RuntimeMoniker.Net47)]
// [SimpleJob(RuntimeMoniker.MonoAOTLLVM)]
[SimpleJob(RuntimeMoniker.Net461)]
[SimpleJob(RuntimeMoniker.Net60)]
// [SimpleJob(RuntimeMoniker.Mono)]
[RPlotExporter] //生成png直方图
[MemoryDiagnoser]
public class JoinAsOneString
{
    [Params(
        new[] {"1", "2", "3", "4", "5"},
        new[] {"a", "b", "c"})]
    public string[] Source;

    public const string Sp = ",";

    [Benchmark(Baseline = true)]
    public string StringJoin()
    {
        return string.Join(Sp, Source);
    }
    
    [Benchmark]
    public string StringJoinExtensions()
    {
        return Source.JoinAsOnString();
    }
    
    [Benchmark]
    public string StringJoinExtensionsWithParma()
    {
        return Source.JoinAsOnString(Sp);
    }

    [Benchmark]
    public string ZStringJoin() 
    {
        return ZString.Join(Sp, Source);
    }
}