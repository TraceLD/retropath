# TraceLD.RDKit.CSharp
[![nuget](https://img.shields.io/nuget/v/TraceLD.RDKit.Csharp?style=for-the-badge)](https://www.nuget.org/packages/TraceLD.RDKit.CSharp/)
[![dotnet](https://img.shields.io/badge/targets-net6.0-blueviolet?style=for-the-badge)](https://dotnet.microsoft.com/en-us/)

C# Wrapper for the RDKit C++ cheminformatics library with support for Windows, Linux and macOS.

## Usage

- Create a new .NET project targeting `net6.0`.
- [Add the package from nuget](https://www.nuget.org/packages/TraceLD.RDKit.CSharp/).
- Build your project.
- Hopefully everything works 🙂 (assuming you're running on a supported system).

See the [examples folder](https://github.com/TraceLD/TraceLD.RDKit.CSharp/tree/main/examples) for example project(s).

## Currently supported operating systems

### Windows

- Supports Windows 10 x86 and x64 and Windows 11 x64.
- Tested on Win 10 and Win 11 21H2 (OS Build 22000.613).

### Linux

- Supports x64 Linux.
- Currently tested distros:
  - Debian GNU/Linux 11 (bullseye): 5.10.16.3-microsoft-standard-WSL2
  - Ubuntu 18.04.5 LTS: 5.10.16.3-microsoft-standard-WSL2
  - Other distros *should* work but you might find some packages need to first be installed.
    - I will be testing (and adding support for, if changes are needed) the package on more distros as time permits.

### macOS

 - Supports ARM64 (Apple Silicon) Monterey.
 - I sadly do not own an Intel Mac to be able to provide x64 binaries and test them.
   - I will look into GitHub Actions/Azure Pipelines in the future for this but can't guarantee success.

## Release info

- RDKit: Release_2021_09_4
- dotnet: targets `net6.0` (built using .NET SDK 6.0.202)
- SWIG: 3.0.12
- Boost: boost_1_74_0-msvc-14.2 (Win), boost 1.74.0 (linux), boost 1.79.0 (macOS)
- Cairo: 1.16.0
- libpng: 1.6.37
- pixman: 0.40.0
- zlib: 1.2.11

## Known issues and limitations

- Using `gzstream` results in an exception on Linux and macOS.

## Acknowledgements

kazuyaujihara/build-rdkit (https://github.com/kazuyaujihara/build-rdkit) repository was of great help when building the package.

