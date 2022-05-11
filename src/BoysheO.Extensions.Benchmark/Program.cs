using BenchmarkDotNet.Running;
using BoysheO.Extensions.Benchmark;

var summary = BenchmarkRunner.Run<CharSpanToInt>();