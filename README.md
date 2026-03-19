# runtimedl

[![Build and Release](https://github.com/KevinGliewe/dotnet_runtime/actions/workflows/build-and-release.yml/badge.svg)](https://github.com/KevinGliewe/dotnet_runtime/actions/workflows/build-and-release.yml)

A self-contained tool to download a .NET runtime.

## Installation

Download the latest release for your platform from the [Releases page](https://github.com/KevinGliewe/dotnet_runtime/releases).

### Available platforms

| Platform | Architecture | Archive |
|----------|-------------|---------|
| Linux | x64 | `runtimedl-linux-x64.tar.gz` |
| Linux | arm64 | `runtimedl-linux-arm64.tar.gz` |
| Linux (musl/Alpine) | x64 | `runtimedl-linux-musl-x64.tar.gz` |
| Linux (musl/Alpine) | arm64 | `runtimedl-linux-musl-arm64.tar.gz` |
| Windows | x64 | `runtimedl-win-x64.zip` |
| Windows | x86 | `runtimedl-win-x86.zip` |
| Windows | arm64 | `runtimedl-win-arm64.zip` |
| macOS | x64 | `runtimedl-osx-x64.tar.gz` |
| macOS | arm64 | `runtimedl-osx-arm64.tar.gz` |

### Quick install (Linux/macOS)

```bash
# Download and extract (replace <RID> with your platform, e.g. linux-x64, osx-arm64)
curl -sL https://github.com/KevinGliewe/dotnet_runtime/releases/latest/download/runtimedl-<RID>.tar.gz | tar xz -C /usr/local/bin/
```

## Usage

```
Usage:
  runtimedl [options]

Options:
  --runtime-type <Runtime|Runtime_aspnetcore|Runtime_Desktop|Sdk>    runtimeType [default: Runtime]
  --platform <Linux|Local|Macos|Windows>                             platform [default: Local]
  --architecture <Arm32|Arm64|Arm64_Alpine|Local|X64|X86>            architecture [default: Local]
  --version-pattern <version-pattern>                                version_pattern [default: ^\d+\.\d+\.\d+$]
  --output <output>                                                  output [default: ]
  --download                                                         download [default: True]
  --version                                                          Show version information
  -?, -h, --help                                                     Show help and usage information
```

## Example

`runtimedl --version-pattern "^5\.0\.3$" --output "./bin"`

Downloads a 5.0.3 runtime for the local system setup and unpacks it into `./bin`.
