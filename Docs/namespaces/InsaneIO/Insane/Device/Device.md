# InsaneIO.Insane.Device

Parent namespace: [InsaneIO.Insane](../Insane.md)

This namespace exposes environment and application metadata helpers.

## Public types

- `DeviceInfo`

## DeviceInfo

`DeviceInfo` is a static convenience surface for:

- device identifiers
- OS metadata
- application metadata
- environment summary output

### Important properties

- `RealDeviceId`
- `DeviceId`
- `Manufacturer`
- `DeviceNameOrModel`
- `OSDescription`
- `Platform`
- `FriendlyName`
- `Architecture`
- `ApplicationName`
- `ApplicationVersion`
- `ApplicationDescription`

### Current implementation notes

- `RealDeviceId` is derived from a fixed internal seed through `SHA256` and Base64 encoding
- `DeviceId` is a second SHA256/Base64 transformation over `RealDeviceId`
- `Manufacturer` and `DeviceNameOrModel` currently return their own property names rather than machine-specific hardware data

### Summary output

`Summary()` returns a multi-line string with the current values.

### Example

```csharp
using InsaneIO.Insane.Device;

string summary = DeviceInfo.Summary();
Console.WriteLine(summary);
```
