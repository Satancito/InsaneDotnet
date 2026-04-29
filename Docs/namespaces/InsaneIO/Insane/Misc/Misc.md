# InsaneIO.Insane.Misc

Parent namespace: [InsaneIO.Insane](../Insane.md)

This namespace contains small helper contracts that support consistent patterns across the library.

## Public types

- `IDefaultInstance<T>`

## IDefaultInstance<T>

`IDefaultInstance<T>` is a static abstract contract for types that expose a canonical default instance.

The built-in encoders use this pattern so callers can access ready-to-use singletons such as:

- `Base32Encoder.DefaultInstance`
- `Base64Encoder.DefaultInstance`
- `HexEncoder.DefaultInstance`

### Contract

```csharp
public interface IDefaultInstance<T>
{
    static abstract T DefaultInstance { get; }
}
```

### When to use it

Implement this interface when your custom type:

- has a natural zero-configuration default
- should be reusable without repeated allocation
- benefits from a conventional singleton-style entry point

### Example

```csharp
public sealed class MyEncoder : IEncoder, IDefaultInstance<MyEncoder>
{
    private static readonly MyEncoder _default = new();
    public static MyEncoder DefaultInstance => _default;

    // Remaining implementation omitted
}
```
