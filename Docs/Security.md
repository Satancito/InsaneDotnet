# Security

This document is the single-file reference for the public security surface of `InsaneIO.Insane`.

It covers:

- `InsaneIO.Insane.Security`
- `InsaneIO.Insane.Security.Enums`
- `InsaneIO.Insane.Security.Extensions`
- the TOTP object API
- the direct TOTP helper API
- serialization behavior
- time-window tolerance behavior

For readers who prefer the namespace tree, the structured index remains available at [Docs/namespaces/Namespaces.md](namespaces/Namespaces.md).

## Covered namespaces

- `InsaneIO.Insane.Security`
- `InsaneIO.Insane.Security.Enums`
- `InsaneIO.Insane.Security.Extensions`

## How the security API is organized

The current public security surface is centered on TOTP.

You can work with it in two ways:

1. Direct static-style extension helpers
2. `TotpManager`, a reusable serializable object

## Public enums

## TwoFactorCodeLength

Defines the length of the generated one-time code:

- `SixDigits`
- `SevenDigits`
- `EightDigits`

## TotpTimeWindowTolerance

Controls how many adjacent time windows are accepted during verification:

- `None`
- `OneWindow`
- `TwoWindows`

### What a time window means

If `TimePeriodInSeconds = 30`, then each TOTP window lasts 30 seconds.

Example:

- `10:01:00` to `10:01:29` -> one window
- `10:01:30` to `10:01:59` -> next window

### Tolerance meanings

If the current time is `10:01:12` and the period is 30 seconds:

- `None`
  - only `10:01:00` to `10:01:29`
- `OneWindow`
  - `10:00:30` to `10:01:59`
- `TwoWindows`
  - `10:00:00` to `10:02:29`

The code is still generated for one specific window. Tolerance only changes what the verifier accepts.

## TotpExtensions

Namespace: `InsaneIO.Insane.Security.Extensions`

This is the direct helper API for generating and verifying TOTP values.

### Public members

#### Constants

- `TotpDefaultPeriod = 30`

#### URI generation

- `GenerateTotpUri(this byte[] secret, string label, string issuer, HashAlgorithm algorithm = HashAlgorithm.Sha1, TwoFactorCodeLength codeLength = TwoFactorCodeLength.SixDigits, long timePeriodInSeconds = TotpDefaultPeriod)`
- `GenerateTotpUri(this string base32EncodedSecret, string label, string issuer)`

#### Code generation

- `ComputeTotpCode(this byte[] secret, DateTimeOffset now, TwoFactorCodeLength length = ..., HashAlgorithm hashAlgorithm = ..., long timePeriodInSeconds = TotpDefaultPeriod)`
- `ComputeTotpCode(this byte[] secret, TwoFactorCodeLength length = ..., HashAlgorithm hashAlgorithm = ..., long timePeriodInSeconds = TotpDefaultPeriod)`
- `ComputeTotpCode(this string base32EncodedSecret)`

#### Verification

- `VerifyTotpCode(this string code, byte[] secret, DateTimeOffset now, TwoFactorCodeLength length = ..., HashAlgorithm hashAlgorithm = ..., long timePeriodInSeconds = TotpDefaultPeriod)`
- `VerifyTotpCode(this string code, byte[] secret, DateTimeOffset now, TotpTimeWindowTolerance tolerance, TwoFactorCodeLength length = ..., HashAlgorithm hashAlgorithm = ..., long timePeriodInSeconds = TotpDefaultPeriod)`
- `VerifyTotpCode(this string code, byte[] secret, TwoFactorCodeLength length = ..., HashAlgorithm hashAlgorithm = ..., long timePeriodInSeconds = TotpDefaultPeriod)`
- `VerifyTotpCode(this string code, byte[] secret, TotpTimeWindowTolerance tolerance, TwoFactorCodeLength length = ..., HashAlgorithm hashAlgorithm = ..., long timePeriodInSeconds = TotpDefaultPeriod)`
- Base32 secret convenience overloads

#### Remaining seconds

- `ComputeTotpRemainingSeconds(this DateTimeOffset now, long timePeriodInSeconds = TotpDefaultPeriod)`

## Important TOTP behavior

### Algorithm names in URIs

Generated `otpauth://` URIs use RFC-style names:

- `SHA1`
- `SHA256`
- `SHA512`

### MD5 normalization

If `HashAlgorithm.Md5` or `HashAlgorithm.Sha384` is supplied to the TOTP helpers:

- code generation normalizes it to `Sha1`
- verification normalizes it to `Sha1`
- URI generation emits `algorithm=SHA1`

This preserves compatibility while keeping the TOTP behavior aligned with supported RFC algorithms.

Supported normalization map:

- `Md5 -> Sha1`
- `Sha384 -> Sha1`
- `Sha1 -> Sha1`
- `Sha256 -> Sha256`
- `Sha512 -> Sha512`

### Dynamic truncation

The implementation uses the low nibble of the last HMAC byte as the dynamic truncation offset. That means SHA1, SHA256, and SHA512 all work correctly with the current implementation.

### Current-window verification

If you call verification without a tolerance overload, it checks only the current time window.

Example:

```csharp
bool valid = code.VerifyTotpCode(secret);
```

### Verification with tolerance

```csharp
bool valid = code.VerifyTotpCode(secret, TotpTimeWindowTolerance.OneWindow);
```

This accepts the previous, current, and next window.

### Remaining seconds behavior

`ComputeTotpRemainingSeconds(...)` returns the remaining seconds in the current window.

At an exact window boundary, the current implementation returns the full window size.

## TotpManager

Namespace: `InsaneIO.Insane.Security`

`TotpManager` is the main object-oriented TOTP entry point.

Use it when you want:

- reusable configuration
- serialization/deserialization
- secret, issuer, and label stored together
- convenience aliases around the direct helper API

### Properties

- `Secret` (required)
- `Label` (required)
- `Issuer` (required)
- `CodeLength`
- `HashAlgorithm`
- `TimePeriodInSeconds`

Defaults:

- `CodeLength = TwoFactorCodeLength.SixDigits`
- `HashAlgorithm = HashAlgorithm.Sha1`
- `TimePeriodInSeconds = TotpExtensions.TotpDefaultPeriod`

### Factory methods

- `FromSecret(...)`
- `FromBase32Secret(...)`
- `FromEncodedSecret(...)`

These let you create equivalent managers from:

- raw bytes
- Base32 text
- any text decoded by an `IEncoder`

### Serialization

`TotpManager` implements `IJsonSerializable`.

JSON shape:

```json
{
  "TypeIdentifier": "Insane-Security-TotpManager",
  "Secret": "MFXGG33EMV2W4YLMONQWG2A=",
  "Label": "user@example.com",
  "Issuer": "Insane IO",
  "CodeLength": 6,
  "HashAlgorithm": 1,
  "TimePeriodInSeconds": 30
}
```

Important serialization rules:

- `Secret` is written as Base32
- `CodeLength` is numeric
- `HashAlgorithm` is numeric
- `Serialize` defaults to `indented: true`

### Deserialization validation

`Deserialize(string json)` rejects:

- missing `TypeIdentifier`
- wrong `TypeIdentifier`
- missing `Secret`
- missing `Label`
- missing `Issuer`
- invalid `CodeLength`
- invalid `HashAlgorithm`
- invalid `TimePeriodInSeconds`

### URI methods

- `ToOtpUri()`
- `GenerateTotpUri()`

These are equivalent. `GenerateTotpUri()` is the compatibility alias.

### Code-generation methods

- `ComputeCode()`
- `ComputeCode(DateTimeOffset now)`
- `ComputeTotpCode()`
- `ComputeTotpCode(DateTimeOffset now)`

`ComputeTotpCode(...)` is the alias pair for `ComputeCode(...)`.

### Verification methods

- `VerifyCode(string code)`
- `VerifyCode(string code, TotpTimeWindowTolerance tolerance)`
- `VerifyCode(string code, DateTimeOffset now)`
- `VerifyCode(string code, DateTimeOffset now, TotpTimeWindowTolerance tolerance)`
- matching `VerifyTotpCode(...)` aliases

### Remaining-seconds methods

- `ComputeRemainingSeconds()`
- `ComputeRemainingSeconds(DateTimeOffset now)`
- `ComputeTotpRemainingSeconds()`
- `ComputeTotpRemainingSeconds(DateTimeOffset now)`

Again, the `Totp` versions are aliases.

## Direct helpers vs manager

### Prefer TotpExtensions when

- you already have the secret bytes
- you want a quick one-off computation
- you do not need serialization

### Prefer TotpManager when

- you want to store label, issuer, algorithm, and period together
- you want to serialize the configuration
- you want an application-level object for dependency injection or reuse
- you want compatibility aliases and a friendlier object API

## Practical examples

## Example 1: direct TOTP helpers

```csharp
using InsaneIO.Insane.Cryptography.Enums;
using InsaneIO.Insane.Security.Enums;
using InsaneIO.Insane.Security.Extensions;

byte[] secret = "insaneiosecret".ToByteArrayUtf8();

string uri = secret.GenerateTotpUri(
    label: "user@example.com",
    issuer: "Insane IO",
    algorithm: HashAlgorithm.Sha256,
    codeLength: TwoFactorCodeLength.SixDigits,
    timePeriodInSeconds: 30);

string code = secret.ComputeTotpCode();
bool valid = code.VerifyTotpCode(secret);
bool validWithTolerance = code.VerifyTotpCode(secret, TotpTimeWindowTolerance.OneWindow);
```

## Example 2: manager-based TOTP

```csharp
using InsaneIO.Insane.Cryptography.Enums;
using InsaneIO.Insane.Security;
using InsaneIO.Insane.Security.Enums;

TotpManager manager = TotpManager.FromSecret(
    secret: "insaneiosecret".ToByteArrayUtf8(),
    label: "user@example.com",
    issuer: "Insane IO",
    codeLength: TwoFactorCodeLength.SixDigits,
    hashAlgorithm: HashAlgorithm.Sha1,
    timePeriodInSeconds: 30);

string otpUri = manager.ToOtpUri();
string code = manager.ComputeCode();

bool okNow = manager.VerifyCode(code);
bool okWithTolerance = manager.VerifyCode(code, TotpTimeWindowTolerance.OneWindow);

string json = manager.Serialize();
TotpManager restored = TotpManager.Deserialize(json);
```

## Example 3: equivalent manager factories

```csharp
byte[] secret = "insaneiosecret".ToByteArrayUtf8();
string base32 = Base32Encoder.DefaultInstance.Encode(secret);
string hex = HexEncoder.DefaultInstance.Encode(secret);

TotpManager fromBytes = TotpManager.FromSecret(secret, "user@example.com", "Insane IO");
TotpManager fromBase32 = TotpManager.FromBase32Secret(base32, "user@example.com", "Insane IO");
TotpManager fromHex = TotpManager.FromEncodedSecret(hex, HexEncoder.DefaultInstance, "user@example.com", "Insane IO");
```

## Summary

The current .NET security surface is intentionally small and focused:

- direct TOTP helpers through `TotpExtensions`
- reusable TOTP configuration through `TotpManager`
- explicit time-window tolerance through `TotpTimeWindowTolerance`

That keeps the API easy to reason about while still covering the real integration scenarios:

- QR/URI provisioning
- current-window validation
- tolerant validation for clock skew
- JSON persistence of TOTP configuration
