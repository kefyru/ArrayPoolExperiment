using ArrayPoolExperiment;
using BenchmarkDotNet.Running;

[assembly: DefaultSharedArrayPool(typeof(ArrayPoolWithTracking<>))]

namespace ArrayPoolExperiment;

public static class Program
{
  public static void Main(params string[]? args)
  {
    var arr = ArrayPoolMock<byte>.Shared.Rent(1024);
    ArrayPoolMock<byte>.Shared.Return(arr); // ok
    ArrayPoolMock<byte>.Shared.Return(arr); // should output message
    
    BenchmarkRunner.Run<Test>();
  }
}