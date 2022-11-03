using Benchmark.ByteExtensions;
using Benchmark.Other;
using Benchmark.StringExtensions;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<SplitAsPooledChars>();
Console.WriteLine("hello world!");
