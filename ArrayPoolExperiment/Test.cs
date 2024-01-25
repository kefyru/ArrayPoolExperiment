using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ArrayPoolExperiment;

[MemoryDiagnoser]
[SimpleJob(invocationCount: 10000000)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[DisassemblyDiagnoser]
public class Test
{
  [Benchmark] public void StandardTest()
  {
    var arr = ArrayPool<byte>.Shared.Rent(1024);
    ArrayPool<byte>.Shared.Return(arr);
  }
  
  [Benchmark] public void MockTest()
  {
    var arr = ArrayPoolMock<byte>.Shared.Rent(1024);
    ArrayPoolMock<byte>.Shared.Return(arr);
  }
  
}