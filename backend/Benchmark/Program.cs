using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Storage;

BenchmarkRunner.Run(typeof(Benchmark));

[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
public class Benchmark
{
    private static readonly Base36Converter Sut = new();

    [Benchmark]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public void ConvertingStaticNumber()
    {
        Sut.ToBase36(long.MaxValue);
    }
}