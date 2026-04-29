# InsaneIO.Insane.Extensions

Parent namespace: [InsaneIO.Insane](../Insane.md)

This namespace contains general-purpose helpers reused across the package and useful to application code.

## Public extension classes

- `ByteArrayExtensions`
- `EncodingExtensions`
- `EnumExtensions`
- `LinqExtensions`
- `MiscExtensions`
- `ReflectionExtensions`
- `TypeExtensions`

## ByteArrayExtensions

Guards for common byte-array validation:

- `ThrowIfNullOrEmpty`
- `ThrowIfNull`
- `ThrowIfEmpty`

These throw `ArgumentException` with library-specific messages.

## EncodingExtensions

UTF-8 and generic text/binary conversion helpers:

- `ToByteArrayUtf8`
- `ToStringUtf8`
- `ToByteArray(string data, Encoding encoding)`
- `ToString(byte[] encodedBytes, Encoding encoding)`

These helpers are widely used by the cryptography and TOTP APIs whenever a string overload converts input to bytes.

## EnumExtensions

Numeric conversion helpers for enums:

- `ShortValue`
- `IntValue`
- `LongValue`
- `UShortValue`
- `UIntValue`
- `ULongValue`
- `ByteValue`
- `SByteValue`
- `NumberValue<TNumber>`
- `ParseEnum<TEnum, TNumber>`

These are especially useful when serializing enum values as numbers.

## LinqExtensions

Contains `GetExpressionReturnMembersNames<T>(Expression<Func<T, object?>> expression)`.

Use it when you want the member names produced by a projection expression. The current implementation supports:

- empty constant expressions
- `new { ... }` projections
- simple member access
- unary convert wrappers

## MiscExtensions

Contains `WriteLine(this string str)`, a tiny convenience wrapper around `Console.WriteLine`.

## ReflectionExtensions

Reflection and generic-type helpers:

- `GetGenericTypeName`
- `GetPrincipalName`
- `ConvertTo<T>`
- `GetGenericMethod`

Typical use cases:

- building readable generic type names for diagnostics
- resolving and closing generic methods at runtime

## TypeExtensions

Simple runtime type predicates:

- `IsIntOrLongType`
- `IsIntType`
- `IsLongType`
- `IsStringType`
