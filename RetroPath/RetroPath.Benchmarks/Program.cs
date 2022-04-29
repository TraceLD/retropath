// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using RetroPath.Benchmarks;

BenchmarkRunner.Run<RuleParserBenchmark>();