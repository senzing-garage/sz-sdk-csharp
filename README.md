# sz-sdk-csharp

If you are beginning your journey with [Senzing],
please start with [Senzing Quick Start guides].

## Overview

The Senzing C# SDK provides the C# interface to the native Senzing SDK's.
This repository is dependent on the Senzing native shared library (`.so`,
`.dylib` or `.dll`) that is part of the Senzing product and function without it.

While this SDK is being made available as open source the actual `Senzing.Sdk.dll`
file that you use should be obtained from Senzing product installation to
ensure that the C# code version matches the native library version.  

### Preequisites
1. Microsoft .NET for your platform: https://dotnet.microsoft.com/en-us/download

1. Senzing v4.0 or later (for running unit tests)

1. Set the `SENZING_DIR` environment variable:
    - Linux: `export SENZING_DIR=/opt/senzing/er`
    - macOS: `export SENZING_DIR=/Library/Senzing/er`
    - Windows: `set SENZING_DIR=C:\Senzing\er`

1. Set your library path appropriately for Senzing libraries:
    - Linux: Set the `LD_LIBRARY_PATH`:
        ```
        export LD_LIBRARY_PATH=/opt/senzing/er/lib:$LD_LIBRARY_PATH
        ```
    - macOS: Set `DYLD_LIBRARY_PATH`:
        ```
        export DYLD_LIBRARY_PATH=/Library/Senzing/er/lib:/Library/Senzing/er/lib/macOS:$DYLD_LIBRARY_PATH
        ```
    - Windows: Set `Path`:
        ```
        set Path=C:\Senzing\er\lib;C:\Senzing\er\lib\windows;%Path%
        ```

### Building
1. Building with Debug:
    ```console
    dotnet build Senzing.Sdk
    ```
    The DLL will be found in `Senzing.Sdk/bin/Debug/netstandard2.0/Senzing.Sdk.dll`

1. Building with Release:
    ```console
    dotnet build -c Release Senzing.Sdk
    ```
    The DLL will be found in `Senzing.Sdk/bin/Release/netstandard2.0/Senzing.Sdk.dll`

1. Running unit tests:
    ```console
    dotnet test Senzing.Sdk.Tests
    ```

1. Verifying the code formatting:
    ```console
    dotnet format --verify-no-changes --verbosity diagnostic Senzing.Sdk
    dotnet format --verify-no-changes --verbosity diagnostic Senzing.Sdk.Tests
    ```

1. Generate documentation:
    ```console
    dotnet docfx docfx.json
    ```
    The generated documentation will reside in `target/apidocs/_site/`

1. Clean up build artfiacts:
    ```console
    dotnet clean Senzing.Sdk
    dotnet clean -c Release Senzing.Sdk
    dotnet clean Senzing.Sdk.Tests
    rm -rf target
    ```

[Senzing]: https://senzing.com/
[Senzing Garage]: https://github.com/senzing-garage
[Senzing Quick Start guides]: https://docs.senzing.com/quickstart/
