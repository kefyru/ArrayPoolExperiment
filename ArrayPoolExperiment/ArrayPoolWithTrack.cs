using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ArrayPoolExperiment;

public sealed class ArrayPoolWithTracking<T> : ArrayPool<T>
{
  private static readonly ArrayPool<T> s_default;
  private static readonly long             s_hash = Random.Shared.NextInt64();
  
  static ArrayPoolWithTracking()
  {
    s_default = Default!;
    if (s_default is null) throw new InvalidOperationException();
  }

  public override T[] Rent(int size)
  {
    var array = s_default.Rent(size);
    if (typeof(T) == typeof(byte) || typeof(T) == typeof(char)) {
      ref var hash = ref Unsafe.As<byte, long>(ref MemoryMarshal.GetArrayDataReference((Array)array));
      if (hash != s_hash) {
        if (GC.GetGeneration(array) != 0) // gen = 0, more likely it is new array
          throw new InvalidOperationException($"{array.GetType()} was used after returning to pool");
      }

      hash = 0;
    }
    // update statistics to detect leaks...
    // other checks...
    return array;
  }
  public override void Return(T[] array)
  {
    if (typeof(T) == typeof(byte) || typeof(T) == typeof(char)) {
      ref var hash = ref Unsafe.As<byte, long>(ref MemoryMarshal.GetArrayDataReference((Array)array));
      if (hash == s_hash) {
        // Possible double return
        // stop the program to check code manually in this point
        var stackTrace = new StackTrace();
        Console.WriteLine("Possible double return array to pool");
        Console.WriteLine(stackTrace);
        // if (Debugger.IsAttached) Debugger.Break();
        // we won't return this array to pool, just in case
        return;
      }

      hash = s_hash;
    }
    // update statistics to detect leaks...
    // other checks...
    s_default.Return(array);
  }
}