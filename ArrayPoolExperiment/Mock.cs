using System;
using System.Reflection;

namespace ArrayPoolExperiment;

[AttributeUsage(AttributeTargets.Assembly)]
public class DefaultSharedArrayPoolAttribute(Type type) : Attribute
{
  public Type Type { get; } = type;
}

/// <summary>
/// Mock class (simulates System.Buffers.ArrayPool[T])
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ArrayPoolMock<T>
{
  protected static ArrayPoolMock<T>? Default { get; private set; }

  private static readonly ArrayPoolMock<T> s_shared;
  public static ArrayPoolMock<T> Shared => s_shared;

  static ArrayPoolMock()
  {
    Default = new SharedArrayPoolMock<T>();
    try {
      var type = Assembly.GetEntryAssembly()
                        ?.GetCustomAttribute<DefaultSharedArrayPoolAttribute>()
                        ?.Type;

      if (type is not null)
        s_shared = (ArrayPoolMock<T>)Activator
         .CreateInstance(type.MakeGenericType([typeof(T)]))!;
    }
    finally {
      s_shared ??= Default;
      Default  =   null;
    }

  }

  public abstract T[] Rent(int size);
  public abstract void Return(T[] array);
}

/// <summary>
/// Mock class (simulates System.Buffers.SharedArrayPool[T])
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class SharedArrayPoolMock<T> : ArrayPoolMock<T>
{
  private static System.Buffers.ArrayPool<T> s_shared = System.Buffers.ArrayPool<T>.Shared;
  public override T[] Rent(int size) => s_shared.Rent(size);
  public override void Return(T[] array) => s_shared.Return(array);
}