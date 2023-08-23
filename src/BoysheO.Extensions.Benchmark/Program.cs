using Benchmark.ByteExtensions;
using Benchmark.Other;
using Benchmark.StringExtensions;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<ForeachForSpanArray>();
Console.WriteLine("hello world!");
// var ins = new SplitAsPooledChars();
// ins.Setup();
// ins.StringSplit();
