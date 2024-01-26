using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ArrayPoolExperiment;

[MemoryDiagnoser]
[SimpleJob(invocationCount: 10000000)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
// [DisassemblyDiagnoser]
public class Test
{
  // on macbook pro M1
  // | Method       | Mean     | Error    | StdDev   | Allocated |
  // |------------- |---------:|---------:|---------:|----------:|
  // | StandardTest | 10.76 ns | 0.029 ns | 0.025 ns |         - |
  // | MockTest     | 11.24 ns | 0.018 ns | 0.015 ns |         - |

  
  [Benchmark] public void StandardTest()
  {
    var arr = System.Buffers.ArrayPool<byte>.Shared.Rent(1024);
    System.Buffers.ArrayPool<byte>.Shared.Return(arr);
  }
  
  [Benchmark] public void MockTest()
  {
    var arr = ArrayPool<byte>.Shared.Rent(1024);
    ArrayPool<byte>.Shared.Return(arr);
  }
  
}