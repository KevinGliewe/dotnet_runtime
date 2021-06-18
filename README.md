# .NET Runtime

## Download runtime

Execute `scripts/download-runtime.ps1` to download and unpack a prebuild runtime into `bin/`.

To download a specific runtime you can use the optional arguments `-architecture <Arm32|Arm64|Arm64_Alpine|Local|X64|X86>` and `-platform <Linux|Local|Macos|Windows>`

### Example

`download-runtime.ps1 -architecture Arm64_Alpine -platform Linux`

## Example project

For an example on how to use this repository check out [this repo](https://github.com/KevinGliewe/dotnet_runtime_test)