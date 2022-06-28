using Benchmark.ByteExtensions;
using Benchmark.Other;
using Benchmark.StringExtensions;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<ReverseBenchmark>();
Console.WriteLine("hello world!");
