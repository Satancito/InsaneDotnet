# InsaneDotnet
Insane library Compatible with .NET 10 and upwards.

## Documentation

- [Cryptography](Docs/Cryptography.md)
- [Security](Docs/Security.md)
- [Publishing](Docs/Publishing.md)
- [Namespace Documentation Index](Docs/namespaces/Namespaces.md)

## Add to your project

**Package Reference**   
```
<PackageReference Include="InsaneIO.Insane" Version="10.5.4" />
```

**Dotnet CLI**   
```
dotnet add package InsaneIO.Insane --version 10.5.4
```

**Package Manager**   
```
Install-Package InsaneIO.Insane -Version 10.5.4
```
<hr />

## NuGet Publishing

This repository supports GitHub Actions trusted publishing for `nuget.org` through:

- [Docs/Publishing.md](Docs/Publishing.md)
- [InsaneIO.Insane-TrustedPublish.yml](.github/workflows/InsaneIO.Insane-TrustedPublish.yml)

Trusted publishing is the recommended release path.
The GitHub Actions publish workflow is restricted to the `main` branch.
