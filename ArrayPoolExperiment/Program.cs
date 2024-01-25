using ArrayPoolExperiment;
using BenchmarkDotNet.Running;


[assembly: DefaultSharedArrayPool(typeof(ArrayPoolWithTracking<>))]
namespace ArrayPoolExperiment;
public static class Program
{
  public static void Main(params string[]? args)
  {
    var arr = ArrayPool<byte>.Shared.Rent(1024);
    ArrayPool<byte>.Shared.Return(arr); // ok
    ArrayPool<byte>.Shared.Return(arr); // should output message
    
    BenchmarkRunner.Run<Test>();
  }
}