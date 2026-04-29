# InsaneIO.Insane.Security.Enums

Parent namespace: [InsaneIO.Insane.Security](../Security.md)

This namespace contains the public enums used by the TOTP APIs.

## Public enums

- `TwoFactorCodeLength`
- `TotpTimeWindowTolerance`

## TwoFactorCodeLength

Defines the number of digits produced by TOTP code generation.

- `SixDigits`
- `SevenDigits`
- `EightDigits`

## TotpTimeWindowTolerance

Defines how many adjacent time windows are accepted during verification.

- `None`
  - current window only
- `OneWindow`
  - previous, current, and next window
- `TwoWindows`
  - two previous windows, current window, and two next windows

The actual duration of a window is controlled by `TimePeriodInSeconds` on `TotpManager` or by the `timePeriodInSeconds` argument in the helper extensions.
