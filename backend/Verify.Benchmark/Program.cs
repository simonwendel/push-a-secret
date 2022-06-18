using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Domain;

BenchmarkRunner.Run(typeof(Benchmark));

[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
public class Benchmark
{
    private static readonly BaseConverter Sut = new();

    [Benchmark]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public void ToBase36()
    {
        Sut.ToBase36(long.MaxValue);
    }
    
    [Benchmark]
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public void FromBase36()
    {
        Sut.FromBase36("1y2p0ij32e8e7");
    }
}