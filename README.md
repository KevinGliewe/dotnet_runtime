# runtimedl

A dotnet tool to download a .NET runtime.

## Installation

´dotnet tool install --global runtimedl --version 1.0.1´

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

`dotnet runtimedl --version-pattern "^5\\.0\\.\\3$" --output "./bin"`

Downloads a 5.0.3 runtime for the local system setup and unpacks it into `./bin`.