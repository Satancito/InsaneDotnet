# InsaneIO.Insane.Core

Parent namespace: [InsaneIO.Insane](../Insane.md)

This namespace currently contains a very small root-level compatibility type.

Most consumers will not need it during day-to-day development, but it remains part of the public surface.

## Public types

- `InsaneCore`

## InsaneCore

`InsaneCore` is a minimal holder for library-wide core constants.

### Public members

- `Empty`

`Empty` is a public constant string set to `""`.

### Typical usage

Most projects will use `string.Empty` directly. `InsaneCore.Empty` is only relevant if you want to stay within the package-provided core symbols for consistency.
