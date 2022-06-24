using Benchmark.Other;
using Benchmark.StringExtensions;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<ForeachForSpanArray>();
Console.WriteLine("hello world!");
