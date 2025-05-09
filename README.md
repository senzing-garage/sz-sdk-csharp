# sz-sdk-csharp

If you are beginning your journey with [Senzing],
please start with [Senzing Quick Start guides].

## Overview

The Senzing C# SDK provides the C# interface to the native Senzing SDK's.
This repository is dependent on the Senzing native shared library (`.so`,
`.dylib` or `.dll`) that is part of the Senzing product and function without it.

While this SDK is being made available as open source, the actual Senzing.Sdk
NuGet package file (`Senzing.Sdk.4.0.0-beta.2.0.nupkg`) that you use should be
obtained from Senzing product installation to ensure that the C# code version
matches the native library version.

### Prerequisites

1. Microsoft .NET for your platform: <https://dotnet.microsoft.com/en-us/download>

1. Senzing v4.0 or later (for running unit tests)

1. Set the `SENZING_PATH` environment variable:
    - Linux: `export SENZING_PATH=/opt/senzing`
    - macOS: `export SENZING_PATH=$HOME/senzing`
    - Windows: `set SENZING_PATH=%USERPROFILE%\senzing`

1. Set your library path appropriately for Senzing libraries:
    - Linux: Set the `LD_LIBRARY_PATH`:

        ```console
        export LD_LIBRARY_PATH=/opt/senzing/er/lib:$LD_LIBRARY_PATH
        ```

    - macOS: Set `DYLD_LIBRARY_PATH`:

        ```console
        export DYLD_LIBRARY_PATH=/Library/Senzing/er/lib:/Library/Senzing/er/lib/macOS:$DYLD_LIBRARY_PATH
        ```

    - Windows: Set `Path`:

        ```console
        set Path=C:\Senzing\er\lib;C:\Senzing\er\lib\windows;%Path%
        ```

### Building

1. Generating the Example Documentation Snippets

    ```console
    dotnet test Senzing.Sdk.Demo -p:GeneratePackageOnBuild=false
    ```

    Alternatively, via `Makefile`:

    ```console
    gmake demo
    ```

1. Building with Debug:

    ```console
    dotnet build Senzing.Sdk -p:GenerateDocumentationFile=true
    ```

    **NOTE:** This will produce warnings if you have **NOT** generated the
    example documentation snippets.

    Alternatively, via `Makefile` it will always generate the documentation
    example snippets first:

    ```console
    gmake debug
    ```

    The DLL will be found in `Senzing.Sdk/bin/Debug/netstandard2.0/Senzing.Sdk.dll`
    The NuGet package will be found in `Senzing.Sdk/bin/Debug/Senzing.Sdk.4.0.0-beta.2.0.nupkg`

1. Building with Release:

    ```console
    dotnet build -c Release Senzing.Sdk -p:GenerateDocumentationFile=true
    ```

    Alternatively, via `Makefile` it will always generate the documentation
    example snippets first:

    ```console
    gmake release
    ```

    **NOTE:** This will produce warnings if you have **NOT** generated the
    example documentation snippets.

    The DLL will be found in `Senzing.Sdk/bin/Release/netstandard2.0/Senzing.Sdk.dll`
    The NuGet package will be found in `Senzing.Sdk/bin/Release/Senzing.Sdk.4.0.0-beta.2.0.nupkg`

1. Running unit tests:

    ```console
    dotnet test Senzing.Sdk.Tests
    ```

    Alternatively, via `Makefile`:

    ```console
    gmake release
    ```

1. Verifying the code formatting:

    ```console
    dotnet format --verify-no-changes --verbosity diagnostic Senzing.Sdk
    dotnet format --verify-no-changes --verbosity diagnostic Senzing.Sdk.Tests
    dotnet format --verify-no-changes --verbosity diagnostic Senzing.Sdk.Demo
    ```

    Alternative, via `Makefile`:

    ```console
    gmake format
    ```

    Or for each module individually:

    ```console
    gmake format-sdk
    gmake format-test
    gmake format-demo
    ```

1. Generate documentation:

    ```console
    dotnet docfx docfx.json
    ```

    Alternatively, via `Makefile`:

    ```console
    gmake docs
    ```

    The generated documentation will reside in `target/apidocs/_site/`

1. Clean up build artifacts:

    ```console
    dotnet clean Senzing.Sdk
    dotnet clean -c Release Senzing.Sdk
    dotnet clean Senzing.Sdk.Tests
    dotnet clean Senzing.Sdk.Demo
    rm -rf Senzing.Sdk/bin
    rm -rf Senzing.Sdk/obj
    rm -rf Senzing.Sdk.Tests/bin
    rm -rf Senzing.Sdk.Tests/obj
    rm -rf Senzing.Sdk.Demo/bin
    rm -rf Senzing.Sdk.Demo/obj
    rm -rf target
    ```

    Alternative, via `Makefile`:

    ```console
    gmake clean
    ```

### Usage

1. Create and configure a local NuGet repository:

    ```console
    mkdir [path]
    dotnet nuget add source [path] -n [local-source-name]
    ```

    - Example (macOS / Linux):

        ```console
        mkdir -p ~/dev/nuget/packages
        dotnet nuget add source ~/dev/nuget/packages -n dev
        ```

    - Example (Windows):

        ```console
        mkdir %USERPROFILE%\dev\nuget\packages
        dotnet nuget add source %USERPROFILE%\dev\nuget\packages -n dev
        ```

1. Push the `Senzing.Sdk` NuGet package from your Senzing distribution to your local repository:

    ```console
    dotnet nuget push [path-to-Senzing.Sdk.4.0.0-beta.2.0.nupkg] --source [local-source-name]
    ```

    - Example (macOS / Linux):

        ```console
        dotnet nuget push /opt/senzing/er/sdk/dotnet/Senzing.Sdk.4.0.0-beta.2.0.nupkg --source dev
        ```

    - Example (Windows):

        ```console
        dotnet nuget push %USERPROFILE%\Senzing\er\sdk\dotnet\Senzing.Sdk.4.0.0-beta.2.0.nupkg --source dev
        ```

1. Add the `Senzing.Sdk` NuGet package as a dependency to your project:
    - **OPTION 1:** Add the latest pre-release version of Senzing.Sdk as your dependency:

        ```console
        dotnet add [your-project-name] package Senzing.Sdk --prerelease
        ```

    - **OPTION 2:** Add a specific version of Senzing.Sdk as your dependency:

        ```console
        dotnet add [your-project-name] package Senzing.Sdk --version 4.0.0-beta.2.0
        ```

    - **OPTION 3:** Add the latest production version of Senzing.Sdk as your dependency
    *(note: this will only function once Senzing.Sdk v4.0.0 is released)*:

        ```console
        dotnet add [your-project-name] package Senzing.Sdk
        ```

## Using Example Code

Senzing encourages and allows you to freely copy the provided example code and modify it to your own
needs as you see fit.  However, please refer to the [Global Suppression Notes] to understand how to
best adapt the example code to your own coding project.

[Senzing]: https://senzing.com/
[Senzing Quick Start guides]: https://docs.senzing.com/quickstart/
[Global Suppression Notes]: https://github.com/Senzing/code-snippets-v4/blob/main/csharp/GlobalSuppressions.md
