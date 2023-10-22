// See https://aka.ms/new-console-template for more information

using Benchmark.StringExtensions;
using BenchmarkDotNet.Running;

Console.WriteLine("Hello, World!");
// var summary = BenchmarkRunner.Run<XYCombine>();
// var summary2 = BenchmarkRunner.Run<XYDepart>();
var summary3 = BenchmarkRunner.Run<CombineAndDepart>();