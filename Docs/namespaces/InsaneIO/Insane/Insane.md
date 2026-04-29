# InsaneIO.Insane

Documentation index: [Namespaces](../../Namespaces.md)

`InsaneIO.Insane` is the root namespace for shared constants and library-wide values that support the public APIs in cryptography, security, and utility helpers.

Use this namespace when you want the canonical constant values that the library itself uses for:

- Base64 variant markers and URL encodings
- line-break lengths used by MIME and PEM formatting
- default cryptographic sizes
- PEM and XML RSA key markers and validation patterns

For everyday application code, most callers use the higher-level namespaces directly. Reach for the root namespace when you need the same constants the built-in extensions and serializable types already rely on.

## Public types

- `Constants`

## Constants

`Constants` is a simple container for shared constant values. It is not stateful and is normally consumed in static form.

### What it contains

- Base64 helper values such as `Base64NoLineBreaks`, `Base64MimeLineBreaksLength`, and `Base64PemLineBreaksLength`
- hash output sizes such as `Sha256HashSizeInBytes`
- default SCrypt and Argon2 parameter values
- RSA PEM headers, footers, and regex patterns
- URL-safe replacement text used by the Base64 helpers

### Typical usage

Most consumers do not need to instantiate or derive anything from `Constants`. The class is mainly useful when you want your own code to line up with the same defaults used by the package.

```csharp
using InsaneIO.Insane;
using InsaneIO.Insane.Cryptography.Extensions;

byte[] data = "hello".ToByteArrayUtf8();
string pemChunked = data.EncodeToBase64(Constants.Base64PemLineBreaksLength);
```

### Guidance

- Prefer the concrete helper methods and serializable types when available.
- Use these constants when you need to configure a helper explicitly and want to stay aligned with the package defaults.
