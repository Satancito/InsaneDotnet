# InsaneIO.Insane.Security.Extensions

Parent namespace: [InsaneIO.Insane.Security](../Security.md)

This namespace contains the direct TOTP helper API.

Use it when you want to generate, verify, or serialize TOTP-related values without creating a `TotpManager` first.

## Public type

- `TotpExtensions`

## TotpExtensions

### What the namespace is for

The helpers in `TotpExtensions` implement the direct TOTP workflow:

- generate `otpauth://` URIs
- compute TOTP codes
- verify TOTP codes
- compute remaining seconds in the current window

### Time period

`TotpDefaultPeriod` is `30`, meaning 30 seconds by default. A time window is one block of `timePeriodInSeconds`.

### URI generation

- `GenerateTotpUri(this byte[] secret, ...)`
- `GenerateTotpUri(this string base32EncodedSecret, ...)`

Behavior:

- the `byte[]` overload exposes full configuration
- the Base32 string overload is a convenience overload that decodes the secret and uses default algorithm, digit count, and period
- algorithm names in the URI use RFC-style strings:
  - `SHA1`
  - `SHA256`
  - `SHA512`

If `Md5` or `Sha384` is supplied, the implementation normalizes it to `SHA1` for TOTP.

### Code generation

- `ComputeTotpCode(this byte[] secret, DateTimeOffset now, ...)`
- `ComputeTotpCode(this byte[] secret, ...)`
- `ComputeTotpCode(this string base32EncodedSecret)`

Behavior:

- the code is derived from the current or supplied time window
- `Md5` and `Sha384` are normalized to `Sha1`
- dynamic truncation uses the low nibble of the last HMAC byte, so SHA1, SHA256, and SHA512 all behave correctly

### Verification

- `VerifyTotpCode(this string code, byte[] secret, DateTimeOffset now, ...)`
- `VerifyTotpCode(this string code, byte[] secret, DateTimeOffset now, TotpTimeWindowTolerance tolerance, ...)`
- `VerifyTotpCode(this string code, byte[] secret, ...)`
- `VerifyTotpCode(this string code, byte[] secret, TotpTimeWindowTolerance tolerance, ...)`
- Base32 convenience overloads

### Window semantics

Assume `timePeriodInSeconds = 30` and the current time is `10:01:12`.

- current window: `10:01:00` to `10:01:29`
- `None`: only that window
- `OneWindow`: `10:00:30` to `10:01:59`
- `TwoWindows`: `10:00:00` to `10:02:29`

The code itself is still generated for one specific window. Tolerance only changes what the verifier accepts.

### Remaining seconds

- `ComputeTotpRemainingSeconds(this DateTimeOffset now, long timePeriodInSeconds = TotpDefaultPeriod)`

This returns the remaining seconds in the active TOTP window. At an exact boundary, the current implementation returns the full window size.

### Example

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
bool okNow = code.VerifyTotpCode(secret);
bool okWithTolerance = code.VerifyTotpCode(secret, TotpTimeWindowTolerance.OneWindow);
```
